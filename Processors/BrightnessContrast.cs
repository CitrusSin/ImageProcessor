using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor.Processors
{
    [ImageProcessor("亮度与对比度调节")]
    public class BrightnessContrast : ImageProcessorPerPixel
    {
        [Option("亮度偏移", Maximum = 1d, Minimum = -1d)]
        public double Brightness = 0d;

        [Option("对比度偏移", Maximum = 1d, Minimum = -1d)]
        public double Contrast = 0d;

        private double LimitRange(double num, double min, double max)
        {
            return Math.Max(min, Math.Min(max, num));
        }

        private double k;

        protected override void StartProcess(Bitmap image)
        {
            k = Math.Tan((45 + 44 * Contrast) / 180d * Math.PI);
        }

        protected override void ProcessPixel(ref byte a, ref byte r, ref byte g, ref byte b)
        {
            r = (byte)LimitRange((r - 127.5 * (1 - Brightness)) * k + 127.5 * (1 + Brightness), 0, 255);
            g = (byte)LimitRange((g - 127.5 * (1 - Brightness)) * k + 127.5 * (1 + Brightness), 0, 255);
            b = (byte)LimitRange((b - 127.5 * (1 - Brightness)) * k + 127.5 * (1 + Brightness), 0, 255);
        }
    }
}