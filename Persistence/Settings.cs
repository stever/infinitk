using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace MoonPad.Persistence
{
    internal class Settings : Dictionary<string, string>
    {
        private const string AppDataFolderName = "MoonPad";
        private const string SettingsFilename = "Settings.json";
        private const string FormWindowGeometryPropertyName = "Geometry_FormWindow";
        private const string SandDockLayoutPropertyName = "SandDockLayout";

        public static Settings Default { get; } = new Settings();

        private static string SettingsFullFilename
            => Path.Combine(AppDataPath, SettingsFilename);

        private Settings()
        {
            base[FormWindowGeometryPropertyName] = "";
            base[SandDockLayoutPropertyName] = "";

            var filename = Path.Combine(AppDataPath, SettingsFilename);
            if (!File.Exists(filename)) return;

            // Restore settings from file.
            var str = File.ReadAllText(SettingsFullFilename);
            var dict = JsonConvert.DeserializeObject<Dictionary<string,string>>(str);
            foreach (var key in dict.Keys)
            {
                // Ignore unsupported properties.
                if (ContainsKey(key))
                {
                    base[key] = dict[key];
                }
            }
        }

        public static string AppDataPath
        {
            get
            {
                var appDataRoot = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                return Path.Combine(appDataRoot, AppDataFolderName);
            }
        }

        public string SandDockLayout
        {
            get => base[SandDockLayoutPropertyName];
            set => base[SandDockLayoutPropertyName] = value;
        }

        public void Save()
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            if (!Directory.Exists(AppDataPath)) Directory.CreateDirectory(AppDataPath);
            File.WriteAllText(SettingsFullFilename, str);
        }
    }
}
