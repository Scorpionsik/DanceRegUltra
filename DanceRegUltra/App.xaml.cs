﻿using CoreWPF.Utilites;
using System;
using System.IO;
using System.Threading;
using System.Windows;

namespace DanceRegUltra
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static TimeSpan Locality;

        public static string AppTitle;

        /// Хранит именованный мьютекс, чтобы сохранить владение им до конца пробега программы
        private static Mutex InstanceCheckMutex;

        /// <summary>
        /// Проверяем, запущено ли приложение
        /// </summary>
        /// <returns>Возвращает true, если приложение не запущено, иначе false</returns>
        private static bool InstanceCheck()
        {
            bool isNew;
            App.InstanceCheckMutex = new Mutex(true, App.AppTitle, out isNew);
            return isNew;
        } //---метод InstanceCheck

        public App()
        {
            AppTitle = "Танцевальный менеджер";
            Locality = DateTimeOffset.Now.Offset;
            if (!App.InstanceCheck())
            {
                MessageBox.Show("Программа уже запущена!", App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Environment.Exit(0);
            }
        }

        public static MessageBoxResult SetMessageBox(string text, MessageBoxButton buttons, MessageBoxImage image, string subtitle = "")
        {
            return MessageBox.Show(text, AppTitle + (subtitle != null && subtitle.Length > 0 ? ": " + subtitle : ""), buttons, image);
        }
    }
}
