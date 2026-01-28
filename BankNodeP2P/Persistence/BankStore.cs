using System.Text.Json;

namespace BankNodeP2P.Persistence;

public class BankStore
{
    private readonly string _path;

    public BankStore(string path)
    {
        _path = path;
        Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
    }

    public BankStateDto Load()
    {
        if (!File.Exists(_path))
            return new BankStateDto();

        var json = File.ReadAllText(_path);

        var state = JsonSerializer.Deserialize<BankStateDto>(json);
        return state ?? new BankStateDto();
    }

    public void SaveAtomic(BankStateDto state)
    {
        var tmp = _path + ".tmp";

        var json = JsonSerializer.Serialize(state, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(tmp, json);
        File.Move(tmp, _path, true);
    }
}
