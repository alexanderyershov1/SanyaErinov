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
        public DateTime entryTime;
        public bool needToUpdate;
        public User oldUser, newUser;
        public Topic MainTopic;
        public List<Topic> oldTopics, newTopics;
        public List<int> indicesOfDeletedTopics = new List<int>();
        public List<Sentence> deletedSentences = new List<Sentence>();
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        public AccountWindow()
        {
            InitializeComponent();
            MenuGrid.Visibility = Visibility.Hidden;
        }

        private void AccountLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MenuGrid.Visibility = Visibility.Hidden;
            Canvas.SetLeft(OpenMenu, 0);
        }

        private void SettingsLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //SettingsWindow sw = new SettingsWindow(Main);
            //sw.Owner = this;
            //sw.Show();
        }

        private void TopicEditorLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            needToUpdate = false;
            TopicEditorWindow tew = new TopicEditorWindow
            {
                needToUpdate = true,
                entryTime = entryTime,
                WindowState = this.WindowState,
                Top = this.Top,
                Left = this.Left,
                Width = this.Width,
                Height = this.Height,
                oldUser = oldUser,
                newUser = newUser,
                oldTopics = oldTopics,
                newTopics = newTopics,
                MainTopic = MainTopic,
                indicesOfDeletedTopics = indicesOfDeletedTopics,
                deletedSentences = deletedSentences
            };
            tew.Show();
            this.Close();
        }

        private void LearnLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow mw = new MainWindow
            {
                needToUpdate = true,
                WindowState = this.WindowState,
                entryTime = entryTime,
                Top = this.Top,
                Left = this.Left,
                Width = this.Width,
                Height = this.Height,
                oldUser = oldUser,
                newUser = newUser,
                oldTopics = oldTopics,
                newTopics = newTopics,
                MainTopic = MainTopic,
                indicesOfDeletedTopics = indicesOfDeletedTopics,
                deletedSentences = deletedSentences
            };
            mw.Show();
            this.Close();
        }

        private void ExitLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
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

        private void OpenMenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Main.MouseLeftButtonDown -= Main_MouseLeftButtonDown;
        }

        private void Main_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MenuGrid.Visibility = Visibility.Hidden;
            Canvas.SetLeft(OpenMenu, 0);
        }

        private void OpenMenu_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (MenuGrid.Visibility == Visibility.Hidden)
            {
                MenuGrid.Visibility = Visibility.Visible;
                Canvas.SetLeft(OpenMenu, 200);
            }
            else
            {
                MenuGrid.Visibility = Visibility.Hidden;
                Canvas.SetLeft(OpenMenu, 0);
            }
            Main.MouseLeftButtonDown += Main_MouseLeftButtonDown;
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Label)sender).FontSize = 18;
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Label)sender).FontSize = 16;
        }

        private void MenuGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Main.MouseLeftButtonDown -= Main_MouseLeftButtonDown;
        }

        private void MenuGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Main.MouseLeftButtonDown += Main_MouseLeftButtonDown;
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
        
        static string Debug = Directory.GetCurrentDirectory();
        static string Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();

        public string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=
        {Shebist}\UserDB.mdf;Integrated Security=True";

        private void AccountWindow_Loaded(object sender, RoutedEventArgs e)
        {
            AccountLabel.Content = newUser.Name;
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
                newUser.Update(oldUser, newUser, oldTopics, newTopics, indicesOfDeletedTopics, deletedSentences, entryTime);
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
