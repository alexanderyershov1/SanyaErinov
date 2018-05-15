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
    public partial class AccountWindow : Window
    {
        public int userid;
        public bool needToUpdate;
        public User oldUser, newUser;
        public Topic MainWords;
        public List<Topic> oldTopics, newTopics;
        List<int> indicesOfDeletedTopics = new List<int>();
        List<Word> deletedWords = new List<Word>();
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        public AccountWindow()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            needToUpdate = true;
            AuthorizationWindow aw = new AuthorizationWindow
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

        private void BackMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mw = new MainWindow
            {
                WindowState = this.WindowState,
                Top = this.Top,
                Left = this.Left,
                Width = this.Width,
                Height = this.Height,
                oldUser = oldUser,
                newUser = newUser,
                oldTopics = oldTopics,
                newTopics = newTopics,
                MainWords = MainWords
            };
            mw.Show();
            this.Close();
        }

        static string Debug = Directory.GetCurrentDirectory();
        static string Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();

        public string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=
        {Shebist}\UserDB.mdf;Integrated Security=True";

        private void AccountPage_Loaded(object sender, RoutedEventArgs e)
        {
            LoginTextBox.Text = newUser.Login;
            NameTextBox.Text = newUser.Name;
            EmailTextBox.Text = newUser.Email;
            PasswordBox.Password = newUser.Password;
            AgainPasswordBox.Password = newUser.Password;
        }

        private void DeleteAccountLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MessageBox.Show("Вы действительно хотите удалить аккаунт?", "Подтверждение", MessageBoxButton.YesNo) ==
                MessageBoxResult.Yes)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = $"SELECT Id FROM Topics WHERE UserID = {userid}";
                    List<int> idOfTopics = new List<int>();
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            idOfTopics.Add(reader.GetInt32(0));
                        }
                    }
                    reader.Close();
                    if (idOfTopics.Count != 0)
                    {
                        foreach (int id in idOfTopics)
                        {
                            command.CommandText = $"DROP TABLE [{id}]";
                            command.ExecuteNonQuery();
                            command.CommandText = $"DELETE FROM [Topics] WHERE Id = {id}";
                            command.ExecuteNonQuery();
                        }
                    }
                    command.CommandText = $"DELETE FROM [UserDB] WHERE Id = {userid}";
                    command.ExecuteNonQuery();
                }

                AuthorizationWindow aw = new AuthorizationWindow
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

        }

        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (needToUpdate)
                newUser.Update(oldUser, newUser, oldTopics, newTopics, indicesOfDeletedTopics, deletedWords);
        }

        MailAddress from = new MailAddress("alexanderyershov1@gmail.com", "Шебист");
        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                bool canUpdate = true;
                connection.Open();
                command.Connection = connection;
                
                if (EmailTextBox.Text != oldUser.Email)
                {
                    command.CommandText = $"SELECT Email FROM Users WHERE Email = N'{EmailTextBox.Text}'";
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        MessageBox.Show("Данная почта занята");
                        canUpdate = false;
                    } 
                    reader.Close();       
                }

                if (AgainPasswordBox.Password != PasswordBox.Password)
                {
                    MessageBox.Show("Пароли не совпадают");
                    canUpdate = false;
                }

                if (canUpdate)
                {
                    command.CommandText = $"UPDATE Users SET Name = N'{NameTextBox.Text}'," +
                        $"Email = N'{EmailTextBox.Text}', Password = N'{PasswordBox.Password}' WHERE Login = N'{oldUser.Login}'";
                    command.ExecuteNonQuery();
                    MessageBox.Show("Данные обновлены");
                    oldUser.Name = NameTextBox.Text;
                    oldUser.Email = EmailTextBox.Text;
                    oldUser.Password = PasswordBox.Password;
                    newUser.Name = NameTextBox.Text;
                    newUser.Email = EmailTextBox.Text;
                    newUser.Password = PasswordBox.Password;
                }
            }
            //if (LoginTextBox.Text == "" || NameTextBox.Text == "" || EmailTextBox.Text == "" || PasswordTextBox.Text == "")
            //{
            //    MessageBox.Show($"Не все поля заполнены");
            //}
            //else
            //{
            //    try
            //    {
            //        MailAddress to = new MailAddress($"{EmailTextBox.Text}");
            //        using (SqlConnection connection = new SqlConnection(connectionString))
            //        {
            //            connection.Open();
            //            if (LoginTextBox.Text != login)
            //            {
            //                SqlCommand command1 = new SqlCommand($"UPDATE UserDB SET Login = N'{LoginTextBox.Text}'", connection);
            //                command1.ExecuteNonQuery();
            //            }
            //            if (NameTextBox.Text != name)
            //            {
            //                SqlCommand command2 = new SqlCommand($"UPDATE UserDB SET  Name = N'{NameTextBox.Text}'", connection);
            //                command2.ExecuteNonQuery();
            //            }
            //            if (EmailTextBox.Text != email)
            //            {
            //                SqlCommand command3 = new SqlCommand($"UPDATE UserDB SET Email = N'{EmailTextBox.Text}'", connection);
            //                command3.ExecuteNonQuery();
            //            }
            //            if (PasswordTextBox.Text != password)
            //            {
            //                SqlCommand command4 = new SqlCommand($"UPDATE UserDB SET Password = N'{PasswordTextBox.Text}'", connection);
            //                command4.ExecuteNonQuery();
            //            }
            //        }

            //        if (LoginTextBox.Text != login || NameTextBox.Text != name || EmailTextBox.Text != email || PasswordTextBox.Text != password)
            //        {
            //            MessageBox.Show("Данные изменены");
            //            MailMessage mailMessage = new MailMessage(from, to);
            //            mailMessage.Subject = "Изменение данных";
            //            mailMessage.Body = $"{NameTextBox.Text}, ваши новые данные:" +
            //                $"\nЛогин: {LoginTextBox.Text}" +
            //                $"\nИмя: {NameTextBox.Text}" +
            //                $"\nПароль: {PasswordTextBox.Text}";
            //            smtp.EnableSsl = true;
            //            smtp.Credentials = new NetworkCredential("alexanderyershov1@gmail.com", "Death4000$");
            //            smtp.Send(mailMessage);
            //        }
            //    }
            //    catch (SqlException)
            //    {
            //        MessageBox.Show("Пользователь с такими данными уже существует");
            //    }
            //    catch (FormatException)
            //    {
            //        MessageBox.Show("Некорректная почта");
            //    }
            //}
        }
    }
}
