using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.Protocol
{
    public class ParsedCommand
    {
        public CommandCodeEnum Code { get; }
        public string RawLine { get; }
        public string? Account { get; }
        public string? BankIp { get; }
        public long? Amount { get; }

        public ParsedCommand(
            CommandCodeEnum code,
            string rawLine,
            string? account = null,
            string? bankIp = null,
            long? amount = null)
        {
            Code = code;
            RawLine = rawLine;
            Account = account;
            BankIp = bankIp;
            Amount = amount;
        }
    }
}
