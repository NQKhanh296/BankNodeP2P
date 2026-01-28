using BankNodeP2P.App;
using BankNodeP2P.Domain;
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

        var (logger, bankStore, state) = AppComposition.BuildCore();

        BankService? bankService = null;
        BankTcpServer? server = null;

        var form = new MainForm();
        form.SetLogger(logger);

        form.SetController(new NodeController
        {
            StartAsync = async (ip, port, cmdTimeoutMs, idleTimeoutMs) =>
            {
                if (server != null)
                    throw new InvalidOperationException("Server already running.");

                bankService ??= AppComposition.BuildBankService(state, bankStore, logger, ip);

                server = new BankTcpServer(
                    bankService,
                    ip,
                    port,
                    cmdTimeoutMs,
                    idleTimeoutMs
                );

                await server.StartAsync(port);

                logger.Info("App", $"Server started. BankIp={ip} Port={port} cmdTimeout={cmdTimeoutMs}ms idleTimeout={idleTimeoutMs}ms");
            },

            StopAsync = async () =>
            {
                if (server == null)
                    return;

                await server.StopAsync();
                server = null;

                logger.Info("App", "Server stopped.");
            }
        });

        logger.Info("App", "UI ready. Configuration is provided by UI (port/timeouts). IP is detected automatically.");

        Application.Run(form);
    }
}
