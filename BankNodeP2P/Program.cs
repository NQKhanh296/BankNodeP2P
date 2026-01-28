using BankNodeP2P.App;
using BankNodeP2P.Networking;
using BankNodeP2P.UI;

namespace BankNodeP2P;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        var (config, logger, _, bankService) = AppComposition.Build();

        var server = new BankTcpServer(
           bankService,                
           config.CommandTimeoutMs,
           config.ClientIdleTimeoutMs
       );

        var form = new MainForm();
        form.SetLogger(logger);

        form.SetController(new NodeController
        {
            StartAsync = () => server.StartAsync(config.Port),
            StopAsync = () => server.StopAsync()
        });

        logger.Info("App", $"UI ready. Port={config.Port} BankIp={config.BankIp}");

        Application.Run(form);
    }
}
