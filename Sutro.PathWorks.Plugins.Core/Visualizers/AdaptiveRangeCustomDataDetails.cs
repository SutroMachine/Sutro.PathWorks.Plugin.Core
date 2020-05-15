using g3;
using System;

namespace Sutro.PathWorks.Plugins.Core.Visualizers
{
    public class AdaptiveRangeCustomDataDetails : CustomDataDetails
    {
        protected Interval1d interval = Interval1d.Empty;

        public override float RangeMin
        {
            get => (float)interval.a;
            protected set => interval.a = value;
        }

        public override float RangeMax
        {
            get => (float)interval.b;
            protected set => interval.b = value;
        }

        public AdaptiveRangeCustomDataDetails(
            Func<string> labelF, Func<float, string> colorScaleLabelerF)
            : base(labelF, colorScaleLabelerF)
        {
        }

        public void ObserveValue(float value)
        {
            interval.Contain(value);
        }
    }
}