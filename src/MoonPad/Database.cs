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

            using (var cmd = new SQLiteCommand("SELECT Name FROM LuaScripts", connection))
            using (var reader = cmd.ExecuteReader())
            {
                while (reader.Read())
                {
                    names.Add(Convert.ToString(reader["Name"]));
                }
            }

            return names;
        }

        public string GetLuaScript(string name)
        {
            using (var cmd = new SQLiteCommand("SELECT Script FROM LuaScripts WHERE Name=@Name", connection))
            {
                cmd.Parameters.Add(new SQLiteParameter("@Name", name));
                return (string) cmd.ExecuteScalar();
            }
        }

        public void UpdateLuaScript(string name, string script)
        {
            using (var cmd = new SQLiteCommand("UPDATE LuaScripts SET Script=@Script WHERE Name=@Name", connection))
            {
                cmd.Parameters.Add(new SQLiteParameter("@Name", name));
                cmd.Parameters.Add(new SQLiteParameter("@Script", script));
                cmd.ExecuteNonQuery();
            }
        }

        public void AddLuaScript(string name, string script = "")
        {
            using (var cmd = new SQLiteCommand("INSERT INTO LuaScripts (Name, Script) VALUES (@Name, @Script)", connection))
            {
                cmd.Parameters.Add(new SQLiteParameter("@Name", name));
                cmd.Parameters.Add(new SQLiteParameter("@Script", script));
                cmd.ExecuteNonQuery();
            }
        }

        public void DeleteLuaScript(string name)
        {
            using (var cmd = new SQLiteCommand("DELETE FROM LuaScripts WHERE Name=@Name", connection))
            {
                cmd.Parameters.Add(new SQLiteParameter("@Name", name));
                cmd.ExecuteNonQuery();
            }
        }
    }
}
