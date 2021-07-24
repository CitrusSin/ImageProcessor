using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ImageProcessor.Processors;

namespace ImageProcessor
{
    public class NumProxifier : Proxifier
    {
        public override StackPanel GetPanel(Window parent, OptionAttribute attribute, FieldInfo opt, Processor p, Action updater)
        {
            var type = opt.FieldType;
            StackPanel sp = new StackPanel
            {
                Orientation = Orientation.Horizontal,
            };
            if (type == typeof(int))
            {
                AddNumAdjust(sp, attribute, opt, p, updater, 0);
            }
            else if (type == typeof(float))
            {
                AddNumAdjust(sp, attribute, opt, p, updater, 2);
            }
            else if (type == typeof(double))
            {
                AddNumAdjust(sp, attribute, opt, p, updater, 3);
            }
            return sp;
        }

        public override IEnumerable<Type> GetProxyTypes()
        {
            return new Type[] { typeof(int), typeof(float), typeof(double) };
        }

        private void AddNumAdjust(StackPanel sp, OptionAttribute attribute, FieldInfo opt, Processor p, Action updater, int digitNum = 2)
        {
            Type optType = opt.FieldType;
            if (attribute.Maximum.GetType() == optType && attribute.Minimum.GetType() == optType)
            {
                Slider slider = new Slider
                {
                    TickFrequency = 1d,
                    MinWidth = 200
                };
                if (optType == typeof(int))
                {
                    slider.Minimum = (int)attribute.Minimum;
                    slider.Maximum = (int)attribute.Maximum;
                    slider.Value = (int)opt.GetValue(p);
                }
                else if (optType == typeof(float))
                {
                    slider.Minimum = (float)attribute.Minimum;
                    slider.Maximum = (float)attribute.Maximum;
                    slider.Value = (float)opt.GetValue(p);
                }
                else if (optType == typeof(double))
                {
                    slider.Minimum = (double)attribute.Minimum;
                    slider.Maximum = (double)attribute.Maximum;
                    slider.Value = (double)opt.GetValue(p);
                }
                slider.ValueChanged += (s, args) =>
                {
                    if (optType == typeof(int))
                    {
                        opt.SetValue(p, (int)slider.Value);
                    }
                    else if (optType == typeof(float))
                    {
                        opt.SetValue(p, (float)slider.Value);
                    }
                    else if (optType == typeof(double))
                    {
                        opt.SetValue(p, (double)slider.Value);
                    }
                    updater();
                };
                TextBlock digit = new TextBlock();
                var digitBinding = new Binding("Value")
                {
                    Source = slider,
                    StringFormat = "{0:F" + digitNum.ToString() + "}"
                };
                digit.SetBinding(TextBlock.TextProperty, digitBinding);
                sp.Children.Add(slider);
                sp.Children.Add(digit);
            }
            else
            {
                TextBox box = new TextBox
                {
                    Text = opt.GetValue(p).ToString(),
                    MinWidth = 100
                };
                bool controlLock = false;
                box.TextChanged += (s, args) =>
                {
                    if (!controlLock)
                    {
                        if (double.TryParse(box.Text, out double val))
                        {
                            if (optType == typeof(int))
                            {
                                opt.SetValue(p, (int)val);
                            }
                            else if (optType == typeof(float))
                            {
                                opt.SetValue(p, (float)val);
                            }
                            else if (optType == typeof(double))
                            {
                                opt.SetValue(p, (double)val);
                            }
                            updater();
                        }
                        else if (box.Text == "")
                        {
                            opt.SetValue(p, 0);
                            updater();
                        }
                        else
                        {
                            controlLock = true;
                            box.Text = opt.GetValue(p).ToString();
                            controlLock = false;
                        }
                    }
                };
                sp.Children.Add(box);
            }
        }
    }
}
