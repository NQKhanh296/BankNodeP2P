using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.Networking
{
    public class BankProxyClient
    {
        private readonly int _port;
        private readonly int _timeoutMs;

        public BankProxyClient(int port, int timeoutMs)
        {
            _port = port;
            _timeoutMs = timeoutMs;
        }

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
