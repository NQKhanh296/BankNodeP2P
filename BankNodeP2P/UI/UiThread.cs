using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.UI
{
    /// <summary>
    /// Helper class for safely executing actions on the UI thread.
    /// Ensures that UI updates are performed on the correct thread.
    /// </summary>
    public static class UiThread
    {
        public static void Post(Control control, Action action)
        {
            if (control.InvokeRequired)
                control.BeginInvoke(action);
            else
                action();
        }
    }
}
