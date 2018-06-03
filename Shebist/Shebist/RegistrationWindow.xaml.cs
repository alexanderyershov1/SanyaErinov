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
    public partial class RegistrationWindow : Window
    {
        public RegistrationWindow()
        {
            InitializeComponent();
            ConfrimRegistrationGrid.Visibility = Visibility.Hidden;
            CheckLoginLabel.Content = 
            CheckNameLabel.Content = 
            CheckEmailLabel.Content = 
            CheckPasswordLabel.Content = 
            CheckPasswordsLabel.Content = "";

        }

        string Debug = Directory.GetCurrentDirectory();
        public List<User> users = new List<User>();
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

            if (CheckLoginLabel.Content.ToString() == "✔" &&
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

                ConfrimRegistrationGrid.Visibility = Visibility.Visible;
                ConfirmRegistrationLabel.Content = $"Введите код, отправленный на {EmailTextBox.Text}";

            }
        }

        private void AlreadyHaveAnAccountLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AuthorizationWindow aw = new AuthorizationWindow
            {
                WindowState = this.WindowState,
                Top = this.Top,
                Left = this.Left,
                Width = this.Width,
                Height = this.Height,
                users = users
            };
            aw.Show();
            this.Close();
        }

        MailAddress to;
        private void ConfirmRegistrationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ConfirmRegistrationTextBox.Text == randomCode)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    try
                    {
                        connection.Open();
                        command.Connection = connection;
                        string sequenceOfIndicesOfMainTopic = "";
                        command.CommandText = "SELECT * FROM MainTopic";
                        reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            int indexOfRow = 0;
                            while (reader.Read())
                            {
                                sequenceOfIndicesOfMainTopic += indexOfRow++ + "~"; 
                            }
                        }
                        reader.Close();

                        command.CommandText = $"INSERT INTO Users (Login, Name, Email, Password, CurrentTopicId, ChoiceOfTopicGridVisibility, DateOfRegistration, LastEntrance, TotalInTheApp, SequenceOfIndicesOfMainTopic, IndexOfMainTopic, Status)" +
                        $" VALUES (N'{LoginTextBox.Text.Trim()}'," +
                            $" N'{NameTextBox.Text.Trim()}', N'{EmailTextBox.Text.Trim()}', N'{PasswordBox.Password.Trim()}'," +
                            $" 0, 'Visible', N'{DateTime.Now.ToString()}', N'', N'{DateTime.MinValue.Subtract(DateTime.MinValue).ToString()}', N'{sequenceOfIndicesOfMainTopic}', 0, N'Offline')";
                        command.ExecuteNonQuery();
                        MessageBox.Show("Вы зарегистрированы");
                        command.CommandText = $"SELECT Login FROM Users WHERE Login = N'{LoginTextBox.Text.Trim()}'";
                        reader = command.ExecuteReader();
                        reader.Read();

                        users.Add(new User
                        {
                            Login = LoginTextBox.Text.Trim(),
                            Name = NameTextBox.Text.Trim(),
                            Email = EmailTextBox.Text.Trim(),
                            Password = PasswordBox.Password,
                            CurrentTopicId = "MainWords",
                            ChoiceOfTopicGridVisibility = "Visible"
                        });
                    }
                    catch (SqlException)
                    {

                        reader.Close();
                        command.CommandText = "SELECT Id, Login, Name, Email, Password, CurrentTopicId, ChoiceOfTopicGridVisibility FROM Users" +
                            $" WHERE Login = N'{LoginTextBox.Text}' OR Email = N'{EmailTextBox.Text}'";
                        reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            reader.Read();
                            if (LoginTextBox.Text == reader.GetString(1))
                            {
                                CheckLoginLabel.Foreground = Brushes.Red;
                                CheckLoginLabel.Content = "✖";
                            }
                            MessageBox.Show($"Пользователь {reader.GetString(1)} уже существует");
                            users.Add(new User
                            {
                                Login = reader.GetString(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Password = reader.GetString(3)
                            });
                        }
                    }
                }

                AuthorizationWindow aw = new AuthorizationWindow
                {
                    WindowState = this.WindowState,
                    Top = this.Top,
                    Left = this.Left,
                    Width = this.Width,
                    Height = this.Height,
                    users = users
                };

                aw.Show();
                this.Close();
            }
        }

        private void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LoginTextBox.Text == "")
            {
                CheckLoginLabel.Foreground = Brushes.Red;
                CheckLoginLabel.Content = "";
            }
            else
            {
                if (users.Count != 0)
                {
                    foreach (User user in users)
                    {
                        if (user.Login == LoginTextBox.Text)
                        {
                            CheckLoginLabel.Foreground = Brushes.Red;
                            CheckLoginLabel.Content = "✖";
                            break;
                        }
                        else
                        {
                            CheckLoginLabel.Foreground = Brushes.Green;
                            CheckLoginLabel.Content = "✔";
                        }
                    }
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
            if (NameTextBox.Text == "")
            {
                CheckNameLabel.Foreground = Brushes.Red;
                CheckNameLabel.Content = "";
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
                CheckEmailLabel.Content = "";
            }
            else
            {
                try
                {
                    to = new MailAddress($"{EmailTextBox.Text}");
                    if (users.Count != 0)
                    {
                        foreach (User user in users)
                        {
                            if (user.Email == EmailTextBox.Text)
                            {
                                CheckEmailLabel.Foreground = Brushes.Red;
                                CheckEmailLabel.Content = "✖";
                                break;
                            }
                            else
                            {
                                CheckEmailLabel.Foreground = Brushes.Green;
                                CheckEmailLabel.Content = "✔";
                            }
                        }
                    }
                    else
                    {
                        CheckEmailLabel.Foreground = Brushes.Green;
                        CheckEmailLabel.Content = "✔";
                    }

                }
                catch (FormatException)
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
                CheckPasswordLabel.Content = "";
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
            if (PasswordBox.Password != "" && PasswordBox.Password == ConfirmPasswordBox.Password)
            {
                CheckPasswordsLabel.Foreground = Brushes.Green;
                CheckPasswordsLabel.Content = "✔";
            }
            else
            {
                CheckPasswordsLabel.Foreground = Brushes.Red;
                CheckPasswordsLabel.Content = "✖";
                if (PasswordBox.Password == "") CheckPasswordsLabel.Content = "";
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }
    }
}



