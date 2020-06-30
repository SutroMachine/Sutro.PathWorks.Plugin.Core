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

            var mesh = new ToolpathPreviewMesh();

            var joints = new ToolpathPreviewJoint[toolpath.VertexCount];

            joints[0] = GenerateButtJoint(toolpath, 0, mesh);
            joints[^1] = GenerateButtJoint(toolpath, joints.Length - 1, mesh);

            for (int i = 1; i < toolpath.VertexCount - 1; i++)
            {
                var segmentBeforeJoint = new Segment3d(toolpath[i - 1].Position, toolpath[i].Position);
                var segmentAfterJoint = new Segment3d(toolpath[i].Position, toolpath[i + 1].Position);

                var angleRad = SignedAngleRad(segmentBeforeJoint.Direction.xy, segmentAfterJoint.Direction.xy);

                if (Math.Abs(angleRad) > miterThreshold)
                {
                        joints[i] = GenerateBevelJoint(toolpath, i, mesh, angleRad);
                }
                else
                {
                    joints[i] = GenerateMiterJoint(toolpath, i, mesh);
                }
            }

            AddEdges(joints, mesh);

            return mesh;
        }

        private ToolpathPreviewJoint GenerateButtJoint(LinearToolpath3<TPrintVertex> toolpath, int i, ToolpathPreviewMesh mesh)
        {
            var joint = new ToolpathPreviewJoint();
            // If joint is not the first point in the toolpath
            if (i > 0)
            {
                AddCapBeforeJoint(toolpath, i, joint, mesh);
            }

            // If joint is not the last point in the toolpath
            if (i < toolpath.VertexCount - 1)
            {
                AddCapAfterJoint(toolpath, i, joint, mesh);
            }

            return joint;
        }

        private void AddCapAfterJoint(LinearToolpath3<TPrintVertex> toolpath, int i, ToolpathPreviewJoint joint, ToolpathPreviewMesh mesh)
        {
            var seg = new Segment3d(toolpath[i].Position, toolpath[i + 1].Position);
            var dimensions = toolpath[i].Dimensions;
            var frame = new Frame3f(new Vector3d(toolpath[i].Position));
            frame.AlignAxis(1, ToVector3f(seg.Direction));

            joint.OutTop = mesh.AddVertex(vertexFactory(toolpath[i],
                frame.FromFrameP(new Vector3d(0, 0, 0)), brightnessMax));

            joint.OutRight = mesh.AddVertex(vertexFactory(toolpath[i],
                frame.FromFrameP(new Vector3d(dimensions.x / 2, 0, -dimensions.y / 2)), brightnessMin));

            joint.OutBottom = mesh.AddVertex(vertexFactory(toolpath[i],
                frame.FromFrameP(new Vector3d(0, 0, -dimensions.y)), brightnessMax));

            joint.OutLeft = mesh.AddVertex(vertexFactory(toolpath[i],
                frame.FromFrameP(new Vector3d(-dimensions.x / 2, 0, -dimensions.y / 2)), brightnessMin));

            mesh.AddTriangle(joint.OutBottom, joint.OutTop, joint.OutLeft);
            mesh.AddTriangle(joint.OutBottom, joint.OutRight, joint.OutTop);
        }

        private void AddCapBeforeJoint(LinearToolpath3<TPrintVertex> toolpath, int i, ToolpathPreviewJoint joint, ToolpathPreviewMesh mesh)
        {
            var seg = new Segment3d(toolpath[i - 1].Position, toolpath[i].Position);
            var dimensions = toolpath[i].Dimensions;
            var frame = new Frame3f(new Vector3d(toolpath[i].Position));
            frame.AlignAxis(1, ToVector3f(seg.Direction));

            joint.InTop = mesh.AddVertex(vertexFactory(toolpath[i],
                frame.FromFrameP(new Vector3d(0, 0, 0)), brightnessMax));

            joint.InRight = mesh.AddVertex(vertexFactory(toolpath[i],
                frame.FromFrameP(new Vector3d(dimensions.x / 2, 0, -dimensions.y / 2)), brightnessMin));

            joint.InBottom = mesh.AddVertex(vertexFactory(toolpath[i],
                frame.FromFrameP(new Vector3d(0, 0, -dimensions.y)), brightnessMax));

            joint.InLeft = mesh.AddVertex(vertexFactory(toolpath[i],
                frame.FromFrameP(new Vector3d(-dimensions.x / 2, 0, -dimensions.y / 2)), brightnessMin));

            mesh.AddTriangle(joint.InBottom, joint.InLeft, joint.InTop);
            mesh.AddTriangle(joint.InBottom, joint.InTop, joint.InRight);
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

        protected ToolpathPreviewJoint GenerateBevelJoint(LinearToolpath3<TPrintVertex> toolpath, int vertexIndex,
            ToolpathPreviewMesh mesh, double angle)
        {
            var segBefore = new Segment3d(toolpath[vertexIndex - 1].Position, toolpath[vertexIndex].Position);
            var segAfter = new Segment3d(toolpath[vertexIndex].Position, toolpath[vertexIndex + 1].Position);

            var averageDirection = (segBefore.Direction + segAfter.Direction).Normalized;
            var scaleFactor = 1 / segBefore.Direction.Dot(averageDirection);

            var dimensions = new Vector2d(toolpath[vertexIndex].Dimensions.x, toolpath[vertexIndex].Dimensions.y);

            var frameMiter = new Frame3f(toolpath[vertexIndex].Position);
            frameMiter.AlignAxis(1, ToVector3f(averageDirection));

            var frameSegBefore = new Frame3f(toolpath[vertexIndex].Position);
            frameSegBefore.AlignAxis(1, ToVector3f(segBefore.Direction));

            var frameSegAfter = new Frame3f(toolpath[vertexIndex].Position);
            frameSegAfter.AlignAxis(1, ToVector3f(segAfter.Direction));

            double angle2 = Math.Abs(segBefore.Direction.AngleR(segAfter.Direction));

            double miterExtensions = Math.Tan(angle2 / 4) * dimensions.x / 2;

            var printVertex = toolpath[vertexIndex];

            if (angle > 0)
                return GenerateRightBevel(mesh, segBefore, segAfter, scaleFactor, dimensions, frameMiter, frameSegBefore, frameSegAfter, miterExtensions, printVertex);
            else
                return GenerateLeftBevel(printVertex, mesh, segBefore, segAfter, scaleFactor, dimensions, frameMiter, frameSegBefore, frameSegAfter, miterExtensions);

        }

        private ToolpathPreviewJoint GenerateRightBevel(ToolpathPreviewMesh mesh, Segment3d segBefore, Segment3d segAfter, double scaleFactor, Vector2d dimensions, Frame3f frameMitre, Frame3f frameSegBefore, Frame3f frameSegAfter, double miterExtensions, TPrintVertex printVertex)
        {
            var joint = new ToolpathPreviewJoint();

            joint.InTop = joint.OutTop = mesh.AddVertex(vertexFactory(printVertex,
                frameMitre.FromFrameP(new Vector3d(0, 0, 0)), brightnessMax));

            if (CornerIsInsideTube(segBefore, segAfter, dimensions.x))
            {
                joint.InLeft = mesh.AddVertex(vertexFactory(printVertex,
                    frameSegBefore.FromFrameP(new Vector3d(-dimensions.x / 2, 0, -dimensions.y / 2)), brightnessMin));

                joint.OutLeft = mesh.AddVertex(vertexFactory(printVertex,
                    frameSegAfter.FromFrameP(new Vector3d(-dimensions.x / 2, 0, -dimensions.y / 2)), brightnessMin));
            }
            else
            {
                joint.InLeft = joint.OutLeft = mesh.AddVertex(vertexFactory(printVertex,
                    frameMitre.FromFrameP(new Vector3d(-dimensions.x / 2 * scaleFactor, 0, -dimensions.y / 2)), brightnessMin));
            }

            joint.InBottom = joint.OutBottom = mesh.AddVertex(vertexFactory(printVertex,
                frameMitre.FromFrameP(new Vector3d(0, 0, -dimensions.y)), brightnessMax));

            joint.InRight = mesh.AddVertex(vertexFactory(printVertex,
                frameSegBefore.FromFrameP(new Vector3d(dimensions.x / 2, miterExtensions, -dimensions.y / 2)), brightnessMin));

            joint.OutRight = mesh.AddVertex(vertexFactory(printVertex,
                frameSegAfter.FromFrameP(new Vector3d(dimensions.x / 2, -miterExtensions, -dimensions.y / 2)), brightnessMin));

            mesh.AddTriangle(joint.InRight, joint.OutRight, joint.InTop);
            mesh.AddTriangle(joint.InRight, joint.InBottom, joint.OutRight);

            return joint;
        }

        private ToolpathPreviewJoint GenerateLeftBevel(TPrintVertex printVertex, ToolpathPreviewMesh mesh, Segment3d segBefore, Segment3d segAfter, double scaleFactor, Vector2d dimensions, Frame3f frameMitre, Frame3f frameSegBefore, Frame3f frameSegAfter, double miterExtensions)
        {
            var joint = new ToolpathPreviewJoint();

            joint.InTop = joint.OutTop = mesh.AddVertex(vertexFactory(printVertex,
                frameMitre.FromFrameP(new Vector3d(0, 0, 0)), brightnessMax));

            if (CornerIsInsideTube(segBefore, segAfter, dimensions.x))
            {
                joint.InRight = mesh.AddVertex(vertexFactory(printVertex,
                    frameSegBefore.FromFrameP(new Vector3d(dimensions.x / 2, 0, -dimensions.y / 2)), brightnessMin));

                joint.OutRight = mesh.AddVertex(vertexFactory(printVertex,
                    frameSegAfter.FromFrameP(new Vector3d(dimensions.x / 2, 0, -dimensions.y / 2)), brightnessMin));
            }
            else
            {
                joint.InRight = joint.OutRight = mesh.AddVertex(vertexFactory(printVertex,
                    frameMitre.FromFrameP(new Vector3d(dimensions.x / 2 * scaleFactor, 0, -dimensions.y / 2)), brightnessMin));
            }

            joint.InBottom = joint.OutBottom = mesh.AddVertex(vertexFactory(printVertex,
                frameMitre.FromFrameP(new Vector3d(0, 0, -dimensions.y)), brightnessMax));

            joint.InLeft = mesh.AddVertex(vertexFactory(printVertex,
                frameSegBefore.FromFrameP(new Vector3d(-dimensions.x / 2, miterExtensions, -dimensions.y / 2)), brightnessMin));

            joint.OutLeft = mesh.AddVertex(vertexFactory(printVertex,
                frameSegAfter.FromFrameP(new Vector3d(-dimensions.x / 2, -miterExtensions, -dimensions.y / 2)), brightnessMin));

            mesh.AddTriangle(joint.InLeft, joint.InTop, joint.OutLeft);
            mesh.AddTriangle(joint.InLeft, joint.OutLeft, joint.InBottom);

            return joint;
        }

        protected virtual int AddVertex(List<ToolpathPreviewVertex> vertices, TPrintVertex printVertex, Vector3d point, float brightness)
        {
            var printVertexCast = printVertex as PrintVertex;
            vertices.Add(vertexFactory(printVertex, point, brightness));
            return vertices.Count - 1;
        }

        protected virtual ToolpathPreviewJoint GenerateMiterJoint(LinearToolpath3<TPrintVertex> toolpath, int vertexIndex, ToolpathPreviewMesh mesh)
        {
            var segBefore = new Segment3d(toolpath[vertexIndex - 1].Position, toolpath[vertexIndex].Position);
            var segAfter = new Segment3d(toolpath[vertexIndex].Position, toolpath[vertexIndex + 1].Position);

            var averageDirection = (segBefore.Direction + segAfter.Direction).Normalized;
            var scaleFactor = 1 / segBefore.Direction.Dot(averageDirection);
            var dimensions = new Vector2d(toolpath[vertexIndex].Dimensions.x * scaleFactor, toolpath[vertexIndex].Dimensions.y);

            var frame = new Frame3f(toolpath[vertexIndex].Position);
            frame.AlignAxis(1, ToVector3f(averageDirection));

            var joint = new ToolpathPreviewJoint();

            joint.InTop = joint.OutTop = mesh.AddVertex(vertexFactory(toolpath[vertexIndex],
                frame.FromFrameP(new Vector3d(0, 0, 0)), brightnessMax));

            joint.InRight = joint.OutRight = mesh.AddVertex(vertexFactory(toolpath[vertexIndex],
                frame.FromFrameP(new Vector3d(dimensions.x / 2, 0, -dimensions.y / 2)), brightnessMin));

            joint.InBottom = joint.OutBottom = mesh.AddVertex(vertexFactory(toolpath[vertexIndex],
                frame.FromFrameP(new Vector3d(0, 0, -dimensions.y)), brightnessMax));

            joint.InLeft = joint.OutLeft = mesh.AddVertex(vertexFactory(toolpath[vertexIndex],
                frame.FromFrameP(new Vector3d(-dimensions.x / 2, 0, -dimensions.y / 2)), brightnessMin));

            return joint;
        }

        private Vector3f ToVector3f(Vector3d vector)
        {
            return new Vector3f(vector.x, vector.y, vector.z);
        }

        protected virtual void AddEdges(ToolpathPreviewJoint[] joints, ToolpathPreviewMesh mesh)
        {
            for (int i = joints.Length - 2; i >= 0; i--)
            {
                var start = joints[i];
                var end = joints[i + 1];

                mesh.AddTriangle(start.OutRight, end.InRight, start.OutTop);
                mesh.AddTriangle(end.InRight, end.InTop, start.OutTop);
                mesh.AddTriangle(start.OutTop, end.InTop, start.OutLeft);
                mesh.AddTriangle(end.InTop, end.InLeft, start.OutLeft);
                mesh.AddTriangle(start.OutLeft, end.InLeft, start.OutBottom);
                mesh.AddTriangle(end.InLeft, end.InBottom, start.OutBottom);
                mesh.AddTriangle(start.OutBottom, end.InBottom, start.OutRight);
                mesh.AddTriangle(end.InBottom, end.InRight, start.OutRight);
            }
        }
    }
}