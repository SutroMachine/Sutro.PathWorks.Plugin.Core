using Sutro.PathWorks.Plugins.API.Visualizers;
using System;

namespace Sutro.PathWorks.Plugins.Core.CustomData
{
    public abstract class CustomDataBase : IVisualizerCustomDataDetails
    {
        public float RangeMin { get; protected set; }
        public float RangeMax { get; protected set; }

        private readonly Func<string> labelF;
        public virtual string Label { get => labelF(); }

        private Func<float, string> colorScaleLabelerF;
        private readonly ColorSpectrum spectrum;

        public virtual string FormatColorScaleLabel(float value)
        {
            return colorScaleLabelerF(value);
        }

        public CustomDataBase(Func<string> labelF, Func<float, string> colorScaleLabelerF, float rangeMin = 0, float rangeMax = 0, ColorSpectrum spectrum = null)
        {
            this.labelF = labelF;
            this.colorScaleLabelerF = colorScaleLabelerF;

            RangeMin = rangeMin;
            RangeMax = rangeMax;
            this.spectrum = spectrum;
        }

        public ColorSpectrum GetSpectrum()
        {
            return spectrum;
        }
    }
}