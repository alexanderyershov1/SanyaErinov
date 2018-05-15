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

        static string Debug = Directory.GetCurrentDirectory();
        static string Shebist = Directory.GetParent(Directory.GetParent(Debug).ToString()).ToString();
        static string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Shebist}\UserDB.mdf;Integrated Security=True";

        SqlCommand command = new SqlCommand();
        SqlDataReader reader;
        User currentUser;

        static List<short> ReadIndices(string s)
        {
            List<short> SequenceOfIndices = new List<short>();
            int startIndex = 0, length = 0;
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == ';')
                {//если символ равен ;, добавляем в список indiciesForSave элемент, начиная с startIndex длиной в length
                    SequenceOfIndices.Add(short.Parse(s.Substring(startIndex, length)));
                    startIndex = i + 1;//прибавляем 1 к startIndex
                    length = 0;//обнуляем длину
                }
                else length++;//иначе увеличиваем на 1 длину строки
            }
            return SequenceOfIndices;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
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

                        command.CommandText = $"SELECT Name, CurrentTopicId, ChoiceOfTopicGridVisibility, SequenceOfIndicesOfMainWords, IndexOfMainWords FROM Users WHERE Login = N'{currentUser.Login}'";
                        reader = command.ExecuteReader();
                        reader.Read();
                        currentUser.Name = reader.GetString(0);
                        currentUser.CurrentTopicId = reader.GetString(1);
                        currentUser.ChoiceOfTopicGridVisibility = reader.GetString(2);
                        currentUser.SequenceOfIndicesOfMainWords = ReadIndices(reader.GetString(3));
                        currentUser.IndexOfMainWords = reader.GetInt16(4);
                        reader.Close();

                        List<Topic> oldTopics = new List<Topic>(), newTopics = new List<Topic>();
                        command.CommandText = $"SELECT Id, Name, SequenceOfIndices, CurrentIndex FROM Topics WHERE UserLogin = N'{currentUser.Login}'";
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
                                command.CommandText = $"SELECT Id, Question, Hint, Answer, Path FROM " +
                                    $"Words WHERE TopicId = {Int32.Parse(topic.Id)}";
                                reader = command.ExecuteReader();
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        topic.Words.Add(new Word
                                        {
                                            Id = reader.GetInt32(0).ToString(),
                                            TopicId = topic.Id,
                                            Question = reader.GetString(1).Trim(),
                                            Hint = reader.GetString(2).Trim(),
                                            Answer = reader.GetString(3).Trim(),
                                            Path = reader.GetString(4).Trim(),
                                        });
                                    }
                                }
                            }
                            reader.Close();
                            
                            foreach (Topic topic in newTopics)
                            {
                                command.CommandText = $"SELECT Id, Question, Hint, Answer, Path FROM " +
                                    $"Words WHERE TopicId = {Int32.Parse(topic.Id)}";
                                reader = command.ExecuteReader();
                                if (reader.HasRows)
                                {
                                    while (reader.Read())
                                    {
                                        topic.Words.Add(new Word
                                        {
                                            Id = reader.GetInt32(0).ToString(),
                                            TopicId = topic.Id,
                                            Question = reader.GetString(1).Trim(),
                                            Hint = reader.GetString(2).Trim(),
                                            Answer = reader.GetString(3).Trim(),
                                            Path = reader.GetString(4).Trim(),
                                        });
                                    }
                                }
                            }

                            foreach (Topic topic in oldTopics)
                            {
                                for (int i = 0; i < topic.Words.Count; i++)
                                {
                                    topic.currentWords.Add(topic.Words[topic.SequenceOfIndices[i]]);
                                }
                            }

                            foreach (Topic topic in newTopics)
                            {
                                for (int i = 0; i < topic.Words.Count; i++)
                                {
                                    topic.currentWords.Add(topic.Words[topic.SequenceOfIndices[i]]);
                                }
                            }
                        }

                        reader.Close();
                        Topic MainWords = new Topic();
                        command.CommandText = "SELECT Id, Question, Hint, Answer, Path FROM MainWords";
                        reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            MainWords.Words.Add(new Word
                            {
                                Id = reader.GetInt32(0).ToString(),
                                Question = reader.GetString(1).Trim(),
                                Hint = reader.GetString(2).Trim(),
                                Answer = reader.GetString(3).Trim(),
                                Path = reader.GetString(4).Trim()

                            });
                        }
                        reader.Close();

                        for (int i = 0; i < MainWords.Words.Count; i++)
                        {
                            MainWords.currentWords.Add(new Word());
                        }

                        for (int i = 0; i < MainWords.Words.Count; i++)
                        {
                            MainWords.currentWords[i] = MainWords.Words[currentUser.SequenceOfIndicesOfMainWords[i]];
                        }

                        List<short> soiomwOldUser = new List<short>(), soiomwNewUser = new List<short>();
                        for(short i = 0; i < currentUser.SequenceOfIndicesOfMainWords.Count; i++)
                        {
                            soiomwOldUser.Add(currentUser.SequenceOfIndicesOfMainWords[i]);
                            soiomwNewUser.Add(currentUser.SequenceOfIndicesOfMainWords[i]);
                        }
                        
                        MainWindow mw = new MainWindow
                        {
                            needToUpdate = true,
                            WindowState = this.WindowState,
                            Top = this.Top,
                            Left = this.Left,
                            Width = this.Width,
                            Height = this.Height,

                            oldUser = new User
                            {
                                Login = currentUser.Login,
                                Password = currentUser.Password,
                                Name = currentUser.Name,
                                Email = currentUser.Email,
                                ChoiceOfTopicGridVisibility = currentUser.ChoiceOfTopicGridVisibility,
                                CurrentTopicId = currentUser.CurrentTopicId,
                                IndexOfMainWords = currentUser.IndexOfMainWords,
                                SequenceOfIndicesOfMainWords = soiomwOldUser
                            },


                            newUser = new User
                            {
                                Login = currentUser.Login,
                                Password = currentUser.Password,
                                Name = currentUser.Name,
                                Email = currentUser.Email,
                                ChoiceOfTopicGridVisibility = currentUser.ChoiceOfTopicGridVisibility,
                                CurrentTopicId = currentUser.CurrentTopicId,
                                IndexOfMainWords = currentUser.IndexOfMainWords,
                                SequenceOfIndicesOfMainWords = soiomwNewUser
                            },

                            MainWords = MainWords,
                            oldTopics = oldTopics,
                            newTopics = newTopics

                        };
                        mw.Show();
                        this.Close();
                    }
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

        private void LoginTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            WrongDataLabel.Visibility = Visibility.Hidden;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            WrongDataLabel.Visibility = Visibility.Hidden;
        }

        private void DataRecoveryLabel_MouseEnter(object sender, MouseEventArgs e)
        {
            DataRecoveryLabel.FontSize = 13;
        }

        private void DataRecoveryLabel_MouseLeave(object sender, MouseEventArgs e)
        {
            DataRecoveryLabel.FontSize = 12;
        }

        private void NoAccountYet_MouseEnter(object sender, MouseEventArgs e)
        {
            NoAccountYet.FontSize = 13;
        }

        private void NoAccountYet_MouseLeave(object sender, MouseEventArgs e)
        {
            NoAccountYet.FontSize = 12;
        }
    }
}
