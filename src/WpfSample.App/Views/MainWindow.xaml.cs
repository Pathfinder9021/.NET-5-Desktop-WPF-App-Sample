using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfSample.App.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            RefreshMaximizeRestoreButton();

            // Subscribe on events
            StateChanged += WindowStateChanged;
        }

        public void OnMinimizeButtonClick(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        public void OnMaximizeRestoreButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {      
                this.WindowState = WindowState.Normal;
            }
            else
            {

                this.WindowState = WindowState.Maximized;
            }
        }

        public void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void WindowStateChanged(object sender, EventArgs e)
        {
            this.RefreshMaximizeRestoreButton();
        }

        private void RefreshMaximizeRestoreButton()
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.maximizeButton.Visibility = Visibility.Collapsed;
                this.restoreButton.Visibility = Visibility.Visible;
            }
            else
            {
                this.maximizeButton.Visibility = Visibility.Visible;
                this.restoreButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}
