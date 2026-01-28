using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.Protocol
{
    /// <summary>
    /// Enumeration of supported bank protocol command codes.
    /// </summary>
    public enum CommandCodeEnum
    {
        BC,
        AC,
        AD,
        AW,
        AB,
        AR,
        BA,
        BN,
        Unknown
    }
}
