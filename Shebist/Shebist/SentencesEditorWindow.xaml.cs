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
    /// Логика взаимодействия для SentencesEditorWindow.xaml
    /// </summary>
    public partial class SentencesEditorWindow : Window
    {
        public SentencesEditorWindow()
        {
            InitializeComponent();
            MenuItem menuItem = new MenuItem() { Header = "Удалить" };
            menuItem.Click += MenuItem_Click;
            contextMenu = new ContextMenu()
            {
                Background = Brushes.White,
                FontWeight = FontWeights.Bold,
                Items = { menuItem }
            };
        }
        ContextMenu contextMenu;
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        static string Debug = Directory.GetCurrentDirectory();
        static string Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();
        static string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Shebist}\UserDB.mdf;Integrated Security=True";

        void UpdateSentence()
        {
            if(WordInputGrid.Children.Count > 0)
            {
                List<string> _questions = new List<string>(),
                _contexts = new List<string>(),
                _translations = new List<string>(),
                _waysToQuestionsVoice = new List<string>();

                for (int i = 0; i < WordInputGrid.Children.Count; i++)
                {
                    if (((TextBox)WordInputGrid.Children[i]).Foreground == Brushes.Green)
                    {
                        if (((TextBox)WordInputGrid.Children[i]).Text != "")
                            _questions.Add(((TextBox)WordInputGrid.Children[i]).Text);
                        else _questions.Add(" ");
                    }

                    else if (((TextBox)WordInputGrid.Children[i]).Foreground == Brushes.Black)
                    {
                        if (((TextBox)WordInputGrid.Children[i]).Text != "")
                            _contexts.Add(((TextBox)WordInputGrid.Children[i]).Text);
                        else _contexts.Add(" ");
                    }

                    if (((TextBox)TranslateInputGrid.Children[i]).Text != "")
                        _translations.Add(((TextBox)TranslateInputGrid.Children[i]).Text);
                    else _translations.Add(" ");
                }

                for (int i = 0; i < QuestionVoiceInputGrid.Children.Count; i++)
                {
                    if (((TextBox)QuestionVoiceInputGrid.Children[i]).IsReadOnly == false)
                    {
                        if (((TextBox)QuestionVoiceInputGrid.Children[i]).Text != "")
                            _waysToQuestionsVoice.Add(((TextBox)QuestionVoiceInputGrid.Children[i]).Text);
                        else _waysToQuestionsVoice.Add(" ");
                    }
                }

                bool _isQuestionFirst = false;
                if (((TextBox)WordInputGrid.Children[0]).Foreground == Brushes.Green) _isQuestionFirst = true;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = $"UPDATE MainTopic SET Questions = N'{WriteParts(_questions)}', " +
                        $"Contexts = N'{WriteParts(_contexts)}', Translations = N'{WriteParts(_translations)}', " +
                        $"WaysToQuestionsVoice = N'{WriteParts(_waysToQuestionsVoice)}', " +
                        $"WayToSentenceVoice = N'{((TextBox)SentenceVoiceInputGrid.Children[0]).Text}', " +
                        $"IsQuestionFirst = N'{_isQuestionFirst.ToString()}' WHERE Id = {MainTopic.Sentences[indexOfElement].id}";
                    command.ExecuteNonQuery();
                }
            }
        }

        private void UserStatisticsLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            UserStatisticsWindow usw = new UserStatisticsWindow()
            {
                WindowState = this.WindowState,
                Top = this.Top,
                Left = this.Left,
                Width = this.Width,
                Height = this.Height
            };
            usw.Show();
            this.Close();
        }

        private void SentenceEditorLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {

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
       
        int indexOfElement = 0;
        Topic MainTopic = new Topic();
        
        private void DownButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (MainTopic.Sentences.Count > 1)
            {
                UpdateSentence();

                if (indexOfElement == MainTopic.Sentences.Count - 1)
                {
                    indexOfElement = 0;
                    CreateSentence(MainTopic, 0);
                }
                else CreateSentence(MainTopic, ++indexOfElement);
            }
        }

        private void DeleteButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           using(SqlConnection connection = new SqlConnection(connectionString))
           {
                connection.Open();
                command.Connection = connection;
                command.CommandText = $"DELETE FROM MainTopic WHERE Id = {Int32.Parse(MainTopic.Sentences[indexOfElement].id)}";
                command.ExecuteNonQuery();
              
                if (MainTopic.Sentences.Count > 1)
                {
                    int lastIndex = MainTopic.Sentences.Count - 1;

                    MainTopic.Sentences.RemoveAt(indexOfElement);

                    if (indexOfElement == lastIndex)
                        CreateSentence(MainTopic, --indexOfElement);
                    else
                        CreateSentence(MainTopic, indexOfElement);
                }
                else
                {
                    MainTopic.Sentences.Clear();
                    ClearGrids(1);
                }
            }
        }

        void ClearGrids(int number)
        {
            if (number == 1)
            {
                WordInputGrid.Children.Clear();
                WordInputGrid.ColumnDefinitions.Clear();
                TranslateInputGrid.Children.Clear();
                TranslateInputGrid.ColumnDefinitions.Clear();
                QuestionVoiceInputGrid.Children.Clear();
                QuestionVoiceInputGrid.ColumnDefinitions.Clear();
                SentenceVoiceInputGrid.Children.Clear();
                SentenceVoiceInputGrid.ColumnDefinitions.Clear();
            }
            else
            {
                WordInputGrid2.Children.Clear();
                WordInputGrid2.ColumnDefinitions.Clear();
                TranslateInputGrid2.Children.Clear();
                TranslateInputGrid2.ColumnDefinitions.Clear();
                QuestionVoiceInputGrid2.Children.Clear();
                QuestionVoiceInputGrid2.ColumnDefinitions.Clear();
                SentenceVoiceInputGrid2.Children.Clear();
                SentenceVoiceInputGrid2.ColumnDefinitions.Clear();
            }
        }

        private void AddButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(WordInputGrid2.Children.Count > 0)
            {
                List<string> _questions = new List<string>(),
                _contexts = new List<string>(),
                _translations = new List<string>(),
                _waysToQuestionsVoice = new List<string>();


                for (int i = 0; i < WordInputGrid2.Children.Count; i++)
                {
                    if (((TextBox)WordInputGrid2.Children[i]).Foreground == Brushes.Green)
                    {
                        if (((TextBox)WordInputGrid2.Children[i]).Text != "")
                            _questions.Add(((TextBox)WordInputGrid2.Children[i]).Text);
                        else _questions.Add(" ");
                    }

                    else if (((TextBox)WordInputGrid2.Children[i]).Foreground == Brushes.Black)
                    {
                        if (((TextBox)WordInputGrid2.Children[i]).Text != "")
                            _contexts.Add(((TextBox)WordInputGrid2.Children[i]).Text);
                        else _contexts.Add(" ");
                    }

                    if (((TextBox)TranslateInputGrid2.Children[i]).Text != "")
                        _translations.Add(((TextBox)TranslateInputGrid2.Children[i]).Text);
                    else _translations.Add(" ");
                }

                for (int i = 0; i < QuestionVoiceInputGrid2.Children.Count; i++)
                {
                    if (((TextBox)QuestionVoiceInputGrid2.Children[i]).IsReadOnly == false)
                    {
                        if (((TextBox)QuestionVoiceInputGrid2.Children[i]).Text != "")
                            _waysToQuestionsVoice.Add(((TextBox)QuestionVoiceInputGrid2.Children[i]).Text);
                        else _waysToQuestionsVoice.Add(" ");
                    }
                }

                bool _isQuestionFirst = false;
                if (((TextBox)WordInputGrid2.Children[0]).Foreground == Brushes.Green) _isQuestionFirst = true;

                int maxId = 0;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = $"INSERT INTO MainTopic (IsQuestionFirst, Questions, Contexts, Translations, WaysToQuestionsVoice," +
                                   $" WayToSentenceVoice)" +
                                   $" VALUES(N'{_isQuestionFirst.ToString()}', N'{WriteParts(_questions)}'," +
                                   $"N'{WriteParts(_contexts)}', N'{WriteParts(_translations)}', " +
                                   $"N'{WriteParts(_waysToQuestionsVoice)}', N'{((TextBox)SentenceVoiceInputGrid2.Children[0]).Text}')";
                    command.ExecuteNonQuery();
                    command.CommandText = $"SELECT MAX(Id) FROM MainTopic";
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        maxId = reader.GetInt32(0);
                    }
                }

                MainTopic.Sentences.Add(new Sentence
                {
                    isQuestionFirst = _isQuestionFirst,
                    id = maxId.ToString(),
                    questions = _questions,
                    contexts = _contexts,
                    translations = _translations,
                    waysToQuestionsVoice = _waysToQuestionsVoice,
                    wayToSentenceVoice = ((TextBox)SentenceVoiceInputGrid2.Children[0]).Text
                });

                if (MainTopic.Sentences.Count == 1)
                {
                    CreateSentence(MainTopic, 0);
                    indexOfElement = 0;
                }
                ClearGrids(2);
            }
        }

        static string WriteParts(List<string> parts)
        {
            string s = "";
            foreach (string part in parts)
            {
                s += part + "~";
            }
            return s;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (indexForDelete > 0 && indexForDelete < WordInputGrid.Children.Count - 1)
            {
                MainTopic.Sentences[indexOfElement].questions.Clear();
                MainTopic.Sentences[indexOfElement].contexts.Clear();
                MainTopic.Sentences[indexOfElement].translations.Clear();
                MainTopic.Sentences[indexOfElement].waysToQuestionsVoice.Clear();

                string text = "", text2 = "";
                Brush currentBrush, currentBrush2;
                if (((TextBox)WordInputGrid.Children[indexForDelete]).Foreground == Brushes.Green)
                {
                    currentBrush = Brushes.Black;
                    currentBrush2 = Brushes.LightGray;
                }
                else
                {
                    currentBrush = Brushes.Green;
                    currentBrush2 = Brushes.LightGreen;
                }

                List<string> texts = new List<string>(), texts2 = new List<string>(), texts3 = new List<string>();
                List<Brush> brushes = new List<Brush>(), brushes2 = new List<Brush>();

                text += ((TextBox)WordInputGrid.Children[indexForDelete - 1]).Text + " " + ((TextBox)WordInputGrid.Children[indexForDelete + 1]).Text;
                text2 += ((TextBox)TranslateInputGrid.Children[indexForDelete - 1]).Text + " " + ((TextBox)TranslateInputGrid.Children[indexForDelete + 1]).Text;
                for (int i = 0; i < WordInputGrid.Children.Count; i++)
                {
                    if (i != indexForDelete && i != indexForDelete + 1 && i != indexForDelete - 1)
                    {
                        texts.Add(((TextBox)WordInputGrid.Children[i]).Text);
                        texts2.Add(((TextBox)TranslateInputGrid.Children[i]).Text);
                        texts3.Add(((TextBox)QuestionVoiceInputGrid.Children[FindIndexOfQuestionVoice(indexForDelete, WordInputGrid)]).Text);
                        brushes.Add(((TextBox)WordInputGrid.Children[i]).Foreground);
                        brushes2.Add(((TextBox)TranslateInputGrid.Children[i]).Foreground);
                    }
                }

                int currentLength = WordInputGrid.Children.Count - 2;

                WordInputGrid.Children.Clear();
                WordInputGrid.ColumnDefinitions.Clear();
                TranslateInputGrid.Children.Clear();
                TranslateInputGrid.ColumnDefinitions.Clear();
                QuestionVoiceInputGrid.Children.Clear();
                QuestionVoiceInputGrid.ColumnDefinitions.Clear();

                int index = 0, indexOfQuestionVoice = 0;
                for (int i = 0; i < currentLength; i++)
                {
                    if (i != indexForDelete - 1)
                    {
                        WordInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
                        MenuItem m = new MenuItem() { Header = "Удалить" };
                        m.Click += MenuItem_Click;
                        WordInputGrid.Children.Add(new TextBox()
                        {
                            Text = texts[index],
                            Foreground = brushes[index],
                            ContextMenu = new ContextMenu() { Items = { m } },
                            FontSize = 30,
                            FontWeight = FontWeights.Bold,
                            Background = background,
                            BorderThickness = new Thickness(0),
                            FontFamily = new FontFamily("Times New Roman"),
                            AllowDrop = false
                        });

                        WordInputGrid.Children[i].GotFocus += TextBox_GotFocus;
                        ((TextBox)WordInputGrid.Children[i]).TextChanged += TextBox_TextChanged;

                        Grid.SetColumn(WordInputGrid.Children[i], i);

                        if (brushes[index] == Brushes.Green)
                            MainTopic.Sentences[indexOfElement].questions.Add(texts[index]);
                        else MainTopic.Sentences[indexOfElement].contexts.Add(texts[index]);

                        MainTopic.Sentences[indexOfElement].translations.Add(texts2[index]);

                        TranslateInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
                        TranslateInputGrid.Children.Add(new TextBox()
                        {
                            Text = texts2[index],
                            Foreground = brushes[index++],
                            FontSize = 30,
                            FontWeight = FontWeights.Bold,
                            Background = background,
                            BorderThickness = new Thickness(0),
                            FontFamily = new FontFamily("Times New Roman"),
                            AllowDrop = false
                        });
                        Grid.SetColumn(TranslateInputGrid.Children[i], i);

                        if (brushes[index - 1] == Brushes.Green)
                        {
                            QuestionVoiceInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
                            QuestionVoiceInputGrid.Children.Add(new TextBox()
                            {
                                Text = texts[index - 1],
                                Foreground = Brushes.Black,
                                FontSize = 30,
                                FontWeight = FontWeights.Bold,
                                Background = background,
                                BorderThickness = new Thickness(0),
                                IsReadOnly = true,
                                FontFamily = new FontFamily("Times New Roman"),
                                AllowDrop = false
                            });
                            Grid.SetColumn(QuestionVoiceInputGrid.Children[QuestionVoiceInputGrid.Children.Count - 1],
                            QuestionVoiceInputGrid.Children.Count - 1);

                            MainTopic.Sentences[indexOfElement].waysToQuestionsVoice.Add(texts3[indexOfQuestionVoice]);

                            QuestionVoiceInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
                            QuestionVoiceInputGrid.Children.Add(new TextBox()
                            {
                                Foreground = (Brush)conv.ConvertFromString("#ffffcc"),
                                Text = texts3[indexOfQuestionVoice++],
                                FontSize = 30,
                                FontWeight = FontWeights.Bold,
                                Background = background,
                                BorderThickness = new Thickness(0),
                                FontFamily = new FontFamily("Times New Roman"),
                                AllowDrop = false
                            });
                            Grid.SetColumn(QuestionVoiceInputGrid.Children[QuestionVoiceInputGrid.Children.Count - 1],
                            QuestionVoiceInputGrid.Children.Count - 1);
                        }
                    }
                    else
                    {
                        WordInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
                        MenuItem m = new MenuItem() { Header = "Удалить" };
                        m.Click += MenuItem_Click;
                        WordInputGrid.Children.Add(new TextBox()
                        {
                            Text = text,
                            Foreground = currentBrush,
                            ContextMenu = new ContextMenu() { Items = { m } },
                            FontSize = 30,
                            FontWeight = FontWeights.Bold,
                            Background = background,
                            BorderThickness = new Thickness(0),
                            FontFamily = new FontFamily("Times New Roman"),
                            AllowDrop = false
                        });

                        if (currentBrush == Brushes.Green)
                            MainTopic.Sentences[indexOfElement].questions.Add(text);
                        else MainTopic.Sentences[indexOfElement].contexts.Add(text);

                        MainTopic.Sentences[indexOfElement].translations.Add(text2);

                        WordInputGrid.Children[i].GotFocus += TextBox_GotFocus;
                        ((TextBox)WordInputGrid.Children[i]).TextChanged += TextBox_TextChanged;
                        Grid.SetColumn(WordInputGrid.Children[i], i);

                        TranslateInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
                        TranslateInputGrid.Children.Add(new TextBox()
                        {
                            Text = text2,
                            Foreground = currentBrush2,
                            FontSize = 30,
                            FontWeight = FontWeights.Bold,
                            Background = background,
                            BorderThickness = new Thickness(0),
                            FontFamily = new FontFamily("Times New Roman"),
                            AllowDrop = false
                        });
                        Grid.SetColumn(TranslateInputGrid.Children[i], i);
                        if (currentBrush == Brushes.Green)
                        {
                            QuestionVoiceInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
                            QuestionVoiceInputGrid.Children.Add(new TextBox()
                            {
                                Text = text,
                                Foreground = Brushes.Black,
                                FontSize = 30,
                                FontWeight = FontWeights.Bold,
                                Background = background,
                                BorderThickness = new Thickness(0),
                                IsReadOnly = true,
                                FontFamily = new FontFamily("Times New Roman"),
                                AllowDrop = false
                            });

                            Grid.SetColumn(QuestionVoiceInputGrid.Children[QuestionVoiceInputGrid.Children.Count - 1],
                            QuestionVoiceInputGrid.Children.Count - 1);

                            MainTopic.Sentences[indexOfElement].waysToQuestionsVoice.Add("");

                            QuestionVoiceInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
                            QuestionVoiceInputGrid.Children.Add(new TextBox()
                            {
                                Foreground = (Brush)conv.ConvertFromString("#ffffcc"),
                                FontSize = 30,
                                FontWeight = FontWeights.Bold,
                                Background = background,
                                BorderThickness = new Thickness(0),
                                FontFamily = new FontFamily("Times New Roman"),
                                AllowDrop = false
                            });
                            Grid.SetColumn(QuestionVoiceInputGrid.Children[QuestionVoiceInputGrid.Children.Count - 1],
                            QuestionVoiceInputGrid.Children.Count - 1);
                        }
                    }
                }
            }
            else if (indexForDelete == WordInputGrid.Children.Count - 1 || indexForDelete == 0)
            {

                if (((TextBox)WordInputGrid.Children[indexForDelete]).Foreground == Brushes.Green)
                {
                    if (indexForDelete == 0)
                    {
                        QuestionVoiceInputGrid.Children.RemoveAt(0);
                        QuestionVoiceInputGrid.ColumnDefinitions.RemoveAt(0);
                        QuestionVoiceInputGrid.Children.RemoveAt(0);
                        QuestionVoiceInputGrid.ColumnDefinitions.RemoveAt(0);
                        for (int i = 0; i < QuestionVoiceInputGrid.Children.Count; i++)
                        {
                            Grid.SetColumn(QuestionVoiceInputGrid.Children[i], i);
                        }
                        MainTopic.Sentences[indexOfElement].waysToQuestionsVoice.RemoveAt(0);
                    }
                    else
                    {
                        QuestionVoiceInputGrid.Children.RemoveAt(QuestionVoiceInputGrid.Children.Count - 1);
                        QuestionVoiceInputGrid.ColumnDefinitions.RemoveAt(QuestionVoiceInputGrid.ColumnDefinitions.Count - 1);
                        QuestionVoiceInputGrid.Children.RemoveAt(QuestionVoiceInputGrid.Children.Count - 1);
                        QuestionVoiceInputGrid.ColumnDefinitions.RemoveAt(QuestionVoiceInputGrid.ColumnDefinitions.Count - 1);

                        MainTopic.Sentences[indexOfElement].waysToQuestionsVoice
                            .RemoveAt(MainTopic.Sentences[indexOfElement].waysToQuestionsVoice.Count - 1);
                    }

                    MainTopic.Sentences[indexOfElement].questions
                            .RemoveAt(FindIndexOfQuestion(indexForDelete));
                }
                else
                {
                    MainTopic.Sentences[indexOfElement].contexts
                            .RemoveAt(FindIndexOfContext(indexForDelete));
                }

                MainTopic.Sentences[indexOfElement].translations
                            .RemoveAt(indexForDelete);

                WordInputGrid.Children.RemoveAt(indexForDelete);
                WordInputGrid.ColumnDefinitions.RemoveAt(indexForDelete);
                TranslateInputGrid.Children.RemoveAt(indexForDelete);
                TranslateInputGrid.ColumnDefinitions.RemoveAt(indexForDelete);
                for (int i = 0; i < WordInputGrid.Children.Count; i++)
                {
                    Grid.SetColumn(WordInputGrid.Children[i], i);
                    Grid.SetColumn(TranslateInputGrid.Children[i], i);
                }
            }

            if (WordInputGrid.Children.Count == 0)
            {
                SentenceVoiceInputGrid.Children.Clear();
                MainTopic.Sentences[indexOfElement].wayToSentenceVoice = "";
            }
            else
            {
                if (((TextBox)WordInputGrid.Children[0]).Foreground == Brushes.Green)
                {
                    MainTopic.Sentences[indexOfElement].isQuestionFirst = true;
                }
                else
                {
                    MainTopic.Sentences[indexOfElement].isQuestionFirst = false;
                }
            }
        }

        int FindIndexOfQuestionVoice(int currentIndex, Grid WordInputGrid)
        {
            int indexOfQuestion = -1, indexOfTextBox = 0;
            for (int i = 0; i < WordInputGrid.Children.Count; i++)
            {
                if (((TextBox)WordInputGrid.Children[i]).Foreground == Brushes.Green)
                {
                    if (i == indexForDelete)
                    {
                        indexOfQuestion++;
                        break;
                    }
                    else indexOfQuestion++;
                }
            }
            for (int i = 0; i < indexOfQuestion; i++)
            {
                if (i == 0)
                {
                    indexOfTextBox = 1;
                }
                else indexOfTextBox += 2;
            }
            return indexOfTextBox;
        }

        private void MenuItem_Click2(object sender, RoutedEventArgs e)
        {
            string text = "", text2 = "";
            Brush currentBrush, currentBrush2;
            if (((TextBox)WordInputGrid2.Children[indexForDelete]).Foreground == Brushes.Green)
            {
                currentBrush = Brushes.Black;
                currentBrush2 = Brushes.LightGray;
            }
            else
            {
                currentBrush = Brushes.Green;
                currentBrush2 = Brushes.LightGreen;
            }

            List<string> texts = new List<string>(), texts2 = new List<string>(), texts3 = new List<string>();
            List<Brush> brushes = new List<Brush>(), brushes2 = new List<Brush>();
            if (indexForDelete > 0 && indexForDelete < WordInputGrid2.Children.Count - 1)
            {
                text += ((TextBox)WordInputGrid2.Children[indexForDelete - 1]).Text + " " + ((TextBox)WordInputGrid2.Children[indexForDelete + 1]).Text;
                text2 += ((TextBox)TranslateInputGrid2.Children[indexForDelete - 1]).Text + " " + ((TextBox)TranslateInputGrid2.Children[indexForDelete + 1]).Text;
                for (int i = 0; i < WordInputGrid2.Children.Count; i++)
                {
                    if (i != indexForDelete && i != indexForDelete + 1 && i != indexForDelete - 1)
                    {
                        texts.Add(((TextBox)WordInputGrid2.Children[i]).Text);
                        texts2.Add(((TextBox)TranslateInputGrid2.Children[i]).Text);
                        texts3.Add(((TextBox)QuestionVoiceInputGrid2.Children[FindIndexOfQuestionVoice(indexForDelete, WordInputGrid2)]).Text);
                        brushes.Add(((TextBox)WordInputGrid2.Children[i]).Foreground);
                        brushes2.Add(((TextBox)TranslateInputGrid2.Children[i]).Foreground);
                    }
                }

                int currentLength = WordInputGrid2.Children.Count - 2;

                WordInputGrid2.Children.Clear();
                WordInputGrid2.ColumnDefinitions.Clear();
                TranslateInputGrid2.Children.Clear();
                TranslateInputGrid2.ColumnDefinitions.Clear();
                QuestionVoiceInputGrid2.Children.Clear();
                QuestionVoiceInputGrid2.ColumnDefinitions.Clear();

                int index = 0, indexOfQuestionVoice = 0;
                for (int i = 0; i < currentLength; i++)
                {
                    if (i != indexForDelete - 1)
                    {
                        WordInputGrid2.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
                        MenuItem m = new MenuItem() { Header = "Удалить" };
                        m.Click += MenuItem_Click2;
                        WordInputGrid2.Children.Add(new TextBox()
                        {
                            Text = texts[index],
                            Foreground = brushes[index],
                            ContextMenu = new ContextMenu() { Items = { m } },
                            FontSize = 30,
                            FontWeight = FontWeights.Bold,
                            Background = background,
                            BorderThickness = new Thickness(0),
                            FontFamily = new FontFamily("Times New Roman"),
                            AllowDrop = false
                        });
                        WordInputGrid2.Children[i].GotFocus += TextBox_GotFocus2;
                        Grid.SetColumn(WordInputGrid2.Children[i], i);

                        TranslateInputGrid2.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
                        TranslateInputGrid2.Children.Add(new TextBox()
                        {
                            Text = texts2[index],
                            Foreground = brushes[index++],
                            FontSize = 30,
                            FontWeight = FontWeights.Bold,
                            Background = background,
                            BorderThickness = new Thickness(0),
                            FontFamily = new FontFamily("Times New Roman"),
                            AllowDrop = false
                        });
                        Grid.SetColumn(TranslateInputGrid2.Children[i], i);

                        if (brushes[index - 1] == Brushes.Green)
                        {
                            QuestionVoiceInputGrid2.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
                            QuestionVoiceInputGrid2.Children.Add(new TextBox()
                            {
                                Text = texts[index - 1],
                                Foreground = Brushes.Black,
                                FontSize = 30,
                                FontWeight = FontWeights.Bold,
                                Background = background,
                                BorderThickness = new Thickness(0),
                                IsReadOnly = true,
                                FontFamily = new FontFamily("Times New Roman"),
                                AllowDrop = false
                            });
                            Grid.SetColumn(QuestionVoiceInputGrid2.Children[QuestionVoiceInputGrid2.Children.Count - 1],
                            QuestionVoiceInputGrid2.Children.Count - 1);

                            QuestionVoiceInputGrid2.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
                            QuestionVoiceInputGrid2.Children.Add(new TextBox()
                            {
                                Foreground = (Brush)conv.ConvertFromString("#ffffcc"),
                                Text = texts3[indexOfQuestionVoice++],
                                FontSize = 30,
                                FontWeight = FontWeights.Bold,
                                Background = background,
                                BorderThickness = new Thickness(0),
                                FontFamily = new FontFamily("Times New Roman"),
                                AllowDrop = false
                            });
                            Grid.SetColumn(QuestionVoiceInputGrid2.Children[QuestionVoiceInputGrid2.Children.Count - 1],
                            QuestionVoiceInputGrid2.Children.Count - 1);
                        }
                    }
                    else
                    {
                        WordInputGrid2.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
                        MenuItem m = new MenuItem() { Header = "Удалить" };
                        m.Click += MenuItem_Click2;
                        WordInputGrid2.Children.Add(new TextBox()
                        {
                            Text = text,
                            Foreground = currentBrush,
                            ContextMenu = new ContextMenu() { Items = { m } },
                            FontSize = 30,
                            FontWeight = FontWeights.Bold,
                            Background = background,
                            BorderThickness = new Thickness(0),
                            FontFamily = new FontFamily("Times New Roman"),
                            AllowDrop = false
                        });

                        WordInputGrid2.Children[i].GotFocus += TextBox_GotFocus2;
                        Grid.SetColumn(WordInputGrid2.Children[i], i);

                        TranslateInputGrid2.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
                        TranslateInputGrid2.Children.Add(new TextBox()
                        {
                            Text = text2,
                            Foreground = currentBrush2,
                            FontSize = 30,
                            FontWeight = FontWeights.Bold,
                            Background = background,
                            BorderThickness = new Thickness(0),
                            FontFamily = new FontFamily("Times New Roman"),
                            AllowDrop = false
                        });
                        Grid.SetColumn(TranslateInputGrid2.Children[i], i);
                        if (currentBrush == Brushes.Green)
                        {
                            QuestionVoiceInputGrid2.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
                            QuestionVoiceInputGrid2.Children.Add(new TextBox()
                            {
                                Text = text,
                                Foreground = Brushes.Black,
                                FontSize = 30,
                                FontWeight = FontWeights.Bold,
                                Background = background,
                                BorderThickness = new Thickness(0),
                                IsReadOnly = true,
                                FontFamily = new FontFamily("Times New Roman"),
                                AllowDrop = false
                            });

                            Grid.SetColumn(QuestionVoiceInputGrid2.Children[QuestionVoiceInputGrid2.Children.Count - 1],
                            QuestionVoiceInputGrid2.Children.Count - 1);

                            QuestionVoiceInputGrid2.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
                            QuestionVoiceInputGrid2.Children.Add(new TextBox()
                            {
                                Foreground = (Brush)conv.ConvertFromString("#ffffcc"),
                                FontSize = 30,
                                FontWeight = FontWeights.Bold,
                                Background = background,
                                BorderThickness = new Thickness(0),
                                FontFamily = new FontFamily("Times New Roman"),
                                AllowDrop = false
                            });
                            Grid.SetColumn(QuestionVoiceInputGrid2.Children[QuestionVoiceInputGrid2.Children.Count - 1],
                            QuestionVoiceInputGrid2.Children.Count - 1);
                        }
                    }
                }
            }
            else if (indexForDelete == WordInputGrid2.Children.Count - 1 || indexForDelete == 0)
            {

                if (((TextBox)WordInputGrid2.Children[indexForDelete]).Foreground == Brushes.Green)
                {
                    if (indexForDelete == 0)
                    {
                        QuestionVoiceInputGrid2.Children.RemoveAt(0);
                        QuestionVoiceInputGrid2.ColumnDefinitions.RemoveAt(0);
                        QuestionVoiceInputGrid2.Children.RemoveAt(0);
                        QuestionVoiceInputGrid2.ColumnDefinitions.RemoveAt(0);
                        for (int i = 0; i < QuestionVoiceInputGrid2.Children.Count; i++)
                        {
                            Grid.SetColumn(QuestionVoiceInputGrid2.Children[i], i);
                        }
                    }
                    else
                    {
                        QuestionVoiceInputGrid2.Children.RemoveAt(QuestionVoiceInputGrid2.Children.Count - 1);
                        QuestionVoiceInputGrid2.ColumnDefinitions.RemoveAt(QuestionVoiceInputGrid2.ColumnDefinitions.Count - 1);
                        QuestionVoiceInputGrid2.Children.RemoveAt(QuestionVoiceInputGrid2.Children.Count - 1);
                        QuestionVoiceInputGrid2.ColumnDefinitions.RemoveAt(QuestionVoiceInputGrid2.ColumnDefinitions.Count - 1);
                    }
                }

                WordInputGrid2.Children.RemoveAt(indexForDelete);
                WordInputGrid2.ColumnDefinitions.RemoveAt(WordInputGrid2.ColumnDefinitions.Count - 1);
                TranslateInputGrid2.Children.RemoveAt(indexForDelete);
                TranslateInputGrid2.ColumnDefinitions.RemoveAt(TranslateInputGrid2.ColumnDefinitions.Count - 1);
                for (int i = 0; i < WordInputGrid2.Children.Count; i++)
                {
                    Grid.SetColumn(WordInputGrid2.Children[i], i);
                    Grid.SetColumn(TranslateInputGrid2.Children[i], i);
                }
            }

            if (WordInputGrid2.Children.Count == 0)
                SentenceVoiceInputGrid2.Children.Clear();
        }
        
        static BrushConverter conv = new BrushConverter();
        Brush background = (Brush)conv.ConvertFromString("#bde0ff");

        void AddNewQuestion(Grid WordInputGrid, Grid TranslateInputGrid, Grid QuestionVoiceInputGrid)
        {
            WordInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
            WordInputGrid.Children.Add(new TextBox()
            {
                Foreground = Brushes.Green,
                Height = 40,
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Background = background,
                BorderThickness = new Thickness(0),
                FontFamily = new FontFamily("Times New Roman"),
                AllowDrop = false
            });

            Grid.SetColumn(WordInputGrid.Children[WordInputGrid.Children.Count - 1], WordInputGrid.ColumnDefinitions.Count - 1);

            TranslateInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
            TranslateInputGrid.Children.Add(new TextBox()
            {
                Foreground = Brushes.LightGreen,
                Height = 40,
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Background = background,
                BorderThickness = new Thickness(0),
                FontFamily = new FontFamily("Times New Roman"),
                AllowDrop = false
            });

            Grid.SetColumn(TranslateInputGrid.Children[TranslateInputGrid.Children.Count - 1], TranslateInputGrid.ColumnDefinitions.Count - 1);

            QuestionVoiceInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
            QuestionVoiceInputGrid.Children.Add(new TextBox()
            {
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Background = background,
                BorderThickness = new Thickness(0),
                IsReadOnly = true,
                FontFamily = new FontFamily("Times New Roman"),
                AllowDrop = false
            });
            Grid.SetColumn(QuestionVoiceInputGrid.Children[QuestionVoiceInputGrid.Children.Count - 1], QuestionVoiceInputGrid.Children.Count - 1);

            QuestionVoiceInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
            QuestionVoiceInputGrid.Children.Add(new TextBox()
            {
                Foreground = (Brush)conv.ConvertFromString("#ffffcc"),
                Height = 40,
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Background = background,
                BorderThickness = new Thickness(0),
                FontFamily = new FontFamily("Times New Roman"),
                AllowDrop = false
            });
            Grid.SetColumn(QuestionVoiceInputGrid.Children[QuestionVoiceInputGrid.Children.Count - 1], QuestionVoiceInputGrid.Children.Count - 1);
        }

        void AddNewQuestion(Topic topic, int i, int indexOfSentence, int indexOfQuestions)
        {
            WordInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
            MenuItem m = new MenuItem() { Header = "Удалить" };
            m.Click += MenuItem_Click;
            WordInputGrid.Children.Add(new TextBox()
            {
                Foreground = Brushes.Green,
                Text = topic.Sentences[indexOfSentence].questions[indexOfQuestions],
                ContextMenu = contextMenu,
                Height = 40,
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Background = background,
                BorderThickness = new Thickness(0),
                FontFamily = new FontFamily("Times New Roman"),
                AllowDrop = false
            });

            if (((TextBox)WordInputGrid.Children[i]).Text == " ")
                ((TextBox)WordInputGrid.Children[i]).Text = "";

            WordInputGrid.Children[i].GotFocus += TextBox_GotFocus;
            ((TextBox)WordInputGrid.Children[i]).TextChanged += TextBox_TextChanged;
            Grid.SetColumn(WordInputGrid.Children[i], i);

            TranslateInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
            TranslateInputGrid.Children.Add(new TextBox()
            {
                Foreground = Brushes.LightGreen,
                Text = topic.Sentences[indexOfSentence].translations[i],
                Height = 40,
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Background = background,
                BorderThickness = new Thickness(0),
                FontFamily = new FontFamily("Times New Roman"),
                AllowDrop = false
            });

            if (((TextBox)TranslateInputGrid.Children[i]).Text == " ")
                ((TextBox)TranslateInputGrid.Children[i]).Text = "";

            Grid.SetColumn(TranslateInputGrid.Children[i], i);

            TranslateInputGrid.Children[i].GotFocus += TranslateTextBox_GotFocus;
            ((TextBox)TranslateInputGrid.Children[i]).TextChanged += TranslateTextBox_TextChanged;

            QuestionVoiceInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
            QuestionVoiceInputGrid.Children.Add(new TextBox()
            {
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Text = topic.Sentences[indexOfSentence].questions[indexOfQuestions],
                Background = background,
                BorderThickness = new Thickness(0),
                IsReadOnly = true,
                FontFamily = new FontFamily("Times New Roman"),
                AllowDrop = false
            });
            Grid.SetColumn(QuestionVoiceInputGrid.Children[QuestionVoiceInputGrid.Children.Count - 1], QuestionVoiceInputGrid.Children.Count - 1);

            QuestionVoiceInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });

            QuestionVoiceInputGrid.Children.Add(new TextBox()
            {
                Foreground = (Brush)conv.ConvertFromString("#ffffcc"),
                Height = 40,
                FontSize = 30,
                Text = topic.Sentences[indexOfSentence].waysToQuestionsVoice[indexOfQuestions],
                FontWeight = FontWeights.Bold,
                Background = background,
                BorderThickness = new Thickness(0),
                FontFamily = new FontFamily("Times New Roman"),
                AllowDrop = false
            });

            if (((TextBox)QuestionVoiceInputGrid.Children[QuestionVoiceInputGrid.Children.Count - 1]).Text == " ")
                ((TextBox)QuestionVoiceInputGrid.Children[QuestionVoiceInputGrid.Children.Count - 1]).Text = "";

            QuestionVoiceInputGrid.Children[QuestionVoiceInputGrid.Children.Count - 1].GotFocus += QuestionVoiceTextBox_GotFocus;
            ((TextBox)QuestionVoiceInputGrid.Children[QuestionVoiceInputGrid.Children.Count - 1]).TextChanged += QuestionVoiceTextBox_TextChanged;

            Grid.SetColumn(QuestionVoiceInputGrid.Children[QuestionVoiceInputGrid.Children.Count - 1], QuestionVoiceInputGrid.Children.Count - 1);
        }

        void AddNewContext(Grid WordInputGrid, Grid TranslateInputGrid, Grid QuestionVoiceInputGrid)
        {

            WordInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
            WordInputGrid.Children.Add(new TextBox()
            {
                Foreground = Brushes.Black,
                Height = 40,
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Background = background,
                BorderThickness = new Thickness(0),
                FontFamily = new FontFamily("Times New Roman"),
                AllowDrop = false
            });
            Grid.SetColumn(WordInputGrid.Children[WordInputGrid.Children.Count - 1], WordInputGrid.ColumnDefinitions.Count - 1);

            TranslateInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
            TranslateInputGrid.Children.Add(new TextBox()
            {
                Foreground = Brushes.Gray,
                Height = 40,
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Background = background,
                BorderThickness = new Thickness(0),
                FontFamily = new FontFamily("Times New Roman"),
                AllowDrop = false
            });

            Grid.SetColumn(TranslateInputGrid.Children[TranslateInputGrid.Children.Count - 1], TranslateInputGrid.ColumnDefinitions.Count - 1);
        }

        void AddNewContext(Topic topic, int i, int indexOfSentence, int indexOfContexts)
        {

            WordInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
            MenuItem m = new MenuItem() { Header = "Удалить" };
            m.Click += MenuItem_Click;
            WordInputGrid.Children.Add(new TextBox()
            {
                Foreground = Brushes.Black,
                ContextMenu = contextMenu,
                Text = topic.Sentences[indexOfSentence].contexts[indexOfContexts],
                Height = 40,
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Background = background,
                BorderThickness = new Thickness(0),
                FontFamily = new FontFamily("Times New Roman"),
                AllowDrop = false
            });

            if (((TextBox)WordInputGrid.Children[i]).Text == " ")
                ((TextBox)WordInputGrid.Children[i]).Text = "";

            WordInputGrid.Children[i].GotFocus += TextBox_GotFocus;
            ((TextBox)WordInputGrid.Children[i]).TextChanged += TextBox_TextChanged;
            Grid.SetColumn(WordInputGrid.Children[i], i);

            TranslateInputGrid.ColumnDefinitions.Add(new ColumnDefinition() { MinWidth = 30, Width = GridLength.Auto });
            TranslateInputGrid.Children.Add(new TextBox()
            {
                Foreground = Brushes.Gray,
                Height = 40,
                Text = topic.Sentences[indexOfSentence].translations[i],
                FontSize = 30,
                FontWeight = FontWeights.Bold,
                Background = background,
                BorderThickness = new Thickness(0),
                FontFamily = new FontFamily("Times New Roman"),
                AllowDrop = false
            });

            if (((TextBox)TranslateInputGrid.Children[i]).Text == " ")
                ((TextBox)TranslateInputGrid.Children[i]).Text = "";

            Grid.SetColumn(TranslateInputGrid.Children[i], i);
            TranslateInputGrid.Children[i].GotFocus += TranslateTextBox_GotFocus;
            ((TextBox)TranslateInputGrid.Children[i]).TextChanged += TranslateTextBox_TextChanged;
        }

        void CheckText(TextBox textBox, string text)
        {
            int selectionStart = textBox.SelectionStart - 1;
            string s = "";
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != '~') s += text[i];
            }
            if (s != text)
            {
                textBox.Text = s;
                textBox.SelectionStart = selectionStart;
            }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            CheckText((TextBox)sender, ((TextBox)sender).Text);

            string s = " ";
            if (!(((TextBox)sender).Text == "")) s = ((TextBox)sender).Text;

            if (((TextBox)sender).Foreground == Brushes.Green)
            {
                int indexOfQuestion = -1, indexOfTextBox = 0;
                for (int i = 0; i < WordInputGrid.Children.Count; i++)
                {
                    if (((TextBox)WordInputGrid.Children[i]).Foreground == Brushes.Green)
                    {
                        if (i == indexForDelete)
                        {
                            indexOfQuestion++;
                            break;
                        }
                        else indexOfQuestion++;
                    }
                }
                if (indexOfQuestion != 0)
                {
                    for (int i = 0; i < indexOfQuestion; i++)
                    {
                        indexOfTextBox += 2;
                    }
                }


                ((TextBox)QuestionVoiceInputGrid.Children[indexOfTextBox]).Text =
                     ((TextBox)WordInputGrid.Children[indexForDelete]).Text;

                MainTopic.Sentences[indexOfElement].questions[FindIndexOfQuestion(indexForDelete)] = s;
            }
            else
            {
                MainTopic.Sentences[indexOfElement].contexts[FindIndexOfContext(indexForDelete)] = s;
            }
        }

        private void TranslateTextBox_TextChanged(object sender, EventArgs e)
        {
            CheckText((TextBox)sender, ((TextBox)sender).Text);
            string s = " ";
            if (!(((TextBox)sender).Text == "")) s = ((TextBox)sender).Text;
            MainTopic.Sentences[indexOfElement].translations[indexForDelete] = s;
        }

        private void QuestionVoiceTextBox_TextChanged(object sender, EventArgs e)
        {
            CheckText((TextBox)sender, ((TextBox)sender).Text);
            string s = " ";
            if (!(((TextBox)sender).Text == "")) s = ((TextBox)sender).Text;
            int indexOfQuestionVoice = -1;
            if (indexForDelete == 1) indexOfQuestionVoice = 0;
            else
            {
                for (int i = 0; i < QuestionVoiceInputGrid.Children.Count; i++)
                {
                    if (i == indexForDelete)
                    {
                        indexOfQuestionVoice++;
                        break;
                    }
                    else if (!((TextBox)QuestionVoiceInputGrid.Children[i]).IsReadOnly)
                        indexOfQuestionVoice++;
                }
            }

            MainTopic.Sentences[indexOfElement].waysToQuestionsVoice[indexOfQuestionVoice] = s;
        }

        private void SentenceVoiceTextBox_TextChanged(object sender, EventArgs e)
        {
            CheckText((TextBox)sender, ((TextBox)sender).Text);
            MainTopic.Sentences[indexOfElement].wayToSentenceVoice = ((TextBox)sender).Text;
        }

        int FindIndexOfQuestion(int currentIndex)
        {
            int indexOfQuestion = -1;
            foreach (TextBox textBox in WordInputGrid.Children)
            {
                if (textBox.Foreground == Brushes.Green)
                    indexOfQuestion++;
            }
            return indexOfQuestion;
        }

        int FindIndexOfContext(int currentIndex)
        {
            int indexOfContext = -1;
            foreach (TextBox textBox in WordInputGrid.Children)
            {
                if (textBox.Foreground == Brushes.Black)
                    indexOfContext++;
            }
            return indexOfContext;
        }

        private void TextBox_TextChanged2(object sender, EventArgs e)
        {
            if (((TextBox)sender).Foreground == Brushes.Green)
            {
                int indexOfQuestion = -1, indexOfTextBox = 0;
                for (int i = 0; i < WordInputGrid2.Children.Count; i++)
                {
                    if (((TextBox)WordInputGrid2.Children[i]).Foreground == Brushes.Green)
                    {
                        if (i == indexForDelete)
                        {
                            indexOfQuestion++;
                            break;
                        }
                        else indexOfQuestion++;
                    }
                }
                if (indexOfQuestion != 0)
                {
                    for (int i = 0; i < indexOfQuestion; i++)
                    {
                        indexOfTextBox += 2;
                    }
                }

                ((TextBox)QuestionVoiceInputGrid2.Children[indexOfTextBox]).Text =
                     ((TextBox)WordInputGrid2.Children[indexForDelete]).Text;
            }

            CheckText((TextBox)sender, ((TextBox)sender).Text);
        }

        int indexForDelete = 0;
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            indexForDelete = WordInputGrid.Children.IndexOf(((TextBox)sender));
        }

        private void TextBox_GotFocus2(object sender, RoutedEventArgs e)
        {
            indexForDelete = WordInputGrid2.Children.IndexOf(((TextBox)sender));
        }

        private void TranslateTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            indexForDelete = TranslateInputGrid.Children.IndexOf(((TextBox)sender));
        }

        private void QuestionVoiceTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            indexForDelete = QuestionVoiceInputGrid.Children.IndexOf(((TextBox)sender));
        }

        void CreateSentence(Topic MainTopic, int indexOfElement)
        {
            ClearGrids(1);

            int sumParts = MainTopic.Sentences[indexOfElement].questions.Count
                    + MainTopic.Sentences[indexOfElement].contexts.Count;

            int indexOfQuestions = 0, indexOfContexts = 0;

            if (sumParts != 0)
            {
                if (MainTopic.Sentences[indexOfElement].isQuestionFirst)
                {
                    for (int i = 0; i < sumParts; i++)
                    {
                        if (i % 2 == 0) AddNewQuestion(MainTopic, i, indexOfElement, indexOfQuestions++);
                        else AddNewContext(MainTopic, i, indexOfElement, indexOfContexts++);
                    }
                }
                else
                {
                    for (int i = 0; i < sumParts; i++)
                    {
                        if (i % 2 == 0) AddNewContext(MainTopic, i, indexOfElement, indexOfContexts++);
                        else AddNewQuestion(MainTopic, i, indexOfElement, indexOfQuestions++);
                    }
                }

                SentenceVoiceInputGrid.Children.Add(new TextBox
                {
                    FontFamily = new FontFamily("Times New Roman"),
                    Text = MainTopic.Sentences[indexOfElement].wayToSentenceVoice,
                    FontSize = 30,
                    FontWeight = FontWeights.Bold,
                    BorderThickness = new Thickness(0),
                    Foreground = Brushes.Black,
                    Background = background,
                    AllowDrop = false
                });

                ((TextBox)SentenceVoiceInputGrid.Children[0]).TextChanged += SentenceVoiceTextBox_TextChanged;

                SentenceVoiceInputGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    MinWidth = 30,
                    Width = GridLength.Auto,
                });

                Grid.SetColumn(SentenceVoiceInputGrid.Children[0], 0);
            }
        }

        private void NewQuestionButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragDrop.DoDragDrop(((Image)sender), ((Image)sender).Name, DragDropEffects.Move);
        }

        private async void WordInputGrid_Drop(object sender, DragEventArgs e)
        {
            if ((string)e.Data.GetData(DataFormats.Text) == "NewQuestionButton")
            {
                if (WordInputGrid.Children.Count != 0)
                {
                    if (((TextBox)WordInputGrid.Children[WordInputGrid.Children.Count - 1]).Foreground == Brushes.Green)
                    {
                        WordInputGrid.Children[WordInputGrid.Children.Count - 1].Opacity = 0;

                        for (int i = 1; i < 11; i++)
                        {
                            WordInputGrid.Children[WordInputGrid.Children.Count - 1].Opacity = 0.1 * i;
                            await Task.Delay(50);
                        }

                        WordInputGrid.Children[WordInputGrid.Children.Count - 1].Focus();
                    }
                    else
                    {
                        AddNewQuestion(WordInputGrid, TranslateInputGrid, QuestionVoiceInputGrid);
                        ((TextBox)WordInputGrid.Children[WordInputGrid.Children.Count - 1]).GotFocus += TextBox_GotFocus;
                        ((TextBox)WordInputGrid.Children[WordInputGrid.Children.Count - 1]).TextChanged += TextBox_TextChanged;
                        MenuItem m = new MenuItem() { Header = "Удалить" };
                        m.Click += MenuItem_Click;
                        ((TextBox)WordInputGrid.Children[WordInputGrid.Children.Count - 1]).ContextMenu = new ContextMenu() { Items = { m } };
                        MainTopic.Sentences[indexOfElement].questions.Add(" ");
                        MainTopic.Sentences[indexOfElement].translations.Add(" ");
                        MainTopic.Sentences[indexOfElement].waysToQuestionsVoice.Add(" ");
                        WordInputGrid.Children[WordInputGrid.Children.Count - 1].Focus();
                    }
                }
                else
                {
                    AddNewQuestion(WordInputGrid, TranslateInputGrid, QuestionVoiceInputGrid);
                    ((TextBox)WordInputGrid.Children[WordInputGrid.Children.Count - 1]).GotFocus += TextBox_GotFocus;
                    ((TextBox)WordInputGrid.Children[WordInputGrid.Children.Count - 1]).TextChanged += TextBox_TextChanged;
                    MenuItem m = new MenuItem() { Header = "Удалить" };
                    m.Click += MenuItem_Click;
                    ((TextBox)WordInputGrid.Children[WordInputGrid.Children.Count - 1]).ContextMenu = new ContextMenu() { Items = { m } };
                    MainTopic.Sentences[indexOfElement].questions.Add(" ");
                    MainTopic.Sentences[indexOfElement].translations.Add(" ");
                    MainTopic.Sentences[indexOfElement].waysToQuestionsVoice.Add(" ");
                    WordInputGrid.Children[WordInputGrid.Children.Count - 1].Focus();
                }
            }
            else if ((string)e.Data.GetData(DataFormats.Text) == "NewContextButton")
            {
                if (WordInputGrid.Children.Count != 0)
                {
                    if (((TextBox)WordInputGrid.Children[WordInputGrid.Children.Count - 1]).Foreground == Brushes.Black)
                    {
                        for (int i = 1; i < 11; i++)
                        {
                            WordInputGrid.Children[WordInputGrid.Children.Count - 1].Opacity = 0.1 * i;
                            await Task.Delay(50);
                        }

                        WordInputGrid.Children[WordInputGrid.Children.Count - 1].Focus();
                    }
                    else
                    {
                        AddNewContext(WordInputGrid, TranslateInputGrid, QuestionVoiceInputGrid);
                        ((TextBox)WordInputGrid.Children[WordInputGrid.Children.Count - 1]).GotFocus += TextBox_GotFocus;
                        ((TextBox)WordInputGrid.Children[WordInputGrid.Children.Count - 1]).TextChanged += TextBox_TextChanged;
                        MenuItem m = new MenuItem() { Header = "Удалить" };
                        m.Click += MenuItem_Click;
                        ((TextBox)WordInputGrid.Children[WordInputGrid.Children.Count - 1]).ContextMenu = new ContextMenu() { Items = { m } };
                        MainTopic.Sentences[indexOfElement].contexts.Add(" ");
                        MainTopic.Sentences[indexOfElement].translations.Add(" ");
                        ((TextBox)WordInputGrid.Children[WordInputGrid.Children.Count - 1]).Focus();
                    }
                }
                else
                {
                    AddNewContext(WordInputGrid, TranslateInputGrid, QuestionVoiceInputGrid);
                    ((TextBox)WordInputGrid.Children[WordInputGrid.Children.Count - 1]).GotFocus += TextBox_GotFocus;
                    ((TextBox)WordInputGrid.Children[WordInputGrid.Children.Count - 1]).TextChanged += TextBox_TextChanged;
                    MenuItem m = new MenuItem() { Header = "Удалить" };
                    m.Click += MenuItem_Click;
                    ((TextBox)WordInputGrid.Children[WordInputGrid.Children.Count - 1]).ContextMenu = new ContextMenu() { Items = { m } };
                    MainTopic.Sentences[indexOfElement].contexts.Add(" ");
                    MainTopic.Sentences[indexOfElement].translations.Add(" ");
                    ((TextBox)WordInputGrid.Children[WordInputGrid.Children.Count - 1]).Focus();
                }
            }


            if (((TextBox)WordInputGrid.Children[0]).Foreground == Brushes.Green)
            {
                MainTopic.Sentences[indexOfElement].isQuestionFirst = true;
            }
            else
            {
                MainTopic.Sentences[indexOfElement].isQuestionFirst = false;
            }

            if (SentenceVoiceInputGrid.Children.Count == 0)
            {
                SentenceVoiceInputGrid.Children.Add(new TextBox
                {
                    FontFamily = new FontFamily("Times New Roman"),
                    FontSize = 30,
                    FontWeight = FontWeights.Bold,
                    BorderThickness = new Thickness(0),
                    Foreground = Brushes.Black,
                    Background = background,
                    AllowDrop = false
                });

                ((TextBox)SentenceVoiceInputGrid.Children[0]).TextChanged += SentenceVoiceTextBox_TextChanged;

                SentenceVoiceInputGrid.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    MinWidth = 30,
                    Width = GridLength.Auto,
                });

                Grid.SetColumn(SentenceVoiceInputGrid.Children[0], 0);
            }
        }

        private async void WordInputGrid2_Drop(object sender, DragEventArgs e)
        {
            if ((string)e.Data.GetData(DataFormats.Text) == "NewQuestionButton2")
            {
                if (WordInputGrid2.Children.Count != 0)
                {
                    if (((TextBox)WordInputGrid2.Children[WordInputGrid2.Children.Count - 1]).Foreground == Brushes.Green)
                    {
                        WordInputGrid2.Children[WordInputGrid2.Children.Count - 1].Opacity = 0;

                        for (int i = 1; i < 11; i++)
                        {
                            WordInputGrid2.Children[WordInputGrid2.Children.Count - 1].Opacity = 0.1 * i;
                            await Task.Delay(50);
                        }

                        WordInputGrid2.Children[WordInputGrid2.Children.Count - 1].Focus();
                    }
                    else
                    {
                        AddNewQuestion(WordInputGrid2, TranslateInputGrid2, QuestionVoiceInputGrid2);
                        ((TextBox)WordInputGrid2.Children[WordInputGrid2.Children.Count - 1]).GotFocus += TextBox_GotFocus2;
                        ((TextBox)WordInputGrid2.Children[WordInputGrid2.Children.Count - 1]).TextChanged += TextBox_TextChanged2;
                        MenuItem m = new MenuItem() { Header = "Удалить" };
                        m.Click += MenuItem_Click2;
                        ((TextBox)WordInputGrid2.Children[WordInputGrid2.Children.Count - 1]).ContextMenu = new ContextMenu() { Items = { m } };
                    }
                }
                else
                {
                    AddNewQuestion(WordInputGrid2, TranslateInputGrid2, QuestionVoiceInputGrid2);
                    ((TextBox)WordInputGrid2.Children[WordInputGrid2.Children.Count - 1]).GotFocus += TextBox_GotFocus2;
                    ((TextBox)WordInputGrid2.Children[WordInputGrid2.Children.Count - 1]).TextChanged += TextBox_TextChanged2;
                    MenuItem m = new MenuItem() { Header = "Удалить" };
                    m.Click += MenuItem_Click2;
                    ((TextBox)WordInputGrid2.Children[WordInputGrid2.Children.Count - 1]).ContextMenu = new ContextMenu() { Items = { m } };
                }
            }
            else if ((string)e.Data.GetData(DataFormats.Text) == "NewContextButton2")
            {
                if (WordInputGrid2.Children.Count != 0)
                {
                    if (((TextBox)WordInputGrid2.Children[WordInputGrid2.Children.Count - 1]).Foreground == Brushes.Black)
                    {
                        for (int i = 1; i < 11; i++)
                        {
                            WordInputGrid2.Children[WordInputGrid2.Children.Count - 1].Opacity = 0.1 * i;
                            await Task.Delay(50);
                        }

                        WordInputGrid2.Children[WordInputGrid2.Children.Count - 1].Focus();
                    }
                    else
                    {
                        AddNewContext(WordInputGrid2, TranslateInputGrid2, QuestionVoiceInputGrid2);
                        ((TextBox)WordInputGrid2.Children[WordInputGrid2.Children.Count - 1]).TextChanged += TextBox_TextChanged2;
                        ((TextBox)WordInputGrid2.Children[WordInputGrid2.Children.Count - 1]).GotFocus += TextBox_GotFocus2;
                        MenuItem m = new MenuItem() { Header = "Удалить" };
                        m.Click += MenuItem_Click2;
                        ((TextBox)WordInputGrid2.Children[WordInputGrid2.Children.Count - 1]).ContextMenu = new ContextMenu() { Items = { m } };
                    }
                }
                else
                {
                    AddNewContext(WordInputGrid2, TranslateInputGrid2, QuestionVoiceInputGrid2);
                    ((TextBox)WordInputGrid2.Children[WordInputGrid2.Children.Count - 1]).TextChanged += TextBox_TextChanged2;
                    ((TextBox)WordInputGrid2.Children[WordInputGrid2.Children.Count - 1]).GotFocus += TextBox_GotFocus2;
                    MenuItem m = new MenuItem() { Header = "Удалить" };
                    m.Click += MenuItem_Click2;
                    ((TextBox)WordInputGrid2.Children[WordInputGrid2.Children.Count - 1]).ContextMenu = new ContextMenu() { Items = { m } };
                }
            }

            if (SentenceVoiceInputGrid2.Children.Count == 0)
            {
                SentenceVoiceInputGrid2.Children.Add(new TextBox
                {
                    FontFamily = new FontFamily("Times New Roman"),
                    FontSize = 30,
                    FontWeight = FontWeights.Bold,
                    BorderThickness = new Thickness(0),
                    Foreground = Brushes.Black,
                    Background = background,
                    AllowDrop = false
                });

                SentenceVoiceInputGrid2.ColumnDefinitions.Add(new ColumnDefinition()
                {
                    MinWidth = 30,
                    Width = GridLength.Auto,
                });

                Grid.SetColumn(SentenceVoiceInputGrid2.Children[0], 0);
            }
        }

        private void UpButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //UpButton.Height = UpButton.Width = 20;

            if (MainTopic.Sentences.Count > 1)
            {
                UpdateSentence();

                if (indexOfElement == 0)
                {
                    indexOfElement = MainTopic.Sentences.Count - 1;
                    CreateSentence(MainTopic, indexOfElement);
                }
                else
                {
                    CreateSentence(MainTopic, --indexOfElement);
                }
            }
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = $"SELECT Id, Questions, Contexts, Translations, WaysToQuestionsVoice, WayToSentenceVoice, IsQuestionFirst FROM MainTopic";
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        MainTopic.Sentences.Add(new Sentence()
                        {
                            id = reader.GetInt32(0).ToString(),
                            questions = ReadSequence(reader.GetString(1)),
                            contexts = ReadSequence(reader.GetString(2)),
                            translations = ReadSequence(reader.GetString(3)),
                            waysToQuestionsVoice = ReadSequence(reader.GetString(4)),
                            wayToSentenceVoice = reader.GetString(5),
                            isQuestionFirst = bool.Parse(reader.GetString(6)),
                        });
                    }
                }
            }

            indexOfElement = 0;
            if (MainTopic.Sentences.Count > 0)
                CreateSentence(MainTopic, 0);
        }

        static List<string> ReadSequence(string s)
        {
            List<string> SequenceOfIndices = new List<string>();
            int startIndex = 0, length = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '~')
                {
                    SequenceOfIndices.Add(s.Substring(startIndex, length));
                    startIndex = i + 1;//прибавляем 1 к startIndex
                    length = 0;//обнуляем длину
                }
                else length++;//иначе увеличиваем на 1 длину строки
            }
            return SequenceOfIndices;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UpdateSentence();
        }
    }
}
