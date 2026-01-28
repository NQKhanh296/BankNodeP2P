using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.Protocol
{
    public static class CommandParser
    {
        public static ParseResult Parse(string? line)
        {
            if (string.IsNullOrWhiteSpace(line))
                return new ParseResult(false, null, "Empty command");

            var raw = line.Trim();
            var parts = raw.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
                return new ParseResult(false, null, "Empty command");

            var codeStr = parts[0].ToUpperInvariant();

            if (!Enum.TryParse<CommandCodeEnum>(codeStr, out var code))
                return new ParseResult(false, null, "Unknown command");

            if (code == CommandCodeEnum.BC)
            {
                if (parts.Length != 1)
                    return new ParseResult(false, null, "BC takes no parameters");

                return new ParseResult(true, new ParsedCommand(code, raw), null);
            }

            return new ParseResult(false, null, "Not implemented");
        }
    }
}
