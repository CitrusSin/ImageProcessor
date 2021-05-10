using ImageProcessor.Processors;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageProcessor
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly List<Processors.ImageProcessor> processors = new List<Processors.ImageProcessor>();
        private System.Drawing.Image originalImage;
        private System.Drawing.Bitmap previewImage;
        private System.Drawing.Bitmap processedPreviewImage;

        private readonly Storyboard showTypeList;
        private readonly Storyboard hideTypeList;

        private readonly Storyboard showProcessorEdit;
        private readonly Storyboard hideProcessorEdit;

        private readonly Storyboard showLoadingTips;
        private readonly Storyboard hideLoadingTips;

        private bool listsMutualLock = false;

        private bool TypeListOpen
        {
            get => typeListGrid.IsHitTestVisible;
            set
            {
                if (value != TypeListOpen)
                {
                    if (!listsMutualLock && value)
                    {
                        listsMutualLock = true;
                        ProcessorEditOpen = false;
                        listsMutualLock = false;
                    }
                    if (value)
                    {
                        typeListGrid.IsHitTestVisible = true;
                        typeListGrid.Visibility = Visibility.Visible;
                        showTypeList.Begin();
                    }
                    else
                    {
                        typeListGrid.IsHitTestVisible = false;
                        hideTypeList.Begin();
                    }
                }
            }
        }

        private bool ProcessorEditOpen
        {
            get => processorEditGrid.IsHitTestVisible;
            set
            {
                if (value != ProcessorEditOpen)
                {
                    if (!listsMutualLock && value)
                    {
                        listsMutualLock = true;
                        TypeListOpen = false;
                        listsMutualLock = false;
                    }
                    if (value)
                    {
                        processorEditGrid.IsHitTestVisible = true;
                        processorEditGrid.Visibility = Visibility.Visible;
                        showProcessorEdit.Begin();
                    }
                    else
                    {
                        processorEditGrid.IsHitTestVisible = false;
                        hideProcessorEdit.Begin();
                    }
                }
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            showTypeList = (Storyboard)Resources["showTypeList"];
            hideTypeList = (Storyboard)Resources["hideTypeList"];
            showProcessorEdit = (Storyboard)Resources["showProcessorEdit"];
            hideProcessorEdit = (Storyboard)Resources["hideProcessorEdit"];
            showLoadingTips = (Storyboard)Resources["showLoadingTips"];
            hideLoadingTips = (Storyboard)Resources["hideLoadingTips"];
        }

        private void HideTypeListCompleted(object sender, EventArgs e)
        {
            typeListGrid.Visibility = Visibility.Collapsed;
        }

        private void HideProcessorEditCompleted(object sender, EventArgs e)
        {
            processorEditGrid.Visibility = Visibility.Collapsed;
            processorEditGrid.IsHitTestVisible = false;
        }

        private void AddProcessor(object sender, RoutedEventArgs e)
        {
            TypeListOpen = true;
            var types = IPCore.ProcessorTypes;
            var strs = types.Select((t) => IPCore.GetProcessorTypeName(t));
            typeList.ItemsSource = strs;
        }

        private void ConfirmAddProcessor(object sender, RoutedEventArgs e)
        {
            int index = typeList.SelectedIndex;
            if (index < 0)
                return;
            var p = IPCore.ProcessorTypes[index];
            var processor = (Processors.ImageProcessor)p.GetConstructor(new Type[0]).Invoke(new object[0]);
            processors.Add(processor);
            TypeListOpen = false;
            processorList.ItemsSource = processors.ToArray();
            UpdateProcessedImage();
        }

        private void CancelAddProcessor(object sender, RoutedEventArgs e)
        {
            TypeListOpen = false;
        }

        private void RemoveProcessor(object sender, RoutedEventArgs e)
        {
            ProcessorEditOpen = false;
            var index = processorList.SelectedIndex;
            if (index < 0)
                return;
            processors.RemoveAt(index);
            processorList.ItemsSource = processors.ToArray();
            UpdateProcessedImage();
        }

        private void AddNumAdjust(StackPanel sp, OptionAttribute attribute, FieldInfo opt, Processors.ImageProcessor p, int digitNum)
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
                    UpdateProcessedImage();
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
                            UpdateProcessedImage();
                        }
                        else if (box.Text == "")
                        {
                            opt.SetValue(p, 0);
                            UpdateProcessedImage();
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

        private void EditProcessor(object sender, RoutedEventArgs e)
        {
            processorStack.Children.Clear();
            var index = processorList.SelectedIndex;
            if (index < 0)
                return;
            Processors.ImageProcessor p = processors[index];
            var options = p.GetType().GetFields().Where((f) => f.GetCustomAttribute<OptionAttribute>() != null);
            foreach (FieldInfo opt in options)
            {
                var attribute = opt.GetCustomAttribute<OptionAttribute>();
                var name = attribute.Name;
                var type = opt.FieldType;
                StackPanel sp = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(5)
                };
                TextBlock tb = new TextBlock
                {
                    Text = name + "："
                };
                sp.Children.Add(tb);
                if (type == typeof(int))
                {
                    AddNumAdjust(sp, attribute, opt, p, 0);
                }
                else if (type == typeof(float))
                {
                    AddNumAdjust(sp, attribute, opt, p, 2);
                }
                else if (type == typeof(double))
                {
                    AddNumAdjust(sp, attribute, opt, p, 3);
                }
                else if (type == typeof(string))
                {
                    TextBox box = new TextBox
                    {
                        Text = opt.GetValue(p).ToString(),
                        MinWidth = 100
                    };
                    box.TextChanged += (s, args) =>
                    {
                        opt.SetValue(p, box.Text);
                        UpdateProcessedImage();
                    };
                    sp.Children.Add(box);
                }
                else if (type == typeof(System.Drawing.Point))
                {
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
                                UpdateProcessedImage();
                            }
                            else if (bX.Text == "")
                            {
                                point.X = 0;
                                opt.SetValue(p, point);
                                UpdateProcessedImage();
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
                                UpdateProcessedImage();
                            }
                            else if (bY.Text == "")
                            {
                                point.Y = 0;
                                opt.SetValue(p, point);
                                UpdateProcessedImage();
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
                }
                processorStack.Children.Add(sp);
            }
            ProcessorEditOpen = true;
        }

        private void ClearProcessor(object sender, RoutedEventArgs e)
        {
            ProcessorEditOpen = false;
            processors.Clear();
            processorList.ItemsSource = processors.ToArray();
        }

        private bool updateLock = false;

        private async void UpdateProcessedImage()
        {
            if (originalImage != null && !updateLock)
            {
                updateLock = true;
                loadingTips.Text = "更新预览图像中";
                showLoadingTips.Begin();
                await Task.Run(() =>
                {
                    try
                    {
                        processedPreviewImage = new System.Drawing.Bitmap(IPCore.MakeProcessedImage(previewImage, processors));
                    }
                    catch (Exception)
                    {
                        processedPreviewImage = previewImage;
                    }
                });
                imagePreview.Source = IPCore.GetBitmapSourceFromImage(processedPreviewImage);
                hideLoadingTips.Begin();
                updateLock = false;
            }
        }

        private void OpenImage(object sender, RoutedEventArgs e)
        {
            var ofp = new OpenFileDialog
            {
                Filter = "图片（PNG）|*.png|图片（BMP）|*.bmp|图片（JPG）|*.jpg",
                DefaultExt = ".png"
            };
            bool? result = ofp.ShowDialog(this);
            if (result == true)
            {
                originalImage = new System.Drawing.Bitmap(ofp.FileName);
                double wph = originalImage.Width / (double)originalImage.Height;
                System.Drawing.Size size1 = new System.Drawing.Size
                {
                    Width = (int)Math.Min(previewGrid.ActualWidth, originalImage.Width),
                };
                size1.Height = (int)(size1.Width / wph);
                System.Drawing.Size size2 = new System.Drawing.Size
                {
                    Height = (int)Math.Min(previewGrid.RowDefinitions[1].ActualHeight, originalImage.Width),
                };
                size2.Width = (int)(size2.Height * wph);
                previewImage = new System.Drawing.Bitmap(originalImage, (size1.Width > size2.Width) ? size1 : size2);
                UpdateProcessedImage();
            }
        }

        private async void SaveImage(object sender, RoutedEventArgs e)
        {
            var sfp = new SaveFileDialog
            {
                Filter = "图片（PNG）|*.png|图片（BMP）|*.bmp|图片（JPG）|*.jpg",
                DefaultExt = ".png"
            };
            bool? result = sfp.ShowDialog(this);
            if (result == true)
            {
                var filename = sfp.FileName;
                saveImageButton.IsEnabled = false;
                loadingTips.Text = "保存图像中";
                showLoadingTips.Begin();
                await Task.Run(() =>
                {
                    var processedImage = IPCore.MakeProcessedImage(originalImage, processors);
                    processedImage.Save(filename);
                });
                hideLoadingTips.Begin();
                saveImageButton.IsEnabled = true;
            }
        }
    }
}
