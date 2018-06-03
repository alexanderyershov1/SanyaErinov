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
using System.IO;

namespace Shebist
{
    /// <summary>
    /// Логика взаимодействия для AdminWindow.xaml
    /// </summary>
    public partial class UserStatisticsWindow : Window
    {
        public UserStatisticsWindow()
        {
            InitializeComponent();
        }
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        static string Debug = Directory.GetCurrentDirectory();
        static string Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();

        public string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=
        {Shebist}\UserDB.mdf;Integrated Security=True";

        public List<UserForAdmin> users = new List<UserForAdmin>();

        void Update()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Id, Login, Name, Email, DateOfRegistration, LastEntrance, TotalInTheApp, Status FROM Users";
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        users.Add(new UserForAdmin()
                        {
                            id = reader.GetInt32(0),
                            login = reader.GetString(1),
                            name = reader.GetString(2),
                            email = reader.GetString(3),
                            dateOfRegistration = reader.GetString(4),
                            lastEntrance = reader.GetString(5),
                            totalInTheApp = reader.GetString(6),
                            status = reader.GetString(7),

                        });
                    }
                }
            }
           
            TotalUsersLabel.Content = "Всего пользователей " + users.Count;
            int onlineUsers = 0, newUsers = 0, todayUsers = 0;
            foreach (UserForAdmin user in users)
            {
                UsersDataGrid.Items.Add(user);
                if (user.status == "Online") onlineUsers++;
                if (user.dateOfRegistration.StartsWith(DateTime.Today.ToString("dd.MM.yyyy"))) newUsers++;
                if (user.lastEntrance.StartsWith(DateTime.Today.ToString("dd.MM.yyyy"))) todayUsers++;
            }
            OnlineUsersLabel.Content = "Сейчас онлайн " + onlineUsers;
            NewForTodayLabel.Content = "Новых за сегодня " + newUsers;
            CameTodayLabel.Content = "Заходили сегодня " + todayUsers;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            users.Clear();
            UsersDataGrid.Items.Clear();
            Update();
        }

        private void UserStatisticsLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

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

        private void DispatchLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DispatchWindow dw = new DispatchWindow()
            {
                WindowState = this.WindowState,
                Top = this.Top,
                Left = this.Left,
                Width = this.Width,
                Height = this.Height
            };
            dw.Show();
            this.Close();
        }

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
    }
}
