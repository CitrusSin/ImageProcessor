using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace ImageProcessor
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        private void AppStartup(object sender, StartupEventArgs e)
        {
            IPCore.Startup();
        }

        private T GetParent<T>(UIElement element) where T : UIElement
        {
            var p = VisualTreeHelper.GetParent(element);
            while (!(p is T) && p != null)
            {
                p = VisualTreeHelper.GetParent(p);
            }
            return (T)p;
        }

        private void StylizedWindowTitleBarMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                GetParent<Window>((UIElement)sender).DragMove();
            }
        }

        private void StylizedWindowMinimizeClick(object sender, RoutedEventArgs e)
        {
            GetParent<Window>((UIElement)sender).WindowState = WindowState.Minimized;
        }

        private void StylizedWindowCloseClick(object sender, RoutedEventArgs e)
        {
            GetParent<Window>((UIElement)sender).Close();
        }
    }
}
