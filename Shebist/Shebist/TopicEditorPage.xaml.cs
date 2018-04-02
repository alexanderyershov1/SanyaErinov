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

        int topicId = 0, wordId = 0;
        static string Debug = Directory.GetCurrentDirectory();
        static string Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();
        static string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Shebist}\UserDB.mdf;Integrated Security=True";
        static SqlCommand command = new SqlCommand();
        List<string> topics;
        SqlDataReader reader;
        SqlDataAdapter adapter;
        DataTable existingtopics = new DataTable(), words = new DataTable();


        private void BackLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MainPage mp = new MainPage();
            this.NavigationService.Navigate(mp);
        }

        private void TopicsNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TopicsNameTextBox.Text == "")
            {
                CheckNameLabel.Foreground = Brushes.Red;
                CheckNameLabel.Content = "✖";
            }
            else
            {
                if (topics.Contains(TopicsNameTextBox.Text))
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

        private void DeleteTopicsNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (DeleteTopicsNameTextBox.Text == "")
            {
                DeleteCheckNameLabel.Foreground = Brushes.Red;
                DeleteCheckNameLabel.Content = "✖";
            }
            else
            {
                if (topics.Contains(DeleteTopicsNameTextBox.Text))
                {
                    DeleteCheckNameLabel.Foreground = Brushes.Green;
                    DeleteCheckNameLabel.Content = "✔";
                }
                else
                {
                    DeleteCheckNameLabel.Foreground = Brushes.Red;
                    DeleteCheckNameLabel.Content = "✖";
                }

            }
        }

        private void DeleteTopicButton_Click(object sender, RoutedEventArgs e)
        {
            if (DeleteCheckNameLabel.Foreground == Brushes.Green)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.CommandText = $"SELECT Id FROM Topics WHERE UserId = {userid} AND Name = N'{DeleteTopicsNameTextBox.Text}'";
                    command.Connection = connection;
                    reader = command.ExecuteReader();
                    reader.Read();
                    topicId = reader.GetInt32(0);
                    reader.Close();

                    command.CommandText = $"DROP TABLE [{topicId}]";
                    command.ExecuteNonQuery();
                    command.CommandText = $"DELETE FROM [Topics] WHERE Id = {topicId}";
                    command.ExecuteNonQuery();
                    existingtopics.Clear();
                    adapter = new SqlDataAdapter($"SELECT Name FROM Topics WHERE UserId = {userid}", connection);
                    adapter.Fill(existingtopics);
                    ExistingTopicsDataGrid.ItemsSource = existingtopics.DefaultView;

                    topics.Remove(DeleteTopicsNameTextBox.Text);
                    DeleteTopicsNameTextBox.Clear();
                }
            }
        }

        private void CreateTopicButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckNameLabel.Foreground == Brushes.Green)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
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

                    command.CommandText = $"CREATE TABLE [dbo].[{topicId}]([Id] INT IDENTITY(1, 1) NOT NULL, [TopicId] INT NOT NULL," +
                    "[Russian] NCHAR(50) NULL, [Description] NCHAR(50) NULL, [English] NCHAR(50) NULL, [Path] NCHAR(50) NULL," +
                    "PRIMARY KEY CLUSTERED([Id] ASC), FOREIGN KEY([TopicId]) REFERENCES[dbo].[Topics] ([Id]))";
                    command.ExecuteNonQuery();
                    
                    existingtopics.Clear();
                    adapter = new SqlDataAdapter($"SELECT Name FROM Topics WHERE UserId = {userid}", connection);
                    adapter.Fill(existingtopics);
                    ExistingTopicsDataGrid.ItemsSource = ExistingTopicsDataGrid2.ItemsSource =
                    existingtopics.DefaultView;

                    topics.Add(TopicsNameTextBox.Text);
                    TopicsNameTextBox.Clear();
                }
            }
        }

        private void EditTabItem_Loaded(object sender, RoutedEventArgs e)
        {
            ExistingTopicsDataGrid2.ItemsSource = existingtopics.DefaultView;
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = $"SELECT COUNT(*) FROM Topics WHERE UserId = {userid}";
                reader = command.ExecuteReader();
                reader.Read();
                topics = new List<string>(reader.GetInt32(0));
                reader.Close();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainPage mp = new MainPage();
            this.NavigationService.Navigate(mp);
        }

        

        private void ChooseTopicTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (topics.Contains(ChooseTopicTextBox.Text))
            {
                CheckTopicLabel.Foreground = Brushes.Green;
                CheckTopicLabel.Content = "✔";
            }
            else
            {
                CheckTopicLabel.Foreground = Brushes.Red;
                CheckTopicLabel.Content = "✖";
            }
        }

        private void ChooseTopicButton_Click(object sender, RoutedEventArgs e)
        {
            if(CheckTopicLabel.Foreground == Brushes.Green)
            {
                words.Clear();
                InputRussianTextBox.Clear();
                InputDescriptionTextBox.Clear();
                InputEnglishTextBox.Clear();
                InputPathTextBox.Clear();
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = $"SELECT Id FROM Topics WHERE UserId = {userid} AND Name = N'{ChooseTopicTextBox.Text}'";
                    reader = command.ExecuteReader();
                    reader.Read();
                    topicId = reader.GetInt32(0);
                    reader.Close();
                    adapter = new SqlDataAdapter($"SELECT Russian, Description, English, Path FROM [{topicId}] ORDER BY Id", connection);
                    adapter.Fill(words);
                    WordsDataGrid.ItemsSource = words.DefaultView;
                    command.CommandText = $"SELECT Id, Russian, Description, English, Path FROM [{topicId}] WHERE Id = (SELECT MIN(Id) FROM [{topicId}])";
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        wordId = reader.GetInt32(0);
                        InputRussianTextBox.Text = reader.GetString(1).Trim();
                        InputDescriptionTextBox.Text = reader.GetString(2).Trim();
                        InputEnglishTextBox.Text = reader.GetString(3).Trim();
                        InputPathTextBox.Text = reader.GetString(4).Trim();
                    }
                    else
                        wordId = 0;
                    reader.Close();
                }
                CreateEditTabControl.IsEnabled = true;
                TopicNameLabel.Content = ChooseTopicTextBox.Text;
                ChooseTopicTextBox.Clear();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = $"INSERT INTO [{topicId.ToString()}]" +
                    $" (TopicId, Russian, Description, English, Path) VALUES ({topicId}, N'{InputRussianTextBox2.Text}', N'{InputDescriptionTextBox2.Text}'," +
                    $"N'{InputEnglishTextBox2.Text}', N'{InputPathTextBox2.Text}')";
                command.ExecuteNonQuery();
                words.Clear();
                adapter = new SqlDataAdapter($"SELECT Russian, Description, English, Path FROM [{topicId}] ORDER BY Id", connection);
                adapter.Fill(words);

                if (wordId == 0)
                {
                    command.CommandText = $"SELECT Id, Russian, Description, English, Path FROM [{topicId}] WHERE Id = (SELECT MIN(Id) FROM [{topicId}])";
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        wordId = reader.GetInt32(0);
                        InputRussianTextBox.Text = reader.GetString(1).Trim();
                        InputDescriptionTextBox.Text = reader.GetString(2).Trim();
                        InputEnglishTextBox.Text = reader.GetString(3).Trim();
                        InputPathTextBox.Text = reader.GetString(4).Trim();
                    }
                }
            }
            InputRussianTextBox2.Clear();
            InputDescriptionTextBox2.Clear();
            InputEnglishTextBox2.Clear();
            InputPathTextBox2.Clear();
        }

        private void DownButton_Click(object sender, RoutedEventArgs e)
        {
            if(wordId != 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = $"SELECT Id, Russian, Description, English, Path FROM [{topicId}] WHERE Id > {wordId}";
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        wordId = reader.GetInt32(0);
                        InputRussianTextBox.Text = reader.GetString(1).Trim();
                        InputDescriptionTextBox.Text = reader.GetString(2).Trim();
                        InputEnglishTextBox.Text = reader.GetString(3).Trim();
                        InputPathTextBox.Text = reader.GetString(4).Trim();
                    }
                }
            } 
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if(wordId != 0)
            {
                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = $"UPDATE [{topicId}] SET Russian = N'{InputRussianTextBox.Text}'" +
                        $", Description = N'{InputDescriptionTextBox.Text}', English = N'{InputEnglishTextBox.Text}'," +
                        $"Path = N'{InputPathTextBox.Text}' WHERE Id = {wordId}";
                    command.ExecuteNonQuery();
                    words.Clear();
                    adapter = new SqlDataAdapter($"SELECT Russian, Description, English, Path FROM [{topicId}] ORDER BY Id", connection);
                    adapter.Fill(words);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if(wordId != 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = $"DELETE FROM [{topicId}] WHERE Id = {wordId}";
                    command.ExecuteNonQuery();
                    words.Clear();
                    adapter = new SqlDataAdapter($"SELECT Russian, Description, English, Path FROM [{topicId}] ORDER BY Id", connection);
                    adapter.Fill(words);
                    command.CommandText = $"SELECT Id, Russian, Description, English, Path FROM [{topicId}] WHERE Id > {wordId}";
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        wordId = reader.GetInt32(0);
                        InputRussianTextBox.Text = reader.GetString(1).Trim();
                        InputDescriptionTextBox.Text = reader.GetString(2).Trim();
                        InputEnglishTextBox.Text = reader.GetString(3).Trim();
                        InputPathTextBox.Text = reader.GetString(4).Trim();
                    }
                    else
                    {
                        reader.Close();
                        command.CommandText = $"SELECT Id, Russian, Description, English, Path FROM [{topicId}] WHERE Id < {wordId}";
                        reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                wordId = reader.GetInt32(0);
                                InputRussianTextBox.Text = reader.GetString(1).Trim();
                                InputDescriptionTextBox.Text = reader.GetString(2).Trim();
                                InputEnglishTextBox.Text = reader.GetString(3).Trim();
                                InputPathTextBox.Text = reader.GetString(4).Trim();
                            }
                        }
                        else
                        {
                            reader.Close();
                            wordId = 0;
                            InputRussianTextBox.Clear();
                            InputDescriptionTextBox.Clear();
                            InputEnglishTextBox.Clear();
                            InputPathTextBox.Clear();
                        }
                    }
                }
            }
        }

        private void UpButton_Click(object sender, RoutedEventArgs e)
        {
            if(wordId != 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = $"SELECT Id, Russian, Description, English, Path FROM [{topicId}] WHERE Id < {wordId}";
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            wordId = reader.GetInt32(0);
                            InputRussianTextBox.Text = reader.GetString(1).Trim();
                            InputDescriptionTextBox.Text = reader.GetString(2).Trim();
                            InputEnglishTextBox.Text = reader.GetString(3).Trim();
                            InputPathTextBox.Text = reader.GetString(4).Trim();
                        }
                    }
                }
            }  
        }

        private void CreateTabItem_Loaded(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                adapter = new SqlDataAdapter($"SELECT Name FROM Topics WHERE UserId = {userid}", connection);
                adapter.Fill(existingtopics);
                ExistingTopicsDataGrid.ItemsSource = existingtopics.DefaultView;
                command.CommandText = $"SELECT Name FROM Topics WHERE UserId = {userid}";
                command.Connection = connection;
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    topics.Add(reader.GetString(0));
                }
            }
        }
    }
}
