using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.Logging
{
    public class Logger
    {
        private readonly LogStoreJsonl _store;

        public event Action<LogEntry>? OnLog;

        public Logger(LogStoreJsonl store)
        {
            _store = store;
        }

        public void Info(string evt, string message = "")
            => Write("INFO", evt, message);

        public void Warn(string evt, string message = "")
            => Write("WARN", evt, message);

        public void Error(string evt, string message = "")
            => Write("ERROR", evt, message);

        private void Write(string level, string evt, string message)
        {
            var entry = new LogEntry
            {
                Level = level,
                Event = evt,
                Message = message
            };

            _store.Append(entry);
            OnLog?.Invoke(entry);
        }
    }
}
