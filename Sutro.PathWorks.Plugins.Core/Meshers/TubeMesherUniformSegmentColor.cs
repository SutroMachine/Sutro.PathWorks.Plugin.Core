using g3;
using gs;

namespace Sutro.PathWorks.Plugins.Core.Meshers
{
    /// <summary>
    /// This is similar to the base TubeMesher, but in order to create
    /// sharp color steps (each segment uniform instead of blended), it will
    /// create more vertices. This trades off better visualization but decreased 
    /// performance on larger meshes.
    /// </summary>
    /// <typeparam name="TPrintVertex"></typeparam>
    public class TubeMesherUniformSegmentColor<TPrintVertex> : TubeMesher<TPrintVertex> where TPrintVertex : IExtrusionVertex
    {
        protected override void AddLeftMiter(ToolpathPreviewMesh mesh, TPrintVertex printVertex, TPrintVertex nextPrintVertex, ref Frame3f frameMiter, ref Frame3f frameSegBefore, ToolpathPreviewJoint joint)
        {
            double miterScaleFactor = GetMiterScaleFactor(ref frameMiter, ref frameSegBefore);
            var left = frameMiter.FromFrameP(DiamondCrossSection.Left(printVertex.Dimensions, miterScaleFactor));
            joint.InLeft = mesh.AddVertex(vertexFactory(printVertex, left, brightnessMin));
            joint.OutLeft = mesh.AddVertex(vertexFactory(nextPrintVertex, left, brightnessMin));
        }

        protected override void AddRightMiter(TPrintVertex printVertex, TPrintVertex nextPrintVertex, ToolpathPreviewMesh mesh, ref Frame3f frameMiter, ref Frame3f frameSegBefore, ToolpathPreviewJoint joint)
        {
            double miterScaleFactor = GetMiterScaleFactor(ref frameMiter, ref frameSegBefore);
            var right = frameMiter.FromFrameP(DiamondCrossSection.Right(printVertex.Dimensions, miterScaleFactor));
            joint.InRight = mesh.AddVertex(vertexFactory(printVertex, right, brightnessMin));
            joint.OutRight = mesh.AddVertex(vertexFactory(nextPrintVertex, right, brightnessMin));
        }

        protected override void AddLeftSquare(ToolpathPreviewMesh mesh, TPrintVertex printVertex, TPrintVertex nextPrintVertex, ref Frame3f frameSegBefore, ref Frame3f frameSegAfter, ToolpathPreviewJoint joint)
        {
            var left = frameSegBefore.FromFrameP(DiamondCrossSection.Left(printVertex.Dimensions));
            joint.InLeft = mesh.AddVertex(vertexFactory(printVertex, left, brightnessMin));
            joint.OutLeft = mesh.AddVertex(vertexFactory(nextPrintVertex, left, brightnessMin));
        }

        protected override void AddRightSquare(TPrintVertex printVertex, TPrintVertex nextPrintVertex, ToolpathPreviewMesh mesh, ref Frame3f frameSegBefore, ref Frame3f frameSegAfter, ToolpathPreviewJoint joint)
        {
            var right = frameSegBefore.FromFrameP(DiamondCrossSection.Right(printVertex.Dimensions));
            joint.InRight = mesh.AddVertex(vertexFactory(printVertex, right, brightnessMin));
            joint.OutRight = mesh.AddVertex(vertexFactory(printVertex, right, brightnessMin));
        }

        protected override ToolpathPreviewJoint GenerateLeftBevel(Segment3d segBefore, Segment3d segAfter, TPrintVertex printVertex, TPrintVertex nextPrintVertex, ToolpathPreviewMesh mesh)
        {
            CreateFrames(segBefore, segAfter, out var frameMiter, out var frameSegBefore, out var frameSegAfter);

            var joint = new ToolpathPreviewJoint();

            var top = frameMiter.FromFrameP(DiamondCrossSection.Top(printVertex.Dimensions));
            joint.InTop = mesh.AddVertex(vertexFactory(printVertex, top, brightnessMax));
            joint.OutTop = mesh.AddVertex(vertexFactory(nextPrintVertex, top, brightnessMax));

            if (CornerIsInsideTube(segBefore, segAfter, printVertex.Dimensions.x))
            {
                AddRightSquare(printVertex, nextPrintVertex, mesh, ref frameSegBefore, ref frameSegAfter, joint);
            }
            else
            {
                AddRightMiter(printVertex, nextPrintVertex, mesh, ref frameMiter, ref frameSegBefore, joint);
            }

            var bottom = frameMiter.FromFrameP(DiamondCrossSection.Bottom(printVertex.Dimensions));

            joint.InBottom = mesh.AddVertex(vertexFactory(printVertex, bottom, brightnessMax));
            joint.OutBottom = mesh.AddVertex(vertexFactory(nextPrintVertex, bottom, brightnessMax));

            double bevelDistance = GetBevelDistance(ref segBefore, ref segAfter, printVertex.Dimensions);


            joint.InLeft = mesh.AddVertex(vertexFactory(printVertex,
                frameSegBefore.FromFrameP(DiamondCrossSection.Left(printVertex.Dimensions, 1, bevelDistance)), brightnessMin));

            joint.OutLeft = mesh.AddVertex(vertexFactory(nextPrintVertex,
                frameSegAfter.FromFrameP(DiamondCrossSection.Left(printVertex.Dimensions, 1, -bevelDistance)), brightnessMin));

            mesh.AddTriangle(joint.InLeft, joint.InTop, joint.OutLeft);
            mesh.AddTriangle(joint.InLeft, joint.OutLeft, joint.InBottom);

            return joint;
        }

