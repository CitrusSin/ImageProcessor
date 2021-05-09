using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageProcessor.Processors
{
    [ImageProcessor("负片处理")]
    public class Reverse : ImageProcessorPerPixel
    {
        protected override void ProcessPixel(ref byte a, ref byte r, ref byte g, ref byte b)
        {
            r = (byte)(byte.MaxValue - r);
            g = (byte)(byte.MaxValue - g);
            b = (byte)(byte.MaxValue - b);
        }
    }
}
