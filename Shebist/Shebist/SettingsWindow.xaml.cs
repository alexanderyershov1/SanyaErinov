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
using System.Windows.Shapes;
using Microsoft.Win32;

namespace Shebist
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        MainPage mp;
        public SettingsWindow(MainPage mp)
        {
            InitializeComponent();
            this.mp = mp;
            MainWindowOpacitySlider.Value = mp.Opacity;
            NextBackButtonsOpacitySlider.Value = mp.NextButton.Opacity;
        }

        private void MainWindowOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mp.Opacity = MainWindowOpacitySlider.Value;
        }

        private void NextBackButtonsOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mp.NextButton.Opacity = mp.BackButton.Opacity = NextBackButtonsOpacitySlider.Value;
        }

        private void BackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            string fileName = ofd.FileName;
            ImageBrush background = new ImageBrush();
            background.ImageSource = new BitmapImage(new Uri(fileName));
            mp.Background = background;
        }
    }
}
