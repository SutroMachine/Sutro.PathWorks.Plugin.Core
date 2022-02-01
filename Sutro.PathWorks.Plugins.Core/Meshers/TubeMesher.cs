using g3;
using gs;
using Sutro.PathWorks.Plugins.API.Visualizers;
using System;

namespace Sutro.PathWorks.Plugins.Core.Meshers
{
    public class TubeMesher<TPrintVertex> : IToolpathPreviewMesher<TPrintVertex> where TPrintVertex : IExtrusionVertex
    {
        protected const float brightnessMax = 1f;
        protected const float brightnessMin = 0.2f;
        protected const double miterThreshold = 40 * MathUtil.Deg2Rad;

        protected Func<TPrintVertex, Vector3d, float, ToolpathPreviewVertex> vertexFactory;

        public ToolpathPreviewMesh Generate(LinearToolpath3<TPrintVertex> toolpath, Func<TPrintVertex, Vector3d, float, ToolpathPreviewVertex> vertexFactory)
        {
            this.vertexFactory = vertexFactory;

            var mesh = new ToolpathPreviewMesh();

            var joints = new ToolpathPreviewJoint[toolpath.VertexCount];

            Segment3d? segmentBeforeJoint = null;

            for (int i = 0; i < toolpath.VertexCount; i++)
            {
                Segment3d? segmentAfterJoint = null;
                var currentVertex = toolpath[i];
                var nextVertex = toolpath[i];

                if (i < toolpath.VertexCount - 1)
                {
                    nextVertex = toolpath[i + 1];
                    segmentAfterJoint = new Segment3d(currentVertex.Position, nextVertex.Position);
                }

                if (segmentBeforeJoint == null || segmentAfterJoint == null)
                {
                    joints[i] = GenerateButtJoint(segmentBeforeJoint, segmentAfterJoint, currentVertex, nextVertex, mesh);
                }
                else
                {
                    var angleRad = SignedAngleRad(segmentBeforeJoint.Value.Direction.xy, segmentAfterJoint.Value.Direction.xy);

                    if (Math.Abs(angleRad) > miterThreshold)
                    {
                        joints[i] = GenerateBevelJoint(segmentBeforeJoint.Value, segmentAfterJoint.Value, currentVertex, nextVertex, mesh);
                    }
                    else
                    {
                        joints[i] = GenerateMiterJoint(segmentBeforeJoint.Value, segmentAfterJoint.Value, currentVertex, nextVertex, mesh);
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

        protected virtual ToolpathPreviewJoint GenerateBevelJoint(
            Segment3d segmentBefore, Segment3d segmentAfter,
            TPrintVertex printVertex, TPrintVertex nextPrintVertex, ToolpathPreviewMesh mesh)
        {
            if (segmentBefore.Direction.Cross(segmentAfter.Direction).z > 0)
                return GenerateRightBevel(segmentBefore, segmentAfter, printVertex, nextPrintVertex, mesh);
            else
                return GenerateLeftBevel(segmentBefore, segmentAfter, printVertex, nextPrintVertex, mesh);
        }

        protected virtual ToolpathPreviewJoint GenerateMiterJoint(Segment3d segmentBefore, Segment3d segmentAfter,
            TPrintVertex printVertex, TPrintVertex nextPrintVertex, ToolpathPreviewMesh mesh)
        {
            var averageDirection = (segmentBefore.Direction + segmentAfter.Direction).Normalized;

            var frame = new Frame3f(printVertex.Position);
            frame.AlignAxis(1, ToVector3f(averageDirection));

            var joint = new ToolpathPreviewJoint();

            var dimensions = CrossSectionDimensionsFromPrintVertex(printVertex);

            joint.InTop = joint.OutTop = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Top(dimensions)), brightnessMax));

            joint.InRight = joint.OutRight = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Right(dimensions)), brightnessMin));

