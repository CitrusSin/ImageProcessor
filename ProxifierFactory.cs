using ImageProcessor.Processors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ImageProcessor
{
    public class ProxifierFactory
    {
        public static void RegisterProxifiers()
        {
            ProxifierManager.AddProxifier(new NumProxifier());
            ProxifierManager.AddProxifier(new PointProxifier());
            ProxifierManager.AddProxifier(new ImageProxifier());
            ProxifierManager.AddProxifier(new SimpleProxifier<string>((attribute, opt, p, updater) =>
            {
                StackPanel sp = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };
                TextBox box = new TextBox
                {
                    Text = opt.GetValue(p).ToString(),
                    MinWidth = 100
                };
                box.TextChanged += (s, args) =>
                {
                    opt.SetValue(p, box.Text);
                    updater();
                };
                sp.Children.Add(box);
                return sp;
            }));
        }

    }
}