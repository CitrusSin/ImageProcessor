using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ImageProcessor.Processors
{
    [ImageProcessor("将图像转化为带颜色文本图案")]
    public class ImageToColoredText : ImageProcessor
    {
        [Option("替代文本")]
        public string transferText = "TEST";

        public override Image ProcessImage(Image source)
        {
            int fontsize = 10;
            Size sourceSize = new Size(source.Width / fontsize, source.Height / fontsize);
            Bitmap retImage = new Bitmap(sourceSize.Width * fontsize, sourceSize.Height * fontsize);
            using (Graphics graph = Graphics.FromImage(retImage))
            {
                graph.Clear(Color.Black);
                Bitmap sourceImage = new Bitmap(source, sourceSize);
                BitmapData data = sourceImage.LockBits(
                    new Rectangle(new Point(0, 0), sourceImage.Size),
                    ImageLockMode.ReadOnly,
                    PixelFormat.Format32bppArgb
                );
                IntPtr baseAddr = data.Scan0;
                int fixlen = data.Stride - (data.Width * 4);
                byte[] rawData = new byte[data.Stride * data.Height];
                Marshal.Copy(baseAddr, rawData, 0, rawData.Length);
                int pixelCount = 0;
                int offset = 0;
                Font f = new Font(FontFamily.GenericMonospace, fontsize, GraphicsUnit.Pixel);
                for (int y = 0; y < data.Height; y++)
                {
                    for (int x = 0; x < data.Width; x++)
                    {
                        int a = rawData[offset + 3];
                        int r = rawData[offset + 2];
                        int g = rawData[offset + 1];
                        int b = rawData[offset];
                        graph.DrawString(
                            transferText[pixelCount % transferText.Length].ToString(),
                            f,
                            new SolidBrush(Color.FromArgb(a, r, g, b)),
                            x*fontsize,
                            y*fontsize
                        );
                        pixelCount++;
                        offset += 4;
                    }
                    offset += fixlen;
                }
                sourceImage.UnlockBits(data);
            }
            return retImage;
        }
    }
}
