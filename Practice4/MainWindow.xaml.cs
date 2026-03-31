using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Media;
using System.Windows;
using Практическая_работа_4_Солодовников_Кураев.Pages;

namespace Практическая_работа_4_Солодовников_Кураев
{
    public partial class MainWindow : Window
    {
        private SoundPlayer _easterEggPlayer;

        public MainWindow()
        {
            InitializeComponent();
            MainFrame.Navigate(new Page1());
            InitializeEasterEgg();
        }

        private void InitializeEasterEgg()
        {
            try
            {
                string soundPath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    "Secret.wav");

                if (!File.Exists(soundPath))
                {
                    soundPath = System.IO.Path.Combine(
                        AppDomain.CurrentDomain.BaseDirectory,
                        "Secret.wav");
                }
                if (File.Exists(soundPath))
                {
                    _easterEggPlayer = new SoundPlayer(soundPath);
                    _easterEggPlayer.LoadAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось инициализировать звук: {ex.Message}",
                    "Упс", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ButtonPage1_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Page1());
        }

        private void ButtonPage2_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Page2());
        }

        private void ButtonPage3_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new Page3());
        }

        private void ButtonAuthPage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new AuthPage());
        }

        private void ButtonRegPage_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new RegPage());
        }

        private void EasterEggButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_easterEggPlayer != null)
                {
                    _easterEggPlayer.Play();
                }
                else
                {
                    MessageBox.Show("Аудиофайл не найден!\n" +
                                   "Ошибка",
                                   "Упс",
                                   MessageBoxButton.OK,
                                   MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка воспроизведения: {ex.Message}",
                    "Упс", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы уверены, что хотите выйти из приложения?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                e.Cancel = true;
            }
            else
            {
                if (_easterEggPlayer != null)
                {
                    _easterEggPlayer.Stop();
                    _easterEggPlayer.Dispose();
                }
            }

            base.OnClosing(e);
        }
    }
}