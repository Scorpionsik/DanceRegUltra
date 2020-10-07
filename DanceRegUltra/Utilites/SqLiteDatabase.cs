using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqLiteDatabase
{
    public class SqLiteDatabase
    {
        private List<string> ManualOpenId;
        public bool IsManualOpen { get; private set; }

        private SQLiteConnection Connection;

        public SqLiteDatabase(string database_path)
        {
            this.IsManualOpen = false;
            this.ManualOpenId = new List<string>();
            this.Connection = new SQLiteConnection("Data Source=" + database_path + ";");
        }

        ~SqLiteDatabase()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            try
            {
                this.Connection.Dispose();
            }
            catch { }
        }

        public void ManualOpen(string manual_id)
        {
            if (!this.IsManualOpen)
            {
                this.Connection.Open();
                this.IsManualOpen = true;
            }
            if (!this.ManualOpenId.Contains(manual_id)) this.ManualOpenId.Add(manual_id);
        }

        public async Task ManualOpenAsync(string manual_id)
        {
            if (!this.IsManualOpen)
            {
                await this.Connection.OpenAsync();
                this.IsManualOpen = true;
            }
            if (!this.ManualOpenId.Contains(manual_id)) this.ManualOpenId.Add(manual_id);
        }

        public void ManualClose(string manual_id)
        {
            if (this.IsManualOpen)
            {
                this.ManualOpenId.Remove(manual_id);
                if (this.ManualOpenId.Count == 0)
                {
                    this.Connection.Close();
                    this.IsManualOpen = false;
                }
            }
        }

        public event EventHandler Disposed
        {
            add
            {
                this.Connection.Disposed -= value;
                this.Connection.Disposed += value;
            }
            remove => this.Connection.Disposed -= value;
        }

        public DbResult ExecuteAndGetQuery(string query)
        {
            if (new Regex("^select", RegexOptions.IgnoreCase).IsMatch(query))
            {
                if(!this.IsManualOpen) this.Connection.Open();
                SQLiteCommand command = new SQLiteCommand(this.UpdateString(query), this.Connection);
                DbResult res = new DbResult(command.ExecuteReader());
                command.Dispose();
                if (!this.IsManualOpen) this.Connection.Close();
                return res;
            }
            else return DbResult.Empty;
        }

        public async Task<DbResult> ExecuteAndGetQueryAsync(string query)
        {
            if (new Regex("^select", RegexOptions.IgnoreCase).IsMatch(query))
            {
                if (!this.IsManualOpen) await this.Connection.OpenAsync();
                SQLiteCommand command = new SQLiteCommand(this.UpdateString(query), this.Connection);
                DbResult res = new DbResult(await command.ExecuteReaderAsync());
                command.Dispose();
                if (!this.IsManualOpen) this.Connection.Close();
                return res;
            }
            else return DbResult.Empty;
        }

        public int ExecuteNonQuery(string query)
        {
            if (new Regex("^(select|\\s+)", RegexOptions.IgnoreCase).IsMatch(query)) return -1;
            else
            {
                if (!this.IsManualOpen) this.Connection.Open();
                SQLiteCommand command = new SQLiteCommand(this.UpdateString(query), this.Connection);
                int count = command.ExecuteNonQuery();
                command.Dispose();
                if (!this.IsManualOpen) this.Connection.Close();
                return count;
            }
        }

        public async Task<int> ExecuteNonQueryAsync(string query)
        {
            if (new Regex("^(select|\\s+)", RegexOptions.IgnoreCase).IsMatch(query)) return -1;
            else
            {
                if (!this.IsManualOpen) await this.Connection.OpenAsync();
                SQLiteCommand command = new SQLiteCommand(this.UpdateString(query), this.Connection);
                int count = await command.ExecuteNonQueryAsync();
                command.Dispose();
                if (!this.IsManualOpen) this.Connection.Close();
                return count;
            }
        }

        private string UpdateString(string update)
        {
            if (new Regex(";$").IsMatch(update)) return update;
            else return update + ";";
        }

        public bool Test()
        {
            if (this.IsManualOpen) return true;
            try
            {
                this.Connection.Open();
                this.Connection.Close();
                return true;
            }
            catch { return false; }
        }

        public async Task<bool> TestAsync()
        {
            if (this.IsManualOpen) return true;
            try
            {
                await this.Connection.OpenAsync();
                this.Connection.Close();
                return true;
            }
            catch { return false; }
        }
    }
}
