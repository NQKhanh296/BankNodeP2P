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

        public ClientHandler(CommandHandler handler)
        {
            this.handler = handler;
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
                    line = await reader.ReadLineAsync();
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

                var response = handler.Execute(parse.Command!);
                await writer.WriteLineAsync(response);
            }

            client.Close();
        }
    }
}
