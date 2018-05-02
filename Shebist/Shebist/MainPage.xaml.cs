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
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            Menu.Opacity = 0;
            EnteringAWordGrid.Visibility = Visibility.Hidden;
        }

        public List<Topic>  topics = new List<Topic>();
        BinaryFormatter formatter = new BinaryFormatter();//для сериализации
        static string Debug = Directory.GetCurrentDirectory(),//путь к папке Debug
        Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();//путь к папке Shebist

        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=
        {Shebist}\UserDB.mdf;Integrated Security=True",//Строка подключения к базе данных
        english,//правильное английское слово
        path;//путь к файлу озвучки
        public int userid;//id пользователя
        int index = 0,//индекс элемента в массивах russianArray, descriptionArray, englishArray, pathArray
        numberofword = 1;//номер слова из тех же массивов по порядку
        SqlCommand command = new SqlCommand();//Создание запросов к бд
        SqlDataReader reader;//Чтение данных из бд
        Random rand = new Random();//генерация рандомных чисел
        List<int> indiciesForSave = new List<int>();//текущее расположение индексов в массивах для сохранения в бд
        List<Word> words = new List<Word>();//список слов

        /// <summary>
        /// Сохранение статуса пользователя
        /// </summary>
        void SaveUserState()
        {
            string indicies = "";//порядок индексов массивов
            if (indiciesForSave.Count != 0)//если список indiciesForSave не пуст
            {
                for (int i = 0; i < indiciesForSave.Count; i++)//от 0 до последнего элемента списка
                {
                    indicies += indiciesForSave.ElementAt(i) + ";";//добавляем к indicies элемент и ;
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))//открываем подключение
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = $"UPDATE UserDB SET ChoiceOfTopicGridVisibility = '{ExistingTopicsDataGrid.Visibility.ToString()}'," +
                        $"EnteringAWordGridVisibility = '{EnteringAWordGrid.Visibility.ToString()}'," +
                        $"Indicies = '{indicies}'," +
                        $"TopicId = '{currentTopic.Id}', MassivIndex = {index} WHERE Id = {userid}";
                //Обновляем таблицу UserDB, Видимость элементов группы выбора темы = ChoiceOfTopicTextBox.Visibility.ToString(),
                //Видимость элементов группы ввода слова = EnteringAWordTextBox.Visibility.ToString(),
                //Индексы = indicies, Id темы = topicId
                command.ExecuteNonQuery();
            }
        }

        //При загрузке страницы
        bool downloadFromDB = true;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (downloadFromDB)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.Connection = connection;
                    command.CommandText = $"SELECT Id, Name FROM Topics WHERE UserID = {userid}";
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ExistingTopicsDataGrid.Items.Add(new Topic { Id = reader.GetInt32(0).ToString(),
                                Name = reader.GetString(1).Trim()
                            });
                            topics.Add(new Topic
                            {
                                Id = reader.GetInt32(0).ToString(),
                                Name = reader.GetString(1).Trim()
                            });
                        }
                    }
                    reader.Close();
                    command.CommandText = $"SELECT Name, ChoiceOfTopicGridVisibility," +
                        $"EnteringAWordGridVisibility," +
                        $"TopicId," +
                        $"Indicies," +
                        $"MassivIndex FROM UserDB WHERE Id = {userid}";
                    reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        //Находим имя пользователя по id
                        AccountMenuItem.Header = reader.GetValue(0);
                        AccountMenuItem.Width = 20;
                        for (int i = 0; i < AccountMenuItem.Header.ToString().Length; i++)
                        {
                            AccountMenuItem.Width += 9;
                        }
                        //Устанавливаем свойству AccountMenuItem.Header значение Name из бд
                        //Узнаём, видимы ли элементы группы выбора темы
                        if (reader.GetString(1) == "Visible")//если видимы, меняем свойство Visibility на Visible
                        {
                            ChoiceOfTopicGrid.Visibility = Visibility.Visible;
                            EnteringAWordGrid.Visibility = Visibility.Hidden;
                        }

                        else//Если нет, то на Hidden
                        {
                            ChoiceOfTopicGrid.Visibility = Visibility.Hidden;
                            EnteringAWordGrid.Visibility = Visibility.Visible;
                        }

                        if (reader.GetString(2) == "Visible")//Аналогично с элементами группы ввода слова
                            EnteringAWordGrid.Visibility = Visibility.Visible;
                        else
                            EnteringAWordGrid.Visibility = Visibility.Hidden;

                        currentTopic.Id = reader.GetString(3).Trim();//получаем название таблицы 
                                                             //алгоритм для считывания порядка индексов элементов в массиве
                        int startIndex = 0, length = 0;//переменные для обозначения startIndex и length метода Substring
                        indiciesForSave.Clear();//Предварительно очищаем список indiciesForSave
                        string indiciesLength = reader.GetString(4).Trim();//получаем количество символов поля Indicies
                        if (indiciesLength != "")//если поле Indicies не пустое
                        {
                            //заполняем список indiciesForSave индексами из поля Indicies при помощи специального алгоритма
                            for (int i = 0; i < reader.GetString(4).Trim().Length; i++)
                            {
                                if (reader.GetString(4).Trim().ElementAt(i) == ';')
                                {//если символ равен ;, добавляем в список indiciesForSave элемент, начиная с startIndex длиной в length
                                    indiciesForSave.Add(Int32.Parse(reader.GetString(4).Trim().Substring(startIndex, length)));
                                    startIndex = i + 1;//прибавляем 1 к startIndex
                                    length = 0;//обнуляем длину
                                }
                                else length++;//иначе увеличиваем на 1 длину строки
                            }
                            WordsCounterLabel.Content = "/" + indiciesForSave.Count;
                            //устанавливаем свойство WordsCounterLabel.Content равным количеству элементов в списке indiciesForSave
                            //создаём список words с длиной, равной количеству
                            //элементов в списке indiciesForSave
                            currentTopic.words = new List<Word>(indiciesForSave.Count);
                            //получаем текущий индекс слова в массивах из бд
                            index = reader.GetInt32(5);
                            //устанавливаем номер слова, прибавляя к индексу 1
                            numberofword = index + 1;
                            //устанавливаем свойство SearchByNumberTextBox.Text равным numberofword, преобразованной к строке
                            SearchByNumberTextBox.Text = numberofword.ToString();
                            reader.Close();
                            command.CommandText = $"SELECT Russian, Description, English, Path FROM [{currentTopic.Id}]";
                            //Выбираем русские слова, описания, английские слова, пути к озвучке из таблицы topicId
                            reader = command.ExecuteReader();
                            int elementIndex = 0;

                            for (int i = 0; i < indiciesForSave.Count; i++)
                            {
                                currentTopic.words.Add(new Word());
                            }

                            while (reader.Read())
                            {//Заполняем список в той последовательности, в которой мы последний раз получили
                                currentTopic.words[indiciesForSave.ElementAt(elementIndex++)] = new Word
                                {
                                    Russian = reader.GetString(0).Trim(),
                                    Description = reader.GetString(1).Trim(),
                                    English = reader.GetString(2).Trim(),
                                    Path = reader.GetString(3).Trim()
                                };
                            }
                            reader.Close();
                            //Уставливаем свойства и переменные значениями из массивов по индексу, сохранившемуся при выходе
                            WordOutputLabel.Content = currentTopic.words[index].Russian;
                            DescriptionLabel.Content = currentTopic.words[index].Description;
                            english = currentTopic.words[index].English;
                            path = currentTopic.words[index].Path;
                        }
                    }
                }
            }
            else
            {
                foreach (Topic topic in topics)
                {
                    ExistingTopicsDataGrid.Items.Add(topic);
                }
            }
            
        }

        //включена ли озвучка
        bool isSoundEnabled = true;

        
        private void EnteringAWordTextBox_KeyDown(object sender, KeyEventArgs e)
        {  
            if (e.Key == Key.Enter)
            {
                if (EnteringAWordTextBox.Text.ToLower() == currentTopic.words[index].English.ToLower())
                {
                    if (isSoundEnabled)
                    {
                        if (File.Exists(path))
                        {
                            player.Open(new Uri(path, UriKind.Absolute));
                            player.Play();
                        }
                        else
                        {
                            player.Open(new Uri(Debug + "\\MainWordsSounds" + path, UriKind.Absolute));
                            player.Play();
                        }
                    }
                    CorrectAnswerTextBlock.Text = "";
                    EnteringAWordTextBox.Clear();

                    if (numberofword < currentTopic.words.Count)
                    {
                        numberofword++;
                        WordOutputLabel.Content = currentTopic.words[++index].Russian;
                        DescriptionLabel.Content = currentTopic.words[index].Description;
                        english = currentTopic.words[index].English;
                        path = currentTopic.words[index].Path;
                        SearchByNumberTextBox.Text = numberofword.ToString();
                    }
                    else
                    {
                        WordOutputLabel.Content = "Выполнено";
                        DescriptionLabel.Content = "";
                        SearchByNumberTextBox.Text = numberofword.ToString();
                    }
                }
                else
                {
                    CorrectAnswerTextBlock.Text = "";
                    List<int> indiciesOfMissedLetters = new List<int>();

                    int delta = currentTopic.words[index].English.Length - EnteringAWordTextBox.Text.Length;
                    if (delta == 0 || delta < 0)
                    {
                        List<int> indiciesOfRightLetters = new List<int>();

                        for (int i = 0; i < EnteringAWordTextBox.Text.Length; i++)
                        {
                            if (i < currentTopic.words[index].English.Length)
                            {
                                if (EnteringAWordTextBox.Text[i] == currentTopic.words[index].English[i])
                                    indiciesOfRightLetters.Add(i);
                            }
                        }


                        for (int i = 0; i < currentTopic.words[index].English.Length; i++)
                        {
                            if (indiciesOfRightLetters.Contains(i))
                            {
                                CorrectAnswerTextBlock.Inlines.Add(new Run
                                {
                                    FontSize = 17,
                                    Foreground = Brushes.Green,
                                    Text = currentTopic.words[index].English[i].ToString()
                                });
                            }
                                
                            else CorrectAnswerTextBlock.Inlines.Add(new Run
                            {
                                FontSize = 17,
                                Foreground = Brushes.Red,
                                Text = currentTopic.words[index].English[i].ToString()
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
                            if (i < currentTopic.words[index].English.Length)
                            {
                                if (EnteringAWordTextBox.Text[j] != currentTopic.words[index].English[i])
                                {
                                    indiciesOfMissedLetters.Add(i);
                                }
                                else j++;

                            }
                        }

                        if (indiciesOfMissedLetters.Count != 0)
                        {
                            for (int i = 0; i < currentTopic.words[index].English.Length; i++)
                            {
                                if (indiciesOfMissedLetters.Contains(i))
                                {   
                                    CorrectAnswerTextBlock.Inlines.Add(new Run
                                    {
                                        FontSize = 17,
                                        Foreground = Brushes.Red,
                                        Text = currentTopic.words[index].English[i].ToString()
                                    });
                                }
                                else
                                {
                                    CorrectAnswerTextBlock.Inlines.Add(new Run
                                    {
                                        FontSize = 17,
                                        Foreground = Brushes.Green,
                                        Text = currentTopic.words[index].English[i].ToString()
                                    });
                                }
                            }
                        }
                    }

                    if (isSoundEnabled)
                    {
                        if (File.Exists(path))
                        {
                            player.Open(new Uri(path, UriKind.Absolute));
                            player.Play();
                        }
                        else
                        {
                            player.Open(new Uri(Debug + "\\MainWordsSounds" + path, UriKind.Absolute));
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
            if (numberofword < currentTopic.words.Count)
            {
                numberofword++;
                WordOutputLabel.Content = currentTopic.words[++index].Russian;
                DescriptionLabel.Content = currentTopic.words[index].Description;
                english = currentTopic.words[index].English;
                path = currentTopic.words[index].Path;
                SearchByNumberTextBox.Text = numberofword.ToString();
            }
            else if (numberofword == currentTopic.words.Count)
            {
                index = 0;
                numberofword = 1;
                WordOutputLabel.Content = currentTopic.words[0].Russian;
                DescriptionLabel.Content = currentTopic.words[0].Description;
                english = currentTopic.words[0].English;
                path = currentTopic.words[0].Path;
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
            //english первое значение из английских слов, path первое значение из массива путей
            index = 0;
            numberofword = 1;
            WordOutputLabel.Content = currentTopic.words[0].Russian;
            DescriptionLabel.Content = currentTopic.words[0].Description;
            english = currentTopic.words[0].English;
            path = currentTopic.words[0].Path;
            SearchByNumberTextBox.Text = "1";
        }

        private void ToTheChoiceOfTopicButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChoiceOfTopicGrid.Visibility = Visibility.Visible;
            EnteringAWordGrid.Visibility = Visibility.Hidden;
            CorrectAnswerTextBlock.Text = "";
            index = 0;
            indiciesForSave.Clear();
            currentTopic.Id = null;
        }

        //При нажатии на MixButton
        private void MixButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Высота и ширина MixButton = 30
            MixButton.Width = MixButton.Height = 30;
            CorrectAnswerTextBlock.Text = "";
            //Очищаем indiciesForSave
            indiciesForSave.Clear();
            //создаём список чисел indicies с длиной russianArray.Length
            List<int> indices = new List<int>(currentTopic.words.Count);
            //Заполняем его от 0 до russianArray.Length - 1
            for (int i = 0; i < currentTopic.words.Count; i++)
            {
                indices.Add(i);
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.CommandText = $"SELECT Russian, Description, English, Path FROM [{currentTopic.Id}]";
                command.Connection = connection;
                reader = command.ExecuteReader();
                int randomIndex;
                int element;
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        element = rand.Next(0, indices.Count - 1);
                        randomIndex = indices.ElementAt(element);
                        indiciesForSave.Add(randomIndex);
                        currentTopic.words[randomIndex] = new Word
                        {
                            Russian = reader.GetString(0).Trim(),
                            Description = reader.GetString(1).Trim(),
                            English = reader.GetString(2).Trim(),
                            Path = reader.GetString(3).Trim()
                        };
                        indices.RemoveAt(element);
                    }
                    reader.Close();
                }
                WordOutputLabel.Content = currentTopic.words[index].Russian;
                DescriptionLabel.Content = currentTopic.words[index].Description;
                english = currentTopic.words[index].English;
                path = currentTopic.words[index].Path;
            }
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
                index = currentTopic.words.Count - 1;
                numberofword = currentTopic.words.Count;
                WordOutputLabel.Content = currentTopic.words[index].Russian;
                DescriptionLabel.Content = currentTopic.words[index].Description;
                english = currentTopic.words[index].English;
                path = currentTopic.words[index].Path;
                SearchByNumberTextBox.Text = numberofword.ToString();
            }
            else if (WordOutputLabel.Content.ToString() == "Выполнено")
            {
                WordOutputLabel.Content = currentTopic.words[index].Russian;
                DescriptionLabel.Content = currentTopic.words[index].Description;
                english = currentTopic.words[index].English;
                path = currentTopic.words[index].Path;

                DescriptionLabel.Visibility = Visibility.Visible;
                SearchByNumberTextBox.Text = numberofword.ToString();
            }
            else
            {
                numberofword--;
                WordOutputLabel.Content = currentTopic.words[--index].Russian;
                DescriptionLabel.Content = currentTopic.words[index].Description;
                english = currentTopic.words[index].English;
                path = currentTopic.words[index].Path;
                SearchByNumberTextBox.Text = numberofword.ToString();
            }
        }

        private void MainWordsButton_Click(object sender, RoutedEventArgs e)
        {
            currentTopic.Id = "MainWords";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.CommandText = $"SELECT COUNT(*) FROM MainWords";
                command.Connection = connection;
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    indiciesForSave = new List<int>(reader.GetInt32(0));
                    currentTopic.words = new List<Word>(reader.GetInt32(0));
                    reader.Close();


                    command.CommandText = $"SELECT Russian, Description, English, Path FROM MainWords";
                    reader = command.ExecuteReader();
                    index = 0;
                    if (reader.HasRows)
                    {

                        while (reader.Read())
                        {
                            currentTopic.words.Add(new Word
                            {
                                Russian = reader.GetString(0).Trim(),
                                Description = reader.GetString(1).Trim(),
                                English = reader.GetString(2).Trim(),
                                Path = Debug + "\\MainWordsSounds" + reader.GetString(3).Trim()
                            });
                            indiciesForSave.Add(index++);
                        }
                        reader.Close();
                    }

                    ChoiceOfTopicGrid.Visibility = Visibility.Hidden;

                    EnteringAWordGrid.Visibility = Visibility.Visible;


                    index = 0;
                    numberofword = 1;
                    WordsCounterLabel.Content = "/" + currentTopic.words.Count;
                    WordOutputLabel.Content = currentTopic.words[0].Russian;
                    DescriptionLabel.Content = currentTopic.words[0].Description;
                    english = currentTopic.words[0].English;
                    path = currentTopic.words[0].Path;
                    SearchByNumberTextBox.Text = "1";
                }
            }
        }
        
        private void TopicEditorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TopicEditorPage tpe = new TopicEditorPage { userid = userid, currentTopicId = currentTopic.Id };
            this.NavigationService.Navigate(tpe);
            SaveUserState();
        }

        private void SettingsMenuItem_Click(object sender, EventArgs e)
        {
            SettingsWindow sw = new SettingsWindow(MainGrid);
            sw.Show();
        }

        Topic currentTopic = new Topic();
        private void ExistingTopicsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Topic topic = (Topic)ExistingTopicsDataGrid.SelectedItem;
            if(topic != null)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.Connection = connection;
                    currentTopic.Id = topic.Id;
                    currentTopic.Name = topic.Name;
                    //Выбираем количество строк в таблице topicId
                    command.CommandText = $"SELECT COUNT(*) FROM [{currentTopic.Id}]";
                    reader = command.ExecuteReader();
                    //Если есть данные

                    reader.Read();
                    //Создаём массивы с длиной, равной количеству строк в таблице topicId
                    if(reader.GetInt32(0) > 0)
                    {
                        currentTopic.words = new List<Word>(reader.GetInt32(0));
                        
                        for (int i = 0; i < reader.GetInt32(0); i++)
                        {
                            currentTopic.words.Add(new Word());
                        }

                        indiciesForSave.Clear();

                        for (int i = 0; i < reader.GetInt32(0); i++)
                        {
                            indiciesForSave.Add(i);
                        }
                        reader.Close();

                        //Выбираем вопросы, подсказки, ответы и пути из таблицы topicId
                        command.CommandText = $"SELECT Russian, Description, English, Path FROM [{currentTopic.Id}]";
                        reader = command.ExecuteReader();
                        //Если есть данные
                        index = 0;
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                //Заполняем массивы данными из бд
                                currentTopic.words[index++] = new Word
                                {
                                    Russian = reader.GetString(0).Trim(),
                                    Description = reader.GetString(1).Trim(),
                                    English = reader.GetString(2).Trim(),
                                    Path = reader.GetString(3).Trim()
                                };
                            }
                            reader.Close();
                            ChoiceOfTopicGrid.Visibility = Visibility.Hidden;

                            EnteringAWordGrid.Visibility = Visibility.Visible;

                            index = 0;
                            numberofword = 1;
                            WordsCounterLabel.Content = "/" + currentTopic.words.Count;
                            WordOutputLabel.Content = currentTopic.words[0].Russian;
                            DescriptionLabel.Content = currentTopic.words[0].Description;
                            english = currentTopic.words[0].English;
                            path = currentTopic.words[0].Path;
                            SearchByNumberTextBox.Text = "1";
                        }
                    }
                }
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
            AccountPage ap = new AccountPage { userid = userid };
            this.NavigationService.Navigate(ap);
            SaveUserState();
        }

        //переход к слову по номеру
        private void SearchByNumberTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    numberofword = Int32.Parse(SearchByNumberTextBox.Text);
                    if (numberofword >= 1 && numberofword <= currentTopic.words.Count)
                    {
                        index = numberofword - 1;
                        WordOutputLabel.Content = currentTopic.words[index].Russian;
                        DescriptionLabel.Content = currentTopic.words[index].Description;
                        english = currentTopic.words[index].English;
                        path = currentTopic.words[index].Path;
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

