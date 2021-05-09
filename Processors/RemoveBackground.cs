using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageProcessor.Processors
{
    [ImageProcessor("去背景")]
    public class RemoveBackground : ImageProcessorPerPixel
    {
        [Option("参照像素坐标")]
        public Point basePoint = Point.Empty;
        Color pointColor;

        [Option("感应阈值", Minimum = 0, Maximum = 255)]
        public int maxOffset = 15;
        [Option("抹除阈值", Minimum = 0, Maximum = 255)]
        public int minOffset = 10;

        protected override void ProcessPixel(ref byte a, ref byte r, ref byte g, ref byte b)
        {
            if (Math.Abs(pointColor.R - r) < maxOffset && Math.Abs(pointColor.G - g) < maxOffset && Math.Abs(pointColor.B - b) < maxOffset)
            {
                int avr = (Math.Abs(pointColor.R - r) + Math.Abs(pointColor.G - g) + Math.Abs(pointColor.B - b)) / 3;
                if (avr < minOffset)
                {
                    a = 0;
                }
                else
                {
                    a = (byte)avr;
                }
            }
        }

        public override Image ProcessImage(Image source)
        {
            using (Bitmap b = new Bitmap(source))
            {
                pointColor = b.GetPixel(basePoint.X, basePoint.Y);
            }
            return base.ProcessImage(source);
        }
    }
}
