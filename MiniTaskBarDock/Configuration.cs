using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MiniTaskBarDock
{
    internal class ConfigurationData
    {
        public string? DockDataPath { get; set; }
        public int IconCountPerLine { get; set; } = 9;
        public int IconSize { get; set; } = 40;
        public int MonitorIndex { get; set; } = 0;
    }
    internal class Configuration
    {
        private string _configPath;

        public ConfigurationData Data
        {
            get;
            set;
        }

        public Configuration(string fileName)
        {
            Data = Load(fileName);
        }

        private ConfigurationData Load(string fileName)
        {
            var localAppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appDataPath = Path.Combine(localAppDataPath, "MiniTaskBarDock");

            if (!Directory.Exists(appDataPath))
            {
                Directory.CreateDirectory(appDataPath);
            }

            _configPath = Path.Combine(appDataPath, fileName);

            if (File.Exists(_configPath))
            {
                string json = File.ReadAllText(_configPath);
                try
                {
                    return JsonSerializer.Deserialize<ConfigurationData>(json) ?? new();
                }
                catch (Exception)
                {
                    return new();
                }
            }

            return new();
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(Data);
            File.WriteAllText( _configPath, json );
        }
    }
}
