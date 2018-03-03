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
    /// Логика взаимодействия для AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        public AuthorizationPage()
        {
            InitializeComponent();
        }

        string cd = Directory.GetCurrentDirectory();
        BinaryFormatter formatter = new BinaryFormatter();

        static System.IO.DirectoryInfo myDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        static string parentDirectory = myDirectory.Parent.FullName;
        static System.IO.DirectoryInfo myDirectory2 = new DirectoryInfo(parentDirectory);
        static string parentDirectory2 = myDirectory2.Parent.FullName;

        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={parentDirectory2}\UserDB.mdf;Integrated Security=True";

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT Name, Password FROM UserDB WHERE Name = N'{LoginTextBox.Text}' AND Password = '{PasswordTextBox.Text}'", connection);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    reader.Close();
                    MainPage mp = new MainPage();
                    this.NavigationService.Navigate(mp);

                }
                else
                {
                    reader.Close();
                    MessageBox.Show("Неверный логин или пароль");
                }
            }
            
            if (RememberMeCheckBox.IsChecked == true)
            {
                using(FileStream fs = new FileStream($"{cd}\\Data\\RememberMeCheckBoxIsChecked", FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, RememberMeCheckBox.IsChecked);
                }
                using (FileStream fs = new FileStream($"{cd}\\Data\\LoginTextBoxText", FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, LoginTextBox.Text);
                }
                using (FileStream fs = new FileStream($"{cd}\\Data\\PasswordTextBoxText", FileMode.OpenOrCreate))
                {
                    formatter.Serialize(fs, PasswordTextBox.Text);
                }
            }
        }
        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT Name, Password FROM UserDB WHERE Name = N'{LoginTextBox.Text}' AND Password = '{PasswordTextBox.Text}'", connection);
                SqlDataReader reader = command.ExecuteReader();
                if (reader.HasRows)
                {
                    MessageBox.Show("Пользователь с такими данными уже существует");
                    LoginTextBox.Text = "";
                    PasswordTextBox.Text = "";
                }
                else
                {
                    SqlCommand command2 = new SqlCommand($"INSERT INTO UserDB (Name, Password) VALUES (N'{LoginTextBox.Text}', N'{PasswordTextBox.Text}')", connection);
                    command.ExecuteNonQuery();
                    MessageBox.Show("Вы зарегистрированы");
                    AutorizationLabel.Content = "Авторизация";
                    LoginButton.Content = "Войти";
                    LoginButton.Click += LoginButton_Click;
                    LoginButton.Click -= RegistrationButton_Click;
                }
            }
        }

        private void AlreadyHaveAnAccountLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AutorizationLabel.Content = "Авторизация";
            LoginButton.Content = "Войти";
            LoginButton.Click += LoginButton_Click;
            LoginButton.Click -= RegistrationButton_Click;
        }

        private void NoAccountYet_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AutorizationLabel.Content = "Регистрация";
            LoginButton.Content = "Регистрация";
            LoginButton.Click -= LoginButton_Click;
            LoginButton.Click += RegistrationButton_Click;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (File.Exists($"{cd}\\Data\\RememberMeCheckBoxIsChecked"))
            {
                using (FileStream fs = new FileStream($"{cd}\\Data\\RememberMeCheckBoxIsChecked", FileMode.OpenOrCreate))
                {
                    RememberMeCheckBox.IsChecked = (bool)formatter.Deserialize(fs);
                }
            }
            if (File.Exists($"{cd}\\Data\\LoginTextBoxText"))
            {
                using (FileStream fs = new FileStream($"{cd}\\Data\\LoginTextBoxText", FileMode.OpenOrCreate))
                {
                    LoginTextBox.Text = (string)formatter.Deserialize(fs);
                }
            }
            if (File.Exists($"{cd}\\Data\\PasswordTextBoxText"))
            {
                using (FileStream fs = new FileStream($"{cd}\\Data\\PasswordTextBoxText", FileMode.OpenOrCreate))
                {
                    PasswordTextBox.Text = (string)formatter.Deserialize(fs);
                }
            }
        }
    }
}
