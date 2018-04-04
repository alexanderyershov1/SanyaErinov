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

namespace WpfApp1
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        double a, b, c, x1, x2, d;
        private void SolveButton_Click(object sender, RoutedEventArgs e)
        {
            SolveTextBox.Clear();
            a = Double.Parse(aTextBox.Text);
            b = Double.Parse(bTextBox.Text);
            c = Double.Parse(cTextBox.Text) - Double.Parse(resultTextBox.Text);
            d = Math.Pow(a, 2) - 4 * a * c;
            SolveTextBox.Text += $"D = {b}² - 4 * {a} * {c} = {d}\n";
            if (d > 0)
            {
                x1 = (-b + Math.Sqrt(d)) / (2 * a);
                x2 = (-b - Math.Sqrt(d)) / (2 * a);
                SolveTextBox.Text += $"x1 = ({-b} + √{d})/(2 * {a}) = {x1}\n";
                SolveTextBox.Text += $"x2 = ({-b} - √{d})/(2 * {a}) = {x2}\n";
            }
            else if (d == 0)
            {
                x1 = x2 = -b / (2 * a);
                SolveTextBox.Text += $"x = {-b}/(2 * {a}) = {x1}\n";
            }
            else
                SolveTextBox.Text += "Решений нет";
        }
    }
}
