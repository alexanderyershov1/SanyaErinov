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
    /// Логика взаимодействия для DataRecoveryPage.xaml
    /// </summary>
    public partial class DataRecoveryPage : Page
    {
        public DataRecoveryPage()
        {
            InitializeComponent();
        }

        static System.IO.DirectoryInfo myDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        static string parentDirectory = myDirectory.Parent.FullName;
        static System.IO.DirectoryInfo myDirectory2 = new DirectoryInfo(parentDirectory);
        static string parentDirectory2 = myDirectory2.Parent.FullName;

        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={parentDirectory2}\UserDB.mdf;Integrated Security=True";

        

        private void BackLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AuthorizationPage ap = new AuthorizationPage();
            this.NavigationService.Navigate(ap);
        }

        string login, name, password;
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if(EnterYourEmailTextBox.Text != "")
            {
                try
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        SqlCommand command = new SqlCommand($"SELECT Login, Name, Password FROM UserDB WHERE Email = N'{EnterYourEmailTextBox.Text}'", connection);
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                login = (string)reader.GetValue(0);
                                name = (string)reader.GetValue(1);
                                password = (string)reader.GetValue(2);
                            }
                        }

                        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                        MailAddress from = new MailAddress("alexanderyershov1@gmail.com", "Шебист");
                        MailAddress to = new MailAddress($"{EnterYourEmailTextBox.Text}");
                        MailMessage mailMessage = new MailMessage(from, to);
                        mailMessage.Subject = "Восстановление данных";
                        mailMessage.Body = $"{name}, ваши данные для входа:" +
                            $"\nЛогин: {login}" +
                            $"\nПароль: {password}";
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential("alexanderyershov1@gmail.com", "Death4000$");
                        smtp.Send(mailMessage);
                        MessageBox.Show("Данные для входа отправлены на вашу почту");
                        AuthorizationPage ap = new AuthorizationPage();
                        this.NavigationService.Navigate(ap);
                    }
                }
                catch (SqlException)
                {
                    MessageBox.Show("Данная почта не найдена");
                }
                catch (FormatException)
                {
                    MessageBox.Show("Данная почта не найдена");
                }
            }
        }
    }
}
