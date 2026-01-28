using System.Text.Json;

namespace BankNodeP2P.App
{
    /// <summary>
    /// Represents application configuration loaded from a JSON file.
    /// Stores network settings and timeout values for the bank node.
    /// </summary>
    public class AppConfig
    {
        public string BankIp { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 65530;

        public int CommandTimeoutMs { get; set; } = 5000;
        public int ClientIdleTimeoutMs { get; set; } = 15000;

        /// <summary>
        /// Loads the application configuration from a JSON file.
        /// If the file does not exist or cannot be parsed,
        /// a new configuration with default values is created and saved.
        /// </summary>
        /// <param name="path">Path to the configuration file.</param>
        /// <returns>Loaded or newly created application configuration.</returns>
        public static AppConfig LoadOrCreate(string path)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            if (!File.Exists(path))
            {
                var cfg = new AppConfig();
                Save(path, cfg);
                return cfg;
            }

            try
            {
                var json = File.ReadAllText(path);
                var cfg = JsonSerializer.Deserialize<AppConfig>(json);
                return cfg ?? new AppConfig();
            }
            catch
            {
                return new AppConfig();
            }
        }

        /// <summary>
        /// Saves the application configuration to a JSON file.
        /// The configuration is written in a human-readable format.
        /// </summary>
        /// <param name="path">Path to the configuration file.</param>
        /// <param name="cfg">Configuration instance to save.</param>
        public static void Save(string path, AppConfig cfg)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);

            var json = JsonSerializer.Serialize(cfg, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(path, json);
        }
    }
}