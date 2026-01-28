using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.Persistence
{
    public class BankStateDto
    {
        public string BankIp { get; set; } = "";
        public int LastAccountNumber { get; set; }
        public List<AccountDto> Accounts { get; set; } = new();
    }

    public class AccountDto
    {
        public int Number { get; set; }
        public long Balance { get; set; }
    }
}
