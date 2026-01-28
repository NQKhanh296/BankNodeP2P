using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.Logging
{
    /// <summary>
    /// Central logging component of the application.
    /// Provides methods for recording informational and error events.
    /// </summary>
    public class Logger
    {
        private readonly LogStoreJsonl _store;

        public event Action<LogEntry>? OnLog;

        /// <summary>
        /// Initializes the logger with a JSONL log storage.
        /// </summary>
        /// <param name="store">Log storage used for persisting log entries.</param>
        public Logger(LogStoreJsonl store)
        {
            _store = store;
        }

        /// <summary>
        /// Writes an informational log entry.
        /// </summary>
        /// <param name="evt">Short event identifier.</param>
        /// <param name="message">Optional descriptive message.</param>
        public void Info(string evt, string message = "")
            => Write("INFO", evt, message);

        /// <summary>
        /// Writes a warning log entry.
        /// </summary>
        /// <param name="evt">Short event identifier.</param>
        /// <param name="message">Optional descriptive message.</param>
        public void Warn(string evt, string message = "")
            => Write("WARN", evt, message);

        /// <summary>
        /// Writes an error log entry.
        /// </summary>
        /// <param name="evt">Short event identifier.</param>
        /// <param name="message">Optional descriptive message.</param>
        public void Error(string evt, string message = "")
            => Write("ERROR", evt, message);

        /// <summary>
        /// Creates a log entry, stores it and notifies subscribers.
        /// </summary>
        /// <param name="level">Severity level of the log entry.</param>
        /// <param name="evt">Short event identifier.</param>
        /// <param name="message">Detailed log message.</param>
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
