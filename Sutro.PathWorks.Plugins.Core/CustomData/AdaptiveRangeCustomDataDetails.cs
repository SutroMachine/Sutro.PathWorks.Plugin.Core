﻿using g3;
using System;

namespace Sutro.PathWorks.Plugins.Core.CustomData
{
    public class AdaptiveRange : CustomDataBase
    {
        protected Interval1d interval;

        public AdaptiveRange(
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