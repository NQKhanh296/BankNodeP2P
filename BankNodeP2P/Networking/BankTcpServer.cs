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
        private CancellationTokenSource? cts;
        private Task? acceptLoopTask;

        private readonly CommandHandler commandHandler;
        private readonly int commandTimeoutMs;
        private readonly int clientIdleTimeoutMs;

        public BankTcpServer(
            IBankService bankService,
            int commandTimeoutMs,
            int clientIdleTimeoutMs)
        {
            commandHandler = new CommandHandler(bankService);
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

            // běží na pozadí
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
            catch { /* ignore */ }

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
