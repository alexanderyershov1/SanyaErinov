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
using System.IO;
using System.Data;

namespace Shebist
{
    /// <summary>
    /// Логика взаимодействия для TopicEditorPage.xaml
    /// </summary>
    public partial class TopicEditorPage : Page
    {
        int userid;
        public TopicEditorPage(int userid)
        {
            InitializeComponent();
            this.userid = userid;

        }

        int topicId = 0;
        static string Debug = Directory.GetCurrentDirectory();
        static string Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();
        static string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Shebist}\UserDB.mdf;Integrated Security=True";
        static SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        SqlDataAdapter adapter;
        DataTable existingtopics = new DataTable(), empty;

        private void BackLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainPage mp = new MainPage();
            this.NavigationService.Navigate(mp);
        }

        private void TopicsNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(TopicsNameTextBox.Text == "")
            {
                CheckNameLabel.Foreground = Brushes.Red;
                CheckNameLabel.Content = "✖";
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.CommandText = $"SELECT Id FROM Topics WHERE UserId = {userid} AND Name = N'{TopicsNameTextBox.Text}'";
                    command.Connection = connection;
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
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
            }     
        }

        private void DeleteTopicsNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DeleteTopicsNameTextBox.Text == "")
            {
                DeleteCheckNameLabel.Foreground = Brushes.Red;
                DeleteCheckNameLabel.Content = "✖";
                topicId = 0;
            }
            else
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.CommandText = $"SELECT Id FROM Topics WHERE UserId = {userid} AND Name = N'{DeleteTopicsNameTextBox.Text}'";
                    command.Connection = connection;
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        DeleteCheckNameLabel.Foreground = Brushes.Green;
                        DeleteCheckNameLabel.Content = "✔";
                        reader.Read();
                        topicId = reader.GetInt32(0);
                    }
                    else
                    {
                        DeleteCheckNameLabel.Foreground = Brushes.Red;
                        DeleteCheckNameLabel.Content = "✖";
                        topicId = 0;
                    }
                }
            }
        }

        private void DeleteTopicButton_Click(object sender, RoutedEventArgs e)
        {
            if(DeleteCheckNameLabel.Foreground == Brushes.Green)
            {
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.CommandText = $"SELECT Id FROM Topics WHERE UserId = {userid} AND Name = N'{DeleteTopicsNameTextBox.Text}'";
                    command.Connection = connection;
                    reader = command.ExecuteReader();
                    reader.Read();
                    topicId = reader.GetInt32(0);
                    reader.Close();

                    command.CommandText = $"DELETE FROM [Topics] WHERE Id = {topicId}";
                    command.ExecuteNonQuery();
                    command.CommandText = $"DROP TABLE [{topicId}]";
                    command.ExecuteNonQuery();
                    DeleteTopicsNameTextBox.Clear();
                    existingtopics.Clear();
                    adapter = new SqlDataAdapter($"SELECT Name FROM Topics WHERE UserId = {userid}", connection);
                    adapter.Fill(existingtopics);
                    ExistingTopicsDataGrid.ItemsSource = existingtopics.DefaultView;
                }
            }
        }

        private void CreateTopicButton_Click(object sender, RoutedEventArgs e)
        {
            if(CheckNameLabel.Foreground == Brushes.Green)
            {
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.CommandText = $"INSERT INTO Topics (UserId, Name) VALUES ({userid}, N'{TopicsNameTextBox.Text}')";
                    command.Connection = connection;
                    command.ExecuteNonQuery();
                    command.CommandText = $"SELECT Id FROM Topics WHERE UserId = {userid} AND Name = N'{TopicsNameTextBox.Text}'";
                    reader = command.ExecuteReader();
                    reader.Read();
                    topicId = reader.GetInt32(0);
                    reader.Close();

                    command.CommandText = $"CREATE TABLE[dbo].[{topicId}]([Id] INT IDENTITY(1, 1) NOT NULL, [TopicId] INT NOT NULL," +
                    "[Russian] NCHAR(50) NULL, [Description] NCHAR(50) NULL, [English] NCHAR(50) NULL, [Path] NCHAR(50) NULL," +
                    "PRIMARY KEY CLUSTERED([Id] ASC), FOREIGN KEY([TopicId]) REFERENCES[dbo].[Topics] ([Id]))";
                    command.ExecuteNonQuery();
                    TopicsNameTextBox.Clear();
                    existingtopics.Clear();
                    adapter = new SqlDataAdapter($"SELECT Name FROM Topics WHERE UserId = {userid}", connection);
                    adapter.Fill(existingtopics);
                    ExistingTopicsDataGrid.ItemsSource = existingtopics.DefaultView;
                }
            }
        }

        private void CreateTabItem_Loaded(object sender, RoutedEventArgs e)
        {
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter($"SELECT Name FROM Topics WHERE UserId = {userid}", connection);
                adapter.Fill(existingtopics);
                ExistingTopicsDataGrid.ItemsSource = existingtopics.DefaultView;
            }
        }
    }
}
