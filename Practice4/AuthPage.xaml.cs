using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Практическая_работа_4_Солодовников_Кураев.Database;

namespace Практическая_работа_4_Солодовников_Кураев.Pages
{
    public partial class AuthPage : Page
    {
        private Dictionary<string, string> _testUsers;
        private DatabaseHelper _dbHelper;
        private int _failedAttempts = 0;
        private string _currentCaptchaCode = "";
        private bool _captchaRequired = false;

        public AuthPage()
        {
            InitializeComponent();
            _dbHelper = new DatabaseHelper();
        }

        public AuthPage(Dictionary<string, string> testUsers)
        {
            InitializeComponent();
            _testUsers = testUsers;
        }

        /// <summary>
        /// Метод авторизации
        /// </summary>
        public bool Auth(string login, string password, string captchaCode = null)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Логин не может быть пустым");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Пароль не может быть пустым");

            if (_captchaRequired)
            {
                if (string.IsNullOrWhiteSpace(captchaCode))
                    throw new ArgumentException("Требуется ввод CAPTCHA");

                if (captchaCode != _currentCaptchaCode)
                    throw new ArgumentException("Неверный код CAPTCHA");
            }

            if (_testUsers != null)
            {
                if (!_testUsers.ContainsKey(login))
                    return false;
                return _testUsers[login] == password;
            }

