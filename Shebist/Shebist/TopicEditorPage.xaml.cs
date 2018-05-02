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
        public int userid;
        public TopicEditorPage()
        {
            InitializeComponent();
            Menu.Opacity = 0;

        }

        int wordId = 0;
        string topicId;
        public string currentTopicId;
        static string Debug = Directory.GetCurrentDirectory();
        static string Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();
        static string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Shebist}\UserDB.mdf;Integrated Security=True";
        static SqlCommand command = new SqlCommand();
        public List<Topic> topics;
        SqlDataReader reader;

        private void TopicsNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TopicsNameTextBox.Text == "")
            {
                CheckNameLabel.Foreground = Brushes.Red;
                CheckNameLabel.Content = "✖";
            }
            else
            {
                if(topics.Count != 0)
                {
                    foreach(Topic topic in topics)
                    {
                        if(topic.Name == TopicsNameTextBox.Text)
                        {
                            CheckNameLabel.Foreground = Brushes.Red;
                            CheckNameLabel.Content = "✖";
                            return;
                        }
                        else
                        {
                            CheckNameLabel.Foreground = Brushes.Green;
                            CheckNameLabel.Content = "✔";
                        }
                    }
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
                foreach(Topic topic in topics)
                {
                    if (topic.Name == DeleteTopicsNameTextBox.Text)
                    {
                        DeleteCheckNameLabel.Foreground = Brushes.Green;
                        DeleteCheckNameLabel.Content = "✔";
                        return;
                    }
                    else
                    {
                        DeleteCheckNameLabel.Foreground = Brushes.Red;
                        DeleteCheckNameLabel.Content = "✖";
                    }
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
                    command.Connection = connection;

                    foreach(Topic topic in topics)
                    {
                        if(topic.Name == DeleteTopicsNameTextBox.Text)
                        {
                            topicId = topic.Id;
                            topics.Remove(topic);
                            break;
                        }
                    }

                    foreach(ItemsControl item in ExistingTopicsDataGrid.Items)
                    {
                        ExistingTopicsDataGrid.Items.Remove(item);
                        ExistingTopicsDataGrid2.Items.Remove(item);
                    }
                    
                    foreach(Topic topic in topics)
                    {
                        ExistingTopicsDataGrid.Items.Add(new Topic { Name = topic.Name });
                        ExistingTopicsDataGrid2.Items.Add(new Topic { Name = topic.Name });
                    }

                    command.CommandText = $"DROP TABLE [{topicId}]";
                    command.ExecuteNonQuery();
                    command.CommandText = $"DELETE FROM [Topics] WHERE Id = {topicId}";
                    command.ExecuteNonQuery();

                    DeleteTopicsNameTextBox.Clear();

                   if(ExistingTopicsDataGrid.Items.Count == 0)
                    {
                        CreateEditTabControl.IsEnabled = false;
                    }
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
                    topicId = reader.GetInt32(0).ToString();
                    reader.Close();

                    command.CommandText = $"CREATE TABLE [dbo].[{topicId}]([Id] INT IDENTITY(1, 1) NOT NULL, [TopicId] INT NOT NULL," +
                    "[Russian] NCHAR(50) NULL, [Description] NCHAR(50) NULL, [English] NCHAR(50) NULL, [Path] NCHAR(50) NULL," +
                    "PRIMARY KEY CLUSTERED([Id] ASC), FOREIGN KEY([TopicId]) REFERENCES[dbo].[Topics] ([Id]))";
                    command.ExecuteNonQuery();

                    topics.Add(new Topic { Id = topicId, Name = TopicsNameTextBox.Text });
                    ExistingTopicsDataGrid.Items.Add(new Topic { Name = TopicsNameTextBox.Text });
                    ExistingTopicsDataGrid2.Items.Add(new Topic { Name = TopicsNameTextBox.Text });
                    TopicsNameTextBox.Clear();
                }
            }
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
                topics = new List<Topic>(reader.GetInt32(0));
                reader.Close();

                command.CommandText = $"SELECT Id, Name FROM Topics WHERE UserId = {userid}";
                reader = command.ExecuteReader();
                while (reader.Read())
                {
                    ExistingTopicsDataGrid.Items.Add(new Topic { Name = reader.GetString(1).Trim() });
                    ExistingTopicsDataGrid2.Items.Add(new Topic { Name = reader.GetString(1).Trim() });
                    topics.Add(new Topic { Id = reader.GetInt32(0).ToString(), Name = reader.GetString(1).Trim() });
                }
            }
        }

        private void ExistingTopicsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Topic topic = (Topic)ExistingTopicsDataGrid.SelectedValue;
            if(topic != null)
            {
                DeleteTopicsNameTextBox.Text = topic.Name;
            }
        }

        int indexOfElement = 0;
        private void ExistingTopicsDataGrid2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            for (int i = 0; i < WordsDataGrid.Items.Count;)
            {
                WordsDataGrid.Items.RemoveAt(i);
            }

            InputRussianTextBox.Clear();
            InputDescriptionTextBox.Clear();
            InputEnglishTextBox.Clear();
            InputPathTextBox.Clear();
            Topic topic = (Topic)ExistingTopicsDataGrid2.SelectedValue;
            if(ExistingTopicsDataGrid2.Items.Count != 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.Connection = connection;
                    //Выбираем Id темы, где Id пользователя = userid и название темы = topic.Name
                    command.CommandText = $"SELECT Id FROM Topics WHERE UserId = {userid} AND Name = N'{topic.Name}'";
                    reader = command.ExecuteReader();
                    reader.Read();
                    topicId = reader.GetInt32(0).ToString();
                    reader.Close();
                    command.CommandText = $"SELECT Id, Russian, Description, English, Path FROM [{topicId}]";
                    reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        WordsDataGrid.Items.Add(new Word
                        {
                            Id = reader.GetInt32(0),
                            Russian = reader.GetString(1).Trim(),
                            Description = reader.GetString(2).Trim(),
                            English = reader.GetString(3).Trim(),
                            Path = reader.GetString(4).Trim()
                        });
                    }
                    reader.Close();
                    if (WordsDataGrid.Items.Count != 0)
                    {
                        indexOfElement = 0;
                        Word word = (Word)WordsDataGrid.Items[indexOfElement];
                        wordId = word.Id;
                        InputRussianTextBox.Text = word.Russian;
                        InputDescriptionTextBox.Text = word.Description;
                        InputEnglishTextBox.Text = word.English;
                        InputPathTextBox.Text = word.Path;
                    }
                    else wordId = indexOfElement = 0;             
                }

                CreateEditTabControl.IsEnabled = true;
            }     
        }

        private void BackMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new MainPage { userid = userid,
                topics = topics
            });
        }


        private void DownButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (indexOfElement < WordsDataGrid.Items.Count - 1)
            {
                Word word = (Word)WordsDataGrid.Items[++indexOfElement];
                wordId = word.Id;
                InputRussianTextBox.Text = word.Russian;
                InputDescriptionTextBox.Text = word.Description;
                InputEnglishTextBox.Text = word.English;
                InputPathTextBox.Text = word.Path;
            }
            else
            {
                indexOfElement = 0;
                Word word = (Word)WordsDataGrid.Items[indexOfElement];
                wordId = word.Id;
                InputRussianTextBox.Text = word.Russian;
                InputDescriptionTextBox.Text = word.Description;
                InputEnglishTextBox.Text = word.English;
                InputPathTextBox.Text = word.Path;
            }
        }

        private void ChangeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (wordId != 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = $"UPDATE [{topicId}] SET Russian = N'{InputRussianTextBox.Text}'" +
                        $", Description = N'{InputDescriptionTextBox.Text}', English = N'{InputEnglishTextBox.Text}'," +
                        $"Path = N'{InputPathTextBox.Text}' WHERE Id = {wordId}";
                    command.ExecuteNonQuery();
                    WordsDataGrid.Items.Clear();
                    command.CommandText = $"SELECT Id, Russian, Description, English, Path FROM [{topicId}]";
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            WordsDataGrid.Items.Add(new Word
                            {
                                Id = reader.GetInt32(0),
                                Russian = reader.GetString(1).Trim(),
                                Description = reader.GetString(2).Trim(),
                                English = reader.GetString(3).Trim(),
                                Path = reader.GetString(4).Trim()

                            });
                        }
                    }
                }
            }
        }

        private void DeleteButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (wordId != 0)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = $"DELETE FROM [{topicId}] WHERE Id = {wordId}";
                    command.ExecuteNonQuery();
                    WordsDataGrid.Items.Clear();
                    command.CommandText = $"SELECT Id, Russian, Description, English, Path FROM [{topicId}]";
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            WordsDataGrid.Items.Add(new Word
                            {
                                Id = reader.GetInt32(0),
                                Russian = reader.GetString(1).Trim(),
                                Description = reader.GetString(2).Trim(),
                                English = reader.GetString(3).Trim(),
                                Path = reader.GetString(4).Trim()

                            });
                        }
                    }
                    reader.Close();

                    if (topicId == currentTopicId)
                    {
                        command.CommandText = $"SELECT indicies FROM UserDb WHERE Id = {userid}";
                        reader = command.ExecuteReader();
                        reader.Read();
                        string oldIndicies = reader.GetString(0);
                        reader.Close();
                        string newIndicies = "";
                        int j = 0;
                        for(int i = 0; i < oldIndicies.Length; i++)
                        {
                            if (j == WordsDataGrid.Items.Count) break;
                            if (oldIndicies[i] == ';') j++;
                            newIndicies += oldIndicies[i];
                        }
                        command.CommandText = $"UPDATE UserDB SET indicies = '{newIndicies}'";
                        command.ExecuteNonQuery();
                    }
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
                    reader.Close();
                }
            }
        }

        private void AddButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = $"INSERT INTO [{topicId}]" +
                    $" (TopicId, Russian, Description, English, Path) VALUES ({topicId}, N'{InputRussianTextBox2.Text}', N'{InputDescriptionTextBox2.Text}'," +
                    $"N'{InputEnglishTextBox2.Text}', N'{InputPathTextBox2.Text}')";
                command.ExecuteNonQuery();

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
                reader.Close();

                if (currentTopicId == topicId)
                {
                    command.CommandText = $"UPDATE UserDB SET indicies = indicies + '{WordsDataGrid.Items.Count};'";
                    command.ExecuteNonQuery();
                }
            }

            WordsDataGrid.Items.Add(new Word
            {
                Russian = InputRussianTextBox2.Text,
                Description = InputDescriptionTextBox2.Text,
                English = InputEnglishTextBox2.Text,
                Path = InputPathTextBox2.Text
            });

            
            
            InputRussianTextBox2.Clear();
            InputDescriptionTextBox2.Clear();
            InputEnglishTextBox2.Clear();
            InputPathTextBox2.Clear();
        }

        private void Menu_MouseEnter(object sender, MouseEventArgs e)
        {
            Menu.Opacity = 1;
        }

        private void Menu_MouseLeave(object sender, MouseEventArgs e)
        {
            Menu.Opacity = 0;
        }

        private void WordsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Word word = (Word)WordsDataGrid.SelectedItem;
            if(word != null)
            {
                for (int i = 0; i < WordsDataGrid.Items.Count; i++)
                {
                    Word word2 = (Word)WordsDataGrid.Items[i];
                    if (word2.Id == word.Id) indexOfElement = i;
                }
                wordId = word.Id;
                InputRussianTextBox.Text = word.Russian;
                InputDescriptionTextBox.Text = word.Description;
                InputEnglishTextBox.Text = word.English;
                InputPathTextBox.Text = word.Path;
            }
        }

        private void UpButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UpButton.Height = UpButton.Width = 20;
            if (indexOfElement != 0)
            {
                Word word = (Word)WordsDataGrid.Items[--indexOfElement];
                wordId = word.Id;
                InputRussianTextBox.Text = word.Russian;
                InputDescriptionTextBox.Text = word.Description;
                InputEnglishTextBox.Text = word.English;
                InputPathTextBox.Text = word.Path;
            }
            else
            {
                indexOfElement = WordsDataGrid.Items.Count - 1;
                Word word = (Word)WordsDataGrid.Items[indexOfElement];
                wordId = word.Id;
                InputRussianTextBox.Text = word.Russian;
                InputDescriptionTextBox.Text = word.Description;
                InputEnglishTextBox.Text = word.English;
                InputPathTextBox.Text = word.Path;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool hasData = false;
            foreach (Topic topic in topics)
            {
                if (topic.Name.ToLower().Contains(SearchTextBox.Text.ToLower()))
                {
                    hasData = true;
                    break;
                }
            }

            if(hasData)
            {
                ExistingTopicsDataGrid2.Items.Clear();
                foreach (Topic topic in topics)
                {
                    if (topic.Name.ToLower().Contains(SearchTextBox.Text.ToLower())) { ExistingTopicsDataGrid2.Items.Add(topic); }
                }
            }
            else
            {
                ExistingTopicsDataGrid2.Items.Clear();
                foreach (Topic topic in topics)
                {
                    ExistingTopicsDataGrid2.Items.Add(topic);
                }
            }
        }
    }
}
