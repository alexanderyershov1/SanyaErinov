using System;
using System.Collections.Generic;
using System.IO;
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
        Canvas canvas;
        public SettingsWindow(Canvas canvas)
        {
            InitializeComponent();
            this.canvas = canvas;
        }

        private void MainWindowOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //mp.Opacity = MainWindowOpacitySlider.Value;
        }

        private void NextBackButtonsOpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //mp.NextButton.Opacity = mp.BackButton.Opacity = NextBackButtonsOpacitySlider.Value;
        }

        private void BackgroundButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog();
            string fileName = ofd.FileName;
            byte[] imageData;
            if(ofd.FileName != "")
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    imageData = new byte[fs.Length];
                    fs.Read(imageData, 0, imageData.Length);
                }
                //MessageBox.Show(imageData);
                ImageBrush background = new ImageBrush();
                background.ImageSource = new BitmapImage(new Uri(fileName, UriKind.Absolute));
                canvas.Background = background;
            }         
        }
    }
}
