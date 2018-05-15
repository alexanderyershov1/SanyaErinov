using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Shebist
{
    [Serializable]
    public class User
    {
        public string Name, Login, Email, Password, ChoiceOfTopicGridVisibility, CurrentTopicId;
        public short IndexOfMainWords;
        public List<short> SequenceOfIndicesOfMainWords;
        [NonSerialized]
        SqlCommand command = new SqlCommand();
        [NonSerialized]
        SqlDataReader reader;
        static string Shebist = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Shebist}\UserDB.mdf;Integrated Security=True";

        static string WriteSequence(List<short> sequence)
        {
            string s = "";
            foreach (int index in sequence)
            {
                s += index + ";";
            }
            return s;
        }

        public void Update(User oldUser, User newUser, List<Topic> oldTopics, List<Topic> newTopics, List<int> indicesOfDeletedTopics, List<Word> deletedWords)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.Connection = connection;

                if (oldUser.IndexOfMainWords != newUser.IndexOfMainWords)
                {
                    MessageBox.Show("Хыч-хыч, Коксал Баба");
                    command.CommandText = $"UPDATE Users SET IndexOfMainWords = {newUser.IndexOfMainWords} WHERE Login = N'{oldUser.Login}'";
                    command.ExecuteNonQuery();
                }

                if (oldUser.Name != newUser.Name)
                {
                    command.CommandText = $"UPDATE Users SET Name = N'{newUser.Name}' WHERE Login = N'{oldUser.Login}'";
                    command.ExecuteNonQuery();
                }

                if (oldUser.Password != newUser.Password)
                {
                    command.CommandText = $"UPDATE Users SET Password = N'{newUser.Password}' WHERE Login = N'{oldUser.Login}'";
                    command.ExecuteNonQuery();
                }

                if (oldUser.CurrentTopicId != newUser.CurrentTopicId)
                {
                    command.CommandText = $"UPDATE Users SET CurrentTopicId = N'{newUser.CurrentTopicId}' WHERE Login = N'{oldUser.Login}'";
                    command.ExecuteNonQuery();
                }

                if (oldUser.ChoiceOfTopicGridVisibility != newUser.ChoiceOfTopicGridVisibility)
                {
                    command.CommandText = $"UPDATE Users SET ChoiceOfTopicGridVisibility = N'{newUser.ChoiceOfTopicGridVisibility}' WHERE Login = N'{oldUser.Login}'";
                    command.ExecuteNonQuery();
                }

                for(int i = 0; i < oldUser.SequenceOfIndicesOfMainWords.Count; i++)
                {
                    if (oldUser.SequenceOfIndicesOfMainWords[i] != newUser.SequenceOfIndicesOfMainWords[i])
                    {
                        MessageBox.Show("Мяучик");
                        command.CommandText = $"UPDATE Users SET SequenceOfIndicesOfMainWords = N'{WriteSequence(newUser.SequenceOfIndicesOfMainWords)}' WHERE Login = N'{oldUser.Login}'";
                        command.ExecuteNonQuery();
                        break;
                    }
                }

                for (int i = 0; i < newTopics.Count; i++)
                {
                   
                        if (i < oldTopics.Count)
                        {
                       
                        if (newTopics[i].Id == oldTopics[i].Id)
                        {
                            if (newTopics[i].CurrentIndex != oldTopics[i].CurrentIndex)
                            {
                                command.CommandText = $"UPDATE Topics SET SequenceOfIndices = N'{WriteSequence(newTopics[i].SequenceOfIndices)}' WHERE UserLogin = N'{oldUser.Login}'";
                                command.ExecuteNonQuery();
                            }

                            if (newTopics[i].CurrentIndex != oldTopics[i].CurrentIndex)
                            {
                                command.CommandText = $"UPDATE Topics SET CurrentIndex = {newTopics[i].CurrentIndex} WHERE UserLogin = N'{oldUser.Login}'";
                                command.ExecuteNonQuery();
                            }

                            if(newTopics[i].SequenceOfIndices != oldTopics[i].SequenceOfIndices)
                            {
                                command.CommandText = $"UPDATE Topics SET SequenceOfIndices = N'{WriteSequence(newTopics[i].SequenceOfIndices)}' WHERE UserLogin = N'{oldUser.Login}'";
                                command.ExecuteNonQuery();
                            }

                            for (int j = 0; j < newTopics[i].Words.Count; j++)
                            {
                                if (j < oldTopics[i].Words.Count)
                                {
                                    if (newTopics[i].Words[j].Question != oldTopics[i].Words[j].Question)
                                    {
                                        command.CommandText = $"UPDATE Words SET Question = N'{newTopics[i].Words[j].Question}' WHERE TopicId = {Int32.Parse(oldTopics[i].Id)}";
                                        command.ExecuteNonQuery();
                                    }
                                    if (newTopics[i].Words[j].Hint != oldTopics[i].Words[j].Hint)
                                    {
                                        command.CommandText = $"UPDATE Words SET Hint = N'{newTopics[i].Words[j].Hint}' WHERE TopicId = {Int32.Parse(oldTopics[i].Id)}";
                                        command.ExecuteNonQuery();
                                    }
                                    if (newTopics[i].Words[j].Answer != oldTopics[i].Words[j].Answer)
                                    {
                                        command.CommandText = $"UPDATE Words SET Answer = N'{newTopics[i].Words[j].Answer}' WHERE TopicId = {Int32.Parse(oldTopics[i].Id)}";
                                        command.ExecuteNonQuery();
                                    }
                                    if (newTopics[i].Words[j].Path != oldTopics[i].Words[j].Path)
                                    {
                                        command.CommandText = $"UPDATE Words SET Path = N'{newTopics[i].Words[j].Path}' WHERE TopicId = {Int32.Parse(oldTopics[i].Id)}";
                                        command.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    command.CommandText = $"INSERT INTO Words (TopicId, Question, Hint, Answer, Path)" +
                                        $" VALUES({Int32.Parse(newTopics[i].Id)}, N'{newTopics[i].Words[j].Question}', N'{newTopics[i].Words[j].Hint}'," +
                                        $"N'{newTopics[i].Words[j].Answer}', N'{newTopics[i].Words[j].Path}')";
                                    command.ExecuteNonQuery();
                                }

                            }
                        }
                            
                        }
                        else
                        {
                        MessageBox.Show("Пошла помидорка");
                        command.CommandText = $"INSERT INTO Topics (UserLogin, Name, SequenceOfIndices, CurrentIndex) VALUES" +
                                $"(N'{oldUser.Login}', N'{newTopics[i].Name}', N'{WriteSequence(newTopics[i].SequenceOfIndices)}', {newTopics[i].CurrentIndex})";
                            command.ExecuteNonQuery();

                        command.CommandText = $"SELECT MAX(Id) FROM Topics WHERE UserLogin = N'{oldUser.Login}'";
                        reader = command.ExecuteReader();
                        reader.Read();
                        int topicId = reader.GetInt32(0);
                        reader.Close();

                            for (int j = 0; j < newTopics[i].Words.Count; j++)
                            {
                                command.CommandText = $"INSERT INTO Words (TopicId, Question, Hint, Answer, Path)" +
                                        $" VALUES({topicId}, N'{newTopics[i].Words[j].Question}', N'{newTopics[i].Words[j].Hint}'," +
                                        $"N'{newTopics[i].Words[j].Answer}', N'{newTopics[i].Words[j].Path}')";
                                command.ExecuteNonQuery();
                            }
                        }
                    
                }

                if (indicesOfDeletedTopics.Count != 0)
                {
                    for (int i = 0; i < indicesOfDeletedTopics.Count; i++)
                    {
                        command.CommandText = $"DELETE FROM Words WHERE TopicId = {indicesOfDeletedTopics[i]}";
                        command.ExecuteNonQuery();
                        command.CommandText = $"DELETE FROM Topics WHERE Id = {indicesOfDeletedTopics[i]}";
                        command.ExecuteNonQuery();
                    }
                }
                
                if(deletedWords.Count != 0)
                {
                    for (int i = 0; i < deletedWords.Count; i++)
                    {
                        if (!indicesOfDeletedTopics.Contains(Int32.Parse(deletedWords[i].TopicId)))
                        {
                            command.CommandText = $"DELETE FROM Words WHERE Id = {Int32.Parse(deletedWords[i].Id)}";
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}