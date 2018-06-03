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
using System.IO;
using System.Data.SqlClient;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net.Mail;
using System.Net;

namespace Shebist
{
    /// <summary>
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationWindow : Window
    {
        public AuthorizationWindow()
        {
            InitializeComponent();
            WrongDataLabel.Visibility = Visibility.Hidden;
        }

        BinaryFormatter formatter = new BinaryFormatter();
        User user = new User();
        static string Debug = Directory.GetCurrentDirectory();
        static string Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();
        static string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Shebist}\UserDB.mdf;Integrated Security=True";

        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        User currentUser;

        static List<int> ReadIndices(string s)
        {
            List<int> SequenceOfIndices = new List<int>();
            int startIndex = 0, length = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '~')
                {//если символ равен ;, добавляем в список indiciesForSave элемент, начиная с startIndex длиной в length
                    SequenceOfIndices.Add(int.Parse(s.Substring(startIndex, length)));
                    startIndex = i + 1;//прибавляем 1 к startIndex
                    length = 0;//обнуляем длину
                }
                else length++;//иначе увеличиваем на 1 длину строки
            }
            return SequenceOfIndices;
        }

        static List<string> ReadSequence(string s)
        {
            List<string> SequenceOfIndices = new List<string>();
            int startIndex = 0, length = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '~')
                {//если символ равен ;, добавляем в список indiciesForSave элемент, начиная с startIndex длиной в length
                    SequenceOfIndices.Add(s.Substring(startIndex, length));
                    startIndex = i + 1;//прибавляем 1 к startIndex
                    length = 0;//обнуляем длину
                }
                else length++;//иначе увеличиваем на 1 длину строки
            }
            return SequenceOfIndices;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (LoginTextBox.Text == "Admin" && PasswordBox.Password == "0000")
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
            else
            {
                foreach (User user in users)
                {
                    if ((LoginTextBox.Text == user.Login || LoginTextBox.Text == user.Email) && PasswordBox.Password == user.Password)
                    {
                        currentUser = user;
                        using (SqlConnection connection = new SqlConnection(connectionString))
                        {
                            connection.Open();
                            command.Connection = connection;

                            if (RememberMeCheckBox.IsChecked == true)
                            {
                                using (FileStream fs = new FileStream($"{Debug}\\Data\\currentUser", FileMode.OpenOrCreate))
                                {
                                    formatter.Serialize(fs, currentUser);
                                }
                            }


                            command.CommandText = $"UPDATE Users SET LastEntrance = N'{DateTime.Now.ToString()}', Status = N'Online' WHERE Login = N'{currentUser.Login}'";
                            command.ExecuteNonQuery();
                             DateTime entryTime = DateTime.Now;

                            command.CommandText = $"SELECT Name, CurrentTopicId, ChoiceOfTopicGridVisibility, SequenceOfIndicesOfMainTopic, IndexOfMainTopic, Id FROM Users WHERE Login = N'{currentUser.Login}'";
                            reader = command.ExecuteReader();
                            reader.Read();
                            currentUser.Name = reader.GetString(0);
                            currentUser.CurrentTopicId = reader.GetString(1);
                            currentUser.ChoiceOfTopicGridVisibility = reader.GetString(2);
                            currentUser.SequenceOfIndicesOfMainTopic = ReadIndices(reader.GetString(3));
                            currentUser.IndexOfMainTopic = reader.GetInt16(4);
                            currentUser.Id = reader.GetInt32(5);
                            reader.Close();

                            List<Topic> oldTopics = new List<Topic>(), newTopics = new List<Topic>();
                            command.CommandText = $"SELECT Id, Name, SequenceOfIndices, CurrentIndex FROM Topics WHERE UserId = {currentUser.Id}";
                            reader = command.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    oldTopics.Add(new Topic
                                    {
                                        Id = reader.GetInt32(0).ToString(),
                                        Name = reader.GetString(1).Trim(),
                                        SequenceOfIndices = ReadIndices(reader.GetString(2)),
                                        CurrentIndex = reader.GetInt16(3)
                                    });

                                    newTopics.Add(new Topic
                                    {
                                        Id = reader.GetInt32(0).ToString(),
                                        Name = reader.GetString(1).Trim(),
                                        SequenceOfIndices = ReadIndices(reader.GetString(2)),
                                        CurrentIndex = reader.GetInt16(3)
                                    });

                                }
                            }
                            reader.Close();

                            if (oldTopics.Count > 0)
                            {
                                foreach (Topic topic in oldTopics)
                                {
                                    command.CommandText = $"SELECT Id, isQuestionFirst, Questions, Contexts, Translations, WaysToQuestionsVoice, " +
                                        $"WayToSentenceVoice FROM " +
                                        $"Sentences WHERE TopicId = {Int32.Parse(topic.Id)}";
                                    reader = command.ExecuteReader();
                                    if (reader.HasRows)
                                    {
                                        while (reader.Read())
                                        {
                                            topic.Sentences.Add(new Sentence
                                            {
                                                id = reader.GetInt32(0).ToString(),
                                                topicId = topic.Id,
                                                isQuestionFirst = bool.Parse(reader.GetString(1)),
                                                questions = ReadSequence(reader.GetString(2)),
                                                contexts = ReadSequence(reader.GetString(3)),
                                                translations = ReadSequence(reader.GetString(4)),
                                                waysToQuestionsVoice = ReadSequence(reader.GetString(5)),
                                                wayToSentenceVoice = reader.GetString(6)
                                            });
                                        }
                                    }
                                    reader.Close();
                                }


                                foreach (Topic topic in newTopics)
                                {
                                    command.CommandText = $"SELECT Id, isQuestionFirst, Questions, Contexts, Translations, WaysToQuestionsVoice, " +
                                        $"WayToSentenceVoice FROM " +
                                        $"Sentences WHERE TopicId = {Int32.Parse(topic.Id)}";
                                    reader = command.ExecuteReader();
                                    if (reader.HasRows)
                                    {
                                        while (reader.Read())
                                        {
                                            topic.Sentences.Add(new Sentence
                                            {
                                                id = reader.GetInt32(0).ToString(),
                                                topicId = topic.Id,
                                                isQuestionFirst = bool.Parse(reader.GetString(1)),
                                                questions = ReadSequence(reader.GetString(2)),
                                                contexts = ReadSequence(reader.GetString(3)),
                                                translations = ReadSequence(reader.GetString(4)),
                                                waysToQuestionsVoice = ReadSequence(reader.GetString(5)),
                                                wayToSentenceVoice = reader.GetString(6)
                                            });
                                        }
                                    }
                                    reader.Close();
                                }

                            }

                            Topic MainTopic = new Topic();
                            command.CommandText = "SELECT Id, isQuestionFirst, Questions, Contexts, Translations, WaysToQuestionsVoice, " +
                                        $"WayToSentenceVoice FROM MainTopic";
                            reader = command.ExecuteReader();

                            while (reader.Read())
                            {
                                MainTopic.Sentences.Add(new Sentence
                                {
                                    id = reader.GetInt32(0).ToString(),
                                    isQuestionFirst = bool.Parse(reader.GetString(1)),
                                    questions = ReadSequence(reader.GetString(2)),
                                    contexts = ReadSequence(reader.GetString(3)),
                                    translations = ReadSequence(reader.GetString(4)),
                                    waysToQuestionsVoice = ReadSequence(reader.GetString(5)),
                                    wayToSentenceVoice = reader.GetString(6)

                                });
                            }
                            reader.Close();

                            if (currentUser.SequenceOfIndicesOfMainTopic.Count == MainTopic.Sentences.Count)
                            {
                                for (int i = 0; i < MainTopic.Sentences.Count; i++)
                                {

                                    MainTopic.currentSentences.Add(MainTopic.Sentences[currentUser.SequenceOfIndicesOfMainTopic[i]]);
                                }
                            }
                            else if (currentUser.SequenceOfIndicesOfMainTopic.Count < MainTopic.Sentences.Count)
                            {
                                int maxIndex = currentUser.SequenceOfIndicesOfMainTopic.Count - 1;
                                if (currentUser.IndexOfMainTopic > maxIndex)
                                    currentUser.IndexOfMainTopic = maxIndex;
                                for (int i = 0; i < MainTopic.Sentences.Count; i++)
                                {
                                    if (i <= maxIndex)
                                        MainTopic.currentSentences.Add(MainTopic.Sentences[currentUser.SequenceOfIndicesOfMainTopic[i]]);
                                    else
                                    {
                                        MainTopic.currentSentences.Add(MainTopic.Sentences[i]);
                                        currentUser.SequenceOfIndicesOfMainTopic.Add(i);
                                    }
                                }
                                command.CommandText = $"UPDATE Users SET SequenceOfIndicesOfMainTopic = N'{User.WriteSequence(currentUser.SequenceOfIndicesOfMainTopic)}' " +
                                    $"WHERE Id = {currentUser.Id}";
                                command.ExecuteNonQuery();
                            }
                            else
                            {
                                currentUser.SequenceOfIndicesOfMainTopic.Clear();
                                for (int i = 0; i < MainTopic.Sentences.Count; i++)
                                {
                                    MainTopic.currentSentences.Add(MainTopic.Sentences[i]);
                                    currentUser.SequenceOfIndicesOfMainTopic.Add(i);
                                }
                                command.CommandText = $"UPDATE Users SET SequenceOfIndicesOfMainTopic = N'{User.WriteSequence(currentUser.SequenceOfIndicesOfMainTopic)}' " +
                                    $"WHERE Id = {currentUser.Id}";
                                command.ExecuteNonQuery();
                            }


                            List<int> soiomtOldUser = new List<int>(), soiomtNewUser = new List<int>();
                            for (int i = 0; i < currentUser.SequenceOfIndicesOfMainTopic.Count; i++)
                            {
                                soiomtOldUser.Add(currentUser.SequenceOfIndicesOfMainTopic[i]);
                                soiomtNewUser.Add(currentUser.SequenceOfIndicesOfMainTopic[i]);
                            }

                            MainWindow mw = new MainWindow
                            {
                                needToUpdate = true,
                                WindowState = this.WindowState,
                                Top = this.Top,
                                Left = this.Left,
                                Width = this.Width,
                                Height = this.Height,
                                entryTime = entryTime,

                                oldUser = new User
                                {
                                    Id = currentUser.Id,
                                    Login = currentUser.Login,
                                    Password = currentUser.Password,
                                    Name = currentUser.Name,
                                    Email = currentUser.Email,
                                    ChoiceOfTopicGridVisibility = currentUser.ChoiceOfTopicGridVisibility,
                                    CurrentTopicId = currentUser.CurrentTopicId,
                                    IndexOfMainTopic = currentUser.IndexOfMainTopic,
                                    SequenceOfIndicesOfMainTopic = soiomtOldUser
                                },


                                newUser = new User
                                {
                                    Id = currentUser.Id,
                                    Login = currentUser.Login,
                                    Password = currentUser.Password,
                                    Name = currentUser.Name,
                                    Email = currentUser.Email,
                                    ChoiceOfTopicGridVisibility = currentUser.ChoiceOfTopicGridVisibility,
                                    CurrentTopicId = currentUser.CurrentTopicId,
                                    IndexOfMainTopic = currentUser.IndexOfMainTopic,
                                    SequenceOfIndicesOfMainTopic = soiomtNewUser
                                },

                                MainTopic = MainTopic,
                                oldTopics = oldTopics,
                                newTopics = newTopics

                            };
                            mw.Show();
                            this.Close();
                        }

                        break;
                    }
                    else
                    {
                        reader.Close();
                        WrongDataLabel.Visibility = Visibility.Visible;
                    }
                }

                using (FileStream fs = new FileStream($"{Debug}\\Data\\RememberMeCheckBoxIsChecked", FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, RememberMeCheckBox.IsChecked);
                }
            }
        }

        private void NoAccountYet_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RegistrationWindow rw = new RegistrationWindow
            {
                WindowState = this.WindowState,
                Top = this.Top,
                Left = this.Left,
                Width = this.Width,
                Height = this.Height,
                users = users
            };

            rw.Show();
            this.Close();
        }

        static string WriteSequence(List<int> sequence)
        {
            string s = "";
            foreach (int index in sequence)
            {
                s += index + ";";
            }
            return s;
        }

        public List<User> users = new List<User>();
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists($"{Debug}\\Data\\RememberMeCheckBoxIsChecked"))
            {
                using (FileStream fs = new FileStream($"{Debug}\\Data\\RememberMeCheckBoxIsChecked", FileMode.OpenOrCreate))
                {
                    RememberMeCheckBox.IsChecked = (bool)formatter.Deserialize(fs);
                }
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.Connection = connection;
                command.CommandText = "SELECT Login, Email, Password FROM Users";
                reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            Login = reader.GetString(0),
                            Email = reader.GetString(1),
                            Password = reader.GetString(2),
                        });
                    }
                }
            }

            if (RememberMeCheckBox.IsChecked == true)
            {
                if (File.Exists($"{Debug}\\Data\\currentUser"))
                {
                    using (FileStream fs = new FileStream($"{Debug}\\Data\\currentUser", FileMode.OpenOrCreate))
                    {

                        User user = (User)formatter.Deserialize(fs);
                        LoginTextBox.Text = user.Login;
                        PasswordBox.Password = user.Password;
                    }
                }
            }
        }

        private void DataRecoveryLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DataRecoveryWindow drw = new DataRecoveryWindow
            {
                WindowState = this.WindowState,
                Top = this.Top,
                Left = this.Left,
                Width = this.Width,
                Height = this.Height
            };
            drw.Show();
            this.Close();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            WrongDataLabel.Visibility = Visibility.Hidden;
        }

        private void Label_MouseEnter(object sender, MouseEventArgs e)
        {
            ((Label)sender).FontSize = 13;
        }

        private void Label_MouseLeave(object sender, MouseEventArgs e)
        {
            ((Label)sender).FontSize = 12;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            WrongDataLabel.Visibility = Visibility.Hidden;
        }
    }
}
