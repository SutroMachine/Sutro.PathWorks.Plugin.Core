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

        protected int layerIndex;
        protected int pointCount;

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
        }

        private void ProcessNewLayer(int newLayerIndex)
        {
            layerIndex = newLayerIndex;
            OnNewPlane(0, newLayerIndex);
        }

        private void ProcessToolpath(IToolpath toolpath)
        {
            if (toolpath is LinearToolpath3<PrintVertex> linearToolpath)
            {
                switch (linearToolpath.Type)
                {
                    case ToolpathTypes.Travel:
                        RaiseLineGenerated(linearToolpath, layerIndex);
                        break;

                    case ToolpathTypes.PlaneChange:
                        RaiseLineGenerated(linearToolpath, layerIndex);
                        break;

                    case ToolpathTypes.Deposition:
                        Emit(linearToolpath, layerIndex, pointCount);
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

        protected virtual void RaiseLineGenerated(LinearToolpath3<PrintVertex> toolpath, int layerIndex)
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

        protected virtual void Emit(LinearToolpath3<PrintVertex> toolpath, int layerIndex, int startPointCount)
        {
            if (toolpath.VertexCount < 2)
                return;

            List<ToolpathPreviewVertex> vertices = new List<ToolpathPreviewVertex>();
            List<int> triangles = new List<int>();

            ToolpathPreviewJoint[] joints = new ToolpathPreviewJoint[toolpath.VertexCount];

            joints[joints.Length - 1] = GenerateMiterJoint(toolpath, joints.Length - 1, layerIndex, startPointCount, vertices);

            for (int i = joints.Length - 2; i > 0; i--)
            {
                Vector3d a = toolpath[i - 1].Position;
                Vector3d b = toolpath[i].Position;
                Vector3d c = toolpath[i + 1].Position;
                Vector3d ab = b - a;
                Vector3d bc = c - b;
                var angleRad = SignedAngleRad(ab.xy, bc.xy);
                if (Math.Abs(angleRad) > 0.698132)
                {
                    if (angleRad < 0)
                    {
                        joints[i] = GenerateRightBevelJoint(toolpath, i, layerIndex, startPointCount, vertices, triangles);
                    }
                    else
                    {
                        joints[i] = GenerateLeftBevelJoint(toolpath, i, layerIndex, startPointCount, vertices, triangles);
                    }
                }
                else
                {
                    joints[i] = GenerateMiterJoint(toolpath, i, layerIndex, startPointCount, vertices);
                }
            }

            joints[0] = GenerateMiterJoint(toolpath, 0, layerIndex, startPointCount, vertices);

            AddEdges(joints, triangles);

            var mesh = new Tuple<ToolpathPreviewVertex[], int[]>(vertices.ToArray(), triangles.ToArray());

            OnPointsGenerated?.Invoke(CreatePoints(toolpath), layerIndex);

            pointCount += mesh.Item1.Length;

            EndEmit(mesh, layerIndex);
        }

        private ToolpathPreviewVertex[] CreatePoints(LinearToolpath3<PrintVertex> toolpath)
        {
            var previewVertices = new ToolpathPreviewVertex[toolpath.VertexCount];

            int i = 0;
            foreach (var printVertex in toolpath)
            {
                int fillTypeIndex = GetFillTypeInteger(printVertex);
                var color = GetColor(fillTypeIndex);

                previewVertices[i++] = new ToolpathPreviewVertex(
                    printVertex.Position, fillTypeIndex, layerIndex,
                    color, 1, new CustomColorData(1, 1, 1));
            }
            return previewVertices;
        }

        protected virtual void AddEdges(ToolpathPreviewJoint[] joints, List<int> triangles)
        {
            for (int i = joints.Length - 2; i >= 0; i--)
            {
                var start = joints[i];
                var end = joints[i + 1];

                triangles.Add(start.out0);
                triangles.Add(start.out1);
                triangles.Add(end.in0);

                triangles.Add(end.in0);
                triangles.Add(start.out1);
                triangles.Add(end.in1);

                triangles.Add(start.out1);
                triangles.Add(start.out2);
                triangles.Add(end.in1);

                triangles.Add(end.in1);
                triangles.Add(start.out2);
                triangles.Add(end.in2);

                triangles.Add(start.out2);
                triangles.Add(start.out3);
                triangles.Add(end.in2);

                triangles.Add(end.in2);
                triangles.Add(start.out3);
                triangles.Add(end.in3);

                triangles.Add(start.out3);
                triangles.Add(start.out0);
                triangles.Add(end.in3);

                triangles.Add(end.in3);
                triangles.Add(start.out0);
                triangles.Add(end.in0);
            }
        }

        protected virtual ToolpathPreviewJoint GenerateMiterJoint(LinearToolpath3<PrintVertex> toolpath, int toolpathIndex, int layerIndex, int startPointCount, List<ToolpathPreviewVertex> vertices)
        {
            double miterSecant = 1;
            Vector3d miterNormal;

            if (toolpathIndex == 0)
            {
                Vector3d miterTangent = toolpath[1].Position - toolpath[0].Position;
                miterNormal = new Vector3d(-miterTangent.y, miterTangent.x, 0).Normalized;
            }
            else if (toolpathIndex == toolpath.VertexCount - 1)
            {
                Vector3d miterTangent = toolpath[toolpathIndex].Position - toolpath[toolpathIndex - 1].Position;
                miterNormal = new Vector3d(-miterTangent.y, miterTangent.x, 0).Normalized;
            }
            else
            {
                Vector3d a = toolpath[toolpathIndex - 1].Position;
                Vector3d b = toolpath[toolpathIndex].Position;
                Vector3d c = toolpath[toolpathIndex + 1].Position;
                Vector3d ab = (b - a).Normalized;
                Vector3d bc = (c - b).Normalized;
                miterNormal = GetNormalAndSecant(ab, bc, out miterSecant);
            }

            var pointCount = startPointCount + toolpathIndex;
            var point = toolpath[toolpathIndex].Position;
            var dimensions = toolpath[toolpathIndex].Dimensions;

            int fillType = GetFillTypeInteger(toolpath[toolpathIndex]);

            var feedRate = toolpath[toolpathIndex].FeedRate;
            ToolpathPreviewJoint joint = new ToolpathPreviewJoint();

            joint.in0 = joint.out0 = AddVertex(vertices, layerIndex, point, fillType, dimensions, feedRate, miterNormal, new Vector2d(0.5f, -0.5f), miterSecant, 0, pointCount);
            joint.in1 = joint.out1 = AddVertex(vertices, layerIndex, point, fillType, dimensions, feedRate, miterNormal, new Vector2d(0, 0), miterSecant, 1, pointCount);
            joint.in2 = joint.out2 = AddVertex(vertices, layerIndex, point, fillType, dimensions, feedRate, miterNormal, new Vector2d(-0.5f, -0.5f), miterSecant, 0, pointCount);
            joint.in3 = joint.out3 = AddVertex(vertices, layerIndex, point, fillType, dimensions, feedRate, miterNormal, new Vector2d(0, -1), miterSecant, 1, pointCount);

            return joint;
        }

        private static int GetFillTypeInteger(PrintVertex vertex)
        {
            if (vertex.Source is string s && FillTypeIntegerId.TryGetValue(s, out int newFillType))
            {
                return newFillType;
            }
            else
            {
                return FillTypeIntegerId[DefaultFillType.Label];
            }
        }

        protected ToolpathPreviewJoint GenerateRightBevelJoint(LinearToolpath3<PrintVertex> toolpath, int toolpathIndex,
            int layerIndex, int startPointCount, List<ToolpathPreviewVertex> vertices, List<int> triangles)
        {
            Vector3d a = toolpath[toolpathIndex - 1].Position;
            Vector3d b = toolpath[toolpathIndex].Position;
            Vector3d c = toolpath[toolpathIndex + 1].Position;
            Vector3d ab = (b - a).Normalized;
            Vector3d bc = (c - b).Normalized;
            Vector3d miterNormal = GetNormalAndSecant(ab, bc, out double miterSecant);
            Vector3d miterTangent = ab + bc;

            var pointCount = startPointCount + toolpathIndex;
            var point = toolpath[toolpathIndex].Position;
            var dimensions = toolpath[toolpathIndex].Dimensions;

            var fillType = GetFillTypeInteger(toolpath[toolpathIndex]);

            var feedRate = toolpath[toolpathIndex].FeedRate;
            ToolpathPreviewJoint joint = new ToolpathPreviewJoint();

            var bevelNormalIn = GetNormalAndSecant(ab, miterTangent, out double bevelSecantIn);
            joint.in0 = AddVertex(vertices, layerIndex, point, fillType, dimensions, feedRate, bevelNormalIn, new Vector2d(0.5, -0.5), bevelSecantIn, 0, pointCount);

            var bevelNormalOut = GetNormalAndSecant(miterTangent, bc, out double bevelSecantOut);
            joint.out0 = AddVertex(vertices, layerIndex, point, fillType, dimensions, feedRate, bevelNormalOut, new Vector2d(0.5, -0.5), bevelSecantOut, 0, pointCount);

            joint.in1 = joint.out1 = AddVertex(vertices, layerIndex, point, fillType, dimensions, feedRate, miterNormal, new Vector2d(0, 0), miterSecant, 1, pointCount);
            joint.in2 = joint.out2 = AddVertex(vertices, layerIndex, point, fillType, dimensions, feedRate, miterNormal, new Vector2d(-0.5, -0.5), miterSecant, 0, pointCount);
            joint.in3 = joint.out3 = AddVertex(vertices, layerIndex, point, fillType, dimensions, feedRate, miterNormal, new Vector2d(0, -1), miterSecant, 1, pointCount);

            triangles.Add(joint.in0);
            triangles.Add(joint.in1);
            triangles.Add(joint.out0);

            triangles.Add(joint.in0);
            triangles.Add(joint.out0);
            triangles.Add(joint.in3);

            return joint;
        }

        protected ToolpathPreviewJoint GenerateLeftBevelJoint(LinearToolpath3<PrintVertex> toolpath, int toolpathIndex,
            int layerIndex, int startPointCount, List<ToolpathPreviewVertex> vertices, List<int> triangles)
        {
            Vector3d a = toolpath[toolpathIndex - 1].Position;
            Vector3d b = toolpath[toolpathIndex].Position;
            Vector3d c = toolpath[toolpathIndex + 1].Position;
            Vector3d ab = (b - a).Normalized;
            Vector3d bc = (c - b).Normalized;
            Vector3d miterNormal = GetNormalAndSecant(ab, bc, out double miterSecant);
            Vector3d miterTangent = ab + bc;

            var pointCount = startPointCount + toolpathIndex;
            var point = toolpath[toolpathIndex].Position;
            var dimensions = toolpath[toolpathIndex].Dimensions;

            var fillType = GetFillTypeInteger(toolpath[toolpathIndex]);

            var feedRate = toolpath[toolpathIndex].FeedRate;
            ToolpathPreviewJoint joint = new ToolpathPreviewJoint();

            joint.in0 = joint.out0 = AddVertex(vertices, layerIndex, point, fillType, dimensions, feedRate, miterNormal, new Vector2d(0.5f, -0.5f), miterSecant, 0, pointCount);
            joint.in1 = joint.out1 = AddVertex(vertices, layerIndex, point, fillType, dimensions, feedRate, miterNormal, new Vector2d(0, 0), miterSecant, 1, pointCount);

            var bevelNormalIn = GetNormalAndSecant(ab, miterTangent, out double bevelSecantIn);
            joint.in2 = AddVertex(vertices, layerIndex, point, fillType, dimensions, feedRate, bevelNormalIn, new Vector2d(-0.5f, -0.5f), bevelSecantIn, 0, pointCount);

            var bevelNormalOut = GetNormalAndSecant(miterTangent, bc, out double bevelSecantOut);
            joint.out2 = AddVertex(vertices, layerIndex, point, fillType, dimensions, feedRate, bevelNormalOut, new Vector2d(-0.5f, -0.5f), bevelSecantOut, 0, pointCount);

            joint.in3 = joint.out3 = AddVertex(vertices, layerIndex, point, fillType, dimensions, feedRate, miterNormal, new Vector2d(0, -1), miterSecant, 1, pointCount);

            triangles.Add(joint.in2);
            triangles.Add(joint.in3);
            triangles.Add(joint.out2);

            triangles.Add(joint.in2);
            triangles.Add(joint.out2);
            triangles.Add(joint.in1);

            return joint;
        }

        protected virtual int AddVertex(List<ToolpathPreviewVertex> vertices, int layerIndex, Vector3d point,
            int fillType, Vector2d dimensions, double feedrate, Vector3d miterNormal,
            Vector2d crossSectionVertex, double secant, float brightness, int pointCount)
        {
            Vector3d offset = miterNormal * (dimensions.x * crossSectionVertex.x * secant) + new Vector3d(0, 0, dimensions.y * crossSectionVertex.y);
            Vector3d vertex = point + offset;

            var color = GetColor(fillType);

            vertices.Add(VertexFactory(layerIndex, fillType, dimensions, feedrate, brightness, pointCount, vertex, color));

            return vertices.Count - 1;
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

        protected virtual ToolpathPreviewVertex VertexFactory(int layerIndex, int fillType, Vector2d dimensions, double feedrate, float brightness, int pointCount, Vector3d vertex, Vector3f color)
        {
            // Update adaptive ranges for custom data
            customDataFeedRate.ObserveValue((float)feedrate);
            customDataCompletion.ObserveValue(pointCount);

            return new ToolpathPreviewVertex(
                vertex, fillType, layerIndex, color, brightness,
                new CustomColorData(dimensions.x, feedrate, pointCount));
        }

        protected static Vector3d GetNormalAndSecant(Vector3d ab, Vector3d bc, out double secant)
        {
            secant = 1 / Math.Cos(Vector3d.AngleR(ab, bc) * 0.5);
            Vector3d tangent = ab + bc;
            return new Vector3d(-tangent.y, tangent.x, 0).Normalized;
        }

        protected static double SignedAngleRad(Vector2d a, Vector2d b)
        {
            var angleB = Math.Atan2(b.y, b.x);
            var angleA = Math.Atan2(a.y, a.x);
            var ret = angleB - angleA;
            if (ret > Math.PI)
                ret -= Math.PI * 2;
            if (ret < -Math.PI)
                ret += Math.PI * 2;
            return ret;
        }

        protected struct ToolpathPreviewJoint
        {
            public int in0;
            public int in1;
            public int in2;
            public int in3;

            public int out0;
            public int out1;
            public int out2;
            public int out3;
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