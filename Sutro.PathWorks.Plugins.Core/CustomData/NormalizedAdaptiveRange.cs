using System;

namespace Sutro.PathWorks.Plugins.Core.CustomData
{
    public class NormalizedAdaptiveRange : AdaptiveRange
    {
        public NormalizedAdaptiveRange(
            Func<string> labelF, Func<float, string> colorScaleLabelerF) : base(labelF, colorScaleLabelerF)
        {
        }

        public override string FormatColorScaleLabel(float value)
        {
            return base.FormatColorScaleLabel((float)interval.GetT(value));
        }
    }
}