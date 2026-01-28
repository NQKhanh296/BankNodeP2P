using BankNodeP2P.Logging;
using BankNodeP2P.UI;

namespace BankNodeP2P;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var baseDir = AppContext.BaseDirectory;
        var dataDir = Path.Combine(baseDir, "data");
        Directory.CreateDirectory(dataDir);

        var logPath = Path.Combine(dataDir, "logs.jsonl");

        var store = new LogStoreJsonl(logPath);
        var logger = new Logger(store);

        var form = new MainForm();
        form.SetLogger(logger);

        logger.Info("App", $"Application started. LogPath={logPath}");

        Application.Run(form);
    }
}
