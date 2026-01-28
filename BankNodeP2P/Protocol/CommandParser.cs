using System;
using System.Collections.Generic;
using System.Globalization;
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

            if (code is CommandCodeEnum.BC or CommandCodeEnum.AC or CommandCodeEnum.BA or CommandCodeEnum.BN)
            {
                if (parts.Length != 1)
                    return new ParseResult(false, null, $"{codeStr} takes no parameters");

                return new ParseResult(true, new ParsedCommand(code, raw), null);
            }

            if (code is CommandCodeEnum.AB or CommandCodeEnum.AR)
            {
                if (parts.Length != 2)
                    return new ParseResult(false, null, $"{codeStr} expects <account>/<ip>");

                var accIp = parts[1];
                var split = accIp.Split('/', 2, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 2)
                    return new ParseResult(false, null, "Invalid <account>/<ip> format");

                return new ParseResult(true, new ParsedCommand(code, raw, split[0], split[1]), null);
            }

            if (code is CommandCodeEnum.AD or CommandCodeEnum.AW)
            {
                if (parts.Length != 3)
                    return new ParseResult(false, null, $"{codeStr} expects <account>/<ip> <amount>");

                var accIp = parts[1];
                var split = accIp.Split('/', 2, StringSplitOptions.RemoveEmptyEntries);
                if (split.Length != 2)
                    return new ParseResult(false, null, "Invalid <account>/<ip> format");

                if (!long.TryParse(parts[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var amount))
                    return new ParseResult(false, null, "Invalid amount");

                return new ParseResult(true, new ParsedCommand(code, raw, split[0], split[1], amount), null);
            }

            return new ParseResult(false, null, "Unknown command");
        }
    }
}
