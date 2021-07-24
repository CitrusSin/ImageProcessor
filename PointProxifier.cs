using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ImageProcessor.Processors;

namespace ImageProcessor
{
    public class PointProxifier : MonotypedProxifier<System.Drawing.Point>
    {
        public override StackPanel GetPanel(Window parent, OptionAttribute attribute, FieldInfo opt, Processor p, Action updater)
        {
            StackPanel sp = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            TextBox bX = new TextBox
            {
                Text = ((System.Drawing.Point)opt.GetValue(p)).X.ToString(),
                MinWidth = 100
            };
            TextBox bY = new TextBox
            {
                Text = ((System.Drawing.Point)opt.GetValue(p)).Y.ToString(),
                MinWidth = 100
            };
            bool controlLockX = false, controlLockY = false;
            bX.TextChanged += (s, args) =>
            {
                if (!controlLockX)
                {
                    var point = (System.Drawing.Point)opt.GetValue(p);
                    if (int.TryParse(bX.Text, out int val))
                    {
                        point.X = val;
                        opt.SetValue(p, point);
                        updater();
                    }
                    else if (bX.Text == "")
                    {
                        point.X = 0;
                        opt.SetValue(p, point);
                        updater();
                    }
                    else
                    {
                        controlLockX = true;
                        bX.Text = point.X.ToString();
                        controlLockX = false;
                    }
                }
            };
            bY.TextChanged += (s, args) =>
            {
                if (!controlLockY)
                {
                    var point = (System.Drawing.Point)opt.GetValue(p);
                    if (int.TryParse(bY.Text, out int val))
                    {
                        point.Y = val;
                        opt.SetValue(p, point);
                        updater();
                    }
                    else if (bY.Text == "")
                    {
                        point.Y = 0;
                        opt.SetValue(p, point);
                        updater();
                    }
                    else
                    {
                        controlLockY = true;
                        bY.Text = point.Y.ToString();
                        controlLockY = false;
                    }
                }
            };
            TextBlock comma = new TextBlock
            {
                Text = ", "
            };
            sp.Children.Add(bX);
            sp.Children.Add(comma);
            sp.Children.Add(bY);
            return sp;
        }
    }
}