            return _dbHelper.ValidateUser(login, password);
        }

        /// <summary>
        /// Метод для проверки с подсчетом попыток
        /// </summary>
        public bool AuthWithAttempts(string login, string password, string captchaCode = null)
        {
            try
            {
                bool result = Auth(login, password, captchaCode);

                if (!result)
                {
                    _failedAttempts++;
                    CheckAndShowCaptcha();
                }
                else
                {
                    _failedAttempts = 0;
                    _captchaRequired = false;
                    HideCaptcha();
                }

                return result;
            }
            catch (ArgumentException)
            {
                _failedAttempts++;
                CheckAndShowCaptcha();
                throw;
            }
        }

        /// <summary>
        /// Проверка, нужно ли показывать CAPTCHA
        /// </summary>
        public bool NeedCaptcha()
        {
            return _failedAttempts >= 3;
        }

        private void CheckAndShowCaptcha()
        {
            if (NeedCaptcha())
            {
                _captchaRequired = true;
                ShowCaptcha();
            }
        }

        /// <summary>
        /// Генерация случайного кода CAPTCHA
        /// </summary>
        private string GenerateCaptchaCode()
        {
            Random random = new Random();
            StringBuilder code = new StringBuilder();
            string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            for (int i = 0; i < 5; i++)
            {
                code.Append(chars[random.Next(chars.Length)]);
            }
            return code.ToString();
        }

        /// <summary>
        /// Отрисовка CAPTCHA с шумом
        /// </summary>
        private void DrawCaptcha(string code)
        {
            _currentCaptchaCode = code;
            CaptchaCanvas.Children.Clear();

            Random random = new Random();
            RenderTargetBitmap renderBitmap = new RenderTargetBitmap(200, 60, 96, 96, PixelFormats.Pbgra32);
            DrawingVisual drawingVisual = new DrawingVisual();

            using (DrawingContext dc = drawingVisual.RenderOpen())
            {
                dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, 200, 60));

                for (int i = 0; i < 12; i++)
                {
                    Pen linePen = new Pen(
                        new SolidColorBrush(Color.FromArgb(100,
                            (byte)random.Next(100, 200),
                            (byte)random.Next(100, 200),
                            (byte)random.Next(100, 200))),
                        random.Next(1, 3));

                    dc.DrawLine(linePen,
                        new Point(random.Next(0, 200), random.Next(0, 60)),
                        new Point(random.Next(0, 200), random.Next(0, 60)));
                }

                for (int i = 0; i < 250; i++)
                {
                    Brush pointBrush = new SolidColorBrush(Color.FromArgb(80,
                        (byte)random.Next(0, 255),
                        (byte)random.Next(0, 255),
                        (byte)random.Next(0, 255)));

                    dc.DrawRectangle(pointBrush, null,
                        new Rect(random.Next(0, 200), random.Next(0, 60), 1, 1));
                }

                for (int i = 0; i < 6; i++)
                {
                    Brush ellipseBrush = new SolidColorBrush(Color.FromArgb(40,
                        (byte)random.Next(0, 255),
                        (byte)random.Next(0, 255),
                        (byte)random.Next(0, 255)));

                    dc.DrawEllipse(ellipseBrush, null,
                        new Point(random.Next(0, 200), random.Next(0, 60)),
                        random.Next(5, 15), random.Next(5, 15));
                }

                for (int i = 0; i < code.Length; i++)
                {
                    FormattedText letterText = new FormattedText(
                        code[i].ToString(),
                        System.Globalization.CultureInfo.CurrentCulture,
                        FlowDirection.LeftToRight,
                        new Typeface("Arial Black"),
                        random.Next(18, 26),
                        new SolidColorBrush(Color.FromRgb(
                            (byte)random.Next(0, 100),
                            (byte)random.Next(0, 100),
                            (byte)random.Next(100, 200))),
                        VisualTreeHelper.GetDpi(this).PixelsPerDip);

                    double letterX = 15 + (i * 30) + random.Next(-5, 5);
                    double letterY = random.Next(10, 35);
                    dc.DrawText(letterText, new Point(letterX, letterY));
                }
            }

            renderBitmap.Render(drawingVisual);
            Image captchaImage = new Image();
            captchaImage.Source = renderBitmap;
            captchaImage.Width = 200;
            captchaImage.Height = 60;
            CaptchaCanvas.Children.Add(captchaImage);
        }

        private void ShowCaptcha()
        {
            string newCode = GenerateCaptchaCode();
            DrawCaptcha(newCode);
            CaptchaPanel.Visibility = Visibility.Visible;
        }

        private void HideCaptcha()
        {
            CaptchaPanel.Visibility = Visibility.Collapsed;
            CaptchaInput.Clear();
            _currentCaptchaCode = "";
        }

        private void RefreshCaptchaButton_Click(object sender, RoutedEventArgs e)
        {
            string newCode = GenerateCaptchaCode();
            DrawCaptcha(newCode);
            CaptchaInput.Clear();
        }

        private void CaptchaCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            RefreshCaptchaButton_Click(sender, e);
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string login = TextBoxLogin.Text;
                string password = PasswordBox.Password;
                string captchaCode = _captchaRequired ? CaptchaInput.Text : null;

                if (AuthWithAttempts(login, password, captchaCode))
                {
                    StatusText.Text = "Вход выполнен успешно!";
                    StatusText.Foreground = System.Windows.Media.Brushes.Green;
                    var mainWindow = Application.Current.MainWindow as MainWindow;
                    if (mainWindow != null)
                    {
                        mainWindow.MainFrame.Navigate(new Page1());
                    }
                    if (_captchaRequired)
                    {
                        _captchaRequired = false;
                        HideCaptcha();
                    }
                }
                else
                {
                    if (_captchaRequired)
                    {
                        StatusText.Text = "Неверный логин, пароль или CAPTCHA";
                        RefreshCaptchaButton_Click(sender, e);
                    }
                    else
                    {
                        int attemptsLeft = 3 - _failedAttempts;
                        if (attemptsLeft > 0)
                        {
                            StatusText.Text = $"Неверный логин или пароль. Осталось попыток: {attemptsLeft}";
                        }
                        else
                        {
                            StatusText.Text = "Превышено количество попыток. Требуется CAPTCHA";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                StatusText.Text = ex.Message;
                StatusText.Foreground = System.Windows.Media.Brushes.Red;
                if (_captchaRequired)
                {
                    RefreshCaptchaButton_Click(sender, e);
                }
            }
        }

        public void ResetFailedAttempts()
        {
            _failedAttempts = 0;
            _captchaRequired = false;
            HideCaptcha();
        }

        public int GetFailedAttempts()
        {
            return _failedAttempts;
        }

        public Dictionary<string, string> GetUsers()
        {
            return _testUsers;
        }

        public string GetCurrentCaptchaCode()
        {
            return _currentCaptchaCode;
        }

        public void RefreshCaptcha()
        {
            if (_captchaRequired)
            {
                string newCode = GenerateCaptchaCode();
                DrawCaptcha(newCode);
                CaptchaInput.Clear();
            }
        }
    }
}