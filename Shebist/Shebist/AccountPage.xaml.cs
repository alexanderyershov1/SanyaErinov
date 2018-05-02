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
using Microsoft.Win32;

namespace Shebist
{
    /// <summary>
    /// Логика взаимодействия для AccountPage.xaml
    /// </summary>
    public partial class AccountPage : Page
    {
        public int userid;
        string login, name, email, password;
        public AccountPage()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationPage ap = new AuthorizationPage();
            this.NavigationService.Navigate(ap);
        }

        private void BackMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainPage mp = new MainPage { userid = userid };
            this.NavigationService.Navigate(mp);
        }

        static string Debug = Directory.GetCurrentDirectory();
        static string Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();

        public string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=
        {Shebist}\UserDB.mdf;Integrated Security=True";

        private void AccountPage_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT Login, Name, Email, Password FROM UserDB WHERE Id = {userid}", connection);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    login = LoginTextBox.Text = reader.GetString(0);
                    name = NameTextBox.Text = reader.GetString(1);
                    email = EmailTextBox.Text = reader.GetString(2);
                    password = PasswordTextBox.Text = reader.GetString(3);
                }
                reader.Close();
            }
        }

        private void DeleteAccountLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Вы действительно хотите удалить аккаунт?", "Подтверждение", MessageBoxButton.YesNo);
            //using(SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    connection.Open();
            //    SqlCommand command = new SqlCommand($"DELETE FROM UserDB WHERE Id = {userid}");

            //}
        }

        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
        MailAddress from = new MailAddress("alexanderyershov1@gmail.com", "Шебист");
        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginTextBox.Text == "" || NameTextBox.Text == "" || EmailTextBox.Text == "" || PasswordTextBox.Text == "")
            {
                MessageBox.Show($"Не все поля заполнены");
            }
            else
            {
                try
                {
                    MailAddress to = new MailAddress($"{EmailTextBox.Text}");
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        if (LoginTextBox.Text != login)
                        {
                            SqlCommand command1 = new SqlCommand($"UPDATE UserDB SET Login = N'{LoginTextBox.Text}'", connection);
                            command1.ExecuteNonQuery();
                        }
                        if (NameTextBox.Text != name)
                        {
                            SqlCommand command2 = new SqlCommand($"UPDATE UserDB SET  Name = N'{NameTextBox.Text}'", connection);
                            command2.ExecuteNonQuery();
                        }
                        if (EmailTextBox.Text != email)
                        {
                            SqlCommand command3 = new SqlCommand($"UPDATE UserDB SET Email = N'{EmailTextBox.Text}'", connection);
                            command3.ExecuteNonQuery();
                        }
                        if (PasswordTextBox.Text != password)
                        {
                            SqlCommand command4 = new SqlCommand($"UPDATE UserDB SET Password = N'{PasswordTextBox.Text}'", connection);
                            command4.ExecuteNonQuery();
                        }
                    }

                    if (LoginTextBox.Text != login || NameTextBox.Text != name || EmailTextBox.Text != email || PasswordTextBox.Text != password)
                    {
                        MessageBox.Show("Данные изменены");
                        MailMessage mailMessage = new MailMessage(from, to);
                        mailMessage.Subject = "Изменение данных";
                        mailMessage.Body = $"{NameTextBox.Text}, ваши новые данные:" +
                            $"\nЛогин: {LoginTextBox.Text}" +
                            $"\nИмя: {NameTextBox.Text}" +
                            $"\nПароль: {PasswordTextBox.Text}";
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential("alexanderyershov1@gmail.com", "Death4000$");
                        smtp.Send(mailMessage);
                    }
                }
                catch (SqlException)
                {
                    MessageBox.Show("Пользователь с такими данными уже существует");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Некорректная почта");
                }
            }
        }
    }
}
