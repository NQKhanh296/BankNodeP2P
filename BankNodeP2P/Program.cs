using BankNodeP2P.App;
using BankNodeP2P.UI;

namespace BankNodeP2P;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        // zatím natvrdo (pozdìji config nebo automatické zjištìní)
        var bankIp = "127.0.0.1";

        var (logger, _, _) = AppComposition.Build(bankIp);

        var form = new MainForm();
        form.SetLogger(logger);

        logger.Info("App", "Application started");

        Application.Run(form);
    }
}
