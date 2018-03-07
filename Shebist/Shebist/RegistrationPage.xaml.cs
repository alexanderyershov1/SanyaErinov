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
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.IO;

namespace Shebist
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
            ConfirmRegistrationLabel.Visibility = Visibility.Hidden;
            ConfirmRegistrationTextBox.Visibility = Visibility.Hidden;
        }
        

        string cd = Directory.GetCurrentDirectory();
        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

        Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        static System.IO.DirectoryInfo myDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        static string parentDirectory = myDirectory.Parent.FullName;
        static System.IO.DirectoryInfo myDirectory2 = new DirectoryInfo(parentDirectory);
        static string parentDirectory2 = myDirectory2.Parent.FullName;

        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={parentDirectory2}\UserDB.mdf;Integrated Security=True";

        MailAddress from = new MailAddress("alexanderyershov1@gmail.com", "Шебист");
        string randomCode;
        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            
            if(CheckLoginLabel.Content.ToString() == "Данный логин свободен" && CheckNameLabel.Visibility == Visibility.Hidden
                && CheckEmailLabel.Content.ToString() == "Данная почта свободна" && CheckPasswordLabel.Visibility == Visibility.Hidden)
            {
                randomCode = RandomString(5);

                MailMessage mailMessage = new MailMessage(from, to);
                mailMessage.Subject = "Подтверждение регистрации";
                mailMessage.Body = $"{NameTextBox.Text}, код для подтверждения регистрации: {randomCode}" +
                    $"\n\nЕсли вы нигде не регистрировались, проигнорируйте это письмо.";
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("alexanderyershov1@gmail.com", "Death4000$");
                smtp.Send(mailMessage);


                LoginLabel.Visibility = LoginTextBox.Visibility = NameLabel.Visibility = NameTextBox.Visibility =
                    EmailLabel.Visibility = EmailTextBox.Visibility = PasswordLabel.Visibility = PasswordTextBox.Visibility =
                    CheckLoginLabel.Visibility = CheckEmailLabel.Visibility = Visibility.Hidden;
                ConfirmRegistrationLabel.Visibility = ConfirmRegistrationTextBox.Visibility = Visibility.Visible;
                ConfirmRegistrationLabel.Content = $"Введите код, отправленный на {EmailTextBox.Text}";

            }
        }

        private void AlreadyHaveAnAccountLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AuthorizationPage ap = new AuthorizationPage();
            this.NavigationService.Navigate(ap);
        }

        MailAddress to;
        private void ConfirmRegistrationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ConfirmRegistrationTextBox.Text == randomCode)
            {   
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand($"INSERT INTO UserDB (Login, Name, Email, Password) VALUES (N'{this.LoginTextBox.Text}'," +
                            $" N'{this.NameTextBox.Text}', N'{this.EmailTextBox.Text}', N'{PasswordTextBox.Text}')", connection);
                        command.ExecuteNonQuery();
                        MessageBox.Show("Вы зарегистрированы");

                    }

                AuthorizationPage ap = new AuthorizationPage();
                this.NavigationService.Navigate(ap);
            }
        }

        private void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(LoginTextBox.Text == "")
            {
                CheckLoginLabel.Foreground = Brushes.Red;
                CheckLoginLabel.Content = "Поле не заполнено";
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand($"SELECT Login FROM UserDB WHERE Login = N'{LoginTextBox.Text}'", connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        CheckLoginLabel.Foreground = Brushes.Red;
                        CheckLoginLabel.Content = "Данный логин уже занят";
                    }
                    else
                    {
                        CheckLoginLabel.Foreground = Brushes.Green;
                        CheckLoginLabel.Content = "Данный логин свободен";
                    }
                }
            }
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(NameTextBox.Text == "")
                CheckNameLabel.Visibility = Visibility.Visible;
            else
                CheckNameLabel.Visibility = Visibility.Hidden;
        }

        private void PasswordTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (PasswordTextBox.Text == "")
                CheckPasswordLabel.Visibility = Visibility.Visible;
            else
                CheckPasswordLabel.Visibility = Visibility.Hidden;
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EmailTextBox.Text == "")
            {
                CheckEmailLabel.Foreground = Brushes.Red;
                CheckEmailLabel.Content = "Поле не заполнено";
            }
            else
            {
                try
                {
                    to = new MailAddress($"{EmailTextBox.Text}");
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand($"SELECT Email FROM UserDB WHERE Email = N'{EmailTextBox.Text}'", connection);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            CheckEmailLabel.Foreground = Brushes.Red;
                            CheckEmailLabel.Content = "Данная почта уже занята";
                        }
                        else
                        {
                            CheckEmailLabel.Foreground = Brushes.Green;
                            CheckEmailLabel.Content = "Данная почта свободна";
                        }
                    }
                }
                catch(FormatException)
                {
                    CheckEmailLabel.Foreground = Brushes.Red;
                    CheckEmailLabel.Content = "Некорректная почта";
                }
                
            }
        }
    }
}
    