            joint.InBottom = joint.OutBottom = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Bottom(dimensions)), brightnessMax));

            joint.InLeft = joint.OutLeft = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Left(dimensions)), brightnessMin));

            return joint;
        }

        protected static bool CornerIsInsideTube(Segment3d segmentBeforeJoint, Segment3d segmentAfterJoint, double tubeWidth)
        {
            var angle = Math.PI - segmentBeforeJoint.Direction.AngleR(segmentAfterJoint.Direction, true);
            var minimumLength = (tubeWidth / 2d) / Math.Tan(angle / 2d);
            return segmentAfterJoint.Length < minimumLength;
        }

        protected static void CreateFrames(Segment3d segmentBefore, Segment3d segmentAfter, out Frame3f frameMiter, out Frame3f frameSegBefore, out Frame3f frameSegAfter)
        {
            var averageDirection = (segmentBefore.Direction + segmentAfter.Direction).Normalized;

            frameMiter = new Frame3f(segmentBefore.P1);
            frameMiter.AlignAxis(1, ToVector3f(averageDirection));

            frameSegBefore = new Frame3f(segmentBefore.P1);
            frameSegBefore.AlignAxis(1, ToVector3f(segmentBefore.Direction));

            frameSegAfter = new Frame3f(segmentBefore.P1);
            frameSegAfter.AlignAxis(1, ToVector3f(segmentAfter.Direction));
        }

        protected static double GetBevelDistance(ref Segment3d segmentBefore, ref Segment3d segmentAfter, Vector2d dimensions)
        {
            double angle2 = Math.Abs(segmentBefore.Direction.AngleR(segmentAfter.Direction));

            double miterExtensions = Math.Tan(angle2 / 4) * dimensions.x / 2;
            return miterExtensions;
        }

        protected static double GetMiterScaleFactor(ref Frame3f frameMiter, ref Frame3f frameSegBefore)
        {
            return 1 / frameSegBefore.Y.Dot(frameMiter.Y);
        }

        protected static Vector3f ToVector3f(Vector3d vector)
        {
            return new Vector3f(vector.x, vector.y, vector.z);
        }

        protected void AddCapAfterJoint(Vector3d segmentDirection, TPrintVertex printVertex, TPrintVertex nextPrintVertex, ToolpathPreviewJoint joint, ToolpathPreviewMesh mesh)
        {
            var frame = new Frame3f(printVertex.Position);
            frame.AlignAxis(1, ToVector3f(segmentDirection));

            var dimensions = CrossSectionDimensionsFromPrintVertex(nextPrintVertex);

            joint.OutTop = mesh.AddVertex(vertexFactory(nextPrintVertex,
                frame.FromFrameP(DiamondCrossSection.Top(dimensions)), brightnessMax));

            joint.OutRight = mesh.AddVertex(vertexFactory(nextPrintVertex,
                frame.FromFrameP(DiamondCrossSection.Right(dimensions)), brightnessMin));

            joint.OutBottom = mesh.AddVertex(vertexFactory(nextPrintVertex,
                frame.FromFrameP(DiamondCrossSection.Bottom(dimensions)), brightnessMax));

            joint.OutLeft = mesh.AddVertex(vertexFactory(nextPrintVertex,
                frame.FromFrameP(DiamondCrossSection.Left(dimensions)), brightnessMin));

            mesh.AddTriangle(joint.OutBottom, joint.OutTop, joint.OutLeft);
            mesh.AddTriangle(joint.OutBottom, joint.OutRight, joint.OutTop);
        }

        protected void AddCapBeforeJoint(Vector3d segmentDirection, TPrintVertex printVertex, ToolpathPreviewJoint joint, ToolpathPreviewMesh mesh)
        {
            var frame = new Frame3f(printVertex.Position);
            frame.AlignAxis(1, ToVector3f(segmentDirection));

            var dimensions = CrossSectionDimensionsFromPrintVertex(printVertex);

            joint.InTop = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Top(dimensions)), brightnessMax));

            joint.InRight = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Right(dimensions)), brightnessMin));

            joint.InBottom = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Bottom(dimensions)), brightnessMax));

            joint.InLeft = mesh.AddVertex(vertexFactory(printVertex,
                frame.FromFrameP(DiamondCrossSection.Left(dimensions)), brightnessMin));

            mesh.AddTriangle(joint.InBottom, joint.InLeft, joint.InTop);
            mesh.AddTriangle(joint.InBottom, joint.InTop, joint.InRight);
        }

        protected virtual void AddLeftMiter(ToolpathPreviewMesh mesh, TPrintVertex printVertex, TPrintVertex nextPrintVertex,
            ref Frame3f frameMiter, ref Frame3f frameSegBefore, ToolpathPreviewJoint joint)
        {
            var dimensions = CrossSectionDimensionsFromPrintVertex(printVertex);

            double miterScaleFactor = GetMiterScaleFactor(ref frameMiter, ref frameSegBefore);
            joint.InLeft = joint.OutLeft = mesh.AddVertex(vertexFactory(printVertex,
                frameMiter.FromFrameP(DiamondCrossSection.Left(dimensions, miterScaleFactor)), brightnessMin));
        }

        protected virtual void AddLeftSquare(ToolpathPreviewMesh mesh, TPrintVertex printVertex, TPrintVertex nextPrintVertex,
            ref Frame3f frameSegBefore, ref Frame3f frameSegAfter, ToolpathPreviewJoint joint)
        {
            var dimensions = CrossSectionDimensionsFromPrintVertex(printVertex);

            joint.InLeft = mesh.AddVertex(vertexFactory(printVertex,
                frameSegBefore.FromFrameP(DiamondCrossSection.Left(dimensions)), brightnessMin));

            joint.OutLeft = mesh.AddVertex(vertexFactory(printVertex,
                frameSegAfter.FromFrameP(DiamondCrossSection.Left(dimensions)), brightnessMin));
        }

        protected virtual void AddRightMiter(TPrintVertex printVertex, TPrintVertex nextPrintVertex, ToolpathPreviewMesh mesh,
            ref Frame3f frameMiter, ref Frame3f frameSegBefore, ToolpathPreviewJoint joint)
        {
            var dimensions = CrossSectionDimensionsFromPrintVertex(printVertex);

            double miterScaleFactor = GetMiterScaleFactor(ref frameMiter, ref frameSegBefore);
            joint.InRight = joint.OutRight = mesh.AddVertex(vertexFactory(printVertex,
                frameMiter.FromFrameP(DiamondCrossSection.Right(dimensions, miterScaleFactor)), brightnessMin));
        }

        protected virtual void AddRightSquare(TPrintVertex printVertex, TPrintVertex nextPrintVertex, ToolpathPreviewMesh mesh,
            ref Frame3f frameSegBefore, ref Frame3f frameSegAfter, ToolpathPreviewJoint joint)
        {
            var dimensions = CrossSectionDimensionsFromPrintVertex(printVertex);

            joint.InRight = mesh.AddVertex(vertexFactory(printVertex,
                frameSegBefore.FromFrameP(DiamondCrossSection.Right(dimensions)), brightnessMin));

            joint.OutRight = mesh.AddVertex(vertexFactory(printVertex,
                frameSegAfter.FromFrameP(DiamondCrossSection.Right(dimensions)), brightnessMin));
        }

        private ToolpathPreviewJoint GenerateButtJoint(Segment3d? segmentBefore, Segment3d? segmentAfter, TPrintVertex printVertex, TPrintVertex nextPrintVertex, ToolpathPreviewMesh mesh)
        {
            var joint = new ToolpathPreviewJoint();

            if (segmentBefore.HasValue)
                AddCapBeforeJoint(segmentBefore.Value.Direction, printVertex, joint, mesh);

            if (segmentAfter.HasValue)
                AddCapAfterJoint(segmentAfter.Value.Direction, printVertex, nextPrintVertex, joint, mesh);

            return joint;
        }

        protected virtual ToolpathPreviewJoint GenerateLeftBevel(Segment3d segBefore, Segment3d segAfter,
            TPrintVertex printVertex, TPrintVertex nextPrintVertex, ToolpathPreviewMesh mesh)
        {
            CreateFrames(segBefore, segAfter, out var frameMiter, out var frameSegBefore, out var frameSegAfter);

            var dimensions = CrossSectionDimensionsFromPrintVertex(printVertex);

            var joint = new ToolpathPreviewJoint();

            joint.InTop = joint.OutTop = mesh.AddVertex(vertexFactory(printVertex,
                frameMiter.FromFrameP(DiamondCrossSection.Top(dimensions)), brightnessMax));

            if (CornerIsInsideTube(segBefore, segAfter, dimensions.x))
            {
                AddRightSquare(printVertex, nextPrintVertex, mesh, ref frameSegBefore, ref frameSegAfter, joint);
            }
            else
            {
                AddRightMiter(printVertex, nextPrintVertex, mesh, ref frameMiter, ref frameSegBefore, joint);
            }

            joint.InBottom = joint.OutBottom = mesh.AddVertex(vertexFactory(printVertex,
                frameMiter.FromFrameP(DiamondCrossSection.Bottom(dimensions)), brightnessMax));

            double bevelDistance = GetBevelDistance(ref segBefore, ref segAfter, dimensions);

            joint.InLeft = mesh.AddVertex(vertexFactory(printVertex,
                frameSegBefore.FromFrameP(DiamondCrossSection.Left(dimensions, 1, bevelDistance)), brightnessMin));

            joint.OutLeft = mesh.AddVertex(vertexFactory(printVertex,
                frameSegAfter.FromFrameP(DiamondCrossSection.Left(dimensions, 1, -bevelDistance)), brightnessMin));

            mesh.AddTriangle(joint.InLeft, joint.InTop, joint.OutLeft);
            mesh.AddTriangle(joint.InLeft, joint.OutLeft, joint.InBottom);

            return joint;
        }

        protected virtual ToolpathPreviewJoint GenerateRightBevel(Segment3d segBefore, Segment3d segAfter,
            TPrintVertex printVertex, TPrintVertex nextPrintVertex, ToolpathPreviewMesh mesh)
        {
            CreateFrames(segBefore, segAfter, out var frameMiter, out var frameSegBefore, out var frameSegAfter);

            var dimensions = CrossSectionDimensionsFromPrintVertex(printVertex);

            var joint = new ToolpathPreviewJoint();

            joint.InTop = joint.OutTop = mesh.AddVertex(vertexFactory(printVertex,
                frameMiter.FromFrameP(DiamondCrossSection.Top(dimensions)), brightnessMax));

            if (CornerIsInsideTube(segBefore, segAfter, dimensions.x))
            {
                AddLeftSquare(mesh, printVertex, nextPrintVertex, ref frameSegBefore, ref frameSegAfter, joint);
            }
            else
            {
                AddLeftMiter(mesh, printVertex, nextPrintVertex, ref frameMiter, ref frameSegBefore, joint);
            }

            joint.InBottom = joint.OutBottom = mesh.AddVertex(vertexFactory(printVertex,
                frameMiter.FromFrameP(DiamondCrossSection.Bottom(dimensions)), brightnessMax));

            double bevelDistance = GetBevelDistance(ref segBefore, ref segAfter, dimensions);

            joint.InRight = mesh.AddVertex(vertexFactory(printVertex,
                frameSegBefore.FromFrameP(DiamondCrossSection.Right(dimensions, 1, bevelDistance)), brightnessMin));

            joint.OutRight = mesh.AddVertex(vertexFactory(printVertex,
                frameSegAfter.FromFrameP(DiamondCrossSection.Right(dimensions, 1, -bevelDistance)), brightnessMin));

            mesh.AddTriangle(joint.InRight, joint.OutRight, joint.InTop);
            mesh.AddTriangle(joint.InRight, joint.InBottom, joint.OutRight);

            return joint;
        }

        protected virtual Vector2d CrossSectionDimensionsFromPrintVertex(TPrintVertex printVertex)
        {
            return printVertex.Dimensions;
        }
    }
}