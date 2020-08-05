using DanceRegUltra.Interfaces;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace DanceRegUltra.Static
{
    internal static class DanceRegDatabase
    {
        public static MatchCollection DatabaseCommands
        {
            get => new Regex(@"(?=(create|insert))[^;]+").Matches(DanceRegUltra.Properties.Resources.dance);
        }

        private static string DatabaseName = "dancebase.sqlite3";

        private static event Action event_StartTask;
        /// <summary>
        /// Срабатывает, когда запускается новый поток для записи в базу данных
        /// </summary>
        public static event Action Event_StartTask
        {
            add
            {
                DanceRegDatabase.event_StartTask -= value;
                DanceRegDatabase.event_StartTask += value;
            }
            remove => DanceRegDatabase.event_StartTask -= value;
        }

        private static event Action event_EndTask;
        /// <summary>
        /// Срабатывает, когда поток записи в базу данных завершен
        /// </summary>
        public static event Action Event_EndTask
        {
            add
            {
                DanceRegDatabase.event_EndTask -= value;
                DanceRegDatabase.event_EndTask += value;
            }
            remove => DanceRegDatabase.event_EndTask -= value;
        }

        private static Mutex DatabaseMutex;

        internal static SqLiteDatabase.SqLiteDatabase Database { get; private set; }

        static DanceRegDatabase()
        {
            DanceRegDatabase.DatabaseMutex = new Mutex();
            if (!Directory.Exists("database")) Directory.CreateDirectory("database");

            DanceRegDatabase.Database = new SqLiteDatabase.SqLiteDatabase("database/" + DatabaseName);
        }

        internal static async void MethodUpdateMember(int eventId, int memberId, string dataColumn, object currentData = null, UpdateStatus status = UpdateStatus.Default, object replaceData = null)
        {
            DanceRegDatabase.event_StartTask?.Invoke();
            DanceRegDatabase.DatabaseMutex.WaitOne();

            DanceRegDatabase.DatabaseMutex.ReleaseMutex();
            DanceRegDatabase.event_EndTask?.Invoke();
        }

        internal static async void MethodUpdateEvent(int eventId, string column)
        {
            DanceRegDatabase.event_StartTask?.Invoke();
            DanceRegDatabase.DatabaseMutex.WaitOne();

            DanceRegDatabase.DatabaseMutex.ReleaseMutex();
            DanceRegDatabase.event_EndTask?.Invoke();
        }

        internal static bool IsExist()
        {
            return File.Exists("database/" + DatabaseName);
        }
    }
}
