using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.Persistence
{
    /// <summary>
    /// Data transfer object representing the persistent state of a bank node.
    /// Contains bank identification and a snapshot of all stored accounts.
    /// </summary>
    public class BankStateDto
    {
        public string BankIp { get; set; } = "";
        public int LastAccountNumber { get; set; }
        public List<AccountDto> Accounts { get; set; } = new();
    }

    /// <summary>
    /// Data transfer object representing a single bank account.
    /// Used for persistence and serialization.
    /// </summary>
    public class AccountDto
    {
        public int Number { get; set; }
        public long Balance { get; set; }
    }
}
