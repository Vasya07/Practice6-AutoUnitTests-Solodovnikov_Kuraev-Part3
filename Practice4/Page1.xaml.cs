using System;
using System.Windows;
using System.Windows.Controls;

namespace Практическая_работа_4_Солодовников_Кураев
{   
    public partial class Page1 : Page
    {
        public Page1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Вычисляет значение функции согласно варианту 12
        /// </summary>
        /// <param name="x">Параметр X</param>
        /// <param name="y">Параметр Y (не может быть 0)</param>
        /// <param name="z">Параметр Z</param>
        /// <returns>Результат вычисления</returns>
        /// <exception cref="DivideByZeroException">Выбрасывается при y=0 или arctg(z)=0</exception>
        public double CalculateFunction(double x, double y, double z)
        {
            if (y == 0)
                throw new DivideByZeroException("Y не может быть равен 0 (деление на ноль)");

            double yPowX = Math.Pow(y, x);
            double firstTerm = Math.Pow(2, yPowX);
            double secondTerm = Math.Pow(3, x) * y;
            double arctgZ = Math.Atan(z);

            if (arctgZ == 0)
                throw new DivideByZeroException("arctg(z) не может быть равен 0");

            double arctgPower = Math.Pow(arctgZ, -Math.PI / 6.0);
            double denominator = Math.Abs(x) + 1.0 / (y * y + 1);
            double fraction = y * arctgPower / denominator;
            double result = firstTerm + secondTerm - fraction;

            return result;
        }

        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!double.TryParse(tbx.Text, out double x))
                    throw new FormatException("Поле X должно быть заполнено числом!");
                if (!double.TryParse(tby.Text, out double y))
                    throw new FormatException("Поле Y должно быть заполнено числом!");
                if (!double.TryParse(tbz.Text, out double z))
                    throw new FormatException("Поле Z должно быть заполнено числом!");

                double result = CalculateFunction(x, y, z);
                ResultTextBox.Text = result.ToString("F5");
            }
            catch (Exception ex)
            {
                ResultTextBox.Text = "ERROR";
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            tbx.Clear();
            tby.Clear();
            tbz.Clear();
            ResultTextBox.Clear();
        }
    }
}