using CommandAI.Models;
using Newtonsoft.Json;

namespace CommandAI.Services
{
    public class ConfigurationService
    {
        private const string ConfigFolder = ".config/commandai";
        private const string ConfigFile = "config.json";
        private readonly string _configPath;

        public ConfigurationService()
        {
            string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            _configPath = Path.Combine(homeDir, ConfigFolder, ConfigFile);
        }

        public Configuration LoadConfiguration()
        {
            if (!File.Exists(_configPath))
            {
                var defaultConfig = new Configuration();
                SaveConfiguration(defaultConfig);
                return defaultConfig;
            }

            string json = File.ReadAllText(_configPath);
            return JsonConvert.DeserializeObject<Configuration>(json) ?? new Configuration();
        }

        public void SaveConfiguration(Configuration config)
        {
            string directory = Path.GetDirectoryName(_configPath)!;
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(_configPath, json);
        }
    }
}