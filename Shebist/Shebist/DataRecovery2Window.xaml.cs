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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Data.SqlClient;

namespace Shebist
{
    /// <summary>
    /// Логика взаимодействия для DataRecoveryPage2.xaml
    /// </summary>
    public partial class DataRecovery2Window : Window
    {
        public DataRecovery2Window()
        {
            InitializeComponent();
            CheckPasswodsLabel.Content = "";
        }

        static string Debug = Directory.GetCurrentDirectory();
        static string Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();
        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Shebist}\UserDB.mdf;Integrated Security=True";
        public string login;
        BinaryFormatter formatter = new BinaryFormatter();

        private void AgainPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
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

        private void NewPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(AgainPasswordBox.Password != "")
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

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckPasswodsLabel.Content.ToString() == "Пароли совпадают")
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand($"UPDATE Users SET Password = N'{NewPasswordBox.Password}' WHERE Login = N'{login}'", connection);
                    command.ExecuteNonQuery();
                }
                MessageBox.Show("Пароль изменён");
                AuthorizationWindow aw = new AuthorizationWindow();
                aw.Show();
                this.Close();
            }
        }
    }
}
