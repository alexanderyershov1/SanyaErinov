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

        BinaryFormatter formatter = new BinaryFormatter();//для сериализации
        static string Debug = Directory.GetCurrentDirectory(),//путь к папке Debug
        Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();//путь к папке Shebist

        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=
        {Shebist}\UserDB.mdf;Integrated Security=True",//Строка подключения к базе данных
        english,//правильное английское слово
        path,//путь к файлу озвучки
        topicId;//название таблицы слов
        int userid,//id пользователя
        index = 0,//индекс элемента в массивах russianArray, descriptionArray, englishArray, pathArray
        numberofword = 1;//номер слова из тех же массивов по порядку
        SqlCommand command = new SqlCommand();//Создание запросов к бд
        SqlDataReader reader;//Чтение данных из бд
        string[] russianArray,//русские слова
        descriptionArray,//описания
        englishArray,//английские слова
        pathArray;//пути
        Random rand = new Random();//генерация рандомных чисел
        List<int> indiciesForSave = new List<int>();//текущее расположение индексов в массивах для сохранения в бд

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
                command.CommandText = $"UPDATE UserDB SET ChoiceOfTopicElementsVisibility = '{ChoiceOfTopicTextBox.Visibility.ToString()}'," +
                        $"EnteringAWordElementsVisibility = '{EnteringAWordGrid.Visibility.ToString()}'," +
                        $"Indicies = '{indicies}'," +
                        $"TopicId = '{topicId}', MassivIndex = {index} WHERE Id = {userid}";
                //Обновляем таблицу UserDB, Видимость элементов группы выбора темы = ChoiceOfTopicTextBox.Visibility.ToString(),
                //Видимость элементов группы ввода слова = EnteringAWordTextBox.Visibility.ToString(),
                //Индексы = indicies, Id темы = topicId
                command.ExecuteNonQuery();
            }
        }

        //При загрузке страницы
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //десериализуем id пользователя
            if (File.Exists($"{Debug}\\Data\\userid"))
            {
                using (FileStream fs = new FileStream($"{Debug}\\Data\\userid", FileMode.OpenOrCreate))
                {
                    userid = (int)formatter.Deserialize(fs);
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = $"SELECT Name, ChoiceOfTopicElementsVisibility," +
                    $"EnteringAWordElementsVisibility," +
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
                    for(int i = 0; i < AccountMenuItem.Header.ToString().Length; i++)
                    {
                        AccountMenuItem.Width += 9;
                    }
                    //Устанавливаем свойству AccountMenuItem.Header значение Name из бд
                    //Узнаём, видимы ли элементы группы выбора темы
                    if (reader.GetString(1) == "Visible")//если видимы, меняем свойство Visibility на Visible
                    {
                        ChoiceOfTopicTextBox.Visibility = ChoiceOfTopicLabel.Visibility =
                        ChoiceOfTopicLine.Visibility = MainWordsButton.Visibility = Visibility.Visible;
                    }

                    else//Если нет, то на Hidden
                    {
                        ChoiceOfTopicTextBox.Visibility = ChoiceOfTopicLabel.Visibility = 
                        ChoiceOfTopicLine.Visibility = MainWordsButton.Visibility = Visibility.Hidden;
                    }
                       
                    if (reader.GetString(2) == "Visible")//Аналогично с элементами группы ввода слова
                        EnteringAWordGrid.Visibility = Visibility.Visible;
                    else
                        EnteringAWordGrid.Visibility = Visibility.Hidden;

                    topicId = reader.GetString(3).Trim();//получаем название таблицы 
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
                        //создаём массивы russianArray, descriptionArray, englishArray, pathArray с длиной, равной количеству
                        //элементов в списке indiciesForSave
                        russianArray = new string[indiciesForSave.Count];
                        descriptionArray = new string[indiciesForSave.Count];
                        englishArray = new string[indiciesForSave.Count];
                        pathArray = new string[indiciesForSave.Count];
                        //получаем текущий индекс слова в массивах из бд
                        index = reader.GetInt32(5);
                        //устанавливаем номер слова, прибавляя к индексу 1
                        numberofword = index + 1;
                        //устанавливаем свойство SearchByNumberTextBox.Text равным numberofword, преобразованной к строке
                        SearchByNumberTextBox.Text = numberofword.ToString();
                        reader.Close();
                        command.CommandText = $"SELECT Russian, Description, English, Path FROM [{topicId}]";
                        //Выбираем русские слова, описания, английские слова, пути к озвучке из таблицы topicId
                        reader = command.ExecuteReader();
                        int elementIndex = 0;
                        while (reader.Read())
                        {//Заполняем массивы в той последовательности, в которой мы последний раз получили
                            russianArray[indiciesForSave.ElementAt(elementIndex)] = reader.GetString(0).Trim();
                            descriptionArray[indiciesForSave.ElementAt(elementIndex)] = reader.GetString(1).Trim();
                            englishArray[indiciesForSave.ElementAt(elementIndex)] = reader.GetString(2).Trim();
                            pathArray[indiciesForSave.ElementAt(elementIndex++)] = reader.GetString(3).Trim();
                        }
                        reader.Close();
                        //Уставливаем свойства и переменные значениями из массивов по индексу, сохранившемуся при выходе
                        WordOutputLabel.Content = russianArray[index];
                        DescriptionLabel.Content = descriptionArray[index];
                        english = englishArray[index];
                        path = pathArray[index];
                    }
                }
            }
        }

        //включена ли озвучка
        bool isSoundEnabled = true;

        private void EnteringAWordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                EnteringAWordTextBox.Clear();
                WordOutputLabel.Content = englishArray[index];
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
            }
        }

        MediaPlayer player = new MediaPlayer();

        private void NextButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            NextButton.Width = NextButton.Height = 30;
            if (numberofword < russianArray.Length)
            {
                numberofword++;
                WordOutputLabel.Content = russianArray[++index];
                DescriptionLabel.Content = descriptionArray[index];
                english = englishArray[index];
                path = pathArray[index];
                SearchByNumberTextBox.Text = numberofword.ToString();
            }
            else if (numberofword == russianArray.Length)
            {
                index = 0;
                numberofword = 1;
                WordOutputLabel.Content = russianArray[0];
                DescriptionLabel.Content = descriptionArray[0];
                english = englishArray[0];
                path = pathArray[0];
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
            //Обнуляем индекс, а номер слова ставим 1,
            //В WordOutputLabel.Content выводим первое значение из массива русских слов,
            //DescriptionLabel.Content первое значение из массива описаний,
            //english первое значение из английских слов, path первое значение из массива путей
            index = 0;
            numberofword = 1;
            WordOutputLabel.Content = russianArray[0];
            DescriptionLabel.Content = descriptionArray[0];
            english = englishArray[0];
            path = pathArray[0];
            SearchByNumberTextBox.Text = "1";
        }

        private void ToTheChoiceOfTopicButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChoiceOfTopicLabel.Visibility = ChoiceOfTopicTextBox.Visibility = ChoiceOfTopicLine.Visibility = MainWordsButton.Visibility = Visibility.Visible;
            EnteringAWordGrid.Visibility = Visibility.Hidden;
            index = 0;
            indiciesForSave.Clear();
        }

        //При нажатии на MixButton
        private void MixButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Высота и ширина MixButton = 30
            MixButton.Width = MixButton.Height = 30;
            //Очищаем indiciesForSave
            indiciesForSave.Clear();
            //создаём список чисел indicies с длиной russianArray.Length
            List<int> indices = new List<int>(russianArray.Length);
            //Заполняем его от 0 до russianArray.Length - 1
            for (int i = 0; i < russianArray.Length; i++)
            {
                indices.Add(i);
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.CommandText = $"SELECT Russian, Description, English, Path FROM [{topicId}]";
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
                        russianArray[randomIndex] = reader.GetString(0).Trim();
                        descriptionArray[randomIndex] = reader.GetString(1).Trim();
                        englishArray[randomIndex] = reader.GetString(2).Trim();
                        pathArray[randomIndex] = Debug + "\\MainWordsSounds" + reader.GetString(3).Trim();
                        indices.RemoveAt(element);
                    }
                    reader.Close();
                }
                WordOutputLabel.Content = russianArray[index];
                DescriptionLabel.Content = descriptionArray[index];
                english = englishArray[index];
                path = pathArray[index];
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
            if (numberofword == 1)
            {
                index = russianArray.Length - 1;
                numberofword = russianArray.Length;
                WordOutputLabel.Content = russianArray[index];
                DescriptionLabel.Content = descriptionArray[index];
                english = englishArray[index];
                path = pathArray[index];
                SearchByNumberTextBox.Text = numberofword.ToString();
            }
            else if (WordOutputLabel.Content.ToString() == "Выполнено")
            {
                WordOutputLabel.Content = russianArray[index];
                DescriptionLabel.Content = descriptionArray[index];
                english = englishArray[index];
                path = pathArray[index];

                DescriptionLabel.Visibility = Visibility.Visible;
                SearchByNumberTextBox.Text = numberofword.ToString();
            }
            else
            {
                numberofword--;
                WordOutputLabel.Content = russianArray[--index];
                DescriptionLabel.Content = descriptionArray[index];
                english = englishArray[index];
                path = pathArray[index];
                SearchByNumberTextBox.Text = numberofword.ToString();
            }
        }

        private void MainWordsButton_Click(object sender, RoutedEventArgs e)
        {
            topicId = "MainWords";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.CommandText = $"SELECT COUNT(*) FROM MainWords";
                command.Connection = connection;
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    russianArray = new string[reader.GetInt32(0)];
                    descriptionArray = new string[reader.GetInt32(0)];
                    englishArray = new string[reader.GetInt32(0)];
                    pathArray = new string[reader.GetInt32(0)];
                    indiciesForSave = new List<int>(reader.GetInt32(0));
                    reader.Close();

                    command.CommandText = $"SELECT Russian, Description, English, Path FROM MainWords";
                    reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {

                        while (reader.Read())
                        {
                            indiciesForSave.Add(index);
                            russianArray[index] = reader.GetString(0).Trim();
                            descriptionArray[index] = reader.GetString(1).Trim();
                            englishArray[index] = reader.GetString(2).Trim();
                            pathArray[index++] = Debug + "\\MainWordsSounds" + reader.GetString(3).Trim();

                        }
                        reader.Close();
                    }

                    ChoiceOfTopicLabel.Visibility = ChoiceOfTopicTextBox.Visibility
                        = ChoiceOfTopicLine.Visibility = MainWordsButton.Visibility = Visibility.Hidden;

                    EnteringAWordGrid.Visibility = Visibility.Visible;


                    index = 0;
                    numberofword = 1;
                    WordsCounterLabel.Content = "/" + russianArray.Length;
                    WordOutputLabel.Content = russianArray[0];
                    DescriptionLabel.Content = descriptionArray[0];
                    english = englishArray[0];
                    path = pathArray[0];
                    SearchByNumberTextBox.Text = "1";
                }
            }
        }

        //Нажатие Enter в поле выбора темы

        private void ChoiceOfTopicTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    //Выбираем из таблицы Topics Id, где UserId = userid и Name = ChoiceOfTopicTextBox.Text
                    command.CommandText = $"SELECT Id FROM Topics WHERE UserId = {userid} AND Name = N'{ChoiceOfTopicTextBox.Text}'";
                    command.Connection = connection;
                    reader = command.ExecuteReader();
                    //Если есть данные
                    if (reader.HasRows)
                    {
                        reader.Read();
                        //Присваиваем переменной topicId значение из запроса
                        topicId = reader.GetInt32(0).ToString();
                        reader.Close();
                        //Выбираем количество строк в таблице topicId
                        command.CommandText = $"SELECT COUNT(*) FROM [{topicId}]";
                        reader = command.ExecuteReader();
                        //Если есть данные
                        if (reader.HasRows)
                        {
                            reader.Read();
                            //Создаём массивы с длиной, равной количеству строк в таблице topicId
                            russianArray = new string[reader.GetInt32(0)];
                            descriptionArray = new string[reader.GetInt32(0)];
                            englishArray = new string[reader.GetInt32(0)];
                            pathArray = new string[reader.GetInt32(0)];
                            reader.Close();
                            //Выбираем русские слова, описания, английские слова и пути из таблицы topicId
                            command.CommandText = $"SELECT Russian, Description, English, Path FROM [{topicId}]";
                            reader = command.ExecuteReader();
                            //Если есть данные
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    //Заполняем массивы данными из бд
                                    russianArray[index] = reader.GetString(0).Trim();
                                    descriptionArray[index] = reader.GetString(1).Trim();
                                    englishArray[index] = reader.GetString(2).Trim();
                                    pathArray[index++] = reader.GetString(3).Trim();
                                }
                                reader.Close();
                                ChoiceOfTopicTextBox.Clear();
                                ChoiceOfTopicLabel.Visibility = ChoiceOfTopicTextBox.Visibility
                                = ChoiceOfTopicLine.Visibility = MainWordsButton.Visibility = Visibility.Hidden;

                                EnteringAWordGrid.Visibility = Visibility.Visible;

                                index = 0;
                                numberofword = 1;
                                WordsCounterLabel.Content = "/" + russianArray.Length;
                                WordOutputLabel.Content = russianArray[0];
                                DescriptionLabel.Content = descriptionArray[0];
                                english = englishArray[0];
                                path = pathArray[0];
                                SearchByNumberTextBox.Text = "1";
                            }
                            else
                            {
                                ChoiceOfTopicTextBox.Clear();
                                return;
                            }
                        }
                    }
                    else
                    {
                        ChoiceOfTopicTextBox.Clear();
                        return;
                    }
                }
            }
        }

        private void TopicEditorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TopicEditorPage tpe = new TopicEditorPage(userid);
            this.NavigationService.Navigate(tpe);
            SaveUserState();
        }

        private void SettingsMenuItem_Click(object sender, EventArgs e)
        {
            SettingsWindow sw = new SettingsWindow(MainGrid);
            sw.Show();
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
            AccountPage ap = new AccountPage(userid);
            this.NavigationService.Navigate(ap);
            SaveUserState();
        }

        //при изменении текста EnteringAWordTextBox 
        private void EnteringAWordTextBox_TextChanged(object sender, EventArgs e)
        {
            if (EnteringAWordTextBox.Text == englishArray[index])
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
                EnteringAWordTextBox.Clear();

                if (numberofword < russianArray.Length)
                {
                    numberofword++;
                    WordOutputLabel.Content = russianArray[++index];
                    DescriptionLabel.Content = descriptionArray[index];
                    english = englishArray[index];
                    path = pathArray[index];
                    SearchByNumberTextBox.Text = numberofword.ToString();
                }
                else
                {
                    WordOutputLabel.Content = "Выполнено";
                    DescriptionLabel.Content = "";
                    SearchByNumberTextBox.Text = numberofword.ToString();
                }
            }
        }

        //переход к слову по номеру
        private void SearchByNumberTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    numberofword = Int32.Parse(SearchByNumberTextBox.Text);
                    if (numberofword >= 1 && numberofword <= russianArray.Length)
                    {
                        index = numberofword - 1;
                        WordOutputLabel.Content = russianArray[index];
                        DescriptionLabel.Content = descriptionArray[index];
                        english = englishArray[index];
                        path = pathArray[index];
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

