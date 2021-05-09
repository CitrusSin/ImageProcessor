using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ImageProcessor.Processors
{
    public abstract class ImageProcessorPerPixel : ImageProcessor
    {
        protected abstract void ProcessPixel(ref byte a, ref byte r, ref byte g, ref byte b);

        protected virtual void StartProcess(Bitmap image) { }

        public override Image ProcessImage(Image source)
        {
            Bitmap retImage = new Bitmap(source);
            StartProcess(retImage);
            BitmapData data = retImage.LockBits(new Rectangle(new Point(0, 0), retImage.Size), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr baseAddr = data.Scan0;
            int fixlen = data.Stride - (data.Width * 4);
            byte[] rawData = new byte[data.Stride * data.Height];
            Marshal.Copy(baseAddr, rawData, 0, rawData.Length);
            int offset = 0;
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    ProcessPixel(
                        ref rawData[offset + 3],
                        ref rawData[offset + 2],
                        ref rawData[offset + 1],
                        ref rawData[offset]
                        );
                    offset += 4;
                }
                offset += fixlen;
            }
            Marshal.Copy(rawData, 0, baseAddr, rawData.Length);
            retImage.UnlockBits(data);
            return retImage;
        }
    }
}
