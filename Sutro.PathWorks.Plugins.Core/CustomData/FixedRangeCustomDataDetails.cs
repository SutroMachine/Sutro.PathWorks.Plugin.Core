using System;

namespace Sutro.PathWorks.Plugins.Core.CustomData
{
    public class FixedRangeCustomDataDetails : CustomDataDetails
    {
        public FixedRangeCustomDataDetails(
            Func<string> labelF, Func<float, string> colorScaleLabelerF,
            float rangeMin, float rangeMax)
            : base(labelF, colorScaleLabelerF, rangeMin, rangeMax)
        {
        }
    }
}