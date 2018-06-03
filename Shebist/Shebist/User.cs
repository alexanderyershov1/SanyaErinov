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
        public int Id;
        public string Name, Login, Email, Password, ChoiceOfTopicGridVisibility, CurrentTopicId;
        public int IndexOfMainTopic;
        public List<int> SequenceOfIndicesOfMainTopic;
        [NonSerialized]
        SqlCommand command = new SqlCommand();
        [NonSerialized]
        SqlDataReader reader;
        static string Shebist = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Shebist}\UserDB.mdf;Integrated Security=True";

        public static string WriteSequence(List<int> sequence)
        {
            string s = "";
            foreach (int index in sequence)
            {
                s += index + "~";
            }
            return s;
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
 
        public void Update(User oldUser, User newUser, List<Topic> oldTopics, List<Topic> newTopics, List<int> indicesOfDeletedTopics, List<Sentence> deletedSenteces, DateTime entryTime)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.Connection = connection;

                command.CommandText = $"SELECT TotalInTheApp FROM Users WHERE Id = {oldUser.Id}";
                reader = command.ExecuteReader();
                reader.Read();
                TimeSpan totalInTheApp = TimeSpan.Parse(reader.GetString(0));
                reader.Close();
                totalInTheApp += DateTime.Now.Subtract(entryTime);
                command.CommandText = $"UPDATE Users SET TotalInTheApp = N'{totalInTheApp.ToString()}', Status = N'Offline' WHERE Id = {oldUser.Id}";
                command.ExecuteNonQuery();

                if (oldUser.IndexOfMainTopic != newUser.IndexOfMainTopic)
                {
                    command.CommandText = $"UPDATE Users SET IndexOfMainTopic = {newUser.IndexOfMainTopic} WHERE Id = {oldUser.Id}";
                    command.ExecuteNonQuery();
                }

                if (oldUser.Name != newUser.Name)
                {
                    command.CommandText = $"UPDATE Users SET Name = N'{newUser.Name}' WHERE Id = {oldUser.Id}";
                    command.ExecuteNonQuery();
                }

                if (oldUser.Password != newUser.Password)
                {
                    command.CommandText = $"UPDATE Users SET Password = N'{newUser.Password}' WHERE Id = {oldUser.Id}";
                    command.ExecuteNonQuery();
                }

                if (oldUser.CurrentTopicId != newUser.CurrentTopicId)
                {
                    command.CommandText = $"UPDATE Users SET CurrentTopicId = N'{newUser.CurrentTopicId}' WHERE Id = {oldUser.Id}";
                    command.ExecuteNonQuery();
                }

                if (oldUser.ChoiceOfTopicGridVisibility != newUser.ChoiceOfTopicGridVisibility)
                {
                    command.CommandText = $"UPDATE Users SET ChoiceOfTopicGridVisibility = N'{newUser.ChoiceOfTopicGridVisibility}' WHERE Id = {oldUser.Id}";
                    command.ExecuteNonQuery();
                }

                for (int i = 0; i < oldUser.SequenceOfIndicesOfMainTopic.Count; i++)
                {
                    if (oldUser.SequenceOfIndicesOfMainTopic[i] != newUser.SequenceOfIndicesOfMainTopic[i])
                    {
                        command.CommandText = $"UPDATE Users SET SequenceOfIndicesOfMainTopic = N'{WriteSequence(newUser.SequenceOfIndicesOfMainTopic)}' WHERE Id = {oldUser.Id}";
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
                                command.CommandText = $"UPDATE Topics SET SequenceOfIndices = N'{WriteSequence(newTopics[i].SequenceOfIndices)}' WHERE Id = {Int32.Parse(newTopics[i].Id)}";
                                command.ExecuteNonQuery();
                            }

                            if (newTopics[i].CurrentIndex != oldTopics[i].CurrentIndex)
                            {
                                command.CommandText = $"UPDATE Topics SET CurrentIndex = {newTopics[i].CurrentIndex} WHERE Id = {Int32.Parse(newTopics[i].Id)}";
                                command.ExecuteNonQuery();
                            }

                            if (newTopics[i].SequenceOfIndices != oldTopics[i].SequenceOfIndices)
                            {
                                command.CommandText = $"UPDATE Topics SET SequenceOfIndices = N'{WriteSequence(newTopics[i].SequenceOfIndices)}' WHERE Id = {Int32.Parse(newTopics[i].Id)}";
                                command.ExecuteNonQuery();
                            }

                            for (int j = 0; j < newTopics[i].Sentences.Count; j++)
                            {
                                if (j < oldTopics[i].Sentences.Count)
                                {
                                    if (newTopics[i].Sentences[j].isQuestionFirst != oldTopics[i].Sentences[j].isQuestionFirst)
                                    {
                                        command.CommandText = $"UPDATE Sentences SET IsQuestionFirst = N'{newTopics[i].Sentences[j].isQuestionFirst.ToString()}' WHERE Id = {Int32.Parse(newTopics[i].Sentences[j].id)}";
                                        command.ExecuteNonQuery();
                                    }
                                    if (newTopics[i].Sentences[j].questions != oldTopics[i].Sentences[j].questions)
                                    {
                                        command.CommandText = $"UPDATE Sentences SET Questions = N'{WriteParts(newTopics[i].Sentences[j].questions)}' WHERE Id = {Int32.Parse(newTopics[i].Sentences[j].id)}";
                                        command.ExecuteNonQuery();
                                    }
                                    if (newTopics[i].Sentences[j].contexts != oldTopics[i].Sentences[j].contexts)
                                    {
                                        command.CommandText = $"UPDATE Sentences SET Contexts = N'{WriteParts(newTopics[i].Sentences[j].contexts)}' WHERE Id = {Int32.Parse(newTopics[i].Sentences[j].id)}";
                                        command.ExecuteNonQuery();
                                    }
                                    if (newTopics[i].Sentences[j].translations != oldTopics[i].Sentences[j].translations)
                                    {
                                        command.CommandText = $"UPDATE Sentences SET Translations = N'{WriteParts(newTopics[i].Sentences[j].translations)}' WHERE Id = {Int32.Parse(newTopics[i].Sentences[j].id)}";
                                        command.ExecuteNonQuery();
                                    }
                                    if (newTopics[i].Sentences[j].waysToQuestionsVoice != oldTopics[i].Sentences[j].waysToQuestionsVoice)
                                    {
                                        command.CommandText = $"UPDATE Sentences SET WaysToQuestionsVoice = N'{WriteParts(newTopics[i].Sentences[j].waysToQuestionsVoice)}' WHERE Id = {Int32.Parse(newTopics[i].Sentences[j].id)}";
                                        command.ExecuteNonQuery();
                                    }
                                    if (newTopics[i].Sentences[j].wayToSentenceVoice != oldTopics[i].Sentences[j].wayToSentenceVoice)
                                    {
                                        command.CommandText = $"UPDATE Sentences SET WayToSentenceVoice = N'{newTopics[i].Sentences[j].wayToSentenceVoice}' WHERE Id = {Int32.Parse(newTopics[i].Sentences[j].id)}";
                                        command.ExecuteNonQuery();
                                    }
                                }
                                else
                                {
                                    command.CommandText = $"INSERT INTO Sentences (TopicId, IsQuestionFirst, Questions, Contexts, Translations, WaysToQuestionsVoice," +
                                        $" WayToSentenceVoice)" +
                                        $" VALUES({Int32.Parse(newTopics[i].Id)}, N'{newTopics[i].Sentences[j].isQuestionFirst.ToString()}', N'{WriteParts(newTopics[i].Sentences[j].questions)}'," +
                                        $"N'{WriteParts(newTopics[i].Sentences[j].contexts)}', N'{WriteParts(newTopics[i].Sentences[j].translations)}', " +
                                        $"N'{WriteParts(newTopics[i].Sentences[j].waysToQuestionsVoice)}', N'{newTopics[i].Sentences[j].wayToSentenceVoice}')";
                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                    }
                    else
                    {
                        command.CommandText = $"INSERT INTO Topics (UserId, Name, SequenceOfIndices, CurrentIndex, TempId) VALUES" +
                                $"({oldUser.Id}, N'{newTopics[i].Name}', N'{WriteSequence(newTopics[i].SequenceOfIndices)}', {newTopics[i].CurrentIndex}, N'{newTopics[i].Id}')";
                        command.ExecuteNonQuery();

                        if (newUser.CurrentTopicId.StartsWith("temp"))
                        {
                            command.CommandText = $"SELECT Id FROM Topics WHERE UserId = {oldUser.Id} AND TempId = N'{newUser.CurrentTopicId}'";
                            reader = command.ExecuteReader();
                            reader.Read();
                            int id = reader.GetInt32(0);
                            reader.Close();
                            command.CommandText = $"UPDATE Users SET CurrentTopicId = N'{id}' WHERE Id = {oldUser.Id}";
                            command.ExecuteNonQuery();
                        }
                        command.CommandText = $"UPDATE Topics SET TempId = '' WHERE UserId = {oldUser.Id}";
                        command.ExecuteNonQuery();

                        command.CommandText = $"SELECT MAX(Id) FROM Topics WHERE UserId = {oldUser.Id}";
                        reader = command.ExecuteReader();
                        reader.Read();
                        int topicId = reader.GetInt32(0);
                        reader.Close();
                        
                        for (int j = 0; j < newTopics[i].Sentences.Count; j++)
                        {
                            command.CommandText = $"INSERT INTO Sentences (TopicId, IsQuestionFirst, Questions, Contexts, Translations, WaysToQuestionsVoice," +
                                    $" WayToSentenceVoice)" +
                                    $" VALUES({topicId}, N'{newTopics[i].Sentences[j].isQuestionFirst.ToString()}', N'{WriteParts(newTopics[i].Sentences[j].questions)}'," +
                                    $"N'{WriteParts(newTopics[i].Sentences[j].contexts)}', N'{WriteParts(newTopics[i].Sentences[j].translations)}', " +
                                    $"N'{WriteParts(newTopics[i].Sentences[j].waysToQuestionsVoice)}', N'{newTopics[i].Sentences[j].wayToSentenceVoice}')";
                            command.ExecuteNonQuery();
                        }
                    }

                }

                if (indicesOfDeletedTopics.Count != 0)
                {
                    for (int i = 0; i < indicesOfDeletedTopics.Count; i++)
                    {
                        command.CommandText = $"DELETE FROM Sentences WHERE TopicId = {indicesOfDeletedTopics[i]}";
                        command.ExecuteNonQuery();
                        command.CommandText = $"DELETE FROM Topics WHERE Id = {indicesOfDeletedTopics[i]}";
                        command.ExecuteNonQuery();
                    }
                }

                if (deletedSenteces.Count != 0)
                {
                    for (int i = 0; i < deletedSenteces.Count; i++)
                    {
                        if (!indicesOfDeletedTopics.Contains(Int32.Parse(deletedSenteces[i].topicId)))
                        {
                            command.CommandText = $"DELETE FROM Sentences WHERE Id = {Int32.Parse(deletedSenteces[i].id)}";
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}