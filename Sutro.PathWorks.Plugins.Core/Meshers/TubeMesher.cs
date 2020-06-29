using g3;
using gs;
using Sutro.PathWorks.Plugins.API.Visualizers;
using System;
using System.Collections.Generic;

namespace Sutro.PathWorks.Plugins.Core.Meshers
{
    public partial class TubeMesher<TPrintVertex> : IToolpathPreviewMesher<TPrintVertex> where TPrintVertex : IExtrusionVertex
    {
        private Func<TPrintVertex, Vector3d, float, ToolpathPreviewVertex> vertexFactory;

        public ToolpathPreviewMesh Generate(LinearToolpath3<TPrintVertex> toolpath, Func<TPrintVertex, Vector3d, float, ToolpathPreviewVertex> vertexFactory)
        {
            this.vertexFactory = vertexFactory;

            var vertices = new List<ToolpathPreviewVertex>();
            var triangles = new List<int>();

            var joints = new ToolpathPreviewJoint[toolpath.VertexCount];

            joints[0] = GenerateButtJoint(toolpath, 0, vertices);
            joints[^1] = GenerateButtJoint(toolpath, joints.Length - 1, vertices);

            for (int i = joints.Length - 2; i > 0; i--)
            {
                var segmentBeforeJoint = new Segment3d(toolpath[i - 1].Position, toolpath[i].Position);
                var segmentAfterJoint = new Segment3d(toolpath[i].Position, toolpath[i + 1].Position);

                var angleRad = SignedAngleRad(segmentAfterJoint.Direction.xy, segmentBeforeJoint.Direction.xy);

                if (CornerIsInsideTube(segmentBeforeJoint, segmentAfterJoint, toolpath[i].Dimensions.x))
                {
                    joints[i] = GenerateButtJoint(toolpath, i, vertices);
                }
                else if (Math.Abs(angleRad) > 0.698132)
                {
                    if (angleRad < 0)
                    {
                        joints[i] = GenerateButtJoint(toolpath, i, vertices);
                    }
                    else
                    {
                        joints[i] = GenerateButtJoint(toolpath, i, vertices);
                    }
                }
                else
                {
                    joints[i] = GenerateButtJoint(toolpath, i, vertices);
                }
            }


            AddEdges(joints, triangles);

            var result = new ToolpathPreviewMesh(vertices.ToArray(), triangles.ToArray());
            return result;
        }

        private ToolpathPreviewJoint GenerateButtJoint(LinearToolpath3<TPrintVertex> toolpath, int i, List<ToolpathPreviewVertex> vertices)
        {
            var joint = new ToolpathPreviewJoint();
            // If joint is not the first point in the toolpath
            if (i > 0)
            {
                AddCapBeforeJoint(toolpath, i, vertices, joint);
            }

            // If joint is not the last point in the toolpath
            if (i < toolpath.VertexCount - 1)
            {
                AddCapAfterJoint(toolpath, i, vertices, joint);
            }

            return joint;
        }

        private void AddCapAfterJoint(LinearToolpath3<TPrintVertex> toolpath, int i, List<ToolpathPreviewVertex> vertices, ToolpathPreviewJoint joint)
        {
            var seg = new Segment3d(toolpath[i].Position, toolpath[i + 1].Position);
            var dimensions = toolpath[i].Dimensions;
            var frame = new Frame3f(new Vector3d(toolpath[i].Position));
            frame.AlignAxis(1, new Vector3f((float)seg.Direction.x, (float)seg.Direction.y, (float)seg.Direction.z));

            joint.OutTop = AddVertex(vertices, toolpath[i], 
                frame.FromFrameP(new Vector3d(0, 0, 0)), 1);

            joint.OutRight = AddVertex(vertices, toolpath[i], 
                frame.FromFrameP(new Vector3d(-dimensions.x / 2, 0, -dimensions.y / 2)), 0.5f);

            joint.OutBottom = AddVertex(vertices, toolpath[i], 
                frame.FromFrameP(new Vector3d(0, 0, -dimensions.y)), 1);

            joint.OutLeft = AddVertex(vertices, toolpath[i], 
                frame.FromFrameP(new Vector3d(dimensions.x / 2, 0, -dimensions.y / 2)), 0.5f);
        }

        private void AddCapBeforeJoint(LinearToolpath3<TPrintVertex> toolpath, int i, List<ToolpathPreviewVertex> vertices, ToolpathPreviewJoint joint)
        {
            var seg = new Segment3d(toolpath[i - 1].Position, toolpath[i].Position);
            var dimensions = toolpath[i].Dimensions;
            var frame = new Frame3f(new Vector3d(toolpath[i].Position));
            frame.AlignAxis(1, new Vector3f((float)seg.Direction.x, (float)seg.Direction.y, (float)seg.Direction.z));

            var v1 = frame.FromFrameP(new Vector3d(0, 0, 0));
            joint.InTop = AddVertex(vertices, toolpath[i], v1, 1);

            var v2 = frame.FromFrameP(new Vector3d(-dimensions.x / 2, 0, -dimensions.y / 2));
            joint.InRight = AddVertex(vertices, toolpath[i], v2, 0.5f);

            var v3 = frame.FromFrameP(new Vector3d(0, 0, -dimensions.y));
            joint.InBottom = AddVertex(vertices, toolpath[i], v3, 1);

            var v4 = frame.FromFrameP(new Vector3d(dimensions.x / 2, 0, -dimensions.y / 2));
            joint.InLeft = AddVertex(vertices, toolpath[i], v4, 0.5f);
        }

