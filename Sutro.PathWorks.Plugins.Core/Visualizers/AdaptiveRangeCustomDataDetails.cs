using g3;
using System;

namespace Sutro.PathWorks.Plugins.Core.Visualizers
{
    public class AdaptiveRangeCustomDataDetails : CustomDataDetails
    {
        protected Interval1d interval = Interval1d.Empty;

        public AdaptiveRangeCustomDataDetails(
            Func<string> labelF, Func<float, string> colorScaleLabelerF)
            : base(labelF, colorScaleLabelerF)
        {
        }

        public void ObserveValue(float value)
        {
            interval.Contain(value);
            RangeMin = (float)interval.a;
            RangeMax = (float)interval.b;
        }
    }
}