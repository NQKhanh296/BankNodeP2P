using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.Networking
{
    /// <summary>
    /// TCP client used for proxying commands to another bank node.
    /// Connects to a remote node, forwards a single command
    /// and returns the received response.
    /// </summary>
    public class BankProxyClient
    {
        private readonly int _port;
        private readonly int _timeoutMs;

        /// <summary>
        /// Initializes the proxy client with target port and timeout.
        /// </summary>
        /// <param name="port">TCP port of the remote bank node.</param>
        /// <param name="timeoutMs">Operation timeout in milliseconds.</param>
        public BankProxyClient(int port, int timeoutMs)
        {
            _port = port;
            _timeoutMs = timeoutMs;
        }

        /// <summary>
        /// Forwards a raw protocol command to a remote bank node
        /// and returns its textual response.
        /// </summary>
        /// <param name="ip">IP address of the target bank node.</param>
        /// <param name="rawLine">Raw command line to forward.</param>
        /// <returns>
        /// Response received from the remote node,
        /// or an error message if the operation fails.
        /// </returns>
        public async Task<string> ForwardAsync(string ip, string rawLine)
        {
            using var cts = new CancellationTokenSource(_timeoutMs);

            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(ip, _port, cts.Token);

                using var stream = client.GetStream();
                using var writer = new StreamWriter(stream, new UTF8Encoding(false)) { AutoFlush = true };
                using var reader = new StreamReader(stream, Encoding.UTF8);

                await writer.WriteLineAsync(rawLine.AsMemory(), cts.Token);

                var response = await reader.ReadLineAsync(cts.Token);
                return response ?? "ER Proxy no response";
            }
            catch (OperationCanceledException)
            {
                return "ER Proxy timeout";
            }
            catch
            {
                return "ER Proxy connection failed";
            }
        }
    }
}
