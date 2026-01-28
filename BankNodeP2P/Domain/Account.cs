using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.Domain
{
    /// <summary>
    /// Represents a single bank account.
    /// Stores the account number and the current balance.
    /// </summary>

    public class Account
    {
        public int Number { get; init; }
        public long Balance { get; set; }
    }
}
