using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace Практическая_работа_4_Солодовников_Кураев.Pages
{
    public partial class RegPage : Page
    {
        private Dictionary<string, string> _users;
        public RegPage(Dictionary<string, string> users = null)
        {
            InitializeComponent();
            _users = users ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Метод регистрации для тестирования
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

            if (_users.ContainsKey(login))
                throw new ArgumentException("Пользователь с таким логином уже существует!");

            if (!Regex.IsMatch(login, @"^[a-zA-Z0-9_]+$"))
                throw new ArgumentException("Логин может содержать только буквы, цифры и знак подчеркивания!");
            _users.Add(login, password);
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