using System;

namespace Sutro.PathWorks.Plugins.Core.Visualizers
{
    public class FixedRangeCustomDataDetails : CustomDataDetails
    {
        public FixedRangeCustomDataDetails(
            Func<string> labelF, Func<float, string> colorScaleLabelerF,
            float rangeMin, float rangeMax)
            : base(labelF, colorScaleLabelerF)
        {
            RangeMin = rangeMin;
            RangeMax = rangeMax;
        }
    }
}