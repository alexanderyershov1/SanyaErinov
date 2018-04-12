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
                StartButton.Visibility = ToTheChoiceOfTopicButton.Visibility = DescriptionLabel.Visibility = Visibility.Hidden;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        static string Debug = Directory.GetCurrentDirectory(),
        Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();

        public string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=
        {Shebist}\UserDB.mdf;Integrated Security=True", english, path, Section = "", topicId;
        public int userid, index = 0, numberofword = 1, count = 0;
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
                command.CommandText = $"UPDATE UserState SET ChoiceOfTopicTextBoxVisibility = '{ChoiceOfTopicTextBox.Visibility.ToString()}'," +
                        $"ChoiceOfTopicLabelVisibility = '{ChoiceOfTopicLabel.Visibility.ToString()}'," +
                        $"MainWordsButtonVisibility = '{MainWordsButton.Visibility.ToString()}'," +
                        $"SearchByNumberTextBoxVisibility = '{SearchByNumberTextBox.Visibility.ToString()}'," +
                        $"WordsCounterLabelVisibility = '{WordsCounterLabel.Visibility.ToString()}'," +
                        $"WordsCounterLabelContent = N'{WordsCounterLabel.Content.ToString()}'," +
                        $"WordOutputLabelVisibility = '{WordOutputLabel.Visibility.ToString()}'," +
                        $"WordOutputLabelContent = N'{WordOutputLabel.Content.ToString()}'," +
                        $"DescriptionLabelVisibility = '{DescriptionLabel.Visibility.ToString()}'," +
                        $"DescriptionLabelContent = N'{DescriptionLabel.Content.ToString()}'," +
                        $"EnteringAWordTextBoxVisibility = '{EnteringAWordTextBox.Visibility.ToString()}'," +
                        $"BackNextButtonsVisibility = '{BackButton.Visibility.ToString()}'," +
                        $"MixButtonVisibility = '{MixButton.Visibility.ToString()}'," +
                        $"StartButtonVisibility = '{StartButton.Visibility.ToString()}'," +
                        $"ToTheChoiceOfTopicButtonVisibility = '{ToTheChoiceOfTopicButton.Visibility.ToString()}'," +
                        $"Indicies = '{indicies}'," +
                        $"TopicId = '{topicId}'";
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

                command.CommandText = $"SELECT " +
                    $"ChoiceOfTopicTextBoxVisibility," +
                        $"ChoiceOfTopicLabelVisibility," +
                        $"MainWordsButtonVisibility," +
                        $"SearchByNumberTextBoxVisibility," +
                        $"WordsCounterLabelVisibility," +
                        $"WordsCounterLabelContent," +
                        $"WordOutputLabelVisibility," +
                        $"WordOutputLabelContent," +
                        $"DescriptionLabelVisibility," +
                        $"DescriptionLabelContent," +
                        $"EnteringAWordTextBoxVisibility," +
                        $"BackNextButtonsVisibility," +
                        $"MixButtonVisibility," +
                        $"StartButtonVisibility," +
                        $"ToTheChoiceOfTopicButtonVisibility," +
                        $"TopicId," +
                        $"Indicies FROM UserState WHERE Id = {userid}";

                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    switch (reader.GetString(0))
                    {
                        case "Visible":
                            ChoiceOfTopicTextBox.Visibility = Visibility.Visible;
                            break;
                        case "Hidden":
                            ChoiceOfTopicTextBox.Visibility = Visibility.Hidden;
                            break;
                    }
                    switch (reader.GetString(1))
                    {
                        case "Visible":
                            ChoiceOfTopicLabel.Visibility = Visibility.Visible;
                            break;
                        case "Hidden":
                            ChoiceOfTopicLabel.Visibility = Visibility.Hidden;
                            break;
                    }
                    switch (reader.GetString(2))
                    {
                        case "Visible":
                            MainWordsButton.Visibility = Visibility.Visible;
                            break;
                        case "Hidden":
                            MainWordsButton.Visibility = Visibility.Hidden;
                            break;
                    }
                    switch (reader.GetString(3))
                    {
                        case "Visible":
                            SearchByNumberTextBox.Visibility = Visibility.Visible;
                            break;
                        case "Hidden":
                            SearchByNumberTextBox.Visibility = Visibility.Hidden;
                            break;
                    }
                    switch (reader.GetString(4))
                    {
                        case "Visible":
                            WordsCounterLabel.Visibility = Visibility.Visible;
                            break;
                        case "Hidden":
                            WordsCounterLabel.Visibility = Visibility.Hidden;
                            break;
                    }
                    WordsCounterLabel.Content = reader.GetString(5);
                    switch (reader.GetString(6))
                    {
                        case "Visible":
                            WordOutputLabel.Visibility = Visibility.Visible;
                            break;
                        case "Hidden":
                            WordOutputLabel.Visibility = Visibility.Hidden;
                            break;
                    }
                    WordOutputLabel.Content = reader.GetString(7);
                    switch (reader.GetString(8))
                    {
                        case "Visible":
                            DescriptionLabel.Visibility = Visibility.Visible;
                            break;
                        case "Hidden":
                            DescriptionLabel.Visibility = Visibility.Hidden;
                            break;
                    }
                    DescriptionLabel.Content = reader.GetString(9);
                    switch (reader.GetString(10))
                    {
                        case "Visible":
                            EnteringAWordTextBox.Visibility = Visibility.Visible;
                            break;
                        case "Hidden":
                            EnteringAWordTextBox.Visibility = Visibility.Hidden;
                            break;
                    }
                    switch (reader.GetString(11))
                    {
                        case "Visible":
                            BackButton.Visibility = NextButton.Visibility = Visibility.Visible;
                            break;
                        case "Hidden":
                            BackButton.Visibility = NextButton.Visibility = Visibility.Hidden;
                            break;
                    }
                    switch (reader.GetString(12))
                    {
                        case "Visible":
                            MixButton.Visibility = Visibility.Visible;
                            break;
                        case "Hidden":
                            MixButton.Visibility = Visibility.Hidden;
                            break;
                    }
                    switch (reader.GetString(13))
                    {
                        case "Visible":
                            StartButton.Visibility = Visibility.Visible;
                            break;
                        case "Hidden":
                            StartButton.Visibility = Visibility.Hidden;
                            break;
                    }
                    switch (reader.GetString(14))
                    {
                        case "Visible":
                            ToTheChoiceOfTopicButton.Visibility = Visibility.Visible;
                            break;
                        case "Hidden":
                            ToTheChoiceOfTopicButton.Visibility = Visibility.Hidden;
                            break;
                    }
                    topicId = reader.GetString(15);
                    int startIndex = 0, length = 0;
                    indiciesForSave.Clear();
                    for (int i = 0; i < reader.GetString(16).Trim().Length; i++)
                    {

                        if (reader.GetString(16).Trim().ElementAt(i) == ';')
                        {
                            indiciesForSave.Add(Int32.Parse(reader.GetString(16).Trim().Substring(startIndex, length)));
                            startIndex = i + 1;
                            length = 0;
                        }
                        else length++;
                    }
                    string str = "";
                    for(int i = 0; i < indiciesForSave.Count; i++)
                    {
                        str += indiciesForSave.ElementAt(i) + " ";
                    }
                    MessageBox.Show(str);
                    russianArray = new string[indiciesForSave.Count];
                    descriptionArray = new string[indiciesForSave.Count];
                    englishArray = new string[indiciesForSave.Count];
                    pathArray = new string[indiciesForSave.Count];
                    reader.Close();
                    command.CommandText = $"SELECT Russian, Description, English, Path FROM [{topicId}]";
                    reader = command.ExecuteReader();
                    int index = 0;
                    while (reader.Read())
                    {
                        russianArray[indiciesForSave.ElementAt(index)] = reader.GetString(0).Trim();
                        descriptionArray[indiciesForSave.ElementAt(index)] = reader.GetString(1).Trim();
                        englishArray[indiciesForSave.ElementAt(index)] = reader.GetString(2).Trim();
                        pathArray[indiciesForSave.ElementAt(index++)] = reader.GetString(3).Trim();
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

                    ChoiceOfTopicLabel.Visibility = ChoiceOfTopicTextBox.Visibility = MainWordsButton.Visibility = Visibility.Hidden;
                    StartButton.Visibility = ToTheChoiceOfTopicButton.Visibility = Visibility.Visible;
                    ChoiceOfTopicTextBox.Clear();
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



                        ChoiceOfTopicLabel.Visibility = ChoiceOfTopicTextBox.Visibility = Visibility.Hidden;
                        StartButton.Visibility = ToTheChoiceOfTopicButton.Visibility = Visibility.Visible;
                        ChoiceOfTopicTextBox.Clear();
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
            StartButton.Content = "Снова";

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
            count = russianArray.Length;
            numberofword = 1;
            WordOutputLabel.Content = russianArray[0];
            DescriptionLabel.Content = descriptionArray[0];
            english = englishArray[0];
            path = pathArray[0];
            WordsCounterLabel.Content = "1/" + count;
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
                if (numberofword < count)
                {
                    numberofword++;
                    WordOutputLabel.Content = russianArray[++index];
                    DescriptionLabel.Content = descriptionArray[index];
                    english = englishArray[index];
                    path = pathArray[index];
                    WordsCounterLabel.Content = numberofword + "/" + count;
                }
                else
                {
                    WordOutputLabel.Content = "Выполнено";
                    DescriptionLabel.Content = "";
                    WordsCounterLabel.Content = numberofword + "/" + count;
                }
            }
        }

        //клик но кнопке ToTheChoiceOfTopicButton
        private void ToTheChoiceOfTopicButton_Click(object sender, EventArgs e)
        {
            ChoiceOfTopicLabel.Visibility = ChoiceOfTopicTextBox.Visibility = MainWordsButton.Visibility = Visibility.Visible;

            StartButton.Visibility = ToTheChoiceOfTopicButton.Visibility =
            BackButton.Visibility = NextButton.Visibility =
            EnteringAWordTextBox.Visibility = WordOutputLabel.Visibility =
            SearchByNumberTextBox.Visibility
            = DescriptionLabel.Visibility = WordsCounterLabel.Visibility = Visibility.Hidden;

            StartButton.Content = "Начать";
            index = 0;
            indiciesForSave.Clear();
        }

        //клик по кнопке NextButton
        private void NextButton_Click(object sender, EventArgs e)
        {
            if (numberofword < count)
            {
                numberofword++;
                WordOutputLabel.Content = russianArray[++index];
                DescriptionLabel.Content = descriptionArray[index];
                english = englishArray[index];
                path = pathArray[index];
                WordsCounterLabel.Content = numberofword + "/" + count;
            }
            else if (numberofword == count)
            {
                index = 0;
                numberofword = 1;
                WordOutputLabel.Content = russianArray[0];
                DescriptionLabel.Content = descriptionArray[0];
                english = englishArray[0];
                path = pathArray[0];
                WordsCounterLabel.Content = numberofword + "/" + count;
            }
        }

        //клик по кнопке BackButton
        private void BackButton_Click(object sender, EventArgs e)
        {
            if (numberofword == 1)
            {
                index = russianArray.Length - 1;
                numberofword = count;
                WordOutputLabel.Content = russianArray[index];
                DescriptionLabel.Content = descriptionArray[index];
                english = englishArray[index];
                path = pathArray[index];
                WordsCounterLabel.Content = numberofword + "/" + count;
            }
            else if (WordOutputLabel.Content.ToString() == "Выполнено")
            {
                WordOutputLabel.Content = russianArray[index];
                DescriptionLabel.Content = descriptionArray[index];
                english = englishArray[index];
                path = pathArray[index];

                DescriptionLabel.Visibility = Visibility.Visible;
                WordsCounterLabel.Content = numberofword + "/" + count;
            }
            else
            {
                numberofword--;
                WordOutputLabel.Content = russianArray[--index];
                DescriptionLabel.Content = descriptionArray[index];
                english = englishArray[index];
                path = pathArray[index];
                WordsCounterLabel.Content = numberofword + "/" + count;
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
                    if (numberofword >= 1 && numberofword <= count)
                    {
                        index = numberofword - 1;
                        WordOutputLabel.Content = russianArray[index];
                        DescriptionLabel.Content = descriptionArray[index];
                        english = englishArray[index];
                        path = pathArray[index];
                        SearchByNumberTextBox.Clear();
                        WordsCounterLabel.Content = numberofword + "/" + count;
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