        protected override ToolpathPreviewJoint GenerateRightBevel(Segment3d segBefore, Segment3d segAfter, TPrintVertex printVertex, TPrintVertex nextPrintVertex, ToolpathPreviewMesh mesh)
        {
            CreateFrames(segBefore, segAfter, out var frameMiter, out var frameSegBefore, out var frameSegAfter);

            var joint = new ToolpathPreviewJoint();

            var top = frameMiter.FromFrameP(DiamondCrossSection.Top(printVertex.Dimensions));
            joint.InTop = mesh.AddVertex(vertexFactory(printVertex, top, brightnessMax));
            joint.OutTop = mesh.AddVertex(vertexFactory(nextPrintVertex, top, brightnessMax));

            if (CornerIsInsideTube(segBefore, segAfter, printVertex.Dimensions.x))
            {
                AddLeftSquare(mesh, printVertex, nextPrintVertex, ref frameSegBefore, ref frameSegAfter, joint);
            }
            else
            {
                AddLeftMiter(mesh, printVertex, nextPrintVertex, ref frameMiter, ref frameSegBefore, joint);
            }

            var bottom = frameMiter.FromFrameP(DiamondCrossSection.Bottom(printVertex.Dimensions));
            joint.InBottom = mesh.AddVertex(vertexFactory(printVertex, bottom, brightnessMax));
            joint.OutBottom = mesh.AddVertex(vertexFactory(nextPrintVertex, bottom, brightnessMax));

            double bevelDistance = GetBevelDistance(ref segBefore, ref segAfter, printVertex.Dimensions);

            joint.InRight = mesh.AddVertex(vertexFactory(printVertex,
                frameSegBefore.FromFrameP(DiamondCrossSection.Right(printVertex.Dimensions, 1, bevelDistance)), brightnessMin));

            joint.OutRight = mesh.AddVertex(vertexFactory(nextPrintVertex,
                frameSegAfter.FromFrameP(DiamondCrossSection.Right(printVertex.Dimensions, 1, -bevelDistance)), brightnessMin));

            mesh.AddTriangle(joint.InRight, joint.OutRight, joint.InTop);
            mesh.AddTriangle(joint.InRight, joint.InBottom, joint.OutRight);

            return joint;
        }

        protected override ToolpathPreviewJoint GenerateMiterJoint(Segment3d segmentBefore, Segment3d segmentAfter, TPrintVertex printVertex, TPrintVertex nextPrintVertex, ToolpathPreviewMesh mesh)
        {
            var averageDirection = (segmentBefore.Direction + segmentAfter.Direction).Normalized;

            var frame = new Frame3f(printVertex.Position);
            frame.AlignAxis(1, ToVector3f(averageDirection));

            var joint = new ToolpathPreviewJoint();

            var top = frame.FromFrameP(DiamondCrossSection.Top(printVertex.Dimensions));
            joint.InTop = mesh.AddVertex(vertexFactory(printVertex, top, brightnessMax));
            joint.OutTop = mesh.AddVertex(vertexFactory(nextPrintVertex, top, brightnessMax));

            var right = frame.FromFrameP(DiamondCrossSection.Right(printVertex.Dimensions));
            joint.InRight = mesh.AddVertex(vertexFactory(printVertex, right, brightnessMin));
            joint.OutRight = mesh.AddVertex(vertexFactory(nextPrintVertex, right, brightnessMin));

            var bottom = frame.FromFrameP(DiamondCrossSection.Bottom(printVertex.Dimensions));
            joint.InBottom = mesh.AddVertex(vertexFactory(printVertex, bottom, brightnessMax));
            joint.OutBottom = mesh.AddVertex(vertexFactory(nextPrintVertex, bottom, brightnessMax));

            var left = frame.FromFrameP(DiamondCrossSection.Left(printVertex.Dimensions));
            joint.InLeft = mesh.AddVertex(vertexFactory(printVertex, left, brightnessMin));
            joint.OutLeft = mesh.AddVertex(vertexFactory(nextPrintVertex, left, brightnessMin));

            return joint;
        }
    }
}