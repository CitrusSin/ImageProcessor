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
        private readonly List<Processors.Processor> processors = new List<Processors.Processor>();
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
            var processor = (Processor)p.GetConstructor(new Type[0]).Invoke(new object[0]);
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

        private void EditProcessor(object sender, RoutedEventArgs e)
        {
            processorStack.Children.Clear();
            var index = processorList.SelectedIndex;
            if (index < 0)
                return;
            Processor p = processors[index];
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
                Proxifier proxy = ProxifierManager.GetProxifier(type);
                sp.Children.Add(proxy.GetPanel(this, attribute, opt, p, UpdateProcessedImage));
                processorStack.Children.Add(sp);
            }
            ProcessorEditOpen = true;
        }

        private void ClearProcessor(object sender, RoutedEventArgs e)
        {
            ProcessorEditOpen = false;
            processors.Clear();
            processorList.ItemsSource = processors.ToArray();
            UpdateProcessedImage();
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
