using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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
using System.Runtime.Serialization.Formatters.Binary;

namespace Scheduler
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
       
        static System.IO.DirectoryInfo myDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        static string parentDirectory = myDirectory.Parent.FullName;
        static System.IO.DirectoryInfo myDirectory2 = new DirectoryInfo(parentDirectory);
        static string parentDirectory2 = myDirectory2.Parent.FullName;

        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={parentDirectory2}\Database1.mdf;Integrated Security=True";

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT Note FROM Notes WHERE Date = '{DatePicker.SelectedDate.ToString()}'", connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows) // если есть данные
                {
                    while (reader.Read()) // построчно считываем данные
                    {
                        TextBox.Text = (string)reader.GetValue(0);
                    }
                }
                else TextBox.Clear();

                reader.Close();
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT Note FROM Notes WHERE Date = '{DatePicker.SelectedDate.ToString()}'", connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Close();
                    SqlCommand command2 = new SqlCommand($"UPDATE Notes SET Note = N'{TextBox.Text}' WHERE Date = '{DatePicker.SelectedDate.ToString()}'", connection);
                    command2.ExecuteNonQuery();

                }
                else
                {
                    reader.Close();
                    SqlCommand command3 = new SqlCommand($"INSERT INTO Notes (Date, Note) VALUES('{DatePicker.SelectedDate.ToString()}', N'{TextBox.Text}')", connection);
                    command3.ExecuteNonQuery();
                }

                reader.Close();
            }
        }

        BinaryFormatter formatter = new BinaryFormatter();
        private void Window_Closed(object sender, EventArgs e)
        {
           
            using (FileStream fs = new FileStream("DatePickerSelectedDate", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, DatePicker.SelectedDate);
            }
            using (FileStream fs = new FileStream("TextBoxText", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, TextBox.Text);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists("DatePickerSelectedDate"))
            {
                using (FileStream fs = new FileStream("DatePickerSelectedDate", FileMode.OpenOrCreate))
                {
                    DatePicker.SelectedDate = (DateTime)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("TextBoxText"))
            {
                using (FileStream fs = new FileStream("TextBoxText", FileMode.OpenOrCreate))
                {
                    TextBox.Text = (string)formatter.Deserialize(fs);
                }
            }
        }
    }
}

