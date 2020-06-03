using g3;
using gs;
using Sutro.Core.Models.GCode;
using Sutro.PathWorks.Plugins.API.Visualizers;
using Sutro.PathWorks.Plugins.Core.CustomData;
using Sutro.PathWorks.Plugins.Core.Decompilers;
using Sutro.PathWorks.Plugins.Core.Meshers;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sutro.PathWorks.Plugins.Core.Visualizers
{
    public class VisualizerCore : VisualizerBase<PrintVertex>
    {
        public override string Name { get; }

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

        public override VisualizerCustomDataDetailsCollection CustomDataDetails =>
            new VisualizerCustomDataDetailsCollection(
                customDataBeadWidth, customDataFeedRate, customDataCompletion);

        public VisualizerCore(
            string name,
            FillTypeMapper mapper, 
            DecompilerBase<PrintVertex> decompiler,
            IToolpathPreviewMesher<PrintVertex> mesher) 
            : base(mapper, decompiler, mesher)
        {
            decompiler.OnToolpathComplete += ProcessToolpath;
            decompiler.OnNewLayer += RaiseNewLayer;
        }

        protected virtual void ProcessToolpath(IToolpath toolpath)
        {
            if (toolpath is LinearToolpath3<PrintVertex> linearToolpath)
            {
                fillTypeInteger = fillTypeMapper.GetIntegerFromLabel(linearToolpath.FillType.GetLabel());
                color = fillTypeMapper.GetColor(fillTypeInteger);

                switch (linearToolpath.Type)
                {
                    case ToolpathTypes.Travel:
                        RaiseLineGenerated(linearToolpath);
                        break;

                    case ToolpathTypes.PlaneChange:
                        RaiseLineGenerated(linearToolpath);
                        break;

                    case ToolpathTypes.Deposition:
                        EmitMesh(linearToolpath);
                        EmitPoints(linearToolpath);
                        break;

                    default:
                        break;
                }
            }
        }

        protected virtual void EmitPoints(LinearToolpath3<PrintVertex> linearToolpath)
        {
            RaisePointsGenerated(CreatePoints(linearToolpath), layerIndex);
        }

        public override void BeginGCodeLineStream()
        {
            Reset();
            decompiler.Begin();
        }

        private void Reset()
        {
            layerIndex = 0;
            pointCount = 0;

            foreach (var customData in EnumerateCustomFields())
            {
                if (customData is AdaptiveRangeCustomDataDetails adaptive)
                {
                    adaptive.Reset();
                }
            }
        }

        private IEnumerable<IVisualizerCustomDataDetails> EnumerateCustomFields()
        {
            if (CustomDataDetails.Field0 != null)
                yield return CustomDataDetails.Field0;

            if (CustomDataDetails.Field1 != null)
                yield return CustomDataDetails.Field1;

            if (CustomDataDetails.Field2 != null)
                yield return CustomDataDetails.Field2;

            if (CustomDataDetails.Field3 != null)
                yield return CustomDataDetails.Field3;

            if (CustomDataDetails.Field4 != null)
                yield return CustomDataDetails.Field4;

            if (CustomDataDetails.Field5 != null)
                yield return CustomDataDetails.Field5;
        }

        public override void ProcessGCodeLine(GCodeLine line)
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

            RaiseLineGenerated(points, layerIndex);
        }

        public override void EndGCodeLineStream()
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
                new CustomColorData(vertex.Dimensions.x, vertex.FeedRate, pointCount)); ;
        }

        protected virtual void EmitMesh(LinearToolpath3<PrintVertex> toolpath)
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
            int fillTypeIndex = fillTypeMapper.GetIntegerFromLabel(toolpath.FillType.GetLabel());
            foreach (var printVertex in toolpath)
            {
                previewVertices[i++] = new ToolpathPreviewVertex(
                    printVertex.Position, fillTypeIndex, layerIndex,
                    color, 1, new CustomColorData(1, 1, 1));
            }
            return previewVertices;
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