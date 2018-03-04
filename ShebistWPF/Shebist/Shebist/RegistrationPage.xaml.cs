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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Net;
using System.Net.Mail;
using System.IO;

namespace Shebist
{
    /// <summary>
    /// Логика взаимодействия для RegistrationPage.xaml
    /// </summary>
    public partial class RegistrationPage : Page
    {
        public RegistrationPage()
        {
            InitializeComponent();
        }

        string cd = Directory.GetCurrentDirectory();
        SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);


        static System.IO.DirectoryInfo myDirectory = new DirectoryInfo(Environment.CurrentDirectory);
        static string parentDirectory = myDirectory.Parent.FullName;
        static System.IO.DirectoryInfo myDirectory2 = new DirectoryInfo(parentDirectory);
        static string parentDirectory2 = myDirectory2.Parent.FullName;

        string connectionString = $@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename={parentDirectory2}\UserDB.mdf;Integrated Security=True";

        MailAddress from = new MailAddress("alexanderyershov1@gmail.com", "Шебист");
        private void RegistrationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
            }
            catch (SqlException)
            {
                MessageBox.Show("Пользователь с такими данными уже существует");
            }

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"INSERT INTO UserDB (Login, Name, Email, Password) VALUES (N'{this.LoginTextBox.Text}', N'{this.NameTextBox.Text}', N'{this.EmailTextBox.Text}, N'{this.PasswordTextBox.Text}')", connection);
                command.ExecuteNonQuery();
                MessageBox.Show("Вы зарегистрированы");

            }
            if (EmailTextBox.Text != "")
            {
                MailAddress to = new MailAddress($"{EmailTextBox.Text}");
                MailMessage mailMessage = new MailMessage(from, to);
                mailMessage.Subject = "Регистрация";
                mailMessage.Body = $"{NameTextBox.Text}, вы успешно зарегистрировались, ваши данные для входа:" +
                    $"\nЛогин: {LoginTextBox.Text}" +
                    $"\nПароль: {PasswordTextBox.Text}";
                smtp.EnableSsl = true;
                smtp.Credentials = new NetworkCredential("alexanderyershov1@gmail.com", "Death4000$");
                smtp.Send(mailMessage);

            }
        }

        private void AlreadyHaveAnAccountLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            AuthorizationPage ap = new AuthorizationPage();
            this.NavigationService.Navigate(ap);
        }
    }
}
