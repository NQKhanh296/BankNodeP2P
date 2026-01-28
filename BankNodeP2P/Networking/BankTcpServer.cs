using BankNodeP2P.Protocol;
using System.Net;
using System.Net.Sockets;

namespace BankNodeP2P.Networking
{
    public class BankTcpServer
    {
        private TcpListener? listener;
        private CancellationTokenSource? cts;
        private Task? acceptLoopTask;

        private readonly CommandHandler commandHandler;
        private readonly int commandTimeoutMs;
        private readonly int clientIdleTimeoutMs;

        public BankTcpServer(
            IBankService bankService,
            string localIp,
            int port,
            int commandTimeoutMs,
            int clientIdleTimeoutMs)
        {
            commandHandler = new CommandHandler(bankService, localIp, port, commandTimeoutMs);
            this.commandTimeoutMs = commandTimeoutMs;
            this.clientIdleTimeoutMs = clientIdleTimeoutMs;
        }

        public async Task StartAsync(int port)
        {
            if (listener != null)
                throw new InvalidOperationException("Server already running.");

            cts = new CancellationTokenSource();
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            acceptLoopTask = Task.Run(() => AcceptLoopAsync(cts.Token));

            await Task.CompletedTask;
        }

        public async Task StopAsync()
        {
            if (listener == null)
                return;

            cts!.Cancel();
            listener.Stop();

            try
            {
                if (acceptLoopTask != null)
                    await acceptLoopTask;
            }
            catch {}

            listener = null;
            cts = null;
        }

        private async Task AcceptLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                TcpClient client;

                try
                {
                    client = await listener!.AcceptTcpClientAsync(token);
                }
                catch
                {
                    break;
                }

                var handler = new ClientHandler(
                    commandHandler,
                    commandTimeoutMs,
                    clientIdleTimeoutMs
                );

                _ = Task.Run(() => handler.RunAsync(client, token), token);
            }
        }
    }
}
