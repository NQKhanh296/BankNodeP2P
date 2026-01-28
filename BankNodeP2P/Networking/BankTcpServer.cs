using BankNodeP2P.Protocol;
using System.Net;
using System.Net.Sockets;

namespace BankNodeP2P.Networking
{
    /// <summary>
    /// TCP server of the bank node.
    /// Listens for incoming client connections and delegates
    /// command processing to client handlers.
    /// </summary>
    public class BankTcpServer
    {
        private TcpListener? listener;
        private CancellationTokenSource? cts;
        private Task? acceptLoopTask;

        private readonly CommandHandler commandHandler;
        private readonly int commandTimeoutMs;
        private readonly int clientIdleTimeoutMs;

        /// <summary>
        /// Initializes the TCP server and its command handler.
        /// </summary>
        /// <param name="bankService">Bank service providing banking operations.</param>
        /// <param name="localIp">IP address of this bank node.</param>
        /// <param name="port">TCP port on which the server will listen.</param>
        /// <param name="commandTimeoutMs">Timeout for command execution.</param>
        /// <param name="clientIdleTimeoutMs">Timeout for idle client connections.</param>
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

        /// <summary>
        /// Starts the TCP server and begins accepting client connections.
        /// </summary>
        /// <param name="port">TCP port to listen on.</param>
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

        /// <summary>
        /// Stops the TCP server and waits for background tasks to finish.
        /// </summary>
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

        /// <summary>
        /// Accept loop that continuously listens for incoming TCP clients
        /// and starts a client handler for each connection.
        /// </summary>
        /// <param name="token">Cancellation token used to stop the loop.</param>
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
