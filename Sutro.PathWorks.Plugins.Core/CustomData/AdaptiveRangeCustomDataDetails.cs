using g3;
using Sutro.PathWorks.Plugins.API.Visualizers;
using System;

namespace Sutro.PathWorks.Plugins.Core.CustomData
{
    public class AdaptiveRange : CustomDataBase
    {
        protected Interval1d interval;

        public AdaptiveRange(
            Func<string> labelF, Func<float, string> colorScaleLabelerF, ColorSpectrum spectrum = null)
            : base(labelF, colorScaleLabelerF, spectrum: spectrum)
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