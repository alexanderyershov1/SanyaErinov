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

        string cd = Directory.GetCurrentDirectory();
        BinaryFormatter formatter = new BinaryFormatter();
        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

        static System.IO.DirectoryInfo myDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        static string parentDirectory = myDirectory.Parent.FullName;
        static System.IO.DirectoryInfo myDirectory2 = new DirectoryInfo(parentDirectory);
        static string parentDirectory2 = myDirectory2.Parent.FullName;

        static string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={parentDirectory2}\UserDB.mdf;Integrated Security=True";
        int userid;

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT Id, Name, Email, Password FROM UserDB WHERE (Login = N'{LoginTextBox.Text}' OR Email = N'{LoginTextBox.Text}') AND Password = N'{PasswordBox.Password} '", connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        userid = (int)reader.GetValue(0);
                    }

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
        
            using (FileStream fs = new FileStream($"{cd}\\Data\\RememberMeCheckBoxIsChecked", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, RememberMeCheckBox.IsChecked);
            }

            using (FileStream fs = new FileStream($"{cd}\\Data\\userid", FileMode.OpenOrCreate))
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
            if (File.Exists($"{cd}\\Data\\RememberMeCheckBoxIsChecked"))
            {
                using (FileStream fs = new FileStream($"{cd}\\Data\\RememberMeCheckBoxIsChecked", FileMode.OpenOrCreate))
                {
                    RememberMeCheckBox.IsChecked = (bool)formatter.Deserialize(fs);
                }
            }

            if (RememberMeCheckBox.IsChecked == true)
            {
                if (File.Exists($"{cd}\\Data\\userid"))
                {
                    using (FileStream fs = new FileStream($"{cd}\\Data\\userid", FileMode.OpenOrCreate))
                    {
                        userid = (int)formatter.Deserialize(fs);
                    }

                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand($"SELECT Login, Password FROM UserDB WHERE Id = {userid}", connection);
                        SqlDataReader reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            LoginTextBox.Text = (string)reader.GetValue(0);
                            PasswordBox.Password = (string)reader.GetValue(1);
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
