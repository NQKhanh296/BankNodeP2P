using System.Text.Json;

namespace BankNodeP2P.App;

public class AppConfig
{
    public string BankIp { get; set; } = "127.0.0.1";
    public int Port { get; set; } = 65530;

    public int CommandTimeoutMs { get; set; } = 5000;
    public int ClientIdleTimeoutMs { get; set; } = 15000;

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
