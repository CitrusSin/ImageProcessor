using ImageProcessor.Processors;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;

namespace ImageProcessor
{
    public sealed class IPCore
    {
        private static List<Type> processorTypes = new List<Type>();

        public static Type[] ProcessorTypes => processorTypes.ToArray();

        public static void Startup()
        {
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (Type type in types)
            {
                if (type.GetCustomAttributes(true).Any((a) => a is ImageProcessorAttribute))
                {
                    processorTypes.Add(type);
                }
            }
        }

        public static string GetProcessorTypeName(Type p)
        {
            var attr = p.GetCustomAttribute<ImageProcessorAttribute>(true);
            if (attr != null)
            {
                return attr.Name;
            }
            else
            {
                throw new ArgumentNullException("Type has no attributes of ImageProcessor");
            }
        }

        public static Image MakeProcessedImage(Image source, IEnumerable<Processors.ImageProcessor> processors)
        {
            var im = source;
            foreach (var p in processors)
            {
                im = p.ProcessImage(im);
            }
            return im;
        }

        [DllImport("gdi32")]
        private static extern bool DeleteObject(IntPtr obj);

        public static BitmapSource GetBitmapSourceFromImage(Image originalImage)
        {
            Bitmap bitmap = new Bitmap(originalImage);
            IntPtr hBits = bitmap.GetHbitmap();
            BitmapSource bs = Imaging.CreateBitmapSourceFromHBitmap(
                hBits,
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
            DeleteObject(hBits);
            return bs;
        }
    }
}
