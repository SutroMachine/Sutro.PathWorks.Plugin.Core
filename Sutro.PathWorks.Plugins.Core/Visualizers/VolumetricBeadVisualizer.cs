using g3;
using gs;
using gs.FillTypes;
using Sutro.Core.Models.GCode;
using Sutro.PathWorks.Plugins.API.Visualizers;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sutro.PathWorks.Plugins.Core.Visualizers
{
    public class VolumetricBeadVisualizer : IVisualizer
    {
        private readonly Decompiler decompiler;
        private readonly IToolpathPreviewMesher<PrintVertex> mesher;

        protected int layerIndex;
        protected int pointCount;
        protected int fillTypeInteger;
        protected Vector3f color;

        public virtual event Action<ToolpathPreviewVertex[], int[], int> OnMeshGenerated;

        public virtual event Action<List<Vector3d>, int> OnLineGenerated;

        public virtual event Action<ToolpathPreviewVertex[], int> OnPointsGenerated;

        public virtual event Action<double, int> OnNewPlane;

        public virtual string Name => "Bead Visualizer";

        public static readonly Dictionary<string, int> FillTypeIntegerId = new Dictionary<string, int>()
        {
            {DefaultFillType.Label, 0},
            {InnerPerimeterFillType.Label, 1},
            {OuterPerimeterFillType.Label, 2},
            {OpenShellCurveFillType.Label, 3},
            {SolidFillType.Label, 4},
            {SparseFillType.Label, 5},
            {SupportFillType.Label, 6},
            {BridgeFillType.Label, 7},
        };

        public static readonly Dictionary<int, string> FillTypeStringInt = new Dictionary<int, string>()
        {
            {0, DefaultFillType.Label},
            {1, InnerPerimeterFillType.Label},
            {2, OuterPerimeterFillType.Label},
            {3, OpenShellCurveFillType.Label},
            {4, SolidFillType.Label},
            {5, SparseFillType.Label},
            {6, SupportFillType.Label},
            {7, BridgeFillType.Label},
        };

        public Dictionary<int, VisualizerFillType> FillTypes { get; protected set; } = new Dictionary<int, VisualizerFillType>()
        {
            {0, new VisualizerFillType("Unknown", new Vector3f(0.5, 0.5, 0.5))},
            {1, new VisualizerFillType("Inner Perimeter", new Vector3f(1, 0, 0))},
            {2, new VisualizerFillType("Outer Perimeter", new Vector3f(1, 1, 0))},
            {3, new VisualizerFillType("Open Mesh Curve", new Vector3f(0, 1, 1))},
            {4, new VisualizerFillType("Solid Fill", new Vector3f(0, 0.5f, 1))},
            {5, new VisualizerFillType("Sparse Fill", new Vector3f(0.5f, 0, 1))},
            {6, new VisualizerFillType("Support", new Vector3f(1, 0, 1))},
            {7, new VisualizerFillType("Bridge", new Vector3f(0, 0, 1))},
        };

        protected readonly FixedRangeCustomDataDetails customDataBeadWidth =
            new FixedRangeCustomDataDetails(
                () => "Bead Width",
                (value) => $"{value:F2} mm", 0.1f, 0.8f);

        protected readonly AdaptiveRangeCustomDataDetails customDataFeedRate =
            new AdaptiveRangeCustomDataDetails(
                () => "Feed Rate",
                (value) => $"{value:F0} mm/min");

        protected readonly NormalizedAdaptiveRangeCustomDataDetails customDataCompletion =
            new NormalizedAdaptiveRangeCustomDataDetails(
                () => "Completion",
                (value) => $"{value:P0}");

        public virtual VisualizerCustomDataDetailsCollection CustomDataDetails =>
            new VisualizerCustomDataDetailsCollection(
                customDataBeadWidth, customDataFeedRate, customDataCompletion);

        public VolumetricBeadVisualizer()
        {
            decompiler = new Decompiler();
            decompiler.OnToolpathComplete += ProcessToolpath;
            decompiler.OnNewLayer += ProcessNewLayer;

            mesher = new TubeMesher<PrintVertex>();
        }

        private void ProcessNewLayer(int newLayerIndex)
        {
            layerIndex = newLayerIndex;
            OnNewPlane?.Invoke(0, newLayerIndex);
        }

        private void ProcessToolpath(IToolpath toolpath)
        {
            if (toolpath is LinearToolpath3<PrintVertex> linearToolpath)
            {
                fillTypeInteger = GetFillTypeInteger(linearToolpath);
                color = GetColor(fillTypeInteger);

                switch (linearToolpath.Type)
                {
                    case ToolpathTypes.Travel:
                        RaiseLineGenerated(linearToolpath);
                        break;

                    case ToolpathTypes.PlaneChange:
                        RaiseLineGenerated(linearToolpath);
                        break;

                    case ToolpathTypes.Deposition:
                        Emit(linearToolpath);
                        CreatePoints(linearToolpath);
                        break;
                    default:
                        break;
                }
            }
        }

        public virtual void BeginGCodeLineStream()
        {
            decompiler.Begin();
        }

        public virtual void ProcessGCodeLine(GCodeLine line)
        {
            decompiler.ProcessGCodeLine(line);
        }

        protected virtual void RaiseLineGenerated(LinearToolpath3<PrintVertex> toolpath)
        {
            var points = new List<Vector3d>(toolpath.VertexCount);
            foreach (var vertex in toolpath)
            {
                points.Add(vertex.Position);
            }
             OnLineGenerated?.Invoke(points, layerIndex);
        }

        public virtual void EndGCodeLineStream()
        {
            decompiler.End();
        }


        protected virtual ToolpathPreviewVertex VertexFactory(PrintVertex vertex, Vector3d position, float brightness)
        {
            // Update adaptive ranges for custom data
            customDataFeedRate.ObserveValue((float)vertex.FeedRate);
            customDataCompletion.ObserveValue(pointCount);

            return new ToolpathPreviewVertex(
                position, fillTypeInteger, layerIndex, color, brightness,
                new CustomColorData(vertex.Dimensions.x, vertex.FeedRate, pointCount));;
        }

        protected virtual void Emit(LinearToolpath3<PrintVertex> toolpath)
        {
            if (toolpath.VertexCount < 2)
                return;

            var mesh = mesher.Generate(toolpath, VertexFactory);
            pointCount += toolpath.VertexCount;

            EndEmit(Tuple.Create(mesh.Vertices, mesh.Triangles), layerIndex);
        }

        private ToolpathPreviewVertex[] CreatePoints(LinearToolpath3<PrintVertex> toolpath)
        {
            var previewVertices = new ToolpathPreviewVertex[toolpath.VertexCount];

            int i = 0;
            int fillTypeIndex = GetFillTypeInteger(toolpath);
            foreach (var printVertex in toolpath)
            {
                var color = GetColor(fillTypeIndex);

                previewVertices[i++] = new ToolpathPreviewVertex(
                    printVertex.Position, fillTypeIndex, layerIndex,
                    color, 1, new CustomColorData(1, 1, 1));
            }
            return previewVertices;
        }

        private static int GetFillTypeInteger(LinearToolpath3<PrintVertex> toolpath)
        {
            if (FillTypeIntegerId.TryGetValue(toolpath.FillType.GetLabel(), out int newFillType))
            {
                return newFillType;
            }
            else
            {
                return FillTypeIntegerId[DefaultFillType.Label];
            }
        }

        private Vector3f GetColor(int fillType)
        {
            Vector3f color = FillTypes[FillTypeIntegerId[DefaultFillType.Label]].Color;

            if (FillTypes.TryGetValue(fillType, out var fillInfo))
            {
                color = fillInfo.Color;
            }

            return color;
        }

        protected virtual void EndEmit(Tuple<ToolpathPreviewVertex[], int[]> mesh, int layerIndex)
        {
            OnMeshGenerated?.Invoke(mesh.Item1, mesh.Item2, layerIndex);
        }

        public virtual void PrintLayerCompleted(PrintLayerData printLayerData)
        {
        }

        protected virtual GenericGCodeParser Parser { get; } = new GenericGCodeParser();

        public virtual void ProcessGCodeLine(string line)
        {
            // Note: if/when GenericGCodeParser exposes the protected method ParseLine,
            // this could be simplified
            foreach (var gcodeLine in Parser.Parse(new StringReader(line)).AllLines())
                ProcessGCodeLine(gcodeLine);
        }
    }
}