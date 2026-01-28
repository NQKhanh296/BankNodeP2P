using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.UI
{
    /// <summary>
    /// Provides control callbacks for starting and stopping the bank node.
    /// Acts as a bridge between the user interface and the application logic.
    /// </summary>
    public class NodeController
    {
        public Func<Task>? StartAsync { get; init; }
        public Func<Task>? StopAsync { get; init; }
    }
}