        private static bool CornerIsInsideTube(Segment3d segmentBeforeJoint, Segment3d segmentAfterJoint, double tubeWidth)
        {
            var angle = Math.PI - segmentBeforeJoint.Direction.AngleR(segmentAfterJoint.Direction, true);
            var minimumLength = (tubeWidth / 2d) / Math.Tan(angle / 2d);
            return segmentAfterJoint.Length < minimumLength;
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
            throw new NotImplementedException();
            Vector3d a = toolpath[toolpathIndex - 1].Position;
            Vector3d b = toolpath[toolpathIndex].Position;
            Vector3d c = toolpath[toolpathIndex + 1].Position;
            Vector3d ab = (b - a).Normalized;
            Vector3d bc = (c - b).Normalized;
            Vector3d miterNormal = GetNormalAndSecant(ab, bc, out double miterSecant);
            Vector3d miterTangent = ab + bc;

            var point = toolpath[toolpathIndex].Position;

            ToolpathPreviewJoint joint = new ToolpathPreviewJoint();

            //joint.InRight = joint.OutRight = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(0.5f, -0.5f), miterSecant, 0f);
            //joint.InTop = joint.OutTop = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(0, 0), miterSecant, 1);

            //var bevelNormalIn = GetNormalAndSecant(ab, miterTangent, out double bevelSecantIn);
            //joint.InLeft = AddVertex(vertices, toolpath[toolpathIndex], point, bevelNormalIn, new Vector2d(-0.5f, -0.5f), bevelSecantIn, 0);

            //var bevelNormalOut = GetNormalAndSecant(miterTangent, bc, out double bevelSecantOut);
            //joint.OutLeft = AddVertex(vertices, toolpath[toolpathIndex], point, bevelNormalOut, new Vector2d(-0.5f, -0.5f), bevelSecantOut, 0);

            //joint.InBottom = joint.OutBotttom = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(0, -1), miterSecant, 1);

            triangles.Add(joint.InLeft);
            triangles.Add(joint.InBottom);
            triangles.Add(joint.OutLeft);

            triangles.Add(joint.InLeft);
            triangles.Add(joint.OutLeft);
            triangles.Add(joint.InTop);

            return joint;
        }

        protected ToolpathPreviewJoint GenerateRightBevelJoint(LinearToolpath3<TPrintVertex> toolpath, int toolpathIndex,
            List<ToolpathPreviewVertex> vertices, List<int> triangles)
        {
            throw new NotImplementedException();
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
            //joint.InRight = AddVertex(vertices, toolpath[toolpathIndex], point, bevelNormalIn, new Vector2d(0.5, -0.5), bevelSecantIn, 0);

            //var bevelNormalOut = GetNormalAndSecant(miterTangent, bc, out double bevelSecantOut);
            //joint.OutRight = AddVertex(vertices, toolpath[toolpathIndex], point, bevelNormalOut, new Vector2d(0.5, -0.5), bevelSecantOut, 0);

            //joint.InTop = joint.OutTop = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(0, 0), miterSecant, 1);
            //joint.InLeft = joint.OutLeft = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(-0.5, -0.5), miterSecant, 0);
            //joint.InBottom = joint.OutBotttom = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(0, -1), miterSecant, 1);

            triangles.Add(joint.InRight);
            triangles.Add(joint.InTop);
            triangles.Add(joint.OutRight);

            triangles.Add(joint.InRight);
            triangles.Add(joint.OutRight);
            triangles.Add(joint.InBottom);

            return joint;
        }

        protected virtual int AddVertex(List<ToolpathPreviewVertex> vertices, TPrintVertex printVertex, Vector3d point, float brightness)
        {
            var printVertexCast = printVertex as PrintVertex;
            vertices.Add(vertexFactory(printVertex, point, brightness));
            return vertices.Count - 1;
        }

        protected virtual ToolpathPreviewJoint GenerateMiterJoint(LinearToolpath3<TPrintVertex> toolpath, int toolpathIndex, int startPointCount, List<ToolpathPreviewVertex> vertices)
        {
            throw new NotImplementedException();
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

            //joint.InRight = joint.OutRight = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(0.5f, -0.5f), miterSecant, 0);
            //joint.InTop = joint.OutTop = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(0, 0), miterSecant, 1);
            //joint.InLeft = joint.OutLeft = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(-0.5f, -0.5f), miterSecant, 0);
            //joint.InBottom = joint.OutBotttom = AddVertex(vertices, toolpath[toolpathIndex], point, miterNormal, new Vector2d(0, -1), miterSecant, 1);

            return joint;
        }

        protected virtual void AddEdges(ToolpathPreviewJoint[] joints, List<int> triangles)
        {
            for (int i = joints.Length - 2; i >= 0; i--)
            {
                var start = joints[i];
                var end = joints[i + 1];

                triangles.Add(start.OutRight);
                triangles.Add(start.OutTop);
                triangles.Add(end.InRight);

                triangles.Add(end.InRight);
                triangles.Add(start.OutTop);
                triangles.Add(end.InTop);

                triangles.Add(start.OutTop);
                triangles.Add(start.OutLeft);
                triangles.Add(end.InTop);

                triangles.Add(end.InTop);
                triangles.Add(start.OutLeft);
                triangles.Add(end.InLeft);

                triangles.Add(start.OutLeft);
                triangles.Add(start.OutBottom);
                triangles.Add(end.InLeft);

                triangles.Add(end.InLeft);
                triangles.Add(start.OutBottom);
                triangles.Add(end.InBottom);

                triangles.Add(start.OutBottom);
                triangles.Add(start.OutRight);
                triangles.Add(end.InBottom);

                triangles.Add(end.InBottom);
                triangles.Add(start.OutRight);
                triangles.Add(end.InRight);
            }
        }
    }
}