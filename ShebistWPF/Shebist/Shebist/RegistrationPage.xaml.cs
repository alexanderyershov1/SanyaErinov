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
        }
        

        string cd = Directory.GetCurrentDirectory();
        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
        char[] randomMassiv = { 'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F',
            'g', 'G', 'h', 'H', 'i', 'I', 'j', 'J', 'k', 'K', 'l', 'L', 'm', 'M', 'n', 'N',
            'o', 'O', 'p', 'P', 'q', 'Q', 'r', 'R', 's', 'S', 't', 'T', 'u', 'U', 'v', 'V',
            'w', 'W', 'x', 'X', 'y', 'Y', 'z', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
        Random random = new Random();

        static System.IO.DirectoryInfo myDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        static string parentDirectory = myDirectory.Parent.FullName;
        static System.IO.DirectoryInfo myDirectory2 = new DirectoryInfo(parentDirectory);
        static string parentDirectory2 = myDirectory2.Parent.FullName;

        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={parentDirectory2}\UserDB.mdf;Integrated Security=True";

        MailAddress from = new MailAddress("alexanderyershov1@gmail.com", "Шебист");
        string randomCode;
        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
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
                        SqlCommand command = new SqlCommand($"SELECT Login, Email FROM UserDB WHERE Login = N'{LoginTextBox.Text}' OR Email = N'{EmailTextBox.Text}'", connection);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                            MessageBox.Show("Пользователь с такими данными уже существует");
                        else
                        {
                            randomCode = $"{randomMassiv.ElementAt(random.Next(0, 61))}" +
                        $"{randomMassiv.ElementAt(random.Next(0, 61))}" +
                        $"{randomMassiv.ElementAt(random.Next(0, 61))}" +
                        $"{randomMassiv.ElementAt(random.Next(0, 61))}" +
                        $"{randomMassiv.ElementAt(random.Next(0, 61))}";

                            MailMessage mailMessage = new MailMessage(from, to);
                            mailMessage.Subject = "Подтверждение регистрации";
                            mailMessage.Body = $"{NameTextBox.Text}, код для подтверждения регистрации: {randomCode}" +
                                $"\n\nЕсли вы нигде не регистрировались, проигнорируйте это письмо.";
                            smtp.EnableSsl = true;
                            smtp.Credentials = new NetworkCredential("alexanderyershov1@gmail.com", "Death4000$");
                            smtp.Send(mailMessage);
                        }
                    }
                }
                catch (FormatException)
                {
                    MessageBox.Show("Некорректная почта");
                }
            }
        }

        private void AlreadyHaveAnAccountLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AuthorizationPage ap = new AuthorizationPage();
            this.NavigationService.Navigate(ap);
        }

        private void ConfirmRegistrationTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ConfirmRegistrationTextBox.Text == randomCode)
            {
                    MailAddress to = new MailAddress($"{EmailTextBox.Text}");
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand($"INSERT INTO UserDB (Login, Name, Email, Password) VALUES (N'{this.LoginTextBox.Text}'," +
                            $" N'{this.NameTextBox.Text}', N'{this.EmailTextBox.Text}', N'{PasswordTextBox.Text}')", connection);
                        command.ExecuteNonQuery();
                        MessageBox.Show("Вы зарегистрированы");

                    }
                    MailMessage mailMessage = new MailMessage(from, to);
                    mailMessage.Subject = "Регистрация";
                    mailMessage.Body = $"{NameTextBox.Text}, вы успешно зарегистрировались, ваши данные для входа:" +
                        $"\nЛогин: {LoginTextBox.Text}" +
                        $"\nПароль: {PasswordTextBox.Text}";
                    smtp.EnableSsl = true;
                    smtp.Credentials = new NetworkCredential("alexanderyershov1@gmail.com", "Death4000$");
                    smtp.Send(mailMessage);

                AuthorizationPage ap = new AuthorizationPage();
                this.NavigationService.Navigate(ap);
            }
        }
    }
}
    


