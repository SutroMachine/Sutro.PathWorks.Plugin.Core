using System;

namespace Sutro.PathWorks.Plugins.Core.Visualizers
{
    public class NormalizedAdaptiveRangeCustomDataDetails : AdaptiveRangeCustomDataDetails
    {
        public NormalizedAdaptiveRangeCustomDataDetails(
            Func<string> labelF, Func<float, string> colorScaleLabelerF) : base(labelF, colorScaleLabelerF)
        {
        }

        public override string FormatColorScaleLabel(float value)
        {
            return base.FormatColorScaleLabel((float)interval.GetT(value));
        }
    }
}