using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor.Processors
{
    [ImageProcessor("添加图层")]
    public class PutLayer : ImageProcessorPerPixel
    {
        [Option("覆盖图片")]
        public Bitmap imageLayer;

        [Option("透明度倍数", Minimum = 0d, Maximum = 1d, SlowUpdate = true)]
        public double alphaMul = 1d;

        private Bitmap sizedImage;

        protected override void StartProcess(Bitmap image)
        {
            sizedImage = new Bitmap(imageLayer, image.Size);
        }

        protected override void ProcessPixel(ref byte a0, ref byte r0, ref byte g0, ref byte b0)
        {
            double a1 = a0 / 255d, r1 = r0 / 255d, g1 = g0 / 255d, b1 = b0 / 255d;
            Color color = sizedImage.GetPixel(X, Y);
            double a2 = color.A / 255d, r2 = color.R / 255d, g2 = color.G / 255d, b2 = color.B / 255d;
            a2 *= alphaMul;
            double a = a2 + a1 - (a1 * a2);
            double r = ((a2 * r2) + a1 * (1 - a2) * r1) / a;
            double g = ((a2 * g2) + a1 * (1 - a2) * g1) / a;
            double b = ((a2 * b2) + a1 * (1 - a2) * b1) / a;
            a0 = (byte)(a * 255);
            r0 = (byte)(r * 255);
            g0 = (byte)(g * 255);
            b0 = (byte)(b * 255);
        }
    }
}
