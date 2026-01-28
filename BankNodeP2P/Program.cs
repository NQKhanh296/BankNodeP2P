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

        var (config, logger, _, _) = AppComposition.Build();

        var form = new MainForm();
        form.SetLogger(logger);

        // zatím prázdný controller, Student A napojí server start/stop
        form.SetController(new NodeController());

        logger.Info("App", "Application started");

        Application.Run(form);
    }
}
