using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Media;


namespace Shebist
{
    /// <summary>
    /// Логика взаимодействия для MainPage.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MenuGrid.Visibility =
            EnteringAWordGrid.Visibility = Visibility.Hidden;
            SentenceOutputGrid.Children.Clear();
            Main.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Main.Arrange(new Rect(new Point(0, 0), Main.DesiredSize));
            Canvas.SetLeft(EnteringAWordGrid, (Main.ActualWidth / 2) - (EnteringAWordGrid.Width / 2));
        }

        public List<Topic> oldTopics, newTopics;
        public DateTime entryTime;
        static string Debug = Directory.GetCurrentDirectory(),//путь к папке Debug
        Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();//путь к папке Shebist

        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=
        {Shebist}\UserDB.mdf;Integrated Security=True";//Строка подключения к базе данных
        int numberOfSentence;
        //номер слова по порядку
        Random rand = new Random();//генерация рандомных чисел
        public List<int> indicesOfDeletedTopics = new List<int>();
        public List<Sentence> deletedSentences = new List<Sentence>();

        //При загрузке страницы
        public User oldUser, newUser;
        Topic currentTopic;
        public Topic MainTopic;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AccountLabel.Content = newUser.Name;

            if (newUser.ChoiceOfTopicGridVisibility == "Visible")
            {
                ChoiceOfTopicGrid.Visibility = Visibility.Visible;
                EnteringAWordGrid.Visibility = Visibility.Hidden;
            }
            else
            {
                ChoiceOfTopicGrid.Visibility = Visibility.Hidden;
                EnteringAWordGrid.Visibility = Visibility.Visible;
            }

            foreach (Topic topic in newTopics)
            {
                ExistingTopicsDataGrid.Items.Add(topic);
            }
            
            if (EnteringAWordGrid.Visibility == Visibility.Visible)
            {
                if (newUser.CurrentTopicId == "MainTopic")
                {
                    currentTopic = MainTopic;
                    currentTopic.Id = "MainTopic";
                    currentTopic.CurrentIndex = newUser.IndexOfMainTopic;
                    currentTopic.SequenceOfIndices = newUser.SequenceOfIndicesOfMainTopic;
                }
                else
                {
                    foreach (Topic topic in newTopics)
                    {
                        if (topic.Id == newUser.CurrentTopicId)
                        {
                            currentTopic = topic;
                            break;
                        } 
                    }
                }
                
                if (currentTopic != null)
                {
                    currentTopic.currentSentences.Clear();

                    for (int i = 0; i < currentTopic.Sentences.Count; i++)
                    {
                        currentTopic.currentSentences.Add(currentTopic.Sentences[currentTopic.SequenceOfIndices[i]]);
                    }

                    if (currentTopic.CurrentIndex > currentTopic.Sentences.Count - 1)
                        currentTopic.CurrentIndex = currentTopic.Sentences.Count - 1;
                    CreateSentence(currentTopic, currentTopic.CurrentIndex);
                    WordsCounterLabel.Content = "/" + currentTopic.currentSentences.Count;
                    numberOfSentence = currentTopic.CurrentIndex + 1;
                    SearchByNumberTextBox.Text = numberOfSentence.ToString();
                }
                else
                {
                    ClearOutput();
                    ChoiceOfTopicGrid.Visibility = Visibility.Visible;
                    EnteringAWordGrid.Visibility = Visibility.Hidden;
                } 
            }
        }

        void AddNewQuestion(Topic topic, int i, int indexOfSentence, int indexOfQuestions)
        {
            SentenceOutputGrid.ColumnDefinitions.Add(
                           new ColumnDefinition()
                           {
                               Width = GridLength.Auto
                           });

            SentenceOutputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });

            SentenceOutputGrid.Children.Add(new TextBox()
            {
                Text = topic.currentSentences[indexOfSentence].questions[indexOfQuestions],
                VerticalContentAlignment = VerticalAlignment.Center,
                FontSize = 30,
                Height = 45,
                FontFamily = new FontFamily("Times New Roman"),
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Left,
                Background = Brushes.Transparent,
                BorderThickness = new Thickness(0)
            });

            Grid.SetColumn(SentenceOutputGrid.Children[i], i);
            ((TextBox)SentenceOutputGrid.Children[i]).KeyDown += EnteringAWordTextBox_KeyDown;

            if (topic.currentSentences[indexOfSentence].translations[i] != " ")
            {
                ToolTip tt = new ToolTip()
                {
                    Content = topic.currentSentences[indexOfSentence].translations[i],
                    Background = Brushes.Tomato,
                    FontSize = 30,
                    BorderThickness = new Thickness(0)
                };

                ((TextBox)(SentenceOutputGrid.Children[i])).ToolTip = tt;
            }
            
            ((TextBox)SentenceOutputGrid.Children[i]).Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            ((TextBox)SentenceOutputGrid.Children[i]).Arrange(new Rect(new Point(0, 0), ((TextBox)SentenceOutputGrid.Children[i]).DesiredSize));
            ((TextBox)SentenceOutputGrid.Children[i]).Width = ((TextBox)SentenceOutputGrid.Children[i]).ActualWidth;
            ((TextBox)SentenceOutputGrid.Children[i]).Clear();
            CheckSentenceGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            CheckSentenceGrid.Children.Add(new Canvas()
            {
                Width = ((TextBox)SentenceOutputGrid.Children[i]).Width,
                Background = Brushes.LightBlue,
            });
            Grid.SetColumn(CheckSentenceGrid.Children[i], i);
            ((Canvas)CheckSentenceGrid.Children[i]).Children.Add(new TextBlock()
            {
                Width = ((TextBox)SentenceOutputGrid.Children[i]).Width,
                Background = Brushes.Transparent,
                Foreground = Brushes.Green,
                FontSize = 30,
                FontFamily = new FontFamily("Times New Roman"),
                FontWeight = FontWeights.Bold,
                Height = 45
            });
            Canvas.SetLeft(((Canvas)CheckSentenceGrid.Children[i]).Children[0], 2);
            Canvas.SetTop(((Canvas)CheckSentenceGrid.Children[i]).Children[0], 6);
        }

        void AddNewContext(Topic topic, int i, int indexOfSentence, int indexOfContexts)
        {
            SentenceOutputGrid.ColumnDefinitions.Add(
                                new ColumnDefinition()
                                {
                                    Width = GridLength.Auto
                                });

            SentenceOutputGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
            SentenceOutputGrid.Children.Add(new Label()
            {
                Content = topic.currentSentences[indexOfSentence].contexts[indexOfContexts],
                FontFamily = new FontFamily("Times New Roman"),
                FontSize = 30,
                VerticalContentAlignment = VerticalAlignment.Center,
                Background = Brushes.Transparent,
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.Black
            });

            Grid.SetColumn(SentenceOutputGrid.Children[i], i);
           

            if (topic.currentSentences[indexOfSentence].translations[i] != " ")
            {
                ToolTip tt = new ToolTip()
                {
                    Content = topic.currentSentences[indexOfSentence].translations[i],
                    Background = Brushes.Tomato,
                    FontSize = 30,
                    BorderThickness = new Thickness(0)
                };

                ((Label)(SentenceOutputGrid.Children[i])).ToolTip = tt;
            }

           ((Label)SentenceOutputGrid.Children[i]).Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
           ((Label)SentenceOutputGrid.Children[i]).Arrange(new Rect(new Point(0, 0), ((Label)SentenceOutputGrid.Children[i]).DesiredSize));
           ((Label)SentenceOutputGrid.Children[i]).Width = ((Label)SentenceOutputGrid.Children[i]).ActualWidth;
            CheckSentenceGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto});
            CheckSentenceGrid.Children.Add(new Canvas()
            {
                Width = ((Label)SentenceOutputGrid.Children[i]).Width,
                Background = Brushes.Transparent,
            });
            Grid.SetColumn(CheckSentenceGrid.Children[i], i);
        }

        void ClearOutput()
        {
            SentenceOutputGrid.Children.Clear();
            SentenceOutputGrid.ColumnDefinitions.Clear();
            CheckSentenceGrid.Children.Clear();
            CheckSentenceGrid.ColumnDefinitions.Clear();
        }

        BrushConverter conv = new BrushConverter();
        void CreateSentence(Topic topic, int indexOfSentence)
        {
            ClearOutput();
                
            int indexOfQuestions = 0, indexOfContexts = 0;
            int sumParts = topic.currentSentences[indexOfSentence].questions.Count
                    + topic.currentSentences[indexOfSentence].contexts.Count;

            if (sumParts != 0)
            {
                    if (topic.currentSentences[indexOfSentence].isQuestionFirst)
                    {
                        for (int i = 0; i < sumParts; i++)
                        {
                            if(i % 2 == 0) AddNewQuestion(topic, i, indexOfSentence, indexOfQuestions++);
                            else AddNewContext(topic, i, indexOfSentence, indexOfContexts++);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < sumParts; i++)
                        {
                            if (i % 2 == 0) AddNewContext(topic, i, indexOfSentence, indexOfContexts++);
                            else AddNewQuestion(topic, i, indexOfSentence, indexOfQuestions++);
                        }
                    }
            }

            foreach(UIElement element in SentenceOutputGrid.Children)
            {
                if(element.GetType() == typeof(TextBox))
                {
                    element.Focus();
                    break;
                }
            }

            Main.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            Main.Arrange(new Rect(new Point(0, 0), Main.DesiredSize));
            Canvas.SetLeft(SentenceOutputGrid, (Main.ActualWidth / 2) - (SentenceOutputGrid.ActualWidth / 2));
            Canvas.SetLeft(CheckSentenceGrid, Canvas.GetLeft(SentenceOutputGrid));
        }

        bool CheckAnswer(string answer, int indexOfCanvas, int numberOfQuestion)
        {
            ((TextBlock)((Canvas)CheckSentenceGrid.Children[indexOfCanvas]).Children[0]).Text = "";

            List<int> indiciesOfMissedLetters = new List<int>();

            int delta = currentTopic.currentSentences[currentTopic.CurrentIndex].questions[numberOfQuestion].Length - answer.Length;
            if (delta == 0 || delta < 0)
            {
                List<int> indiciesOfRightLetters = new List<int>();

                for (int i = 0; i < answer.Length; i++)
                {
                    if (i < currentTopic.currentSentences[currentTopic.CurrentIndex].questions[numberOfQuestion].Length)
                    {
                        if (answer[i] == currentTopic.currentSentences[currentTopic.CurrentIndex].questions[numberOfQuestion][i])
                            indiciesOfRightLetters.Add(i);
                    }
                }

                for (int i = 0; i < currentTopic.currentSentences[currentTopic.CurrentIndex].questions[numberOfQuestion].Length; i++)
                {
                    if (indiciesOfRightLetters.Contains(i))
                    {
                        ((TextBlock)((Canvas)CheckSentenceGrid.Children[indexOfCanvas]).Children[0]).Inlines.Add(new Run
                        {
                            FontSize = 30,
                            FontWeight = FontWeights.Bold,
                            Foreground = Brushes.Green,
                            Text = currentTopic.currentSentences[currentTopic.CurrentIndex].questions[numberOfQuestion][i].ToString()
                        });
                    }

                    else ((TextBlock)((Canvas)CheckSentenceGrid.Children[indexOfCanvas]).Children[0]).Inlines.Add(new Run
                    {
                        FontSize = 30,
                        FontWeight = FontWeights.Bold,
                        Foreground = Brushes.Red,
                        Text = currentTopic.currentSentences[currentTopic.CurrentIndex].questions[numberOfQuestion][i].ToString()
                    });
                }
            }
            else
            {
                for (int i = 0; i < delta; i++)
                {
                    answer += " ";
                }

                int j = 0;
                for (int i = 0; i < answer.Length; i++)
                {
                    if (i < currentTopic.currentSentences[currentTopic.CurrentIndex].questions[numberOfQuestion].Length)
                    {
                        if (answer[j] != currentTopic.currentSentences[currentTopic.CurrentIndex].questions[numberOfQuestion][i])
                        {
                            indiciesOfMissedLetters.Add(i);
                        }
                        else j++;
                    }
                }

                if (indiciesOfMissedLetters.Count != 0)
                {
                    for (int i = 0; i < currentTopic.currentSentences[currentTopic.CurrentIndex].questions[numberOfQuestion].Length; i++)
                    {
                        if (indiciesOfMissedLetters.Contains(i))
                        {
                            ((TextBlock)((Canvas)CheckSentenceGrid.Children[indexOfCanvas]).Children[0]).Inlines.Add(new Run
                            {
                                FontSize = 30,
                                FontWeight = FontWeights.Bold,
                                Foreground = Brushes.Red,
                                Text = currentTopic.currentSentences[currentTopic.CurrentIndex].questions[numberOfQuestion][i].ToString()
                            });
                        }
                        else
                        {
                            ((TextBlock)((Canvas)CheckSentenceGrid.Children[indexOfCanvas]).Children[0]).Inlines.Add(new Run
                            {
                                FontSize = 30,
                                FontWeight = FontWeights.Bold,
                                Foreground = Brushes.Green,
                                Text = currentTopic.currentSentences[currentTopic.CurrentIndex].questions[numberOfQuestion][i].ToString()
                            });
                        }
                    }
                }
            }
            bool isAnswerWrong = false;
            foreach(Run run in ((TextBlock)((Canvas)CheckSentenceGrid.Children[indexOfCanvas]).Children[0]).Inlines)
            {
                if(run.Foreground == Brushes.Red)
                {
                    isAnswerWrong = true;
                    break;
                }
            }

            return isAnswerWrong;
        }
        //включена ли озвучка
        bool isSoundEnabled = true;

        private async void EnteringAWordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                bool areAllTheAnswersCorrect = true;
                int numberOfQuestion = 0;
                for (int i = 0; i < SentenceOutputGrid.Children.Count; i++)
                {
                    if (SentenceOutputGrid.Children[i].GetType() == typeof(TextBox))
                    {
                        if (((TextBox)SentenceOutputGrid.Children[i]).Text.ToLower() !=
                            currentTopic.
                            currentSentences[currentTopic.CurrentIndex].
                            questions[numberOfQuestion].ToLower())
                        {
                            areAllTheAnswersCorrect = false;
                            break;
                        }
                        else numberOfQuestion++;
                    }
                }

                if (areAllTheAnswersCorrect)
                {
                    if (currentTopic.Id == "MainTopic")
                    {
                        player.Open(new Uri(Debug + "\\MainTopicSounds" + currentTopic.currentSentences[currentTopic.CurrentIndex].wayToSentenceVoice));
                        player.Play();
                    }
                    else
                    {
                        player.Open(new Uri(currentTopic.currentSentences[currentTopic.CurrentIndex].wayToSentenceVoice));
                        player.Play();
                    }

                    if(currentTopic.CurrentIndex < currentTopic.currentSentences.Count - 1)
                    {
                        ++numberOfSentence;
                        SearchByNumberTextBox.Text = numberOfSentence.ToString();
                        SentenceOutputGrid.Children.Clear();
                        CheckSentenceGrid.Children.Clear();
                        CreateSentence(currentTopic, ++currentTopic.CurrentIndex);
                    }
                    else
                    {
                        ClearOutput();
                        SentenceOutputGrid.Children.Add(new Label()
                        {
                            Content = "Выполнено",
                            VerticalContentAlignment = VerticalAlignment.Center,
                            FontSize = 30,
                            Height = 45,
                            FontFamily = new FontFamily("Times New Roman"),
                            FontWeight = FontWeights.Bold,
                            Background = Brushes.Transparent
                        });

                        Main.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                        Main.Arrange(new Rect(new Point(0, 0), Main.DesiredSize));
                        Canvas.SetLeft(SentenceOutputGrid, (Main.ActualWidth / 2) - (SentenceOutputGrid.ActualWidth / 2));
                    }
                }
                else
                {
                    numberOfQuestion = 0;
                    for (int i = 0; i < SentenceOutputGrid.Children.Count; i++)
                    {
                        if (SentenceOutputGrid.Children[i].GetType() == typeof(TextBox))
                        {
                            if (CheckAnswer(((TextBox)SentenceOutputGrid.Children[i]).Text,
                                i, numberOfQuestion))
                            {
                                ((TextBox)SentenceOutputGrid.Children[i]).Clear();
                                
                                if (currentTopic.Id == "MainTopic")
                                {
                                    player.Open(new Uri((Debug + "\\MainTopicSounds" + currentTopic.currentSentences[currentTopic.CurrentIndex].waysToQuestionsVoice[numberOfQuestion++])));
                                    player.Play();
                                }
                                else
                                {
                                    player.Open(new Uri(currentTopic.currentSentences[currentTopic.CurrentIndex].waysToQuestionsVoice[numberOfQuestion++]));
                                    player.Play();
                                }

                            }
                            else numberOfQuestion++;
                        }
                    }
                }
                
            }
        }
        
        MediaPlayer player = new MediaPlayer();

        private void NextButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            player.Stop();
            NextButton.Width = NextButton.Height = 30;
            SentenceOutputGrid.Children.Clear();
            SentenceOutputGrid.Children.Clear();

            if (numberOfSentence < currentTopic.currentSentences.Count)
            {
                numberOfSentence++;
                CreateSentence(currentTopic, ++currentTopic.CurrentIndex);
                SearchByNumberTextBox.Text = numberOfSentence.ToString();
            }
            else if (numberOfSentence == currentTopic.currentSentences.Count)
            {
                currentTopic.CurrentIndex = 0;
                numberOfSentence = 1;
                CreateSentence(currentTopic, 0);
                SearchByNumberTextBox.Text = numberOfSentence.ToString();
            }
        }

        //При нажатии на AgainButton
        private void AgainButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {//Высота и ширина AgainButton равна 30
            AgainButton.Width = AgainButton.Height = 30;    
            currentTopic.CurrentIndex = 0;
            numberOfSentence = 1;
            SentenceOutputGrid.Children.Clear();
            SentenceOutputGrid.Children.Clear();
            SearchByNumberTextBox.Text = "1";
            CreateSentence(currentTopic, 0);
        }

        private void ToTheChoiceOfTopicButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChoiceOfTopicGrid.Visibility = Visibility.Visible;
            EnteringAWordGrid.Visibility =
            SentenceOutputGrid.Visibility =
            CheckSentenceGrid.Visibility = Visibility.Hidden;


            if (currentTopic.Id != "MainTopic")
            {
                foreach (Topic topic in newTopics)
                {
                    if (topic.Name == currentTopic.Name)
                    {
                        topic.CurrentIndex = currentTopic.CurrentIndex;
                        topic.SequenceOfIndices = currentTopic.SequenceOfIndices;
                    }
                }
            }
            else
            {
                newUser.SequenceOfIndicesOfMainTopic = currentTopic.SequenceOfIndices;
                newUser.IndexOfMainTopic = currentTopic.CurrentIndex;
            }
            newUser.ChoiceOfTopicGridVisibility = "Visible";
        }   

        //При нажатии на MixButton
        private void MixButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Высота и ширина MixButton = 30
            MixButton.Width = MixButton.Height = 30;
            SentenceOutputGrid.Children.Clear();
            CheckSentenceGrid.Children.Clear();

            //создаём список чисел indicies с длиной currentTopic.currentWords.Count
            List<int> indices = new List<int>(currentTopic.Sentences.Count);
            //Заполняем его от 0 до currentTopic.currentWords.Count
            for (int i = 0; i < currentTopic.Sentences.Count; i++)
                indices.Add(i);
            
            int randomIndex, element;
            currentTopic.SequenceOfIndices.Clear();

            for (int i = 0; i < currentTopic.Sentences.Count; i++)
            {
                element = rand.Next(0, indices.Count - 1);
                randomIndex = indices.ElementAt(element);
                currentTopic.currentSentences[i] = currentTopic.Sentences[randomIndex];
                indices.RemoveAt(element);
                currentTopic.SequenceOfIndices.Add(randomIndex);
            }
            
            CreateSentence(currentTopic, currentTopic.CurrentIndex);
        }
 
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Image)sender).Width = ((Image)sender).Height = 35;
            Canvas.SetZIndex(((Image)sender), 2);
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Label)sender).FontSize = 18;
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Label)sender).FontSize = 16;
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Image)sender).Width = ((Image)sender).Height = 30;
            Canvas.SetZIndex(((Image)sender), 0);
        }

        private void Button_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((Image)sender).Width = ((Image)sender).Height = 35;
        }

        private void BackButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BackButton.Width = BackButton.Height = 30;
            if (numberOfSentence == 1)
            {
                currentTopic.CurrentIndex = currentTopic.currentSentences.Count - 1;
                numberOfSentence = currentTopic.currentSentences.Count;
                CreateSentence(currentTopic, currentTopic.CurrentIndex);
                SearchByNumberTextBox.Text = numberOfSentence.ToString();
            }
            else
            {
                numberOfSentence--;
                CreateSentence(currentTopic, --currentTopic.CurrentIndex);
                SearchByNumberTextBox.Text = numberOfSentence.ToString();
            }
        }

        private void MainWordsButton_Click(object sender, RoutedEventArgs e)
        {
            if(MainTopic.Sentences.Count != 0)
            {
                currentTopic = MainTopic;
                ChoiceOfTopicGrid.Visibility = Visibility.Hidden;
                EnteringAWordGrid.Visibility = Visibility.Visible;
                newUser.ChoiceOfTopicGridVisibility = "Hidden";
                currentTopic.CurrentIndex = newUser.IndexOfMainTopic;
                newUser.CurrentTopicId = "MainTopic";
                numberOfSentence = currentTopic.CurrentIndex + 1;
                WordsCounterLabel.Content = "/" + currentTopic.currentSentences.Count;
                SearchByNumberTextBox.Text = numberOfSentence.ToString();
                CreateSentence(currentTopic, currentTopic.CurrentIndex);
                SentenceOutputGrid.Visibility =
                CheckSentenceGrid.Visibility = Visibility.Visible;
            }
        }
        
        private void ExistingTopicsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Topic topic = (Topic)ExistingTopicsDataGrid.SelectedItem;
            ExistingTopicsDataGrid.SelectedItem = null;

            if (topic != null && topic.Sentences.Count != 0)
            {
                currentTopic = topic;

                currentTopic.currentSentences.Clear();

                for (int i = 0; i < currentTopic.Sentences.Count; i++)
                {
                    currentTopic.currentSentences.Add(currentTopic.Sentences[currentTopic.SequenceOfIndices[i]]);
                }

                if (currentTopic.CurrentIndex > currentTopic.currentSentences.Count - 1)
                    currentTopic.CurrentIndex = currentTopic.Sentences.Count - 1;
                newUser.CurrentTopicId = currentTopic.Id;
                numberOfSentence = currentTopic.CurrentIndex + 1;
                SearchByNumberTextBox.Text = numberOfSentence.ToString();
                CreateSentence(currentTopic, currentTopic.CurrentIndex);
                WordsCounterLabel.Content = "/" + currentTopic.currentSentences.Count;
                ChoiceOfTopicGrid.Visibility = Visibility.Hidden;
                EnteringAWordGrid.Visibility =
                SentenceOutputGrid.Visibility =
                CheckSentenceGrid.Visibility = Visibility.Visible;
                newUser.ChoiceOfTopicGridVisibility = "Hidden";
            }
        }

        private void TopicSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool hasData = false;

            foreach (Topic topic in newTopics)
                if (topic.Name.ToLower().Contains(TopicSearchTextBox.Text.ToLower()))
                {
                    hasData = true;
                    break;
                }
            
            if (hasData)
            {
                ExistingTopicsDataGrid.Items.Clear();
                foreach (Topic topic in newTopics)
                    if (topic.Name.ToLower().Contains(TopicSearchTextBox.Text.ToLower()))
                        ExistingTopicsDataGrid.Items.Add(topic);
            }
            else
            {
                ExistingTopicsDataGrid.Items.Clear();
                foreach (Topic topic in newTopics)
                    ExistingTopicsDataGrid.Items.Add(topic);
            }
        }
        
        public bool needToUpdate;
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (EnteringAWordGrid.Visibility == Visibility.Visible)
            {
                if (currentTopic.Id != "MainTopic")
                {
                    for (int i = 0; i < newTopics.Count; i++)
                    {
                        if (newTopics[i].Id == currentTopic.Id)
                        {
                            newTopics[i] = currentTopic;
                        }
                    }
                }
                else
                {
                    newUser.CurrentTopicId = "MainTopic";
                    newUser.IndexOfMainTopic = currentTopic.CurrentIndex;
                    newUser.SequenceOfIndicesOfMainTopic = currentTopic.SequenceOfIndices;
                }
            }

            if (needToUpdate)
            {
                newUser.Update(oldUser, newUser, oldTopics, newTopics, indicesOfDeletedTopics, deletedSentences, entryTime);
            }
        }

        private void SortByDefaultButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SortByDefaultButton.Width = SortByDefaultButton.Height = 30;

            if(newUser.CurrentTopicId == "MainTopic")
            {
                currentTopic.currentSentences.Clear();
                newUser.SequenceOfIndicesOfMainTopic.Clear();
                for(int i = 0; i < MainTopic.Sentences.Count; i++)
                {
                    currentTopic.currentSentences.Add(MainTopic.Sentences[i]);
                    newUser.SequenceOfIndicesOfMainTopic.Add(i);
                }
                CreateSentence(currentTopic, currentTopic.CurrentIndex);
            }
            else
            {
                currentTopic.currentSentences.Clear();
                currentTopic.SequenceOfIndices.Clear();
                for (int i = 0; i < currentTopic.Sentences.Count; i++)
                {
                    currentTopic.currentSentences.Add(currentTopic.Sentences[i]);
                    currentTopic.SequenceOfIndices.Add(i);
                }
                CreateSentence(currentTopic, currentTopic.CurrentIndex);
            }
        }
        
        
        private void AccountLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            needToUpdate = false;

            AccountWindow aw = new AccountWindow
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

            aw.Show();
            this.Close();
        }

        private void SettingsLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow(Main);
            sw.Owner = this;
            sw.Show();
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
            MenuGrid.Visibility = Visibility.Hidden;
            Canvas.SetLeft(OpenMenu, 0);
        }

        private void ExitLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            needToUpdate = true;
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

        private void OpenMenu_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Main.MouseLeftButtonDown -= Main_MouseLeftButtonDown;
        }

        private void MenuGrid_MouseEnter(object sender, MouseEventArgs e)
        {
            Main.MouseLeftButtonDown -= Main_MouseLeftButtonDown;
        }

        private void MenuGrid_MouseLeave(object sender, MouseEventArgs e)
        {
            Main.MouseLeftButtonDown += Main_MouseLeftButtonDown;
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

        //переход к предложению по номеру по номеру
        private void SearchByNumberTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    numberOfSentence = int.Parse(SearchByNumberTextBox.Text);
                    if (numberOfSentence >= 1 && numberOfSentence <= currentTopic.currentSentences.Count)
                    {
                        currentTopic.CurrentIndex = numberOfSentence - 1;
                        CreateSentence(currentTopic, currentTopic.CurrentIndex);
                    }
                    else SearchByNumberTextBox.Clear();
                }
                catch (FormatException)
                {
                    SearchByNumberTextBox.Clear();
                } 
            }
        }
    }
}

