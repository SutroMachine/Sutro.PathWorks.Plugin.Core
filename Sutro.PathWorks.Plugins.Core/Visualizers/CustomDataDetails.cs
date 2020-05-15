using Sutro.PathWorks.Plugins.API.Visualizers;
using System;

namespace Sutro.PathWorks.Plugins.Core.Visualizers
{
    public abstract class CustomDataDetails : IVisualizerCustomDataDetails
    {
        public float RangeMin { get; protected set; }
        public float RangeMax { get; protected set; }

        private readonly Func<string> labelF;
        public virtual string Label { get => labelF(); }

        private Func<float, string> colorScaleLabelerF;

        public virtual string FormatColorScaleLabel(float value)
        {
            return colorScaleLabelerF(value);
        }

        public CustomDataDetails(Func<string> labelF, Func<float, string> colorScaleLabelerF, float rangeMin = 0, float rangeMax = 0)
        {
            this.labelF = labelF;
            this.colorScaleLabelerF = colorScaleLabelerF;

            RangeMin = rangeMin;
            RangeMax = rangeMax;
        }
    }
}