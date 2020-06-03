using g3;
using System;

namespace Sutro.PathWorks.Plugins.Core.Visualizers
{
    public class AdaptiveRangeCustomDataDetails : CustomDataDetails
    {
        protected Interval1d interval;

        public AdaptiveRangeCustomDataDetails(
            Func<string> labelF, Func<float, string> colorScaleLabelerF)
            : base(labelF, colorScaleLabelerF)
        {
            Reset();
        }

        public void ObserveValue(float value)
        {
            interval.Contain(value);
            RangeMin = (float)interval.a;
            RangeMax = (float)interval.b;
        }

        public void Reset()
        {
            interval = Interval1d.Empty;
        }
    }
}