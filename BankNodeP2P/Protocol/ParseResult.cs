using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.Protocol
{
    /// <summary>
    /// Represents the result of parsing a protocol command.
    /// Contains either a successfully parsed command or an error message.
    /// </summary>
    public class ParseResult
    {
        public bool Ok { get; }
        public ParsedCommand? Command { get; }
        public string? Error { get; }

        public ParseResult(bool ok, ParsedCommand? command, string? error)
        {
            Ok = ok;
            Command = command;
            Error = error;
        }
    }
}
