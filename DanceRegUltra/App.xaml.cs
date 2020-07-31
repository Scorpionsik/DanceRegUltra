using System;
using System.Threading;
using System.Windows;

namespace DanceRegUltra
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string AppTitle = "Танцевальный менеджер";

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
            if (!App.InstanceCheck())
            {
                MessageBox.Show("Программа уже запущена!", App.AppTitle, MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Environment.Exit(0);
            }
        }
    }
}
