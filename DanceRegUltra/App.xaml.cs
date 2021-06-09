using CoreWPF.Utilites;
using DanceRegUltra.Models.PrintTempletes;
using PrintTemplate.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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

        public static string CapitalizeAllWords(string s)
        {
            var sb = new StringBuilder(s.Length);
            bool inWord = false;
            foreach (var c in s)
            {
                if (char.IsLetter(c))
                {
                    sb.Append(inWord ? char.ToLower(c) : char.ToUpper(c));
                    inWord = true;
                }
                else
                {
                    sb.Append(c);
                    inWord = false;
                }
            }
            return sb.ToString();
        }

        public static bool PrintPages(string title, IPrintTemplate temp)
        {
            bool isPrint = false;
            PrintTemplate.Views.MainView window = new PrintTemplate.Views.MainView(title, temp.GetPages(), true, true);
            window.EventIsPrint += new Action<bool>(value =>
            {
                isPrint = value;
            });
            window.ShowDialog();
            return isPrint;
        }
    }
}
