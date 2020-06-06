using System;

namespace Sutro.PathWorks.Plugins.Core.CustomData
{
    public class FixedRange : CustomDataBase
    {
        public FixedRange(
            Func<string> labelF, Func<float, string> colorScaleLabelerF,
            float rangeMin, float rangeMax)
            : base(labelF, colorScaleLabelerF, rangeMin, rangeMax)
        {
        }
    }
}