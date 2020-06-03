using g3;
using gs;
using Sutro.PathWorks.Plugins.API.Visualizers;
using System;
using System.Collections.Generic;

namespace Sutro.PathWorks.Plugins.Core.Visualizers
{
    public class TubeMesher<TPrintVertex> : IToolpathPreviewMesher<TPrintVertex> where TPrintVertex : IToolpathVertex
    {
        private Func<TPrintVertex, Vector3d, float, ToolpathPreviewVertex> vertexFactory;

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

        public ToolpathPreviewMesh Generate(LinearToolpath3<TPrintVertex> toolpath, Func<TPrintVertex, Vector3d, float, ToolpathPreviewVertex> vertexFactory)
        {
            this.vertexFactory = vertexFactory;

            List<ToolpathPreviewVertex> vertices = new List<ToolpathPreviewVertex>();
            List<int> triangles = new List<int>();

            ToolpathPreviewJoint[] joints = new ToolpathPreviewJoint[toolpath.VertexCount];

            joints[joints.Length - 1] = GenerateMiterJoint(toolpath, joints.Length - 1, 0, vertices);

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
                        joints[i] = GenerateRightBevelJoint(toolpath, i, vertices, triangles);
                    }
                    else
                    {
                        joints[i] = GenerateLeftBevelJoint(toolpath, i, vertices, triangles);
                    }
                }
                else
                {
                    joints[i] = GenerateMiterJoint(toolpath, i, 0, vertices);
                }
            }

            joints[0] = GenerateMiterJoint(toolpath, 0, 0, vertices);

            AddEdges(joints, triangles);

            var result = new ToolpathPreviewMesh(vertices.ToArray(), triangles.ToArray());
            return result;
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

        protected ToolpathPreviewJoint GenerateLeftBevelJoint(LinearToolpath3<TPrintVertex> toolpath, int toolpathIndex,
            List<ToolpathPreviewVertex> vertices, List<int> triangles)
        {
            Vector3d a = toolpath[toolpathIndex - 1].Position;
            Vector3d b = toolpath[toolpathIndex].Position;
            Vector3d c = toolpath[toolpathIndex + 1].Position;
            Vector3d ab = (b - a).Normalized;
            Vector3d bc = (c - b).Normalized;
            Vector3d miterNormal = GetNormalAndSecant(ab, bc, out double miterSecant);
            Vector3d miterTangent = ab + bc;

            var point = toolpath[toolpathIndex].Position;

            ToolpathPreviewJoint joint = new ToolpathPreviewJoint();

            joint.in0 = joint.out0 = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(0.5f, -0.5f), miterSecant, 0f);
            joint.in1 = joint.out1 = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(0, 0), miterSecant, 1);

            var bevelNormalIn = GetNormalAndSecant(ab, miterTangent, out double bevelSecantIn);
            joint.in2 = AddVertex(vertices, toolpath[toolpathIndex], point, bevelNormalIn, new Vector2d(-0.5f, -0.5f), bevelSecantIn, 0);

            var bevelNormalOut = GetNormalAndSecant(miterTangent, bc, out double bevelSecantOut);
            joint.out2 = AddVertex(vertices, toolpath[toolpathIndex], point, bevelNormalOut, new Vector2d(-0.5f, -0.5f), bevelSecantOut, 0);

            joint.in3 = joint.out3 = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(0, -1), miterSecant, 1);

            triangles.Add(joint.in2);
            triangles.Add(joint.in3);
            triangles.Add(joint.out2);

            triangles.Add(joint.in2);
            triangles.Add(joint.out2);
            triangles.Add(joint.in1);

            return joint;
        }

        protected ToolpathPreviewJoint GenerateRightBevelJoint(LinearToolpath3<TPrintVertex> toolpath, int toolpathIndex,
            List<ToolpathPreviewVertex> vertices, List<int> triangles)
        {
            Vector3d a = toolpath[toolpathIndex - 1].Position;
            Vector3d b = toolpath[toolpathIndex].Position;
            Vector3d c = toolpath[toolpathIndex + 1].Position;
            Vector3d ab = (b - a).Normalized;
            Vector3d bc = (c - b).Normalized;
            Vector3d miterNormal = GetNormalAndSecant(ab, bc, out double miterSecant);
            Vector3d miterTangent = ab + bc;

            var point = toolpath[toolpathIndex].Position;

            ToolpathPreviewJoint joint = new ToolpathPreviewJoint();

            var bevelNormalIn = GetNormalAndSecant(ab, miterTangent, out double bevelSecantIn);
            joint.in0 = AddVertex(vertices, toolpath[toolpathIndex], point, bevelNormalIn, new Vector2d(0.5, -0.5), bevelSecantIn, 0);

            var bevelNormalOut = GetNormalAndSecant(miterTangent, bc, out double bevelSecantOut);
            joint.out0 = AddVertex(vertices, toolpath[toolpathIndex], point, bevelNormalOut, new Vector2d(0.5, -0.5), bevelSecantOut, 0);

            joint.in1 = joint.out1 = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(0, 0), miterSecant, 1);
            joint.in2 = joint.out2 = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(-0.5, -0.5), miterSecant, 0);
            joint.in3 = joint.out3 = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(0, -1), miterSecant, 1);

            triangles.Add(joint.in0);
            triangles.Add(joint.in1);
            triangles.Add(joint.out0);

            triangles.Add(joint.in0);
            triangles.Add(joint.out0);
            triangles.Add(joint.in3);

            return joint;
        }

        protected virtual int AddVertex(List<ToolpathPreviewVertex> vertices, TPrintVertex printVertex, Vector3d point,
            Vector3d miterNormal, Vector2d crossSectionVertex, double secant, float brightness)
        {
            // TODO: Remove need for cast
            var printVertexCast = printVertex as PrintVertex;

            Vector3d offset = miterNormal * (printVertexCast.Dimensions.x * crossSectionVertex.x * secant) + new Vector3d(0, 0, printVertexCast.Dimensions.y * crossSectionVertex.y);
            Vector3d vertex = point + offset;

            var color = Vector3f.AxisX; // TODO fix this TI GetColor(fillType);

            vertices.Add(vertexFactory(printVertex, vertex, brightness));

            return vertices.Count - 1;
        }

        protected virtual ToolpathPreviewJoint GenerateMiterJoint(LinearToolpath3<TPrintVertex> toolpath, int toolpathIndex, int startPointCount, List<ToolpathPreviewVertex> vertices)
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

            ToolpathPreviewJoint joint = new ToolpathPreviewJoint();

            joint.in0 = joint.out0 = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(0.5f, -0.5f), miterSecant, 0);
            joint.in1 = joint.out1 = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(0, 0), miterSecant, 1);
            joint.in2 = joint.out2 = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(-0.5f, -0.5f), miterSecant, 0);
            joint.in3 = joint.out3 = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(0, -1), miterSecant, 1);

            return joint;
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
    }
}