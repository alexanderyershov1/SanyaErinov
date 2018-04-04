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

        static string Scheduler = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).ToString();
        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={Scheduler}\Database1.mdf;Integrated Security=True";
        SqlCommand command = new SqlCommand();
        SqlDataReader reader;

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                command.CommandText = $"SELECT Note FROM Notes WHERE Date = '{DatePicker.SelectedDate.ToString()}'";
                command.Connection = connection;
                reader = command.ExecuteReader();

                reader.Read();
                if (reader.HasRows)
                {
                    TextBox.Text = reader.GetString(0);

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
                command.CommandText = $"SELECT Note FROM Notes WHERE Date = '{DatePicker.SelectedDate.ToString()}'";
                command.Connection = connection;
                reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Close();
                    command.CommandText = $"UPDATE Notes SET Note = N'{TextBox.Text}' WHERE Date = '{DatePicker.SelectedDate.ToString()}'";
                    command.ExecuteNonQuery();

                }
                else
                {
                    reader.Close();
                    command.CommandText = $"INSERT INTO Notes (Date, Note) VALUES('{DatePicker.SelectedDate.ToString()}', N'{TextBox.Text}')";
                    command.ExecuteNonQuery();
                }

                reader.Close();
            }
            
        }

        BinaryFormatter formatter = new BinaryFormatter();

        string day, month, year;

        private void FontSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (FontSizeTextBox.Text != "")
                TextBox.FontSize = Int32.Parse(FontSizeTextBox.Text);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            using (FileStream fs = new FileStream("Data\\TextBoxFontFamily", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, TextBox.FontFamily.ToString());
            }
            using (FileStream fs = new FileStream("Data\\TextBoxFontSize", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, TextBox.FontSize);
            }
            using (FileStream fs = new FileStream("Data\\FontSizeTextBoxText", FileMode.OpenOrCreate))
            {
                formatter.Serialize(fs, FontSizeTextBox.Text);
            }
        }

        private void ArialComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            TextBox.FontFamily  = new FontFamily("Arial");
        }

        private void TimesNewRomanComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            TextBox.FontFamily = new FontFamily("Times New Roman");
        }

        private void CalibriComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {
            TextBox.FontFamily = new FontFamily("Calibri");
        }

        private void FontSizeTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !(Char.IsDigit(e.Text, 0));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {  
            if (File.Exists("Data\\TextBoxFontFamily"))
            {
                using (FileStream fs = new FileStream("Data\\TextBoxFontFamily", FileMode.OpenOrCreate))
                {
                    TextBox.FontFamily = new FontFamily((string)formatter.Deserialize(fs));
                }
            }
            if (File.Exists("Data\\TextBoxFontSize"))
            {
                using (FileStream fs = new FileStream("Data\\TextBoxFontSize", FileMode.OpenOrCreate))
                {
                    TextBox.FontSize = (double)formatter.Deserialize(fs);
                }
            }
            if (File.Exists("Data\\FontSizeTextBoxText"))
            {
                using (FileStream fs = new FileStream("Data\\FontSizeTextBoxText", FileMode.OpenOrCreate))
                {
                    FontSizeTextBox.Text = (string)formatter.Deserialize(fs);
                }
            }

            TextBox.Text = DateTime.Today.ToString();
            DatePicker.SelectedDate = Convert.ToDateTime(TextBox.Text);

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT Note FROM Notes WHERE Date = '{DatePicker.SelectedDate.ToString()}'", connection);
                command.Connection = connection;
                SqlDataReader reader = command.ExecuteReader();

                reader.Read();
                if (reader.HasRows)
                {
                    TextBox.Text = reader.GetString(0);
                }
            }
        }
    }
}

