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
    public partial class TopicEditorWindow : Window
    {
        public TopicEditorWindow()
        {
            InitializeComponent();
            Menu.Opacity = 0;

        }

        public User oldUser, newUser;
        public bool needToUpdate;
        public List<Topic> oldTopics, newTopics;
        List<int> indicesOfDeletedTopics = new List<int>();
        List<Word> deletedWords = new List<Word>();
        public Topic MainWords;
        public int currentTopicId;
        static string Debug = Directory.GetCurrentDirectory();
        static string Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();
        static string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Shebist}\UserDB.mdf;Integrated Security=True";
        static SqlCommand command = new SqlCommand();

        private void TopicsNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (TopicsNameTextBox.Text == "")
            {
                CheckNameLabel.Foreground = Brushes.Red;
                CheckNameLabel.Content = "✖";
            }
            else
            {
                if (newTopics.Count != 0)
                {
                    foreach (Topic topic in newTopics)
                    {
                        if (topic.Name == TopicsNameTextBox.Text)
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
                foreach (Topic topic in newTopics)
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
                foreach (Topic topic in newTopics)
                {
                    if (topic.Name == DeleteTopicsNameTextBox.Text)
                    {
                        newTopics.Remove(topic);
                        if(!topic.Id.StartsWith("temp"))
                            indicesOfDeletedTopics.Add(Int32.Parse(topic.Id));
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
                newTopics.Add(new Topic
                { Id = "temp" + (newTopics.Count + 1), 
                  Name = TopicsNameTextBox.Text,
                  CurrentIndex = 0,
                  SequenceOfIndices = new List<short>()
                });
                ExistingTopicsDataGrid.Items.Add(new Topic { Name = TopicsNameTextBox.Text });
                ExistingTopicsDataGrid2.Items.Add(new Topic { Name = TopicsNameTextBox.Text });
                TopicsNameTextBox.Clear();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Topic topic in newTopics)
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
            if (currentTopic != null)
            {
                for (int i = 0; i < newTopics.Count; i++)
                {
                    if (newTopics[i].Name == currentTopic.Name)
                    {
                        newTopics[i] = currentTopic;
                    }
                }
            }

            WordsDataGrid.Items.Clear();
            InputQuestionTextBox.Clear();
            InputHintTextBox.Clear();
            InputAnswerTextBox.Clear();
            InputPathTextBox.Clear();
            currentTopic = (Topic)ExistingTopicsDataGrid2.SelectedValue;

            if (currentTopic != null)
            {
                foreach (Topic topic in newTopics)
                {
                    if (topic.Name == currentTopic.Name)
                    {
                        currentTopic = topic;
                        break;
                    }
                }

                if (ExistingTopicsDataGrid2.Items.Count != 0)
                {
                    CreateEditTabControl.IsEnabled = true;

                    foreach (Word word in currentTopic.Words)
                    {
                        WordsDataGrid.Items.Add(word);
                    }

                    if (WordsDataGrid.Items.Count != 0)
                    {
                        indexOfElement = 0;
                        InputQuestionTextBox.Text = currentTopic.Words[0].Question;
                        InputHintTextBox.Text = currentTopic.Words[0].Hint;
                        InputAnswerTextBox.Text = currentTopic.Words[0].Answer;
                        InputPathTextBox.Text = currentTopic.Words[0].Path;
                    }
                    else indexOfElement = 0;
                }
            }
            else
            {
                CreateEditTabControl.IsEnabled = false;
                WordsDataGrid.Items.Clear();
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

        private void BackMenuItem_Click(object sender, RoutedEventArgs e)
        {
            needToUpdate = false;
            if (currentTopic != null)
            {
                for (int i = 0; i < newTopics.Count; i++)
                {
                    if (newTopics[i].Name == currentTopic.Name)
                    {
                        newTopics[i] = currentTopic;
                        break;
                    }
                }
            }

            MainWindow mw = new MainWindow
            {
                needToUpdate = true,
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


        private void DownButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentTopic.Words.Count > 1)
            {
                if (indexOfElement == currentTopic.Words.Count - 1)
                {
                    indexOfElement = 0;
                    InputQuestionTextBox.Text = currentTopic.Words[0].Question;
                    InputHintTextBox.Text = currentTopic.Words[0].Hint;
                    InputAnswerTextBox.Text = currentTopic.Words[0].Answer;
                    InputPathTextBox.Text = currentTopic.Words[0].Path;
                }
                else
                {
                    InputQuestionTextBox.Text = currentTopic.Words[++indexOfElement].Question;
                    InputHintTextBox.Text = currentTopic.Words[indexOfElement].Hint;
                    InputAnswerTextBox.Text = currentTopic.Words[indexOfElement].Answer;
                    InputPathTextBox.Text = currentTopic.Words[indexOfElement].Path;
                }
            }
        }

        private void ChangeButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(currentTopic.Words.Count.ToString());   
            if (currentTopic.Words.Count > 0)
            {
                currentTopic.Words[indexOfElement].Question = InputQuestionTextBox.Text.Trim();
                currentTopic.Words[indexOfElement].Hint = InputHintTextBox.Text.Trim();
                currentTopic.Words[indexOfElement].Answer = InputAnswerTextBox.Text.Trim();
                currentTopic.Words[indexOfElement].Path = InputPathTextBox.Text.Trim();

                WordsDataGrid.Items.Clear();

                foreach(Word word in currentTopic.Words)
                {
                    WordsDataGrid.Items.Add(word);
                }

                for (int i = 0; i < currentTopic.SequenceOfIndices.Count; i++)
                {
                    if (currentTopic.SequenceOfIndices[i] == indexOfElement)
                    {
                        currentTopic.currentWords[i].Question = InputQuestionTextBox.Text.Trim();
                        currentTopic.currentWords[i].Hint = InputHintTextBox.Text.Trim();
                        currentTopic.currentWords[i].Answer = InputAnswerTextBox.Text.Trim();
                        currentTopic.currentWords[i].Path = InputPathTextBox.Text.Trim();
                        break;
                    }
                }
            }
        }

        private void DeleteButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(currentTopic.currentWords.Count.ToString());
            MessageBox.Show("Индекс " + indexOfElement);

            if (currentTopic.Words.Count > 0)
            {
                if (!currentTopic.Words[indexOfElement].Id.StartsWith("temp"))
                    deletedWords.Add(currentTopic.Words[indexOfElement]);

                for (int i = 0; i < currentTopic.SequenceOfIndices.Count; i++)
                {
                    if (currentTopic.SequenceOfIndices[i] == indexOfElement)
                    {
                        currentTopic.currentWords.RemoveAt(i);
                        break;
                    }
                }

                if (indexOfElement == 0)
                {
                    for (int i = 0; i < currentTopic.SequenceOfIndices.Count; i++)
                    {
                        if (currentTopic.SequenceOfIndices[i] == 0)
                        {
                            currentTopic.SequenceOfIndices.RemoveAt(i);
                            break;
                        }
                    }

                    for (int i = 0; i < currentTopic.SequenceOfIndices.Count; i++)
                    {
                        currentTopic.SequenceOfIndices[i]--;
                    }
                }
                else
                {
                    List<int> indiciesForDecrement = new List<int>();

                    for (int i = 0; i < currentTopic.SequenceOfIndices.Count; i++)
                    {
                        if (currentTopic.SequenceOfIndices[i] == indexOfElement)
                        {
                            currentTopic.SequenceOfIndices.RemoveAt(i);
                            break;
                        }
                    }

                    for (int i = 0; i < currentTopic.SequenceOfIndices.Count; i++)
                    {
                        if (currentTopic.SequenceOfIndices[i] > indexOfElement)
                        {
                            indiciesForDecrement.Add(currentTopic.SequenceOfIndices[i]);
                        }
                    }

                    for (int i = 0; i < currentTopic.SequenceOfIndices.Count; i++)
                    {
                        if (indiciesForDecrement.Contains(currentTopic.SequenceOfIndices[i]))
                        {
                            currentTopic.SequenceOfIndices[i]--;

                            indiciesForDecrement.Remove(currentTopic.SequenceOfIndices[i]);
                        }
                    }
                }

                string s = "Последовательность индексов";
                foreach (int i in currentTopic.SequenceOfIndices)
                {
                    s += i + " ";
                }
                MessageBox.Show(s);

                if (currentTopic.Words.Count > 1)
                {
                    WordsDataGrid.Items.RemoveAt(indexOfElement);
                    
                    int lastIndex = currentTopic.Words.Count - 1;

                    currentTopic.Words.RemoveAt(indexOfElement);

                    if (indexOfElement == lastIndex)
                    {
                        InputQuestionTextBox.Text = currentTopic.Words[--indexOfElement].Question;
                        InputHintTextBox.Text = currentTopic.Words[indexOfElement].Hint;
                        InputAnswerTextBox.Text = currentTopic.Words[indexOfElement].Answer;
                        InputPathTextBox.Text = currentTopic.Words[indexOfElement].Path;
                    }
                    else
                    {
                        InputQuestionTextBox.Text = currentTopic.Words[indexOfElement].Question;
                        InputHintTextBox.Text = currentTopic.Words[indexOfElement].Hint;
                        InputAnswerTextBox.Text = currentTopic.Words[indexOfElement].Answer;
                        InputPathTextBox.Text = currentTopic.Words[indexOfElement].Path;
                    }

                    
                }
                else
                {
                    WordsDataGrid.Items.Clear();
                    currentTopic.Words.Clear();
                    currentTopic.currentWords.Clear();

                    InputQuestionTextBox.Clear();
                    InputHintTextBox.Clear();
                    InputAnswerTextBox.Clear();
                    InputPathTextBox.Clear();
                }
            }

            MessageBox.Show("Текущий индекс " + indexOfElement.ToString());
            MessageBox.Show(currentTopic.currentWords.Count.ToString());
        }

        private void AddButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (currentTopic.Words.Count == 0)
                currentTopic.SequenceOfIndices.Add(0);
            else if (currentTopic.Words.Count == 1)
                currentTopic.SequenceOfIndices.Add(1);
            else currentTopic.SequenceOfIndices
                    .Add((short)currentTopic.SequenceOfIndices.Count);
            string s = " ";

            foreach (int i in currentTopic.SequenceOfIndices)
            {
                s += i + " ";
            }
            MessageBox.Show(s);
            

            WordsDataGrid.Items.Add(new Word
            {
                Id = "temp" + (WordsDataGrid.Items.Count + 1),
                Question = InputQuestionTextBox2.Text.Trim(),
                Hint = InputHintTextBox2.Text.Trim(),
                Answer = InputAnswerTextBox2.Text.Trim(),
                Path = InputPathTextBox2.Text.Trim()
            });

            currentTopic.Words.Add(new Word
            {
                Id = "temp" + (currentTopic.Words.Count + 1),
                TopicId = currentTopic.Id,
                Question = InputQuestionTextBox2.Text.Trim(),
                Hint = InputHintTextBox2.Text.Trim(),
                Answer = InputAnswerTextBox2.Text.Trim(),
                Path = InputPathTextBox2.Text.Trim()
            });

            currentTopic.currentWords.Add(new Word
            {
                Id = "temp" + (currentTopic.currentWords.Count+1),
                TopicId = currentTopic.Id,
                Question = InputQuestionTextBox2.Text.Trim(),
                Hint = InputHintTextBox2.Text.Trim(),
                Answer = InputAnswerTextBox2.Text.Trim(),
                Path = InputPathTextBox2.Text.Trim()
            });

            MessageBox.Show(currentTopic.currentWords.Count.ToString());

            if (WordsDataGrid.Items.Count == 1)
            {
                indexOfElement = 0;
                InputQuestionTextBox.Text = currentTopic.Words[0].Question;
                InputHintTextBox.Text = currentTopic.Words[0].Hint;
                InputAnswerTextBox.Text = currentTopic.Words[0].Answer;
                InputPathTextBox.Text = currentTopic.Words[0].Path;
            }

            InputQuestionTextBox2.Clear();
            InputHintTextBox2.Clear();
            InputAnswerTextBox2.Clear();
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

            if (word != null)
            {
                for (int i = 0; i < currentTopic.Words.Count; i++)
                {
                    if (currentTopic.Words[i].Id == word.Id)
                    {
                        indexOfElement = i;
                        break;
                    }
                }
                MessageBox.Show(indexOfElement.ToString());
                InputQuestionTextBox.Text = word.Question;
                InputHintTextBox.Text = word.Hint;
                InputAnswerTextBox.Text = word.Answer;
                InputPathTextBox.Text = word.Path;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBox.Show(oldTopics.Count + " " + newTopics.Count);
            if (currentTopic != null)
            {
                for (int i = 0; i < newTopics.Count; i++)
                {
                    if (newTopics[i].Name == currentTopic.Name)
                    {
                        newTopics[i] = currentTopic;
                    }
                }
            }

            if (needToUpdate)
                newUser.Update(oldUser, newUser, oldTopics, newTopics, indicesOfDeletedTopics, deletedWords);
        }

        private void UpButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UpButton.Height = UpButton.Width = 20;

            if (currentTopic.Words.Count > 1)
            {
                if (indexOfElement == 0)
                {
                    indexOfElement = currentTopic.Words.Count - 1;
                    InputQuestionTextBox.Text = currentTopic.Words[indexOfElement].Question;
                    InputHintTextBox.Text = currentTopic.Words[indexOfElement].Hint;
                    InputAnswerTextBox.Text = currentTopic.Words[indexOfElement].Answer;
                    InputPathTextBox.Text = currentTopic.Words[indexOfElement].Path;
                }
                else
                {
                    InputQuestionTextBox.Text = currentTopic.Words[--indexOfElement].Question;
                    InputHintTextBox.Text = currentTopic.Words[indexOfElement].Hint;
                    InputAnswerTextBox.Text = currentTopic.Words[indexOfElement].Answer;
                    InputPathTextBox.Text = currentTopic.Words[indexOfElement].Path;
                }
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool hasData = false;
            foreach (Topic topic in newTopics)
            {
                if (topic.Name.ToLower().Contains(SearchTextBox.Text.ToLower()))
                {
                    hasData = true;
                    break;
                }
            }

            if (hasData)
            {
                ExistingTopicsDataGrid2.Items.Clear();
                foreach (Topic topic in newTopics)
                {
                    if (topic.Name.ToLower().Contains(SearchTextBox.Text.ToLower())) { ExistingTopicsDataGrid2.Items.Add(topic); }
                }
            }
            else
            {
                ExistingTopicsDataGrid2.Items.Clear();
                foreach (Topic topic in newTopics)
                {
                    ExistingTopicsDataGrid2.Items.Add(topic);
                }
            }
        }
    }
}
