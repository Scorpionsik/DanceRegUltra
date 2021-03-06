﻿using DanceRegUltra.Interfaces;
using System;
using System.Data.Common;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace DanceRegUltra.Static
{
    internal static class DanceRegDatabase
    {
        public static MatchCollection DatabaseCommands
        {
            get => new Regex(@"(?=(create|insert))[^;]+", RegexOptions.IgnoreCase).Matches(DanceRegUltra.Properties.Resources.dance);
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

        private static SemaphoreSlim DatabaseMutex;

        private static SqLiteDatabase.SqLiteDatabase Database;

        static DanceRegDatabase()
        {
            DanceRegDatabase.DatabaseMutex = new SemaphoreSlim(1,1);
            if (!Directory.Exists("database")) Directory.CreateDirectory("database");

            DanceRegDatabase.Database = new SqLiteDatabase.SqLiteDatabase("database/" + DatabaseName + "; Version=3; Pooling=True; Max Pool Size=100");
        }

        internal static async Task ManualOpen(string manual_id)
        {
            await Database.ManualOpenAsync(manual_id);
        }

        internal static void ManualClose(string manual_id)
        {
            Database.ManualClose(manual_id);
        }

        internal static async Task<int> ExecuteNonQueryAsync(string query)
        {
            DanceRegDatabase.event_StartTask?.Invoke();
            await DanceRegDatabase.DatabaseMutex.WaitAsync();
            int result = await Database.ExecuteNonQueryAsync(query);
            DanceRegDatabase.DatabaseMutex.Release();
            DanceRegDatabase.event_EndTask?.Invoke();
            return result;
        }

        internal static async Task<DbResult> ExecuteAndGetQueryAsync(string query)
        {
            DanceRegDatabase.event_StartTask?.Invoke();
            await DanceRegDatabase.DatabaseMutex.WaitAsync();
            DbResult result = await Database.ExecuteAndGetQueryAsync(query);
            DanceRegDatabase.DatabaseMutex.Release();
            DanceRegDatabase.event_EndTask?.Invoke();
            return result;
        }

        internal static bool IsExist()
        {
            return File.Exists("database/" + DatabaseName);
        }
    }
}
