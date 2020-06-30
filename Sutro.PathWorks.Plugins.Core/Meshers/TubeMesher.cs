using g3;
using gs;
using Sutro.PathWorks.Plugins.API.Visualizers;
using System;

namespace Sutro.PathWorks.Plugins.Core.Meshers
{
    public class TubeMesher<TPrintVertex> : IToolpathPreviewMesher<TPrintVertex> where TPrintVertex : IExtrusionVertex
    {
        private const float brightnessMax = 1f;
        private const float brightnessMin = 0.2f;
        private const double miterThreshold = 40 * MathUtil.Deg2Rad;

        private Func<TPrintVertex, Vector3d, float, ToolpathPreviewVertex> vertexFactory;

        public ToolpathPreviewMesh Generate(LinearToolpath3<TPrintVertex> toolpath, Func<TPrintVertex, Vector3d, float, ToolpathPreviewVertex> vertexFactory)
        {
            this.vertexFactory = vertexFactory;

            var mesh = new ToolpathPreviewMesh();

            var joints = new ToolpathPreviewJoint[toolpath.VertexCount];

            Segment3d? segmentBeforeJoint = null;

            for (int i = 0; i < toolpath.VertexCount; i++)
            {
                Segment3d? segmentAfterJoint = null;

                if (i < toolpath.VertexCount - 1)
                {
                    segmentAfterJoint = new Segment3d(toolpath[i].Position, toolpath[i + 1].Position);
                }

                if (segmentBeforeJoint == null || segmentAfterJoint == null)
                {
                    joints[i] = GenerateButtJoint(segmentBeforeJoint, segmentAfterJoint, toolpath[i], mesh);
                }
                else
                {
                    var angleRad = SignedAngleRad(segmentBeforeJoint.Value.Direction.xy, segmentAfterJoint.Value.Direction.xy);

                    if (Math.Abs(angleRad) > miterThreshold)
                    {
                        joints[i] = GenerateBevelJoint(segmentBeforeJoint.Value, segmentAfterJoint.Value, toolpath[i], mesh);
                    }
                    else
                    {
                        joints[i] = GenerateMiterJoint(segmentBeforeJoint.Value, segmentAfterJoint.Value, toolpath[i], mesh);
                    }
                }

                segmentBeforeJoint = segmentAfterJoint;
            }

            AddEdges(joints, mesh);

            return mesh;
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

        protected ToolpathPreviewJoint GenerateBevelJoint(
            Segment3d segmentBefore, Segment3d segmentAfter,
            TPrintVertex printVertex, ToolpathPreviewMesh mesh)
        {
            if (segmentBefore.Direction.Cross(segmentAfter.Direction).z > 0)
                return GenerateRightBevel(segmentBefore, segmentAfter, printVertex, mesh);
            else
                return GenerateLeftBevel(segmentBefore, segmentAfter, printVertex, mesh);
        }

        protected virtual ToolpathPreviewJoint GenerateMiterJoint(Segment3d segmentBefore, Segment3d segmentAfter, TPrintVertex printVertex, ToolpathPreviewMesh mesh)
        {
            var averageDirection = (segmentBefore.Direction + segmentAfter.Direction).Normalized;
            var scaleFactor = 1 / segmentBefore.Direction.Dot(averageDirection);

            var frame = new Frame3f(printVertex.Position);
            frame.AlignAxis(1, ToVector3f(averageDirection));

            var joint = new ToolpathPreviewJoint();

            joint.InTop = joint.OutTop = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Top(printVertex.Dimensions)), brightnessMax));

