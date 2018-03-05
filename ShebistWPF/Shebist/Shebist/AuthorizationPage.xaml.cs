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
        }

        string cd = Directory.GetCurrentDirectory();
        BinaryFormatter formatter = new BinaryFormatter();
        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);


        static System.IO.DirectoryInfo myDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        static string parentDirectory = myDirectory.Parent.FullName;
        static System.IO.DirectoryInfo myDirectory2 = new DirectoryInfo(parentDirectory);
        static string parentDirectory2 = myDirectory2.Parent.FullName;

        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={parentDirectory2}\UserDB.mdf;Integrated Security=True";

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT Name, Email, Password FROM UserDB WHERE Login = N'{LoginTextBox.Text}' OR Email = N'{LoginTextBox.Text}' AND Password = N'{PasswordTextBox.Text}'", connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Close();
                    MainPage mp = new MainPage();
                    this.NavigationService.Navigate(mp);

                }
                else
                {
                    reader.Close();
                    MessageBox.Show("Неверный логин или пароль");
                }
            }
            
                using(FileStream fs = new FileStream($"{cd}\\Data\\RememberMeCheckBoxIsChecked", FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, RememberMeCheckBox.IsChecked);
                }

                using (FileStream fs = new FileStream($"{cd}\\Data\\LoginTextBoxText", FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, LoginTextBox.Text);
                }
                using (FileStream fs = new FileStream($"{cd}\\Data\\PasswordTextBoxText", FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, PasswordTextBox.Text);
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

            if(RememberMeCheckBox.IsChecked == true)
            {
                if (File.Exists($"{cd}\\Data\\LoginTextBoxText"))
                {
                    using (FileStream fs = new FileStream($"{cd}\\Data\\LoginTextBoxText", FileMode.OpenOrCreate))
                    {
                        LoginTextBox.Text = (string)formatter.Deserialize(fs);
                    }
                }
                if (File.Exists($"{cd}\\Data\\PasswordTextBoxText"))
                {
                    using (FileStream fs = new FileStream($"{cd}\\Data\\PasswordTextBoxText", FileMode.OpenOrCreate))
                    {
                        PasswordTextBox.Text = (string)formatter.Deserialize(fs);
                    }
                }
            }
            else
            {
                LoginTextBox.Text = "";
                PasswordTextBox.Text = "";
            }
        }

        private void DataRecoveryLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataRecoveryPage drp = new DataRecoveryPage();
            this.NavigationService.Navigate(drp);
        }
    }
}
