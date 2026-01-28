using BankNodeP2P.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.Networking
{
    public class ClientHandler
    {
        private readonly CommandHandler handler;
        private readonly TimeSpan commandTimeout;
        private readonly TimeSpan clientIdleTimeout;

        public ClientHandler(CommandHandler handler, int commandTimeoutMs, int clientIdleTimeoutMs)
        {
            this.handler = handler;
            commandTimeout = TimeSpan.FromMilliseconds(commandTimeoutMs);
            clientIdleTimeout = TimeSpan.FromMilliseconds(clientIdleTimeoutMs);
        }

        public async Task RunAsync(TcpClient client, CancellationToken token)
        {
            using var stream = client.GetStream();
            using var reader = new StreamReader(stream, Encoding.UTF8);
            using var writer = new StreamWriter(stream, new UTF8Encoding(false))
            {
                AutoFlush = true
            };

            while (!token.IsCancellationRequested)
            {
                string? line;

                try
                {
                    var readTask = reader.ReadLineAsync();
                    line = await readTask.WaitAsync(clientIdleTimeout, token);
                }
                catch (TimeoutException)
                {
                    try { await writer.WriteLineAsync("ER Client idle timeout"); } catch { }
                    break;
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    break;
                }

                if (line == null)
                    break;

                var parse = CommandParser.Parse(line);
                if (!parse.Ok)
                {
                    await writer.WriteLineAsync($"ER {parse.Error}");
                    continue;
                }

                try
                {
                    var responseTask = Task.Run(() => handler.Execute(parse.Command!), token);
                    var response = await responseTask.WaitAsync(commandTimeout, token);

                    await writer.WriteLineAsync(response);
                }
                catch (TimeoutException)
                {
                    await writer.WriteLineAsync("ER Timeout");
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch
                {
                    await writer.WriteLineAsync("ER Internal error");
                }
            }

            try { client.Close(); } catch { }
        }
    }
}
