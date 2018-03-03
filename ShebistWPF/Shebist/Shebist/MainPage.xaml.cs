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
        }
        
        BinaryFormatter formatter = new BinaryFormatter();
        static System.IO.DirectoryInfo myDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        static string parentDirectory = myDirectory.Parent.FullName;
        static System.IO.DirectoryInfo myDirectory2 = new DirectoryInfo(parentDirectory);
        static string parentDirectory2 = myDirectory2.Parent.FullName;

        public string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=
        {parentDirectory2}\Topics.mdf;Integrated Security=True";
        public int id, count = 0;
        public string russian, description, english, path, Section = "";
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }


        bool isWordsCounterLabelEnabled,
        isPCMEnabled,
        isSearchByNumberTextBoxEnabled,
        isNextBackButtonsEnabled,
        isProgressBarEnabled,
        isSoundEnabled = true;

        MediaPlayer player = new MediaPlayer();

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationPage ap = new AuthorizationPage();
            this.NavigationService.Navigate(ap);
        }

        private void QueryRussianDescriptionEnglishPath()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT Russian, Description, English, Path FROM {Section} where Id = {id}", connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {

                        russian = (string)reader.GetValue(0);
                        russian = russian.Trim();
                        WordOutputLabel.Content = russian;

                        description = (string)reader.GetValue(1);
                        description = description.Trim();
                        DescriptionLabel.Content = description;

                        english = (string)reader.GetValue(2);
                        english = english.Trim();
                        path = (string)reader.GetValue(3);
                    }
                }

            }
        }

        //Нажатие Enter в поле выбора темы
        private void ChoiceOfTopicTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                switch (ChoiceOfTopicTextBox.Text.ToLower().Trim())
                {
                    case "первая":
                        Section = "First";
                        break;
                    default:
                        ChoiceOfTopicTextBox.Clear();
                        return;
                }

                ChoiceOfTopicLabel.IsEnabled =
                ChoiceOfTopicTextBox.IsEnabled = false;
                ChoiceOfTopicLabel.Visibility = ChoiceOfTopicTextBox.Visibility = Visibility.Hidden;
                StartButton.IsEnabled = ToTheChoiceOfTopicButton.IsEnabled = true;
                StartButton.Visibility = ToTheChoiceOfTopicButton.Visibility = Visibility.Visible;
                ChoiceOfTopicTextBox.Clear();
            }
        }

        private void SettingsMenuItem_Click(object sender, EventArgs e)
        {
            SettingsWindow sw = new SettingsWindow(this);
            sw.Show();
        }

        private void StartButton_Click(object sender, EventArgs e)
        {
            EnteringAWordTextBox.IsEnabled = DescriptionLabel.IsEnabled = true;
            EnteringAWordTextBox.Visibility = DescriptionLabel.Visibility = Visibility.Visible;
            StartButton.Content = "Снова";

            if (isSearchByNumberTextBoxEnabled)
            {
                SearchByNumberTextBox.IsEnabled = true;
                SearchByNumberTextBox.Visibility = Visibility.Visible;
            }

            WordOutputLabel.IsEnabled = true;
            WordOutputLabel.Visibility = Visibility.Visible;

            if (isWordsCounterLabelEnabled)
            {
                WordsCounterLabel.IsEnabled = true;
                WordsCounterLabel.Visibility = Visibility.Visible;
            }

            if (isNextBackButtonsEnabled)
            {
                NextButton.IsEnabled = BackButton.IsEnabled = true;
                NextButton.Visibility = BackButton.Visibility = Visibility.Visible;
            }

            if (isProgressBarEnabled)
            {
                ProgressBar.IsEnabled = true;
                ProgressBar.Visibility = Visibility.Visible;
            }

            id = 1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                id = 1;
                SqlCommand command = new SqlCommand($"SELECT COUNT(*) FROM {Section}", connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        count = (int)reader.GetValue(0);
                    }
                }
            }

            QueryRussianDescriptionEnglishPath();

            ProgressBar.Value = 1;
            ProgressBar.Maximum = count;
            WordsCounterLabel.Content = "1/" + count;
        }


        //при изменении текста EnteringAWordTextBox 
        private void EnteringAWordTextBox_TextChanged(object sender, EventArgs e)
        {
            if (EnteringAWordTextBox.Text == english.Trim())
            {
                if (isSoundEnabled)
                {
                    if (File.Exists(Directory.GetCurrentDirectory() + path))
                    {
                        player.Open(new Uri(Directory.GetCurrentDirectory() + path, UriKind.Absolute));
                        player.Play();
                    }
                }
                EnteringAWordTextBox.Clear();
                if (id < count)
                {
                    id++;
                    QueryRussianDescriptionEnglishPath();

                    ProgressBar.Value = id;
                    WordsCounterLabel.Content = id + "/" + count;
                }
                else
                {
                    WordOutputLabel.Content = "Выполнено";
                    DescriptionLabel.Content = "";
                    ProgressBar.Value = id;
                    WordsCounterLabel.Content = id + "/" + count;
                }
            }
        }

        //клик но кнопке ToTheChoiceOfTopicButton
        private void ToTheChoiceOfTopicButton_Click(object sender, EventArgs e)
        {
            ChoiceOfTopicLabel.IsEnabled = ChoiceOfTopicTextBox.IsEnabled = true;
            ChoiceOfTopicLabel.Visibility = ChoiceOfTopicTextBox.Visibility = Visibility.Visible;

            WordsCounterLabel.IsEnabled = SearchByNumberTextBox.IsEnabled =
            ToTheChoiceOfTopicButton.IsEnabled =
            BackButton.IsEnabled = NextButton.IsEnabled =
            EnteringAWordTextBox.IsEnabled = WordOutputLabel.IsEnabled =
            ProgressBar.IsEnabled = DescriptionLabel.IsEnabled =
            StartButton.IsEnabled
             = false;


            StartButton.Visibility = ToTheChoiceOfTopicButton.Visibility =
            BackButton.Visibility = NextButton.Visibility =
            EnteringAWordTextBox.Visibility = WordOutputLabel.Visibility =
            ProgressBar.Visibility = SearchByNumberTextBox.Visibility
            = DescriptionLabel.Visibility = Visibility.Hidden;

            StartButton.Content = "Начать";
        }

        //клик по кнопке NextButton
        private void NextButton_Click(object sender, EventArgs e)
        {
            if (id < count)
            {
                id++;
                QueryRussianDescriptionEnglishPath();

                ProgressBar.Value = id;
                WordsCounterLabel.Content = id + "/" + count;
            }
            else if (id == count)
            {
                id = 1;
                QueryRussianDescriptionEnglishPath();

                ProgressBar.Value = id;
                WordsCounterLabel.Content = id + "/" + count;
            }
        }

        //клик по кнопке BackButton
        private void BackButton_Click(object sender, EventArgs e)
        {
            if (id == 1)
            {
                id = count;
                QueryRussianDescriptionEnglishPath();
                ProgressBar.Value = id;
                WordsCounterLabel.Content = "1/" + count;
            }
            else if (WordOutputLabel.Content.ToString() == "Выполнено")
            {
                QueryRussianDescriptionEnglishPath();

                DescriptionLabel.IsEnabled = true;
                DescriptionLabel.Visibility = Visibility.Visible;
                ProgressBar.Value = id;
                WordOutputLabel.Content = id + "/" + count;
            }
            else
            {
                id--;
                QueryRussianDescriptionEnglishPath();
                ProgressBar.Value = id;
                WordsCounterLabel.Content = id + "/" + count;
            }

        }

        //переход к слову по номеру
        private void SearchByNumberTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                try
                {
                    id = Int32.Parse(SearchByNumberTextBox.Text);
                    if (id >= 1 && id <= count)
                    {
                        QueryRussianDescriptionEnglishPath();

                        SearchByNumberTextBox.Clear();
                        WordsCounterLabel.Content = id + "/" + count;
                        ProgressBar.Value = id;
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
