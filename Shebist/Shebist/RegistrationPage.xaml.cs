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
        
        string Debug = Directory.GetCurrentDirectory();
        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;

        Random random = new Random();
        public string RandomString(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        static string Shebist = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();

        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Shebist}\UserDB.mdf;Integrated Security=True";

        MailAddress from = new MailAddress("alexanderyershov1@gmail.com", "Шебист");
        string randomCode;
        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            
            if(CheckLoginLabel.Content.ToString() == "✔" &&
                CheckNameLabel.Content.ToString() == "✔" &&
                CheckEmailLabel.Content.ToString() == "✔" &&
                CheckPasswordLabel.Content.ToString() == "✔" &&
                CheckPasswordsLabel.Content.ToString() == "✔")
            {
                randomCode = RandomString(5);

                MailMessage mailMessage = new MailMessage(from, to);
                mailMessage.Subject = "Подтверждение регистрации";
                mailMessage.Body = $"{NameTextBox.Text}, код для подтверждения регистрации: {randomCode}" +
                    $"\n\nЕсли вы нигде не регистрировались, проигнорируйте это письмо.";
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("alexanderyershov1@gmail.com", "Death4000$");
                smtp.Send(mailMessage);
                
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
                            $" N'{this.NameTextBox.Text}', N'{this.EmailTextBox.Text}', N'{PasswordBox.Password}')", connection);
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
                CheckLoginLabel.Content = "✖";
            }
            else
            {
                    if (logins.Contains(LoginTextBox.Text))
                    {
                        CheckLoginLabel.Foreground = Brushes.Red;
                        CheckLoginLabel.Content = "✖";
                    }
                    else
                    {
                        CheckLoginLabel.Foreground = Brushes.Green;
                        CheckLoginLabel.Content = "✔";
                    }
            }
        }

        private void NameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(NameTextBox.Text == "")
            {
                CheckNameLabel.Foreground = Brushes.Red;
                CheckNameLabel.Content = "✖";
            }
            else
            {
                CheckNameLabel.Foreground = Brushes.Green;
                CheckNameLabel.Content = "✔";
            }
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (EmailTextBox.Text == "")
            {
                CheckEmailLabel.Foreground = Brushes.Red;
                CheckEmailLabel.Content = "✖";
            }
            else
            {
                try
                {
                    to = new MailAddress($"{EmailTextBox.Text}");
                        if (emails.Contains(EmailTextBox.Text))
                        {
                            CheckEmailLabel.Foreground = Brushes.Red;
                            CheckEmailLabel.Content = "✖";
                        }
                        else
                        {
                            CheckEmailLabel.Foreground = Brushes.Green;
                            CheckEmailLabel.Content = "✔";
                        }
                    
                }
                catch(FormatException)
                {
                    CheckEmailLabel.Foreground = Brushes.Red;
                    CheckEmailLabel.Content = "✖";
                }
                
            }
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (PasswordBox.Password == "")
            {
                CheckPasswordLabel.Foreground = Brushes.Red;
                CheckPasswordLabel.Content = "✖";
            }
            else
            {
                CheckPasswordLabel.Foreground = Brushes.Green;
                CheckPasswordLabel.Content = "✔";
            }

            if (PasswordBox.Password != "" && PasswordBox.Password == ConfirmPasswordBox.Password)
            {
                CheckPasswordsLabel.Foreground = Brushes.Green;
                CheckPasswordsLabel.Content = "✔";
            }
            else
            {
                CheckPasswordsLabel.Foreground = Brushes.Red;
                CheckPasswordsLabel.Content = "✖";
            }
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if(PasswordBox.Password != "" && PasswordBox.Password == ConfirmPasswordBox.Password)
            {
                CheckPasswordsLabel.Foreground = Brushes.Green;
                CheckPasswordsLabel.Content = "✔";
            }
            else
            {
                CheckPasswordsLabel.Foreground = Brushes.Red;
                CheckPasswordsLabel.Content = "✖";
            }
        }

        private void AlreadyHaveAnAccountLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            AlreadyHaveAnAccountLabel.FontSize = 13;
        }

        private void AlreadyHaveAnAccountLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            AlreadyHaveAnAccountLabel.FontSize = 12;
        }

        string[] logins, emails;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(*) FROM UserDB";
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    reader.Read();
                    logins = new string[reader.GetInt32(0)];
                    emails = new string[reader.GetInt32(0)];

                }
                reader.Close();

                command.CommandText = $"SELECT Login, Email FROM UserDB";
                reader = command.ExecuteReader();
                int n = 0;
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        logins[n] = reader.GetString(0);
                        emails[n++] = reader.GetString(1);
                    }
                }
            }
        }
    }
}
    


