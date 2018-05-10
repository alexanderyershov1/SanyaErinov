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
using System.Runtime.Serialization.Formatters.Binary;

namespace Shebist
{
    /// <summary>
    /// Логика взаимодействия для DataRecoveryPage.xaml
    /// </summary>
    public partial class DataRecoveryWindow : Window
    {
        public DataRecoveryWindow()
        {
            InitializeComponent();
            ConfirmGrid.Visibility = Visibility.Hidden;
        }

        RegistrationWindow rw = new RegistrationWindow();
        BinaryFormatter formatter = new BinaryFormatter();
        static string Debug = Directory.GetCurrentDirectory();
        static string Shebist = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Shebist}\UserDB.mdf;Integrated Security=True";

        private void BackLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AuthorizationWindow aw = new AuthorizationWindow();
            aw.Show();
            this.Close();
        }

        string login, name;

        private void BackLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            BackLabel.FontSize = 12;
        }

        private void ConfirmTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ConfirmTextBox.Text == randomeCode)
            {
                DataRecovery2Window dr2w = new DataRecovery2Window { login = login };
                dr2w.Show();
                this.Close();
            }
        }

        private void BackLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            BackLabel.FontSize = 13;
        }

        string randomeCode;
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            if (EnterYourEmailTextBox.Text != "")
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand($"SELECT Login, Name FROM Users WHERE Email = N'{EnterYourEmailTextBox.Text}'", connection);
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            login = reader.GetString(0);
                            name = reader.GetString(1);
                        }

                        randomeCode = rw.RandomString(5);
                        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                        MailAddress from = new MailAddress("alexanderyershov1@gmail.com", "Шебист");
                        MailAddress to = new MailAddress($"{EnterYourEmailTextBox.Text}");
                        MailMessage mailMessage = new MailMessage(from, to);
                        mailMessage.Subject = "Восстановление пароля";
                        mailMessage.Body = $"{name}, ваш код для восстановления пароля: {randomeCode}";
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential("alexanderyershov1@gmail.com", "Death4000$");
                        smtp.Send(mailMessage);
                        
                        ConfirmLabel.Content = $"Введите код, отправленный на {EnterYourEmailTextBox.Text}";
                        ConfirmGrid.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        MessageBox.Show("Данная почта не найдена");
                    }
                }
            }
        }
    }
}
