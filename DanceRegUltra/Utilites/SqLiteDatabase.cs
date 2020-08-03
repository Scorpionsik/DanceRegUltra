using System;
using System.Data.Common;
using System.Data.SQLite;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SqLiteDatabase
{
    public class SqLiteDatabase
    {
        private SQLiteConnection Connection;

        public SqLiteDatabase(string database_path)
        {
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
                this.Connection.Open();
                SQLiteCommand command = new SQLiteCommand(this.UpdateString(query), this.Connection);
                DbResult res = new DbResult(command.ExecuteReader());
                command.Dispose();
                this.Connection.Close();
                return res;
            }
            else return null;
        }

        public async Task<DbResult> ExecuteAndGetQueryAsync(string query)
        {
            if (new Regex("^select", RegexOptions.IgnoreCase).IsMatch(query))
            {
                await this.Connection.OpenAsync();
                SQLiteCommand command = new SQLiteCommand(this.UpdateString(query), this.Connection);
                DbResult res = new DbResult(await command.ExecuteReaderAsync());
                command.Dispose();
                this.Connection.Close();
                return res;
            }
            else return null;
        }

        public int ExecuteNonQuery(string query)
        {
            if (new Regex("^(select|\\s+)", RegexOptions.IgnoreCase).IsMatch(query)) return -1;
            else
            {
                this.Connection.Open();
                SQLiteCommand command = new SQLiteCommand(this.UpdateString(query), this.Connection);
                int count = command.ExecuteNonQuery();
                command.Dispose();
                this.Connection.Close();
                return count;
            }
        }

        public async Task<int> ExecuteNonQueryAsync(string query)
        {
            if (new Regex("^(select|\\s+)", RegexOptions.IgnoreCase).IsMatch(query)) return -1;
            else
            {
                await this.Connection.OpenAsync();
                SQLiteCommand command = new SQLiteCommand(this.UpdateString(query), this.Connection);
                int count = await command.ExecuteNonQueryAsync();
                command.Dispose();
                this.Connection.Close();
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
