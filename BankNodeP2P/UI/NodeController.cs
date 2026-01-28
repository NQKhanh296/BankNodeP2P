using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.UI
{

    public class NodeController
    {
        public Func<Task>? StartAsync { get; init; }
        public Func<Task>? StopAsync { get; init; }
    }
}
