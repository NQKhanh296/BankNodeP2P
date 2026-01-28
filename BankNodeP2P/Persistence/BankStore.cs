using System.Text.Json;

namespace BankNodeP2P.Persistence
{
    /// <summary>
    /// Provides persistent storage for the bank state using a JSON file.
    /// Supports loading and atomic saving of the bank data.
    /// </summary>
    public class BankStore
    {
        private readonly string _path;

        /// <summary>
        /// Initializes the bank state storage and ensures
        /// that the target directory exists.
        /// </summary>
        /// <param name="path">Path to the JSON state file.</param>
        public BankStore(string path)
        {
            _path = path;
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        }

        /// <summary>
        /// Loads the bank state from the JSON file.
        /// If the file does not exist or cannot be deserialized,
        /// a new empty state is returned.
        /// </summary>
        /// <returns>Loaded or newly created bank state.</returns>
        public BankStateDto Load()
        {
            if (!File.Exists(_path))
                return new BankStateDto();

            var json = File.ReadAllText(_path);

            var state = JsonSerializer.Deserialize<BankStateDto>(json);
            return state ?? new BankStateDto();
        }

        /// <summary>
        /// Saves the bank state atomically to the JSON file.
        /// Data is first written to a temporary file and then
        /// replaced to prevent data corruption.
        /// </summary>
        /// <param name="state">Bank state to persist.</param>
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
}