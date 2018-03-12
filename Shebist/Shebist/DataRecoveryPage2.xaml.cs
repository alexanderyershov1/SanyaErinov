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

namespace Shebist
{
    /// <summary>
    /// Логика взаимодействия для DataRecoveryPage2.xaml
    /// </summary>
    public partial class DataRecoveryPage2 : Page
    {
        public DataRecoveryPage2()
        {
            InitializeComponent();
        }

        private void AgainPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(NewPasswordBox.Password != "" && NewPasswordBox.Password == AgainPasswordBox.Password)
            {
                CheckPasswodsLabel.Foreground = Brushes.Green;
                CheckPasswodsLabel.Content = "Пароли совпадают";
            }
            else
            {
                CheckPasswodsLabel.Foreground = Brushes.Red;
                CheckPasswodsLabel.Content = "Пароли не совпадают";
            }
        }

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (NewPasswordBox.Password != "" && NewPasswordBox.Password == AgainPasswordBox.Password)
            {
                CheckPasswodsLabel.Foreground = Brushes.Green;
                CheckPasswodsLabel.Content = "Пароли совпадают";
            }
            else
            {
                CheckPasswodsLabel.Foreground = Brushes.Red;
                CheckPasswodsLabel.Content = "Пароли не совпадают";
            }
        }
    }
}
