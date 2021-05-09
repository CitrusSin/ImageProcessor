using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageProcessor.Processors
{
    [ImageProcessor("调节饱和度")]
    public class SaturationAdjust : ImageProcessorPerPixel
    {
        [Option("调节倍数", Minimum = 0d, Maximum = 3d)]
        public double multiple = 1d;

        protected override void ProcessPixel(ref byte a, ref byte r, ref byte g, ref byte b)
        {
            int avr = (r + g + b) / 3;
            int rOffset = r - avr;
            int gOffset = g - avr;
            int bOffset = b - avr;
            int R = avr + (int)(rOffset * multiple);
            int G = avr + (int)(gOffset * multiple);
            int B = avr + (int)(bOffset * multiple);
            r = (byte)Math.Max(Math.Min(R, byte.MaxValue), byte.MinValue);
            g = (byte)Math.Max(Math.Min(G, byte.MaxValue), byte.MinValue);
            b = (byte)Math.Max(Math.Min(B, byte.MaxValue), byte.MinValue);
        }
    }
}
