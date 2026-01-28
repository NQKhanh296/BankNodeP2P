using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.Logging
{
    public class LogEntry
    {
        public DateTime Timestamp { get; init; } = DateTime.Now;
        public string Level { get; init; } = "";
        public string Event { get; init; } = "";
        public string ClientIp { get; init; } = "";
        public string Command { get; init; } = "";
        public string Response { get; init; } = "";
        public string Message { get; init; } = "";
    }
}
