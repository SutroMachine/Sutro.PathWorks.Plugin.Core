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

        private const float brightnessMax = 1f;
        private const float brightnessMin = 0.2f;
        private const double miterThreshold = 40 * MathUtil.Deg2Rad;

        public ToolpathPreviewMesh Generate(LinearToolpath3<TPrintVertex> toolpath, Func<TPrintVertex, Vector3d, float, ToolpathPreviewVertex> vertexFactory)
        {
            this.vertexFactory = vertexFactory;

            var vertices = new List<ToolpathPreviewVertex>();
            var triangles = new List<int>();

            var joints = new ToolpathPreviewJoint[toolpath.VertexCount];

            joints[0] = GenerateButtJoint(toolpath, 0, vertices, triangles);
            joints[^1] = GenerateButtJoint(toolpath, joints.Length - 1, vertices, triangles);

            for (int i = joints.Length - 2; i > 0; i--)
            {
                var segmentBeforeJoint = new Segment3d(toolpath[i - 1].Position, toolpath[i].Position);
                var segmentAfterJoint = new Segment3d(toolpath[i].Position, toolpath[i + 1].Position);

                var angleRad = SignedAngleRad(segmentAfterJoint.Direction.xy, segmentBeforeJoint.Direction.xy);

                if (CornerIsInsideTube(segmentBeforeJoint, segmentAfterJoint, toolpath[i].Dimensions.x))
                {
                    joints[i] = GenerateButtJoint(toolpath, i, vertices, triangles);
                }
                else if (Math.Abs(angleRad) > miterThreshold)
                {
                    if (angleRad < 0)
                    {
                        joints[i] = GenerateLeftBevelJoint(toolpath, i, vertices, triangles);
                    }
                    else
                    {
                        joints[i] = GenerateRightBevelJoint(toolpath, i, vertices, triangles);
                    }
                }
                else
                {
                    joints[i] = GenerateMiterJoint(toolpath, i, vertices, triangles);
                }
            }

            AddEdges(joints, triangles);

            var result = new ToolpathPreviewMesh(vertices.ToArray(), triangles.ToArray());
            return result;
        }

        private ToolpathPreviewJoint GenerateButtJoint(LinearToolpath3<TPrintVertex> toolpath, int i, List<ToolpathPreviewVertex> vertices, List<int> triangles)
        {
            var joint = new ToolpathPreviewJoint();
            // If joint is not the first point in the toolpath
            if (i > 0)
            {
                AddCapBeforeJoint(toolpath, i, vertices, joint, triangles);
            }

            // If joint is not the last point in the toolpath
            if (i < toolpath.VertexCount - 1)
            {
                AddCapAfterJoint(toolpath, i, vertices, joint, triangles);
            }

            return joint;
        }

        private void AddCapAfterJoint(LinearToolpath3<TPrintVertex> toolpath, int i, List<ToolpathPreviewVertex> vertices, ToolpathPreviewJoint joint, List<int> triangles)
        {
            var seg = new Segment3d(toolpath[i].Position, toolpath[i + 1].Position);
            var dimensions = toolpath[i].Dimensions;
            var frame = new Frame3f(new Vector3d(toolpath[i].Position));
            frame.AlignAxis(1, ToVector3f(seg.Direction));

            joint.OutTop = AddVertex(vertices, toolpath[i],
                frame.FromFrameP(new Vector3d(0, 0, 0)), brightnessMax);

            joint.OutRight = AddVertex(vertices, toolpath[i],
                frame.FromFrameP(new Vector3d(-dimensions.x / 2, 0, -dimensions.y / 2)), brightnessMin);

            joint.OutBottom = AddVertex(vertices, toolpath[i],
                frame.FromFrameP(new Vector3d(0, 0, -dimensions.y)), brightnessMax);

            joint.OutLeft = AddVertex(vertices, toolpath[i],
                frame.FromFrameP(new Vector3d(dimensions.x / 2, 0, -dimensions.y / 2)), brightnessMin);

            triangles.Add(joint.OutBottom);
            triangles.Add(joint.OutLeft);
            triangles.Add(joint.OutTop);

            triangles.Add(joint.OutBottom);
            triangles.Add(joint.OutTop);
            triangles.Add(joint.OutRight);
        }

        private void AddCapBeforeJoint(LinearToolpath3<TPrintVertex> toolpath, int i, List<ToolpathPreviewVertex> vertices, ToolpathPreviewJoint joint, List<int> triangles)
        {
            var seg = new Segment3d(toolpath[i - 1].Position, toolpath[i].Position);
            var dimensions = toolpath[i].Dimensions;
            var frame = new Frame3f(new Vector3d(toolpath[i].Position));
            frame.AlignAxis(1, ToVector3f(seg.Direction));

            joint.InTop = AddVertex(vertices, toolpath[i],
                frame.FromFrameP(new Vector3d(0, 0, 0)), brightnessMax);

            joint.InRight = AddVertex(vertices, toolpath[i],
                frame.FromFrameP(new Vector3d(-dimensions.x / 2, 0, -dimensions.y / 2)), brightnessMin);

            joint.InBottom = AddVertex(vertices, toolpath[i],
                frame.FromFrameP(new Vector3d(0, 0, -dimensions.y)), brightnessMax);

            joint.InLeft = AddVertex(vertices, toolpath[i],
                frame.FromFrameP(new Vector3d(dimensions.x / 2, 0, -dimensions.y / 2)), brightnessMin);

            triangles.Add(joint.InBottom);
            triangles.Add(joint.InTop);
            triangles.Add(joint.InLeft);

            triangles.Add(joint.InBottom);
            triangles.Add(joint.InRight);
            triangles.Add(joint.InTop);
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

        protected ToolpathPreviewJoint GenerateRightBevelJoint(LinearToolpath3<TPrintVertex> toolpath, int vertexIndex,
            List<ToolpathPreviewVertex> vertices, List<int> triangles)
        {
            var segBefore = new Segment3d(toolpath[vertexIndex - 1].Position, toolpath[vertexIndex].Position);
            var segAfter = new Segment3d(toolpath[vertexIndex].Position, toolpath[vertexIndex + 1].Position);

            var averageDirection = (segBefore.Direction + segAfter.Direction).Normalized;
            var scaleFactor = 1 / segBefore.Direction.Dot(averageDirection);

            var dimensions = new Vector2d(toolpath[vertexIndex].Dimensions.x, toolpath[vertexIndex].Dimensions.y);

            var frameMitre = new Frame3f(toolpath[vertexIndex].Position);
            frameMitre.AlignAxis(1, ToVector3f(averageDirection));

            var frameSegBefore = new Frame3f(toolpath[vertexIndex].Position);
            frameSegBefore.AlignAxis(1, ToVector3f(segBefore.Direction));

            var frameSegAfter = new Frame3f(toolpath[vertexIndex].Position);
            frameSegAfter.AlignAxis(1, ToVector3f(segAfter.Direction));


            double angle = Math.Abs(segBefore.Direction.AngleR(segAfter.Direction));

            double miterExtensions = Math.Tan(angle / 4) * dimensions.x / 2;

            var joint = new ToolpathPreviewJoint();

            joint.InTop = joint.OutTop = AddVertex(vertices, toolpath[vertexIndex],
                frameMitre.FromFrameP(new Vector3d(0, 0, 0)), brightnessMax);

            joint.InLeft = joint.OutLeft = AddVertex(vertices, toolpath[vertexIndex],
                frameMitre.FromFrameP(new Vector3d(dimensions.x / 2 * scaleFactor, 0, -dimensions.y / 2)), brightnessMin);

            joint.InBottom = joint.OutBottom = AddVertex(vertices, toolpath[vertexIndex],
                frameMitre.FromFrameP(new Vector3d(0, 0, -dimensions.y)), brightnessMax);

            joint.InRight = AddVertex(vertices, toolpath[vertexIndex],
                frameSegBefore.FromFrameP(new Vector3d(-dimensions.x / 2, miterExtensions, -dimensions.y / 2)), brightnessMin);

            joint.OutRight = AddVertex(vertices, toolpath[vertexIndex],
                frameSegAfter.FromFrameP(new Vector3d(-dimensions.x / 2, -miterExtensions, -dimensions.y / 2)), brightnessMin);

            triangles.Add(joint.InRight);
            triangles.Add(joint.InTop);
            triangles.Add(joint.OutRight);

            triangles.Add(joint.InRight);
            triangles.Add(joint.OutRight);
            triangles.Add(joint.InBottom);

            return joint;

        }

        protected ToolpathPreviewJoint GenerateLeftBevelJoint(LinearToolpath3<TPrintVertex> toolpath, int vertexIndex,
            List<ToolpathPreviewVertex> vertices, List<int> triangles)
        {
            var segBefore = new Segment3d(toolpath[vertexIndex - 1].Position, toolpath[vertexIndex].Position);
            var segAfter = new Segment3d(toolpath[vertexIndex].Position, toolpath[vertexIndex + 1].Position);

            var averageDirection = (segBefore.Direction + segAfter.Direction).Normalized;
            var scaleFactor = 1 / segBefore.Direction.Dot(averageDirection);

            var dimensions = new Vector2d(toolpath[vertexIndex].Dimensions.x, toolpath[vertexIndex].Dimensions.y);

            var frameMitre = new Frame3f(toolpath[vertexIndex].Position);
            frameMitre.AlignAxis(1, ToVector3f(averageDirection));

            var frameSegBefore = new Frame3f(toolpath[vertexIndex].Position);
            frameSegBefore.AlignAxis(1, ToVector3f(segBefore.Direction));

            var frameSegAfter = new Frame3f(toolpath[vertexIndex].Position);
            frameSegAfter.AlignAxis(1, ToVector3f(segAfter.Direction));


            double angle = segBefore.Direction.AngleR(segAfter.Direction);

            double miterExtensions = Math.Tan(angle / 4) * dimensions.x / 2; 

            var joint = new ToolpathPreviewJoint();

            joint.InTop = joint.OutTop = AddVertex(vertices, toolpath[vertexIndex],
                frameMitre.FromFrameP(new Vector3d(0, 0, 0)), brightnessMax);

            joint.InRight = joint.OutRight = AddVertex(vertices, toolpath[vertexIndex],
                frameMitre.FromFrameP(new Vector3d(-dimensions.x / 2 * scaleFactor, 0, -dimensions.y / 2)), brightnessMin);

            joint.InBottom = joint.OutBottom = AddVertex(vertices, toolpath[vertexIndex],
                frameMitre.FromFrameP(new Vector3d(0, 0, -dimensions.y)), brightnessMax);

            joint.InLeft = AddVertex(vertices, toolpath[vertexIndex],
                frameSegBefore.FromFrameP(new Vector3d(dimensions.x / 2, miterExtensions, -dimensions.y / 2)), brightnessMin);

            joint.OutLeft = AddVertex(vertices, toolpath[vertexIndex],
                frameSegAfter.FromFrameP(new Vector3d(dimensions.x / 2, -miterExtensions, -dimensions.y / 2)), brightnessMin);

            triangles.Add(joint.InLeft);
            triangles.Add(joint.OutLeft);
            triangles.Add(joint.InTop);

            triangles.Add(joint.InLeft);
            triangles.Add(joint.InBottom);
            triangles.Add(joint.OutLeft);

            return joint;
        }

        protected virtual int AddVertex(List<ToolpathPreviewVertex> vertices, TPrintVertex printVertex, Vector3d point, float brightness)
        {
            var printVertexCast = printVertex as PrintVertex;
            vertices.Add(vertexFactory(printVertex, point, brightness));
            return vertices.Count - 1;
        }

        protected virtual ToolpathPreviewJoint GenerateMiterJoint(LinearToolpath3<TPrintVertex> toolpath, int vertexIndex, List<ToolpathPreviewVertex> vertices, List<int> triangles)
        {
            var segBefore = new Segment3d(toolpath[vertexIndex - 1].Position, toolpath[vertexIndex].Position);
            var segAfter = new Segment3d(toolpath[vertexIndex].Position, toolpath[vertexIndex + 1].Position);

            var averageDirection = (segBefore.Direction + segAfter.Direction).Normalized;
            var scaleFactor = 1 / segBefore.Direction.Dot(averageDirection);
            var dimensions = new Vector2d(toolpath[vertexIndex].Dimensions.x * scaleFactor, toolpath[vertexIndex].Dimensions.y);

            var frame = new Frame3f(toolpath[vertexIndex].Position);
            frame.AlignAxis(1, ToVector3f(averageDirection));

            var joint = new ToolpathPreviewJoint();

            joint.InTop = joint.OutTop = AddVertex(vertices, toolpath[vertexIndex],
                frame.FromFrameP(new Vector3d(0, 0, 0)), brightnessMax);

            joint.InRight = joint.OutRight = AddVertex(vertices, toolpath[vertexIndex],
                frame.FromFrameP(new Vector3d(-dimensions.x / 2, 0, -dimensions.y / 2)), brightnessMin);

            joint.InBottom = joint.OutBottom = AddVertex(vertices, toolpath[vertexIndex],
                frame.FromFrameP(new Vector3d(0, 0, -dimensions.y)), brightnessMax);

            joint.InLeft = joint.OutLeft = AddVertex(vertices, toolpath[vertexIndex],
                frame.FromFrameP(new Vector3d(dimensions.x / 2, 0, -dimensions.y / 2)), brightnessMin);

            return joint;
        }

        private Vector3f ToVector3f(Vector3d vector)
        {
            return new Vector3f(vector.x, vector.y, vector.z);
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