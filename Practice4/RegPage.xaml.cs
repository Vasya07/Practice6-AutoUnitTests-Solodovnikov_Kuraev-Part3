using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using Практическая_работа_4_Солодовников_Кураев.Database;

namespace Практическая_работа_4_Солодовников_Кураев.Pages
{
    public partial class RegPage : Page
    {
        private Dictionary<string, string> _testUsers;
        private DatabaseHelper _dbHelper;

        public RegPage()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper();
        }

        public RegPage(Dictionary<string, string> testUsers)
        {
            InitializeComponent();
            _testUsers = testUsers;
        }

        /// <summary>
        /// Метод регистрации
        /// </summary>
        public bool Register(string login, string password, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Логин не может быть пустым!");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Пароль не может быть пустым!");

            if (password.Length < 4)
                throw new ArgumentException("Пароль должен содержать как минимум 4 символа!");

            if (password != confirmPassword)
                throw new ArgumentException("Пароли не совпадают!");

            if (!Regex.IsMatch(login, @"^[a-zA-Z0-9_]+$"))
                throw new ArgumentException("Логин может содержать только буквы, цифры и знак подчеркивания!");

            if (_testUsers != null)
            {
                if (_testUsers.ContainsKey(login))
                    throw new ArgumentException("Пользователь с таким логином уже существует!");

                _testUsers.Add(login, password);
                return true;
            }

            if (_dbHelper == null)
                throw new Exception("Ошибка инициализации подключения к базе данных");

            if (_dbHelper.UserExists(login))
                throw new ArgumentException("Пользователь с таким логином уже существует!");

            bool result = _dbHelper.AddUser(login, password, "", login);
            if (!result)
                throw new Exception("Ошибка при сохранении пользователя в базу данных");

            return true;
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = TextBoxLogin.Text;
                string password = PasswordBox.Password;
                string confirmPassword = ConfirmPasswordBox.Password;

                if (Register(login, password, confirmPassword))
                {
                    StatusText.Text = "Регистрация успешно завершена!";
                    StatusText.Foreground = System.Windows.Media.Brushes.Green; 
                    NavigationService?.Navigate(new AuthPage());
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = ex.Message;
                StatusText.Foreground = System.Windows.Media.Brushes.Red;
            }
        }
    }
}