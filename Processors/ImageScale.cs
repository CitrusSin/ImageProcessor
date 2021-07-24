using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace ImageProcessor.Processors
{
    [ImageProcessor("缩放图片")]
    public class ImageScale : Processor
    {
        [Option("缩放倍数", Minimum = 0d)]
        public double Scale = 1d;
        public override Image ProcessImage(Image source)
        {
            Image scaled = new Bitmap(
                source,
                new Size((int)(source.Width * Scale), (int)(source.Height * Scale))
            );
            return scaled;
        }
    }
}
