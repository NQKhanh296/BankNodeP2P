using BankNodeP2P.Protocol;
using System.Net.Sockets;
using System.Text;

namespace BankNodeP2P.Networking
{
    /// <summary>
    /// Handles communication with a single connected TCP client.
    /// Reads protocol commands, processes them using a command handler
    /// and sends responses back to the client.
    /// </summary>
    public class ClientHandler
    {
        private readonly CommandHandler handler;
        private readonly TimeSpan commandTimeout;
        private readonly TimeSpan clientIdleTimeout;

        /// <summary>
        /// Initializes the client handler with command and idle timeouts.
        /// </summary>
        /// <param name="handler">Command handler used to execute commands.</param>
        /// <param name="commandTimeoutMs">Command execution timeout in milliseconds.</param>
        /// <param name="clientIdleTimeoutMs">Idle client timeout in milliseconds.</param>
        public ClientHandler(CommandHandler handler, int commandTimeoutMs, int clientIdleTimeoutMs)
        {
            this.handler = handler;
            commandTimeout = TimeSpan.FromMilliseconds(commandTimeoutMs);
            clientIdleTimeout = TimeSpan.FromMilliseconds(clientIdleTimeoutMs);
        }

        /// <summary>
        /// Runs the client communication loop.
        /// Continuously reads commands from the client until
        /// the connection is closed, a timeout occurs, or cancellation is requested.
        /// </summary>
        /// <param name="client">Connected TCP client.</param>
        /// <param name="token">Cancellation token used to stop the handler.</param>
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
