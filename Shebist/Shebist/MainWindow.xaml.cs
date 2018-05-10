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
using System.Data.SqlClient;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Win32;
using System.Data;


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
            Menu.Opacity = 0;
            EnteringAWordGrid.Visibility = Visibility.Hidden;
        }

        public List<Topic> topics = new List<Topic>();
        static string Debug = Directory.GetCurrentDirectory(),//путь к папке Debug
        Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();//путь к папке Shebist

        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=
        {Shebist}\UserDB.mdf;Integrated Security=True";//Строка подключения к базе данных
        int numberofword;
        //номер слова по порядку
        SqlCommand command = new SqlCommand();//Создание запросов к бд
        SqlDataReader reader;//Чтение данных из бд
        Random rand = new Random();//генерация рандомных чисел
        List<Word> currentWords = new List<Word>();//список слов

        //При загрузке страницы
        public User user;
        Topic currentTopic;
        public Topic MainWords;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if(user != null)
            {
                AccountMenuItem.Header = user.Name;
                AccountMenuItem.Width = 20;
                for (int i = 0; i < AccountMenuItem.Header.ToString().Length; i++)
                {
                    AccountMenuItem.Width += 9;
                }
                if (user.ChoiceOfTopicGridVisibility == "Visible")
                {
                    ChoiceOfTopicGrid.Visibility = Visibility.Visible;
                    EnteringAWordGrid.Visibility = Visibility.Hidden;
                }
                else
                {
                    ChoiceOfTopicGrid.Visibility = Visibility.Hidden;
                    EnteringAWordGrid.Visibility = Visibility.Visible;
                }
                foreach (Topic topic in topics)
                {
                    ExistingTopicsDataGrid.Items.Add(topic);
                }

                foreach (Topic topic in topics)
                {
                    if (topic.Id == user.CurrentTopicId) currentTopic = topic;
                }
                if (user.CurrentTopicId == 0) currentTopic = MainWords;

                if (EnteringAWordGrid.Visibility == Visibility.Visible)
                {
                    WordOutputLabel.Content = currentTopic.currentWords[currentTopic.CurrentIndex].Question;
                    WordsCounterLabel.Content = "/" + currentTopic.currentWords.Count;
                    numberofword = currentTopic.CurrentIndex + 1;
                    SearchByNumberTextBox.Text = numberofword.ToString();
                    DescriptionLabel.Content = currentTopic.currentWords[currentTopic.CurrentIndex].Hint;
                }
            }
        }

        //включена ли озвучка
        bool isSoundEnabled = true;

        private async void EnteringAWordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (EnteringAWordTextBox.Text.ToLower() == currentTopic.currentWords[currentTopic.CurrentIndex].Answer.ToLower())
                {
                    if (isSoundEnabled)
                    {
                        if (File.Exists(currentTopic.currentWords[currentTopic.CurrentIndex].Path))
                        {
                            player.Open(new Uri(currentTopic.currentWords[currentTopic.CurrentIndex].Path, UriKind.Absolute));
                            player.Play();
                        }
                        else
                        {
                            player.Open(new Uri(Debug + "\\MainWordsSounds" + currentTopic.currentWords[currentTopic.CurrentIndex].Path, UriKind.Absolute));
                            player.Play();
                        }
                    }

                    CorrectAnswerTextBlock.Text = "";
                    EnteringAWordTextBox.Foreground = Brushes.Green;
                    EnteringAWordTextBox.Text = currentTopic.currentWords[currentTopic.CurrentIndex].Answer;

                    while (player.HasAudio)
                    {
                        await Task.Delay(1);
                    }
                    await Task.Delay(1000);

                    EnteringAWordTextBox.Clear();
                    EnteringAWordTextBox.Foreground = Brushes.Black;

                    if (numberofword < currentTopic.currentWords.Count)
                    {
                        numberofword++;
                        WordOutputLabel.Content = currentTopic.currentWords[++currentTopic.CurrentIndex].Question;
                        DescriptionLabel.Content = currentTopic.currentWords[currentTopic.CurrentIndex].Hint;
                        SearchByNumberTextBox.Text = numberofword.ToString();
                    }
                    else
                    {
                        WordOutputLabel.Content = "Выполнено";
                        DescriptionLabel.Content = "";
                    }
                }
                else
                {
                    CorrectAnswerTextBlock.Text = "";
                    List<int> indiciesOfMissedLetters = new List<int>();

                    int delta = currentTopic.currentWords[currentTopic.CurrentIndex].Answer.Length - EnteringAWordTextBox.Text.Length;
                    if (delta == 0 || delta < 0)
                    {
                        List<int> indiciesOfRightLetters = new List<int>();

                        for (int i = 0; i < EnteringAWordTextBox.Text.Length; i++)
                        {
                            if (i < currentTopic.currentWords[currentTopic.CurrentIndex].Answer.Length)
                            {
                                if (EnteringAWordTextBox.Text[i] == currentTopic.currentWords[currentTopic.CurrentIndex].Answer[i])
                                    indiciesOfRightLetters.Add(i);
                            }
                        }

                        for (int i = 0; i < currentTopic.currentWords[currentTopic.CurrentIndex].Answer.Length; i++)
                        {
                            if (indiciesOfRightLetters.Contains(i))
                            {
                                CorrectAnswerTextBlock.Inlines.Add(new Run
                                {
                                    FontSize = 17,
                                    Foreground = Brushes.Green,
                                    Text = currentTopic.currentWords[currentTopic.CurrentIndex].Answer[i].ToString()
                                });
                            }

                            else CorrectAnswerTextBlock.Inlines.Add(new Run
                            {
                                FontSize = 17,
                                Foreground = Brushes.Red,
                                Text = currentTopic.currentWords[currentTopic.CurrentIndex].Answer[i].ToString()
                            });
                        }
                    }
                    else
                    {
                        for (int i = 0; i < delta; i++)
                        {
                            EnteringAWordTextBox.Text += " ";
                        }

                        int j = 0;
                        for (int i = 0; i < EnteringAWordTextBox.Text.Length; i++)
                        {
                            if (i < currentTopic.currentWords[currentTopic.CurrentIndex].Answer.Length)
                            {
                                if (EnteringAWordTextBox.Text[j] != currentTopic.currentWords[currentTopic.CurrentIndex].Answer[i])
                                {
                                    indiciesOfMissedLetters.Add(i);
                                }
                                else j++;

                            }
                        }

                        if (indiciesOfMissedLetters.Count != 0)
                        {
                            for (int i = 0; i < currentTopic.currentWords[currentTopic.CurrentIndex].Answer.Length; i++)
                            {
                                if (indiciesOfMissedLetters.Contains(i))
                                {
                                    CorrectAnswerTextBlock.Inlines.Add(new Run
                                    {
                                        FontSize = 17,
                                        Foreground = Brushes.Red,
                                        Text = currentTopic.currentWords[currentTopic.CurrentIndex].Answer[i].ToString()
                                    });
                                }
                                else
                                {
                                    CorrectAnswerTextBlock.Inlines.Add(new Run
                                    {
                                        FontSize = 17,
                                        Foreground = Brushes.Green,
                                        Text = currentTopic.currentWords[currentTopic.CurrentIndex].Answer[i].ToString()
                                    });
                                }
                            }
                        }
                    }

                    if (isSoundEnabled)
                    {
                        if (File.Exists(currentTopic.currentWords[currentTopic.CurrentIndex].Path))
                        {
                            player.Open(new Uri(currentTopic.currentWords[currentTopic.CurrentIndex].Path, UriKind.Absolute));
                            player.Play();
                        }
                        else
                        {
                            player.Open(new Uri(Debug + "\\MainWordsSounds" + currentTopic.currentWords[currentTopic.CurrentIndex].Path, UriKind.Absolute));
                            player.Play();
                        }
                    }
                    EnteringAWordTextBox.Clear();
                }
            }
        }

        MediaPlayer player = new MediaPlayer();

        private void NextButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NextButton.Width = NextButton.Height = 30;
            CorrectAnswerTextBlock.Text = "";
            if (numberofword < currentTopic.currentWords.Count)
            {
                numberofword++;
                WordOutputLabel.Content = currentTopic.currentWords[++currentTopic.CurrentIndex].Question;
                DescriptionLabel.Content = currentTopic.currentWords[currentTopic.CurrentIndex].Hint;
                SearchByNumberTextBox.Text = numberofword.ToString();
            }
            else if (numberofword == currentTopic.currentWords.Count)
            {
                currentTopic.CurrentIndex = 0;
                numberofword = 1;
                WordOutputLabel.Content = currentTopic.currentWords[0].Question;
                DescriptionLabel.Content = currentTopic.currentWords[0].Hint;
                SearchByNumberTextBox.Text = numberofword.ToString();
            }
        }

        private void NextButton_MouseEnter(object sender, MouseEventArgs e)
        {
            NextButton.Width = NextButton.Height = 35;
        }

        private void NextButton_MouseLeave(object sender, MouseEventArgs e)
        {
            NextButton.Width = NextButton.Height = 30;
        }

        private void BackButton_MouseEnter(object sender, MouseEventArgs e)
        {
            BackButton.Width = BackButton.Height = 35;
        }

        private void BackButton_MouseLeave(object sender, MouseEventArgs e)
        {
            BackButton.Width = BackButton.Height = 30;
        }

        //При нажатии на AgainButton
        private void AgainButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {//Высота и ширина AgainButton равна 30
            AgainButton.Width = AgainButton.Height = 30;
            CorrectAnswerTextBlock.Text = "";
            //Обнуляем индекс, а номер слова ставим 1,
            //В WordOutputLabel.Content выводим первое значение из массива русских слов,
            //DescriptionLabel.Content первое значение из массива описаний,
            //Answer первое значение из английских слов, path первое значение из массива путей
            currentTopic.CurrentIndex = 0;
            numberofword = 1;
            WordOutputLabel.Content = currentTopic.currentWords[0].Question;
            DescriptionLabel.Content = currentTopic.currentWords[0].Hint;
            SearchByNumberTextBox.Text = "1";
        }

        private void ToTheChoiceOfTopicButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChoiceOfTopicGrid.Visibility = Visibility.Visible;
            EnteringAWordGrid.Visibility = Visibility.Hidden;
            CorrectAnswerTextBlock.Text = "";
            EnteringAWordTextBox.Clear();
            if (currentTopic.Id != 0)
            {
                foreach (Topic topic in topics)
                {
                    if (topic.Name == currentTopic.Name)
                    {
                        topic.CurrentIndex = currentTopic.CurrentIndex;
                        topic.SequenceOfIndices = currentTopic.SequenceOfIndices;
                    }
                }
            }
            else user.IndexOfMainWords = currentTopic.CurrentIndex;
        }

        //При нажатии на MixButton
        private void MixButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Высота и ширина MixButton = 30
            MixButton.Width = MixButton.Height = 30;
            CorrectAnswerTextBlock.Text = "";

            //создаём список чисел indicies с длиной currentTopic.currentWords.Count
            List<int> indices = new List<int>(currentTopic.currentWords.Count);
            //Заполняем его от 0 до currentTopic.currentWords.Count
            for (int i = 0; i < currentTopic.currentWords.Count; i++)
            {
                indices.Add(i);
            }

            int randomIndex, element;
            currentTopic.SequenceOfIndices.Clear();

            for (int i = 0; i < currentTopic.currentWords.Count; i++)
            {
                element = rand.Next(0, indices.Count - 1);
                randomIndex = indices.ElementAt(element);
                currentTopic.currentWords[randomIndex] = new Word
                {
                    Question = currentTopic.Words[i].Question,
                    Hint = currentTopic.Words[i].Hint,
                    Answer = currentTopic.Words[i].Answer,
                    Path = currentTopic.Words[i].Path,
                };
                indices.RemoveAt(element);
                currentTopic.SequenceOfIndices.Add(randomIndex);
            }
            string s = " ";
            foreach (int i in currentTopic.SequenceOfIndices)
            {
                s += i + " ";
            }
            MessageBox.Show(s);
            WordOutputLabel.Content = currentTopic.currentWords[currentTopic.CurrentIndex].Question;
            DescriptionLabel.Content = currentTopic.currentWords[currentTopic.CurrentIndex].Hint;
        }

        private void NextButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            NextButton.Width = NextButton.Height = 35;
        }

        private void BackButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            BackButton.Width = BackButton.Height = 35;
        }

        private void AgainButton_MouseEnter(object sender, MouseEventArgs e)
        {
            AgainButton.Width = AgainButton.Height = 35;
        }

        private void MixButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            MixButton.Width = MixButton.Height = 35;
        }

        private void MixButton_MouseEnter(object sender, MouseEventArgs e)
        {
            MixButton.Width = MixButton.Height = 35;
        }

        private void MixButton_MouseLeave(object sender, MouseEventArgs e)
        {
            MixButton.Width = MixButton.Height = 30;
        }

        private void ToTheChoiceOfTopicButton_MouseEnter(object sender, MouseEventArgs e)
        {
            ToTheChoiceOfTopicButton.Width = ToTheChoiceOfTopicButton.Height = 35;
        }

        private void ToTheChoiceOfTopicButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ToTheChoiceOfTopicButton.Width = ToTheChoiceOfTopicButton.Height = 30;
        }

        private void AgainButton_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            AgainButton.Width = AgainButton.Height = 35;
        }

        private void AgainButton_MouseLeave(object sender, MouseEventArgs e)
        {
            AgainButton.Width = AgainButton.Height = 30;
        }

        private void BackButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            BackButton.Width = BackButton.Height = 30;
            CorrectAnswerTextBlock.Text = "";
            if (numberofword == 1)
            {
                currentTopic.CurrentIndex = currentTopic.currentWords.Count - 1;
                numberofword = currentTopic.currentWords.Count;
                WordOutputLabel.Content = currentTopic.currentWords[currentTopic.CurrentIndex].Question;
                DescriptionLabel.Content = currentTopic.currentWords[currentTopic.CurrentIndex].Hint;
                SearchByNumberTextBox.Text = numberofword.ToString();
            }
            else if (WordOutputLabel.Content.ToString() == "Выполнено")
            {
                WordOutputLabel.Content = currentTopic.currentWords[currentTopic.CurrentIndex].Question;
                DescriptionLabel.Content = currentTopic.currentWords[currentTopic.CurrentIndex].Hint;
                DescriptionLabel.Visibility = Visibility.Visible;
            }
            else
            {
                numberofword--;
                WordOutputLabel.Content = currentTopic.currentWords[--currentTopic.CurrentIndex].Question;
                DescriptionLabel.Content = currentTopic.currentWords[currentTopic.CurrentIndex].Hint;
                SearchByNumberTextBox.Text = numberofword.ToString();
            }
        }

        private void MainWordsButton_Click(object sender, RoutedEventArgs e)
        {
            currentTopic = MainWords;
            ChoiceOfTopicGrid.Visibility = Visibility.Hidden;
            EnteringAWordGrid.Visibility = Visibility.Visible;
            user.ChoiceOfTopicGridVisibility = "Hidden";
            currentTopic.CurrentIndex = user.IndexOfMainWords;
            user.CurrentTopicId = 0;
            numberofword = currentTopic.CurrentIndex + 1;
            WordsCounterLabel.Content = "/" + currentTopic.Words.Count;
            SearchByNumberTextBox.Text = numberofword.ToString();
            WordOutputLabel.Content = currentTopic.currentWords[currentTopic.CurrentIndex].Question;
            DescriptionLabel.Content = currentTopic.currentWords[currentTopic.CurrentIndex].Hint;
        }

        private void TopicEditorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TopicEditorWindow tew = new TopicEditorWindow
            {
                WindowState = this.WindowState,
                Top = this.Top,
                Left = this.Left,
                Width = this.Width,
                Height = this.Height,
                user = user,
                topics = topics,
                MainWords = MainWords
            };
            tew.Show();
            this.Close();
        }

        private void SettingsMenuItem_Click(object sender, EventArgs e)
        {
            SettingsWindow sw = new SettingsWindow(MainGrid);
            sw.Owner = this;
            sw.Show();
        }

        private void ExistingTopicsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Topic topic = (Topic)ExistingTopicsDataGrid.SelectedItem;
            ExistingTopicsDataGrid.SelectedItem = null;

            if (topic != null && topic.Words.Count != 0)
            {
                currentTopic = topic;
                MessageBox.Show(currentTopic.currentWords.Count.ToString());
                if (currentTopic.CurrentIndex > currentTopic.Words.Count - 1)
                    currentTopic.CurrentIndex = currentTopic.Words.Count - 1;
                user.CurrentTopicId = currentTopic.Id;
                numberofword = currentTopic.CurrentIndex + 1;
                SearchByNumberTextBox.Text = numberofword.ToString();
                WordOutputLabel.Content = currentTopic.currentWords[currentTopic.CurrentIndex].Question;
                DescriptionLabel.Content = currentTopic.currentWords[currentTopic.CurrentIndex].Hint;
                WordsCounterLabel.Content = "/" + currentTopic.currentWords.Count;
                ChoiceOfTopicGrid.Visibility = Visibility.Hidden;
                user.ChoiceOfTopicGridVisibility = "Hidden";
                EnteringAWordGrid.Visibility = Visibility.Visible;
            }
        }

        private void TopicSearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool hasData = false;
            foreach (Topic topic in topics)
            {
                if (topic.Name.ToLower().Contains(TopicSearchTextBox.Text.ToLower()))
                {
                    hasData = true;
                    break;
                }
            }

            if (hasData)
            {
                ExistingTopicsDataGrid.Items.Clear();
                foreach (Topic topic in topics)
                {
                    if (topic.Name.ToLower().Contains(TopicSearchTextBox.Text.ToLower())) { ExistingTopicsDataGrid.Items.Add(topic); }
                }
            }
            else
            {
                ExistingTopicsDataGrid.Items.Clear();
                foreach (Topic topic in topics)
                {
                    ExistingTopicsDataGrid.Items.Add(topic);
                }
            }
        }

        private void Menu_MouseEnter(object sender, MouseEventArgs e)
        {
            Menu.Opacity = 1;
        }

        private void Menu_MouseLeave(object sender, MouseEventArgs e)
        {
            Menu.Opacity = 0;
        }

        private void AccountMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AccountWindow aw = new AccountWindow
            {
                WindowState = this.WindowState,
                Top = this.Top,
                Left = this.Left,
                Width = this.Width,
                Height = this.Height,
                user = user,
                topics = topics,
                MainWords = MainWords };
            aw.Show();
            this.Close();
        }

        //переход к слову по номеру
        private void SearchByNumberTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    numberofword = Int32.Parse(SearchByNumberTextBox.Text);
                    if (numberofword >= 1 && numberofword <= currentTopic.currentWords.Count)
                    {
                        currentTopic.CurrentIndex = numberofword - 1;
                        WordOutputLabel.Content = currentTopic.currentWords[currentTopic.CurrentIndex].Question;
                        DescriptionLabel.Content = currentTopic.currentWords[currentTopic.CurrentIndex].Hint;
                    }
                    else
                    {
                        SearchByNumberTextBox.Clear();
                    }
                }
                catch (FormatException)
                {
                    SearchByNumberTextBox.Clear();
                }
            }
        }
    }
}

