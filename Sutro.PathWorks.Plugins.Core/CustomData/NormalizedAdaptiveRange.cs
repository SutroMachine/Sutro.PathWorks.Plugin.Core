using Sutro.PathWorks.Plugins.API.Visualizers;
using System;

namespace Sutro.PathWorks.Plugins.Core.CustomData
{
    public class NormalizedAdaptiveRange : AdaptiveRange
    {
        public NormalizedAdaptiveRange(
            Func<string> labelF, Func<float, string> colorScaleLabelerF, ColorSpectrum spectrum = null) : base(labelF, colorScaleLabelerF, spectrum)
        {
        }

        public override string FormatColorScaleLabel(float value)
        {
            return base.FormatColorScaleLabel((float)interval.GetT(value));
        }
    }
}