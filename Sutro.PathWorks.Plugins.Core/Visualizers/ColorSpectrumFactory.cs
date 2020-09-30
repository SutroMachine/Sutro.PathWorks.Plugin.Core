using Sutro.PathWorks.Plugins.API.Visualizers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Sutro.PathWorks.Plugins.Core.Visualizers
{
    public static class ColorSpectrumFactory
    {
        public static ColorSpectrum CyanToPink()
        {
            return new ColorSpectrum(
                Color.FromArgb(110, 246, 250),
                Color.FromArgb(38, 238, 255),
                Color.FromArgb(0, 229, 255),
                Color.FromArgb(0, 218, 255),
                Color.FromArgb(0, 206, 255),
                Color.FromArgb(0, 191, 255),
                Color.FromArgb(0, 173, 255),
                Color.FromArgb(116, 152, 255),
                Color.FromArgb(170, 125, 255),
                Color.FromArgb(213, 90, 255),
                Color.FromArgb(245, 23, 224));
        }
    }
}
