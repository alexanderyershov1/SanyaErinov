﻿using System;
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
            NextButton.Visibility = BackButton.Visibility = ProgressBar.Visibility = SearchByNumberTextBox.Visibility =
                WordOutputLabel.Visibility = WordsCounterLabel.Visibility = EnteringAWordTextBox.Visibility =
                StartButton.Visibility = ToTheChoiceOfTopicButton.Visibility = DescriptionLabel.Visibility = Visibility.Hidden;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        static string Debug = Directory.GetCurrentDirectory();
        static string Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();

        public string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=
        {Shebist}\UserDB.mdf;Integrated Security=True";
        public int userid, idofword, numberofword = 1, count = 0;
        public string english, path, Section = "";
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;

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

                //reader.Close();

                //command.CommandText = $"SELECT " +
                //    $"ChoiceOfTopicTextBoxVisibility FROM UserState WHERE Id = {userid}";
                //command.Connection = connection;

                //reader = command.ExecuteReader();

                //if (reader.HasRows)
                //{
                //    reader.Read();
                //    switch (reader.GetString(0))
                //    {
                //        case "Visible":
                //            ChoiceOfTopicTextBox.Visibility = Visibility.Visible;
                //            break;
                //        case "Hidden":
                //            ChoiceOfTopicTextBox.Visibility = Visibility.Hidden;
                //            break;
                //    }
                //}
                //else
                //{
                //    reader.Close();
                //    command.CommandText = $"INSERT INTO UserState (Id, " +
                //        $" ChoiceOfTopicTextBoxVisibility) VALUES ({userid}," +
                //        $" '{ChoiceOfTopicTextBox.Visibility.ToString()}')";
                //    command.Connection = connection;
                //    command.ExecuteNonQuery();
                //}
            }
        }


        bool isWordsCounterLabelVisible = true,
        isPCMEnabled = true,
        isSearchByNumberTextBoxVisible = true,
        isNextBackButtonsVisible = true,
        isProgressBarVisible = true,
        isSoundEnabled = true;

        MediaPlayer player = new MediaPlayer();

        private void QueryRussianDescriptionEnglishPath()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                command.CommandText = $"SELECT Russian, Description, English, Path FROM [{topicId}] where Id = {idofword}";
                command.Connection = connection;
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    WordOutputLabel.Content = reader.GetString(0).Trim();
                    DescriptionLabel.Content = reader.GetString(1).Trim();
                    english = reader.GetString(2).Trim();
                    path = reader.GetString(3).Trim();

                }
            }
        }

        //Нажатие Enter в поле выбора темы
        string topicId;
        private void ChoiceOfTopicTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Section = ChoiceOfTopicTextBox.Text;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.CommandText = $"SELECT Id FROM Topics WHERE UserId = {userid} AND Name = '{ChoiceOfTopicTextBox.Text}'";
                    command.Connection = connection;
                    reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        reader.Read();
                        topicId = reader.GetInt32(0).ToString();
                        reader.Close();

                        command.CommandText = $"SELECT MIN(Id) FROM [{topicId}]";
                        reader = command.ExecuteReader();
                        reader.Read();
                        idofword = reader.GetInt32(0);
                        reader.Close();

                        command.CommandText = $"SELECT Russian, Description, English, Path FROM [{topicId}] where Id = {idofword}";
                        reader = command.ExecuteReader();

                        if (reader.HasRows)
                        {
                            reader.Read();
                            WordOutputLabel.Content = reader.GetString(0).Trim();
                            DescriptionLabel.Content = reader.GetString(1).Trim();
                            english = reader.GetString(2).Trim();
                            path = reader.GetString(3).Trim();

                            reader.Close();

                            ChoiceOfTopicLabel.Visibility = ChoiceOfTopicTextBox.Visibility = Visibility.Hidden;
                            StartButton.Visibility = ToTheChoiceOfTopicButton.Visibility = Visibility.Visible;
                            ChoiceOfTopicTextBox.Clear();

                            command.CommandText = $"UPDATE UserState SET ChoiceOfTopicTextBoxVisibility = '{ChoiceOfTopicTextBox.Visibility.ToString()}' WHERE Id = {userid}";
                            command.ExecuteNonQuery();

                        }
                        else
                        {
                            ChoiceOfTopicTextBox.Clear();
                            return;
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

            if (isProgressBarVisible)
            {
                ProgressBar.Visibility = Visibility.Visible;
            }

            numberofword = 1;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.CommandText = $"SELECT COUNT(*) FROM [{topicId}]";
                command.Connection = connection;
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Read();
                    count = reader.GetInt32(0);
                    reader.Close();
                }

                command.CommandText = $"SELECT MIN(Id) FROM [{topicId}]";
                command.Connection = connection;
                reader = command.ExecuteReader();
                reader.Read();
                idofword = reader.GetInt32(0);
            }
            
            QueryRussianDescriptionEnglishPath();

            ProgressBar.Value = 1;
            ProgressBar.Maximum = count;
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
        }


        //при изменении текста EnteringAWordTextBox 
        private void EnteringAWordTextBox_TextChanged(object sender, EventArgs e)
        {
            if (EnteringAWordTextBox.Text == english.Trim())
            {
                if (isSoundEnabled)
                {
                    if (File.Exists(Debug + path))
                    {
                        player.Open(new Uri(Debug + path, UriKind.Absolute));
                        player.Play();
                    }
                }
                EnteringAWordTextBox.Clear();
                if (numberofword < count)
                {
                    numberofword++;
                    using(SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        command.CommandText = $"SELECT Id FROM [{topicId}] WHERE Id > {idofword}";
                        command.Connection = connection;
                        reader = command.ExecuteReader();
                        reader.Read();
                        idofword = reader.GetInt32(0);
                    }
                    QueryRussianDescriptionEnglishPath();

                    ProgressBar.Value = numberofword;
                    WordsCounterLabel.Content = numberofword + "/" + count;
                }
                else
                {
                    WordOutputLabel.Content = "Выполнено";
                    DescriptionLabel.Content = "";
                    ProgressBar.Value = numberofword;
                    WordsCounterLabel.Content = numberofword + "/" + count;
                }
            }
        }

        //клик но кнопке ToTheChoiceOfTopicButton
        private void ToTheChoiceOfTopicButton_Click(object sender, EventArgs e)
        {
            ChoiceOfTopicLabel.Visibility = ChoiceOfTopicTextBox.Visibility = Visibility.Visible;

            StartButton.Visibility = ToTheChoiceOfTopicButton.Visibility =
            BackButton.Visibility = NextButton.Visibility =
            EnteringAWordTextBox.Visibility = WordOutputLabel.Visibility =
            ProgressBar.Visibility = SearchByNumberTextBox.Visibility
            = DescriptionLabel.Visibility = WordsCounterLabel.Visibility = Visibility.Hidden;

            StartButton.Content = "Начать";
        }

        //клик по кнопке NextButton
        private void NextButton_Click(object sender, EventArgs e)
        {
            if (numberofword < count)
            {   
                numberofword++;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.CommandText = $"SELECT Id FROM [{topicId}] WHERE Id > {idofword}";
                    command.Connection = connection;
                    reader = command.ExecuteReader();
                    reader.Read();
                    idofword = reader.GetInt32(0);
                }
                QueryRussianDescriptionEnglishPath();

                ProgressBar.Value = numberofword;
                WordsCounterLabel.Content = numberofword + "/" + count;
            }
            else if (numberofword == count)
            {
                numberofword = 1;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.CommandText = $"SELECT MIN(Id) FROM [{topicId}]";
                    command.Connection = connection;
                    reader = command.ExecuteReader();
                    reader.Read();
                    idofword = reader.GetInt32(0);
                }
                QueryRussianDescriptionEnglishPath();

                ProgressBar.Value = numberofword;
                WordsCounterLabel.Content = numberofword + "/" + count;
            }
        }

        //клик по кнопке BackButton
        private void BackButton_Click(object sender, EventArgs e)
        {
            if (numberofword == 1)
            {
                numberofword = count;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.CommandText = $"SELECT MAX(Id) FROM [{topicId}]";
                    command.Connection = connection;
                    reader = command.ExecuteReader();
                    reader.Read();
                    idofword = reader.GetInt32(0);
                }

                QueryRussianDescriptionEnglishPath();
                ProgressBar.Value = numberofword;
                WordsCounterLabel.Content = numberofword + "/" + count;
            }
            else if (WordOutputLabel.Content.ToString() == "Выполнено")
            {
                QueryRussianDescriptionEnglishPath();

                DescriptionLabel.Visibility = Visibility.Visible;
                ProgressBar.Value = numberofword;
                WordsCounterLabel.Content = numberofword + "/" + count;
            }
            else
            {
                numberofword--;
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    command.CommandText = $"SELECT Id FROM [{topicId}] WHERE Id < {idofword}";
                    command.Connection = connection;
                    reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        idofword = reader.GetInt32(0);
                    }
                }

                QueryRussianDescriptionEnglishPath();
                ProgressBar.Value = numberofword;
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
                        using(SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            command.CommandText = $"SELECT MIN(Id) FROM [{topicId}]";
                            command.Connection = connection;
                            reader = command.ExecuteReader();
                            reader.Read();
                            idofword = reader.GetInt32(0);
                            reader.Close();
                            command.CommandText = $"SELECT Id FROM [{topicId}] WHERE Id > {idofword}";
                            for(int i = 1; i < numberofword; i++)
                            {
                                reader = command.ExecuteReader();
                                reader.Read();
                                idofword = reader.GetInt32(0);
                                reader.Close();
                            }
                        }
                        QueryRussianDescriptionEnglishPath();

                        SearchByNumberTextBox.Clear();
                        WordsCounterLabel.Content = numberofword + "/" + count;
                        ProgressBar.Value = numberofword;
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

