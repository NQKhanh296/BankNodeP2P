using BankNodeP2P.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.Networking
{
    public class BankTcpServer
    {
        private TcpListener? listener;
        private CancellationTokenSource? cancellationTokenSource;

        private readonly CommandHandler commandHandler;
        private readonly int commandTimeoutMs;
        private readonly int clientIdleTimeoutMs;

        public BankTcpServer(IBankService bankService, int commandTimeoutMs, int clientIdleTimeoutMs)
        {
            commandHandler = new CommandHandler(bankService);
            this.commandTimeoutMs = commandTimeoutMs;
            this.clientIdleTimeoutMs = clientIdleTimeoutMs;
        }

        public void Start(int port)
        {
            cancellationTokenSource = new CancellationTokenSource();
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();

            Task.Run(() => AcceptLoopAsync(cancellationTokenSource.Token));
        }

        public void Stop()
        {
            cancellationTokenSource?.Cancel();
            listener?.Stop();
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

                var handler = new ClientHandler(commandHandler, commandTimeoutMs, clientIdleTimeoutMs);
                _ = Task.Run(() => handler.RunAsync(client, token), token);
            }
        }
    }
}
