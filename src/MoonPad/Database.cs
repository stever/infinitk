using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace MoonPad
{
    internal class Database : IDisposable
    {
        private readonly SQLiteConnection connection;

        public Database(string filename)
        {
            connection = new SQLiteConnection($"Data Source={filename};Version=3;");
            connection.Open();
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
        }

        public List<string> GetLuaScriptNames()
        {
            var names = new List<string>();

            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "SELECT Name FROM LuaScripts";
                var reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    names.Add(Convert.ToString(reader["Name"]));
                }
            }

            return names;
        }

        public string GetLuaScript(string name)
        {
            throw new NotImplementedException();
        }
    }
}
