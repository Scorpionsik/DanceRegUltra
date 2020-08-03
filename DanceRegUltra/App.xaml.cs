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
        public static string AppTitle = "Танцевальный менеджер";

        public static SqLiteDatabase.SqLiteDatabase Database { get; private set; }
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

            if (!Directory.Exists("databases")) Directory.CreateDirectory("databases");

            App.Database = new SqLiteDatabase.SqLiteDatabase("databases/dancebase.sqlite3");
            App.Database.ExecuteNonQuery("create table if not exists 'Test' ('Id' integer primary key autoincrement not null unique, 'Name' varchar(150))");
        }
    }
}
