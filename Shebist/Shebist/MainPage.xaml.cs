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
            NextButton.Visibility = BackButton.Visibility = SearchByNumberTextBox.Visibility =
                WordOutputLabel.Visibility = WordsCounterLabel.Visibility = EnteringAWordTextBox.Visibility =
                StartButton.Visibility = ToTheChoiceOfTopicButton.Visibility = DescriptionLabel.Visibility
                = MixButton.Visibility = EnteringAWordLine.Visibility =  Visibility.Hidden;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        static string Debug = Directory.GetCurrentDirectory(),
        Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();

        public string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=
        {Shebist}\UserDB.mdf;Integrated Security=True", english, path, Section = "", topicId;
        public int userid, index = 0, numberofword = 1;
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        string[] russianArray, descriptionArray, englishArray, pathArray;
        Random rand = new Random();

        void SaveUserState()
        {
            string indicies = "";
            if(indiciesForSave.Count != 0)
            {
                for(int i = 0; i < indiciesForSave.Count; i++)
                {
                    indicies += indiciesForSave.ElementAt(i) + ";";
                }
            }
           using(SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = $"UPDATE UserState SET ChoiceOfTopicElementsVisibility = '{ChoiceOfTopicTextBox.Visibility.ToString()}'," +
                        $"EnteringAWordElementsVisibility = '{EnteringAWordTextBox.Visibility.ToString()}'," +
                        $"Indicies = '{indicies}'," +
                        $"TopicId = '{topicId}', MassivIndex = {index} WHERE Id = {userid}";
                command.ExecuteNonQuery();
            }
        }
        
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
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
                command.CommandText = $"SELECT Name FROM UserDB WHERE Id = {userid}";
                command.Connection = connection;
                reader = command.ExecuteReader();

                reader.Read();
                AccountMenuItem.Header = reader.GetValue(0);
                reader.Close();

                reader.Close();

                command.CommandText = $"SELECT ChoiceOfTopicElementsVisibility," +
                    $"EnteringAWordElementsVisibility," +
                    $"TopicId," +
                    $"Indicies," +
                    $"MassivIndex FROM UserState WHERE Id = {userid}";
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    if(reader.GetString(0) == "Visible")
                        ChoiceOfTopicTextBox.Visibility = ChoiceOfTopicLabel.Visibility =
                        ChoiceOfTopicLine.Visibility = MainWordsButton.Visibility = Visibility.Visible;                    
                    else ChoiceOfTopicTextBox.Visibility = ChoiceOfTopicLabel.Visibility =
                        ChoiceOfTopicLine.Visibility = MainWordsButton.Visibility = Visibility.Hidden;

                    if (reader.GetString(1) == "Visible")
                        SearchByNumberTextBox.Visibility = WordsCounterLabel.Visibility =
                        WordOutputLabel.Visibility = EnteringAWordLine.Visibility =
                        EnteringAWordTextBox.Visibility = NextButton.Visibility = BackButton.Visibility
                        = DescriptionLabel.Visibility = ToTheChoiceOfTopicButton.Visibility =
                        StartButton.Visibility = MixButton.Visibility = Visibility.Visible;
                    else
                        SearchByNumberTextBox.Visibility = WordsCounterLabel.Visibility =
                        WordOutputLabel.Visibility = EnteringAWordLine.Visibility =
                        EnteringAWordTextBox.Visibility = NextButton.Visibility = BackButton.Visibility
                        = DescriptionLabel.Visibility = ToTheChoiceOfTopicButton.Visibility =
                        StartButton.Visibility = MixButton.Visibility = Visibility.Hidden;

                    topicId = reader.GetString(2).Trim();
                    int startIndex = 0, length = 0;
                    indiciesForSave.Clear();
                    string indiciesLength = reader.GetString(3).Trim();
                    if (indiciesLength != "")
                    {
                        for (int i = 0; i < reader.GetString(3).Trim().Length; i++)
                        {
                            if (reader.GetString(3).Trim().ElementAt(i) == ';')
                            {
                                indiciesForSave.Add(Int32.Parse(reader.GetString(3).Trim().Substring(startIndex, length)));
                                startIndex = i + 1;
                                length = 0;
                            }
                            else length++;
                        }
                        WordsCounterLabel.Content = "/" + indiciesForSave.Count;
                        russianArray = new string[indiciesForSave.Count];
                        descriptionArray = new string[indiciesForSave.Count];
                        englishArray = new string[indiciesForSave.Count];
                        pathArray = new string[indiciesForSave.Count];
                        index =  reader.GetInt32(4);
                        numberofword = index + 1;
                        SearchByNumberTextBox.Text = numberofword.ToString();
                        reader.Close();
                        command.CommandText = $"SELECT Russian, Description, English, Path FROM [{topicId}]";
                        reader = command.ExecuteReader();
                        int elementIndex = 0;
                        while (reader.Read())
                        {
                            russianArray[indiciesForSave.ElementAt(elementIndex)] = reader.GetString(0).Trim();
                            descriptionArray[indiciesForSave.ElementAt(elementIndex)] = reader.GetString(1).Trim();
                            englishArray[indiciesForSave.ElementAt(elementIndex)] = reader.GetString(2).Trim();
                            pathArray[indiciesForSave.ElementAt(elementIndex++)] = reader.GetString(3).Trim();
                        }
                        reader.Close();
                        WordOutputLabel.Content = russianArray[index];
                        DescriptionLabel.Content = descriptionArray[index];
                        english = englishArray[index];
                        path = pathArray[index];
                    }
                }
            }
        }

        bool isWordsCounterLabelVisible = true,
        isSearchByNumberTextBoxVisible = true,
        isNextBackButtonsVisible = true,
        isSoundEnabled = true;

        private void MixButton_Click(object sender, RoutedEventArgs e)
        {
            indiciesForSave.Clear();
            List<int> indices = new List<int>(russianArray.Length);
            for (int i = 0; i < russianArray.Length; i++)
            {
                indices.Add(i);
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.CommandText = $"SELECT Russian, Description, English, Path FROM [{topicId}] ORDER BY Id";
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
                }
            }
        }

        MediaPlayer player = new MediaPlayer();

        private void NextButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
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

        private void StartButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            EnteringAWordTextBox.Visibility = DescriptionLabel.Visibility = Visibility.Visible;
            StartButton.Source = new BitmapImage(new Uri(Debug + "\\again.png"));

            if (isSearchByNumberTextBoxVisible)
            {
                SearchByNumberTextBox.Visibility = Visibility.Visible;
            }

            WordOutputLabel.Visibility = Visibility.Visible;

            if (isWordsCounterLabelVisible)
            {
                WordsCounterLabel.Visibility = Visibility.Visible;
            }

            if (isNextBackButtonsVisible)
            {
                NextButton.Visibility = BackButton.Visibility = Visibility.Visible;
            }

            index = 0;
            numberofword = 1;
            WordsCounterLabel.Content = "/" + russianArray.Length;
            WordOutputLabel.Content = russianArray[0];
            DescriptionLabel.Content = descriptionArray[0];
            english = englishArray[0];
            path = pathArray[0];
            SearchByNumberTextBox.Text = "1";
        }

        private void ToTheChoiceOfTopicButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ChoiceOfTopicLabel.Visibility = ChoiceOfTopicTextBox.Visibility = ChoiceOfTopicLine.Visibility = MainWordsButton.Visibility = Visibility.Visible;

            StartButton.Visibility = ToTheChoiceOfTopicButton.Visibility =
            BackButton.Visibility = NextButton.Visibility =
            EnteringAWordTextBox.Visibility = WordOutputLabel.Visibility =
            SearchByNumberTextBox.Visibility
            = DescriptionLabel.Visibility = WordsCounterLabel.Visibility = MixButton.Visibility = EnteringAWordLine.Visibility =  Visibility.Hidden;
            index = 0;
            indiciesForSave.Clear();
        }

        private void MixButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            indiciesForSave.Clear();
            List<int> indices = new List<int>(russianArray.Length);
            for (int i = 0; i < russianArray.Length; i++)
            {
                indices.Add(i);
            }
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.CommandText = $"SELECT Russian, Description, English, Path FROM [{topicId}] ORDER BY Id";
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

        private void BackButton_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
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

        List<int> indiciesForSave = new List<int>();
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

                    StartButton.Visibility = EnteringAWordLine.Visibility =
                    MixButton.Visibility = ToTheChoiceOfTopicButton.Visibility =
                    EnteringAWordTextBox.Visibility = EnteringAWordLine.Visibility =
                    DescriptionLabel.Visibility = Visibility.Visible;

                    if (isSearchByNumberTextBoxVisible)
                    {
                        SearchByNumberTextBox.Visibility = Visibility.Visible;
                    }

                    WordOutputLabel.Visibility = Visibility.Visible;

                    if (isWordsCounterLabelVisible)
                    {
                        WordsCounterLabel.Visibility = Visibility.Visible;
                    }

                    if (isNextBackButtonsVisible)
                    {
                        NextButton.Visibility = BackButton.Visibility = Visibility.Visible;
                    }

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
                Section = ChoiceOfTopicTextBox.Text;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.CommandText = $"SELECT Id FROM Topics WHERE UserId = {userid} AND Name = N'{ChoiceOfTopicTextBox.Text}'";
                    command.Connection = connection;
                    reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        topicId = reader.GetInt32(0).ToString();
                        reader.Close();

                        command.CommandText = $"SELECT COUNT(*) FROM [{topicId}]";
                        reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            reader.Read();
                            russianArray = new string[reader.GetInt32(0)];
                            descriptionArray = new string[reader.GetInt32(0)];
                            englishArray = new string[reader.GetInt32(0)];
                            pathArray = new string[reader.GetInt32(0)];
                            reader.Close();
                        }

                        command.CommandText = $"SELECT Russian, Description, English, Path FROM [{topicId}]";
                        reader = command.ExecuteReader();
                        if (reader.HasRows)
                        {

                            while (reader.Read())
                            {
                                russianArray[index] = reader.GetString(0).Trim();
                                descriptionArray[index] = reader.GetString(1).Trim();
                                englishArray[index] = reader.GetString(2).Trim();
                                pathArray[index++] = reader.GetString(3).Trim();
                            }
                            reader.Close();
                        }



                        ChoiceOfTopicLabel.Visibility = ChoiceOfTopicTextBox.Visibility
                        = ChoiceOfTopicLine.Visibility = MainWordsButton.Visibility =  Visibility.Hidden;
 
                        ChoiceOfTopicTextBox.Clear();
                        StartButton.Visibility = EnteringAWordLine.Visibility =
                        MixButton.Visibility = ToTheChoiceOfTopicButton.Visibility = EnteringAWordTextBox.Visibility = DescriptionLabel.Visibility = Visibility.Visible;
                        StartButton.Source = new BitmapImage(new Uri(Debug + "\\again.png"));

                        if (isSearchByNumberTextBoxVisible)
                        {
                            SearchByNumberTextBox.Visibility = Visibility.Visible;
                        }

                        WordOutputLabel.Visibility = Visibility.Visible;

                        if (isWordsCounterLabelVisible)
                        {
                            WordsCounterLabel.Visibility = Visibility.Visible;
                        }

                        if (isNextBackButtonsVisible)
                        {
                            NextButton.Visibility = BackButton.Visibility = Visibility.Visible;
                        }

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
        }


        private void TopicEditorMenuItem_Click(object sender, RoutedEventArgs e)
        {
            TopicEditorPage tpe = new TopicEditorPage(userid);
            this.NavigationService.Navigate(tpe);
            SaveUserState();
        }

        private void SettingsMenuItem_Click(object sender, EventArgs e)
        {
            SettingsWindow sw = new SettingsWindow(this);
            sw.Show();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            EnteringAWordTextBox.Visibility = DescriptionLabel.Visibility = Visibility.Visible;
            StartButton.Source = new BitmapImage(new Uri(Debug + "\\again.png"));

            if (isSearchByNumberTextBoxVisible)
            {
                SearchByNumberTextBox.Visibility = Visibility.Visible;
            }

            WordOutputLabel.Visibility = Visibility.Visible;

            if (isWordsCounterLabelVisible)
            {
                WordsCounterLabel.Visibility = Visibility.Visible;
            }

            if (isNextBackButtonsVisible)
            {
                NextButton.Visibility = BackButton.Visibility = Visibility.Visible;
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

        //клик но кнопке ToTheChoiceOfTopicButton
        private void ToTheChoiceOfTopicButton_Click(object sender, EventArgs e)
        {
            ChoiceOfTopicLabel.Visibility = ChoiceOfTopicTextBox.Visibility = ChoiceOfTopicLine.Visibility = MainWordsButton.Visibility = Visibility.Visible;

            StartButton.Visibility = ToTheChoiceOfTopicButton.Visibility =
            BackButton.Visibility = NextButton.Visibility =
            EnteringAWordTextBox.Visibility = WordOutputLabel.Visibility =
            SearchByNumberTextBox.Visibility
            = DescriptionLabel.Visibility = WordsCounterLabel.Visibility = MixButton.Visibility = Visibility.Hidden;
            index = 0;
            indiciesForSave.Clear();
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

