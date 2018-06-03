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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Net.Mail;
using System.IO;
using System.Net;

namespace Shebist
{
    /// <summary>
    /// Логика взаимодействия для DispatchWindow.xaml
    /// </summary>
    public partial class DispatchWindow : Window
    {
        public DispatchWindow()
        {
            InitializeComponent();
        }
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;

        static string Shebist = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Shebist}\UserDB.mdf;Integrated Security=True";

        private void ExitLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AuthorizationWindow aw = new AuthorizationWindow()
            {
                WindowState = this.WindowState,
                Top = this.Top,
                Left = this.Left,
                Width = this.Width,
                Height = this.Height
            };
            aw.Show();
            this.Close();
        }

        private void UserStatisticsLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UserStatisticsWindow usw = new UserStatisticsWindow()
            {
                WindowState = this.WindowState,
                Top = this.Top,
                Left = this.Left,
                Width = this.Width,
                Height = this.Height
            };
            usw.Show();
            this.Close();
        }

        private void SentenceEditorLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SentencesEditorWindow sew = new SentencesEditorWindow()
            {
                WindowState = this.WindowState,
                Top = this.Top,
                Left = this.Left,
                Width = this.Width,
                Height = this.Height
            };
            sew.Show();
            this.Close();
        }

        private void SendMessageButton_Click(object sender, RoutedEventArgs e)
        {
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Email FROM Users";
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        MailAddress from = new MailAddress("alexanderyershov1@gmail.com", "Шебист");
                        MailAddress to = new MailAddress(reader.GetString(0));
                        MailMessage mailMessage = new MailMessage(from, to);
                        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                        mailMessage.Subject = SubjectMessageTextbox.Text;
                        mailMessage.Body = BodyMessageTextBox.Text;
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential("alexanderyershov1@gmail.com", "Death4000$");
                        smtp.Send(mailMessage);
                    }
                }
            }

            SubjectMessageTextbox.Clear();
            BodyMessageTextBox.Clear();
        }
    }
}
