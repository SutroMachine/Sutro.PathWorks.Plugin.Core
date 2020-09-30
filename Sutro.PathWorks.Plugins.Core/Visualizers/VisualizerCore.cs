using g3;
using gs;
using Sutro.Core.Decompilers;
using Sutro.PathWorks.Plugins.API.Visualizers;
using Sutro.PathWorks.Plugins.Core.CustomData;
using Sutro.PathWorks.Plugins.Core.Meshers;

namespace Sutro.PathWorks.Plugins.Core.Visualizers
{
    public class VisualizerCore : VisualizerBase<PrintVertex>
    {
        protected readonly FixedRange customDataBeadWidth =
            new FixedRange(
                () => "Bead Width",
                (value) => $"{value:F2} mm", 0.1f, 0.8f);

        protected readonly AdaptiveRange customDataFeedRate =
            new AdaptiveRange(
                () => "Feed Rate",
                (value) => $"{value:F0} mm/min", ColorSpectrumFactory.CyanToPink());

        protected readonly NormalizedAdaptiveRange customDataCompletion =
            new NormalizedAdaptiveRange(
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
            : base(name, mapper, decompiler, mesher)
        {
            decompiler.OnToolpathComplete += ProcessToolpath;
            decompiler.OnNewLayer += RaiseNewLayer;
        }

        protected void ProcessToolpath(IToolpath toolpath)
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

        protected override ToolpathPreviewVertex VertexFactory(PrintVertex vertex, Vector3d position, float brightness)
        {
            // Update adaptive ranges for custom data
            customDataFeedRate.ObserveValue((float)vertex.FeedRate);
            customDataCompletion.ObserveValue(pointCount);

            return new ToolpathPreviewVertex(
                position, fillTypeInteger, layerIndex, color, brightness,
                new CustomColorData(vertex.Dimensions.x, vertex.FeedRate, pointCount)); ;
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
    }
}