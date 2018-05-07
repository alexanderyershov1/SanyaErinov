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

        public User user;
        public List<Topic> topics;
        public Topic MainWords;
        int wordId = 0;
        int topicId;
        public int currentTopicId;
        static string Debug = Directory.GetCurrentDirectory();
        static string Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();
        static string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Shebist}\UserDB.mdf;Integrated Security=True";
        static SqlCommand command = new SqlCommand();
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
                foreach (Topic topic in topics)
                {
                    if (topic.Name == DeleteTopicsNameTextBox.Text)
                    {
                        topicId = topic.Id;
                        topics.Remove(topic);
                        break;
                    }
                }

                foreach (Topic topic in ExistingTopicsDataGrid.Items)
                {
                    if (topic.Name == DeleteTopicsNameTextBox.Text)
                    {
                        ExistingTopicsDataGrid.Items.Remove(topic);                  
                        break;
                    }
                }

                foreach (Topic topic in ExistingTopicsDataGrid2.Items)
                {
                    if (topic.Name == DeleteTopicsNameTextBox.Text)
                    {
                        ExistingTopicsDataGrid2.Items.Remove(topic);
                        break;
                    }
                }

                DeleteTopicsNameTextBox.Clear();

                if (ExistingTopicsDataGrid.Items.Count == 0)
                {
                    CreateEditTabControl.IsEnabled = false;
                    InputQuestionTextBox.Clear();
                    InputHintTextBox.Clear();
                    InputAnswerTextBox.Clear();
                    InputPathTextBox.Clear();
                    InputQuestionTextBox2.Clear();
                    InputHintTextBox2.Clear();
                    InputAnswerTextBox2.Clear();
                    InputPathTextBox2.Clear();
                }
            }
        }

        private void CreateTopicButton_Click(object sender, RoutedEventArgs e)
        {
            if (CheckNameLabel.Foreground == Brushes.Green)
            {
                topics.Add(new Topic { Name = TopicsNameTextBox.Text, CurrentIndex = 0, SequenceOfIndices = new List<int>() });
                ExistingTopicsDataGrid.Items.Add(new Topic { Name = TopicsNameTextBox.Text });
                ExistingTopicsDataGrid2.Items.Add(new Topic { Name = TopicsNameTextBox.Text });
                TopicsNameTextBox.Clear();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            foreach(Topic topic in topics)
            {
                ExistingTopicsDataGrid.Items.Add(topic);
                ExistingTopicsDataGrid2.Items.Add(topic);
            }
        }

        private void ExistingTopicsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Topic topic = (Topic)ExistingTopicsDataGrid.SelectedValue;
            if (topic != null)
            {
                DeleteTopicsNameTextBox.Text = topic.Name;
            }
        }

        int indexOfElement = 0;
        Topic currentTopic = null;
        private void ExistingTopicsDataGrid2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           if(currentTopic != null)
            {
                for (int i = 0; i < topics.Count; i++)
                {
                    if (topics[i].Name == currentTopic.Name)
                    {
                        topics[i] = currentTopic;
                    }
                }
            }
            
            WordsDataGrid.Items.Clear();
            InputQuestionTextBox.Clear();
            InputHintTextBox.Clear();
            InputAnswerTextBox.Clear();
            InputPathTextBox.Clear();
            currentTopic = (Topic)ExistingTopicsDataGrid2.SelectedValue;
            

            foreach (Topic topic in topics)
            {
                if(topic.Name == currentTopic.Name)
                {
                    currentTopic = topic;
                    break;
                }
            }

            if(ExistingTopicsDataGrid2.Items.Count != 0)
            {
                CreateEditTabControl.IsEnabled = true;

                foreach(Word word in currentTopic.Words)
                {
                    WordsDataGrid.Items.Add(word);
                }

                if (WordsDataGrid.Items.Count != 0)
                {
                    indexOfElement = 0;
                    wordId = currentTopic.Words[0].Id;
                    InputQuestionTextBox.Text = currentTopic.Words[0].Question;
                    InputHintTextBox.Text = currentTopic.Words[0].Hint;
                    InputAnswerTextBox.Text = currentTopic.Words[0].Answer;
                    InputPathTextBox.Text = currentTopic.Words[0].Path;
                }
                else wordId = indexOfElement = 0;
            }
        }

        private void BackMenuItem_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new MainPage { user = user,
                topics = topics, MainWords = MainWords
            });
        }


        private void DownButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (indexOfElement < WordsDataGrid.Items.Count - 1)
            {
                Word word = (Word)WordsDataGrid.Items[++indexOfElement];
                wordId = word.Id;
                InputQuestionTextBox.Text = word.Question;
                InputHintTextBox.Text = word.Hint;
                InputAnswerTextBox.Text = word.Answer;
                InputPathTextBox.Text = word.Path;
            }
            else
            {
                indexOfElement = 0;
                Word word = (Word)WordsDataGrid.Items[indexOfElement];
                wordId = word.Id;
                InputQuestionTextBox.Text = word.Question;
                InputHintTextBox.Text = word.Hint;
                InputAnswerTextBox.Text = word.Answer;
                InputPathTextBox.Text = word.Path;
            }
        }

        private void ChangeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (wordId != 0)
            {
                foreach(Word word in currentTopic.Words)
                {
                    if(word.Id == wordId)
                    {
                        word.Question = InputQuestionTextBox.Text.Trim();
                        word.Hint = InputHintTextBox.Text.Trim();
                        word.Answer = InputAnswerTextBox.Text.Trim();
                        word.Path = InputPathTextBox.Text.Trim();
                    }
                }
            }
        }

        private void DeleteButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (wordId != 0)
            {
                WordsDataGrid.Items.Clear();

                foreach (Word word in currentTopic.Words)
                {
                    if (word.Id == wordId)
                    {
                        currentTopic.Words.Remove(word);
                    }
                }

                foreach (Word word in currentTopic.Words)
                {
                    WordsDataGrid.Items.Add(word);
                }

                if (currentTopic.SequenceOfIndices.Count != 0)
                {
                    currentTopic.SequenceOfIndices.Clear();

                    for (int i = 0; i < currentTopic.Words.Count; i++)
                    {
                        currentTopic.SequenceOfIndices.Add(i);
                    }
                }

                foreach (Word word in currentTopic.Words)
                {
                    if (word.Id > wordId)
                    {
                        wordId = word.Id;
                        break;
                    }
                    else if(word.Id < wordId)
                    {
                        wordId = word.Id;
                        break;
                    }
                }

                foreach (Word word in currentTopic.Words)
                {
                    if(word.Id == wordId)
                    {
                        InputQuestionTextBox.Text = word.Question;
                        InputHintTextBox.Text = word.Hint;
                        InputAnswerTextBox.Text = word.Answer;
                        InputPathTextBox.Text = word.Path;
                    }
                }
            }
        }

        private void AddButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            int newWordId = 0;
            foreach(Word word in currentTopic.Words)
            {
                if (word.Id > newWordId) newWordId = word.Id;
            }
            newWordId++;

            WordsDataGrid.Items.Add(new Word
            {
                Id = newWordId,
                Question = InputQuestionTextBox2.Text.Trim(),
                Hint = InputHintTextBox2.Text.Trim(),
                Answer = InputAnswerTextBox2.Text.Trim(),
                Path = InputPathTextBox2.Text.Trim()
            });

            currentTopic.Words.Add(new Word
            {
                Id = newWordId,
                TopicId = currentTopic.Id,
                Question = InputQuestionTextBox2.Text.Trim(),
                Hint = InputHintTextBox2.Text.Trim(),
                Answer = InputAnswerTextBox2.Text.Trim(),
                Path = InputPathTextBox2.Text.Trim()
            });
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
                wordId = word.Id;
                InputQuestionTextBox.Text = word.Question;
                InputHintTextBox.Text = word.Hint;
                InputAnswerTextBox.Text = word.Answer;
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
                InputQuestionTextBox.Text = word.Question;
                InputHintTextBox.Text = word.Hint;
                InputAnswerTextBox.Text = word.Answer;
                InputPathTextBox.Text = word.Path;
            }
            else
            {
                indexOfElement = WordsDataGrid.Items.Count - 1;
                Word word = (Word)WordsDataGrid.Items[indexOfElement];
                wordId = word.Id;
                InputQuestionTextBox.Text = word.Question;
                InputHintTextBox.Text = word.Hint;
                InputAnswerTextBox.Text = word.Answer;
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