            joint.InRight = joint.OutRight = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Right(printVertex.Dimensions)), brightnessMin));

            joint.InBottom = joint.OutBottom = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Bottom(printVertex.Dimensions)), brightnessMax));

            joint.InLeft = joint.OutLeft = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Left(printVertex.Dimensions)), brightnessMin));

            return joint;
        }

        private static bool CornerIsInsideTube(Segment3d segmentBeforeJoint, Segment3d segmentAfterJoint, double tubeWidth)
        {
            var angle = Math.PI - segmentBeforeJoint.Direction.AngleR(segmentAfterJoint.Direction, true);
            var minimumLength = (tubeWidth / 2d) / Math.Tan(angle / 2d);
            return segmentAfterJoint.Length < minimumLength;
        }

        private static void CreateFrames(Segment3d segmentBefore, Segment3d segmentAfter, out Frame3f frameMiter, out Frame3f frameSegBefore, out Frame3f frameSegAfter)
        {
            var averageDirection = (segmentBefore.Direction + segmentAfter.Direction).Normalized;

            frameMiter = new Frame3f(segmentBefore.P1);
            frameMiter.AlignAxis(1, ToVector3f(averageDirection));

            frameSegBefore = new Frame3f(segmentBefore.P1);
            frameSegBefore.AlignAxis(1, ToVector3f(segmentBefore.Direction));

            frameSegAfter = new Frame3f(segmentBefore.P1);
            frameSegAfter.AlignAxis(1, ToVector3f(segmentAfter.Direction));
        }

        private static double GetBevelDistance(ref Segment3d segmentBefore, ref Segment3d segmentAfter, Vector2d dimensions)
        {
            double angle2 = Math.Abs(segmentBefore.Direction.AngleR(segmentAfter.Direction));

            double miterExtensions = Math.Tan(angle2 / 4) * dimensions.x / 2;
            return miterExtensions;
        }

        private static double GetMiterScaleFactor(ref Frame3f frameMiter, ref Frame3f frameSegBefore)
        {
            return 1 / frameSegBefore.Y.Dot(frameMiter.Y);
        }

        private static Vector3f ToVector3f(Vector3d vector)
        {
            return new Vector3f(vector.x, vector.y, vector.z);
        }

        private void AddCapAfterJoint(Vector3d segmentDirection, TPrintVertex printVertex, ToolpathPreviewJoint joint, ToolpathPreviewMesh mesh)
        {
            var frame = new Frame3f(printVertex.Position);
            frame.AlignAxis(1, ToVector3f(segmentDirection));

            joint.OutTop = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Top(printVertex.Dimensions)), brightnessMax));

            joint.OutRight = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Right(printVertex.Dimensions)), brightnessMin));

            joint.OutBottom = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Bottom(printVertex.Dimensions)), brightnessMax));

            joint.OutLeft = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Left(printVertex.Dimensions)), brightnessMin));

            mesh.AddTriangle(joint.OutBottom, joint.OutTop, joint.OutLeft);
            mesh.AddTriangle(joint.OutBottom, joint.OutRight, joint.OutTop);
        }

        private void AddCapBeforeJoint(Vector3d segmentDirection, TPrintVertex printVertex, ToolpathPreviewJoint joint, ToolpathPreviewMesh mesh)
        {
            var frame = new Frame3f(printVertex.Position);
            frame.AlignAxis(1, ToVector3f(segmentDirection));

            joint.InTop = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Top(printVertex.Dimensions)), brightnessMax));

            joint.InRight = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Right(printVertex.Dimensions)), brightnessMin));

            joint.InBottom = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Bottom(printVertex.Dimensions)), brightnessMax));

            joint.InLeft = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Left(printVertex.Dimensions)), brightnessMin));

            mesh.AddTriangle(joint.InBottom, joint.InLeft, joint.InTop);
            mesh.AddTriangle(joint.InBottom, joint.InTop, joint.InRight);
        }

        private void AddLeftMiter(ToolpathPreviewMesh mesh, TPrintVertex printVertex, ref Frame3f frameMiter, ref Frame3f frameSegBefore, ToolpathPreviewJoint joint)
        {
            double miterScaleFactor = GetMiterScaleFactor(ref frameMiter, ref frameSegBefore);
            joint.InLeft = joint.OutLeft = mesh.AddVertex(vertexFactory(printVertex,
                frameMiter.FromFrameP(DiamondCrossSection.Left(printVertex.Dimensions, miterScaleFactor)), brightnessMin));
        }

        private void AddLeftSquare(ToolpathPreviewMesh mesh, TPrintVertex printVertex, ref Frame3f frameSegBefore, ref Frame3f frameSegAfter, ToolpathPreviewJoint joint)
        {
            joint.InLeft = mesh.AddVertex(vertexFactory(printVertex,
                frameSegBefore.FromFrameP(DiamondCrossSection.Left(printVertex.Dimensions)), brightnessMin));

            joint.OutLeft = mesh.AddVertex(vertexFactory(printVertex,
                frameSegAfter.FromFrameP(DiamondCrossSection.Left(printVertex.Dimensions)), brightnessMin));
        }

        private void AddRightMiter(TPrintVertex printVertex, ToolpathPreviewMesh mesh, ref Frame3f frameMiter, ref Frame3f frameSegBefore, ToolpathPreviewJoint joint)
        {
            double miterScaleFactor = GetMiterScaleFactor(ref frameMiter, ref frameSegBefore);
            joint.InRight = joint.OutRight = mesh.AddVertex(vertexFactory(printVertex,
                frameMiter.FromFrameP(DiamondCrossSection.Right(printVertex.Dimensions, miterScaleFactor)), brightnessMin));
        }

        private void AddRightSquare(TPrintVertex printVertex, ToolpathPreviewMesh mesh, ref Frame3f frameSegBefore, ref Frame3f frameSegAfter, ToolpathPreviewJoint joint)
        {
            joint.InRight = mesh.AddVertex(vertexFactory(printVertex,
                frameSegBefore.FromFrameP(DiamondCrossSection.Left(printVertex.Dimensions)), brightnessMin));

            joint.OutRight = mesh.AddVertex(vertexFactory(printVertex,
                frameSegAfter.FromFrameP(DiamondCrossSection.Right(printVertex.Dimensions)), brightnessMin));
        }

        private ToolpathPreviewJoint GenerateButtJoint(Segment3d? segmentBefore, Segment3d? segmentAfter, TPrintVertex printVertex, ToolpathPreviewMesh mesh)
        {
            var joint = new ToolpathPreviewJoint();

            if (segmentBefore.HasValue)
                AddCapBeforeJoint(segmentBefore.Value.Direction, printVertex, joint, mesh);

            if (segmentAfter.HasValue)
                AddCapAfterJoint(segmentAfter.Value.Direction, printVertex, joint, mesh);

            return joint;
        }

        private ToolpathPreviewJoint GenerateLeftBevel(Segment3d segBefore, Segment3d segAfter, TPrintVertex printVertex, ToolpathPreviewMesh mesh)
        {
            CreateFrames(segBefore, segAfter, out var frameMiter, out var frameSegBefore, out var frameSegAfter);

            var joint = new ToolpathPreviewJoint();

            joint.InTop = joint.OutTop = mesh.AddVertex(vertexFactory(printVertex,
                frameMiter.FromFrameP(DiamondCrossSection.Top(printVertex.Dimensions)), brightnessMax));

            if (CornerIsInsideTube(segBefore, segAfter, printVertex.Dimensions.x))
            {
                AddRightSquare(printVertex, mesh, ref frameSegBefore, ref frameSegAfter, joint);
            }
            else
            {
                AddRightMiter(printVertex, mesh, ref frameMiter, ref frameSegBefore, joint);
            }

            joint.InBottom = joint.OutBottom = mesh.AddVertex(vertexFactory(printVertex,
                frameMiter.FromFrameP(DiamondCrossSection.Bottom(printVertex.Dimensions)), brightnessMax));

            double bevelDistance = GetBevelDistance(ref segBefore, ref segAfter, printVertex.Dimensions);

            joint.InLeft = mesh.AddVertex(vertexFactory(printVertex,
                frameSegBefore.FromFrameP(DiamondCrossSection.Left(printVertex.Dimensions, 1, bevelDistance)), brightnessMin));

            joint.OutLeft = mesh.AddVertex(vertexFactory(printVertex,
                frameSegAfter.FromFrameP(DiamondCrossSection.Left(printVertex.Dimensions, 1, -bevelDistance)), brightnessMin));

            mesh.AddTriangle(joint.InLeft, joint.InTop, joint.OutLeft);
            mesh.AddTriangle(joint.InLeft, joint.OutLeft, joint.InBottom);

            return joint;
        }

        private ToolpathPreviewJoint GenerateRightBevel(Segment3d segBefore, Segment3d segAfter, TPrintVertex printVertex, ToolpathPreviewMesh mesh)
        {
            CreateFrames(segBefore, segAfter, out var frameMiter, out var frameSegBefore, out var frameSegAfter);

            var joint = new ToolpathPreviewJoint();

            joint.InTop = joint.OutTop = mesh.AddVertex(vertexFactory(printVertex,
                frameMiter.FromFrameP(DiamondCrossSection.Top(printVertex.Dimensions)), brightnessMax));

            if (CornerIsInsideTube(segBefore, segAfter, printVertex.Dimensions.x))
            {
                AddLeftSquare(mesh, printVertex, ref frameSegBefore, ref frameSegAfter, joint);
            }
            else
            {
                AddLeftMiter(mesh, printVertex, ref frameMiter, ref frameSegBefore, joint);
            }

            joint.InBottom = joint.OutBottom = mesh.AddVertex(vertexFactory(printVertex,
                frameMiter.FromFrameP(DiamondCrossSection.Bottom(printVertex.Dimensions)), brightnessMax));

            double bevelDistance = GetBevelDistance(ref segBefore, ref segAfter, printVertex.Dimensions);

            joint.InRight = mesh.AddVertex(vertexFactory(printVertex,
                frameSegBefore.FromFrameP(DiamondCrossSection.Right(printVertex.Dimensions, 1, bevelDistance)), brightnessMin));

            joint.OutRight = mesh.AddVertex(vertexFactory(printVertex,
                frameSegAfter.FromFrameP(DiamondCrossSection.Right(printVertex.Dimensions, 1, -bevelDistance)), brightnessMin));

            mesh.AddTriangle(joint.InRight, joint.OutRight, joint.InTop);
            mesh.AddTriangle(joint.InRight, joint.InBottom, joint.OutRight);

            return joint;
        }
    }
}