using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace BankNodeP2P.Logging
{
    /// <summary>
    /// Provides persistent storage for log entries using the JSON Lines format.
    /// Each log entry is serialized as a single JSON object and appended
    /// as a separate line to the log file.
    /// </summary>
    public class LogStoreJsonl
    {
        private readonly string _path;
        private readonly object _lock = new();

        /// <summary>
        /// Initializes the JSONL log storage and ensures
        /// that the target directory exists.
        /// </summary>
        /// <param name="path">Path to the log file.</param>
        public LogStoreJsonl(string path)
        {
            _path = path;
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        }

        /// <summary>
        /// Appends a log entry to the log file.
        /// The entry is serialized to JSON and written as a single line.
        /// </summary>
        /// <param name="entry">Log entry to be stored.</param>
        public void Append(LogEntry entry)
        {
            var json = JsonSerializer.Serialize(entry);

            lock (_lock)
            {
                File.AppendAllText(_path, json + Environment.NewLine);
            }
        }
    }
}
