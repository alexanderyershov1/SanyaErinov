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

namespace Shebist
{
    /// <summary>
    /// Логика взаимодействия для AccountPage.xaml
    /// </summary>
    public partial class AccountPage : Page
    {
        public AccountPage()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            AuthorizationPage ap = new AuthorizationPage();
            this.NavigationService.Navigate(ap);
        }

        private void BackMenuItem_Click(object sender, RoutedEventArgs e)
        {
            MainPage mp = new MainPage();
            this.NavigationService.Navigate(mp);
        }

        static System.IO.DirectoryInfo myDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        static string parentDirectory = myDirectory.Parent.FullName;
        static System.IO.DirectoryInfo myDirectory2 = new DirectoryInfo(parentDirectory);
        static string parentDirectory2 = myDirectory2.Parent.FullName;

        public string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=
        {parentDirectory2}\UserDB.mdf;Integrated Security=True";

        private void AccountPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists($"{myDirectory}\\Data\\LoginTextBoxText"))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                using (FileStream fs = new FileStream($"{myDirectory}\\Data\\LoginTextBoxText", FileMode.OpenOrCreate))
                {
                    LoginTextBox.Text = (string)formatter.Deserialize(fs);
                }

                using(SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand($"SELECT Name, Email, Password FROM UserDB WHERE Login = '{LoginTextBox.Text}'", connection);
                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        NameTextBox.Text = (string)reader.GetValue(0);
                        EmailTextBox.Text = (string)reader.GetValue(1);
                        PasswordTextBox.Text = (string)reader.GetValue(2);
                    }
                    reader.Close();
                }
            }
        }

    }
}
