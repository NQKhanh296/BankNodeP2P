using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

namespace BankNodeP2P.Logging
{

    public class LogStoreJsonl
    {
        private readonly string _path;
        private readonly object _lock = new();

        public LogStoreJsonl(string path)
        {
            _path = path;
            Directory.CreateDirectory(Path.GetDirectoryName(_path)!);
        }

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
