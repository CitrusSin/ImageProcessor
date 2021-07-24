using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ImageProcessor.Processors;
using Microsoft.Win32;

namespace ImageProcessor
{
    public class ImageProxifier : Proxifier
    {
        public override StackPanel GetPanel(Window parent, OptionAttribute attribute, FieldInfo opt, Processor p, Action updater)
        {
            StackPanel sp = new StackPanel
            {
                Orientation = Orientation.Horizontal,
            };
            TextBlock pathText = new TextBlock
            {
                Text = "待打开图片"
            };
            Button button = new Button
            {
                Content = "打开图片"
            };
            button.Click += (sender, arg) =>
            {
                var ofp = new OpenFileDialog
                {
                    Filter = "图片（PNG）|*.png|图片（BMP）|*.bmp|图片（JPG）|*.jpg",
                    DefaultExt = ".png"
                };
                bool? result = ofp.ShowDialog(parent);
                if (result == true)
                {
                    pathText.Text = ofp.FileName;
                    Bitmap image = new Bitmap(ofp.FileName);
                    opt.SetValue(p, image);
                    updater();
                }
            };
            sp.Children.Add(pathText);
            sp.Children.Add(button);
            return sp;
        }

        public override IEnumerable<Type> GetProxyTypes()
        {
            return new Type[] { typeof(System.Drawing.Image), typeof(Bitmap) };
        }
    }
}
