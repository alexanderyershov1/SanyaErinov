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
using System.Data.SqlClient;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Mail;
using System.Net;

namespace Shebist
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        public AuthorizationPage()
        {
            InitializeComponent();
            WrongDataLabel.Visibility = Visibility.Hidden;
        }


        BinaryFormatter formatter = new BinaryFormatter();
        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

        static string Debug = Directory.GetCurrentDirectory();
        static string Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();
        static string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Shebist}\UserDB.mdf;Integrated Security=True";
        int userid;
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.CommandText = $"SELECT Id, Name, Email, Password FROM UserDB WHERE (Login = N'{LoginTextBox.Text}' OR Email = N'{LoginTextBox.Text}') AND Password = N'{PasswordBox.Password}'";
                command.Connection = connection;
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    userid = reader.GetInt32(0);

                    reader.Close();
                    MainPage mp = new MainPage();
                    this.NavigationService.Navigate(mp);
                }
                else
                {
                    reader.Close();
                    WrongDataLabel.Visibility = Visibility.Visible;
                }
            }

            using (FileStream fs = new FileStream($"{Debug}\\Data\\RememberMeCheckBoxIsChecked", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, RememberMeCheckBox.IsChecked);
            }

            using (FileStream fs = new FileStream($"{Debug}\\Data\\userid", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, userid);
            }
        }

        private void NoAccountYet_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegistrationPage rp = new RegistrationPage();
            this.NavigationService.Navigate(rp);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists($"{Debug}\\Data\\RememberMeCheckBoxIsChecked"))
            {
                using (FileStream fs = new FileStream($"{Debug}\\Data\\RememberMeCheckBoxIsChecked", FileMode.OpenOrCreate))
                {
                    RememberMeCheckBox.IsChecked = (bool)formatter.Deserialize(fs);
                }
            }

            if (RememberMeCheckBox.IsChecked == true)
            {
                if (File.Exists($"{Debug}\\Data\\userid"))
                {
                    using (FileStream fs = new FileStream($"{Debug}\\Data\\userid", FileMode.OpenOrCreate))
                    {
                        userid = (int)formatter.Deserialize(fs);
                    }

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        command.CommandText = $"SELECT Login, Password FROM UserDB WHERE Id = {userid}";
                        command.Connection = connection;
                        reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            reader.Read();
                            LoginTextBox.Text = reader.GetString(0);
                            PasswordBox.Password = reader.GetString(1);
                        }
                    }
                }
            }
        }

        private void DataRecoveryLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataRecoveryPage drp = new DataRecoveryPage();
            this.NavigationService.Navigate(drp);
        }

        private void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            WrongDataLabel.Visibility = Visibility.Hidden;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            WrongDataLabel.Visibility = Visibility.Hidden;
        }

        private void DataRecoveryLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            DataRecoveryLabel.FontSize = 13;
        }

        private void DataRecoveryLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            DataRecoveryLabel.FontSize = 12;
        }

        private void NoAccountYet_MouseEnter(object sender, MouseEventArgs e)
        {
            NoAccountYet.FontSize = 13;
        }

        private void NoAccountYet_MouseLeave(object sender, MouseEventArgs e)
        {
            NoAccountYet.FontSize = 12;
        }
    }
}
