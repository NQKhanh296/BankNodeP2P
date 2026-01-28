using BankNodeP2P.Networking;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

namespace BankNodeP2P.Protocol
{
    public class CommandHandler
    {
        private readonly IBankService bank;
        private readonly BankProxyClient proxy;
        private readonly string localIp;

        public CommandHandler(
            IBankService bank,
            string localIp,
            int port,
            int timeoutMs)
        {
            this.bank = bank;
            this.localIp = localIp;
            proxy = new BankProxyClient(port, timeoutMs);
        }

        public string Execute(ParsedCommand cmd)
        {
            try
            {
                switch (cmd.Code)
                {
                    case CommandCodeEnum.BC:
                        return $"BC {localIp}";

                    case CommandCodeEnum.AC:
                        {
                            var (acc, ip) = bank.CreateAccount();
                            return $"AC {acc}/{ip}";
                        }

                    case CommandCodeEnum.AD:
                        return HandleLocalOrProxy(
                            cmd,
                            local: () =>
                            {
                                var (a, ip, amount) = RequireAccIpAmount(cmd);
                                bank.Deposit(a, ip, amount);
                                return "AD";
                            });

                    case CommandCodeEnum.AW:
                        return HandleLocalOrProxy(
                            cmd,
                            local: () =>
                            {
                                var (a, ip, amount) = RequireAccIpAmount(cmd);
                                bank.Withdraw(a, ip, amount);
                                return "AW";
                            });

                    case CommandCodeEnum.AB:
                        return HandleLocalOrProxy(
                            cmd,
                            local: () =>
                            {
                                var (a, ip) = RequireAccIp(cmd);
                                var bal = bank.GetAccountBalance(a, ip);
                                return $"AB {bal.ToString(CultureInfo.InvariantCulture)}";
                            });

                    case CommandCodeEnum.AR:
                        {
                            var (acc, ip) = RequireAccIp(cmd);
                            bank.RemoveAccount(acc, ip);
                            return "AR";
                        }

                    case CommandCodeEnum.BA:
                        return $"BA {bank.GetTotalBalance()}";

                    case CommandCodeEnum.BN:
                        return $"BN {bank.GetAccountCount()}";

                    default:
                        return "ER Unknown command";
                }
            }
            catch (ArgumentException ex)
            {
                return $"ER {ex.Message}";
            }
            catch (InvalidOperationException ex)
            {
                return $"ER {ex.Message}";
            }
            catch
            {
                return "ER Internal error";
            }
        }


        private string HandleLocalOrProxy(ParsedCommand cmd, Func<string> local)
        {
            var (_, ip) = RequireAccIp(cmd);

            if (ip == localIp)
            {
                return local();
            }

            return proxy.ForwardAsync(ip, cmd.RawLine!).GetAwaiter().GetResult();
        }

        private static (int acc, string ip) RequireAccIp(ParsedCommand cmd)
        {
            if (string.IsNullOrWhiteSpace(cmd.Account) || string.IsNullOrWhiteSpace(cmd.BankIp))
                throw new ArgumentException("Invalid parameters");

            if (!int.TryParse(cmd.Account, out var acc))
                throw new ArgumentException("Invalid account number");

            return (acc, cmd.BankIp!);
        }

        private static (int acc, string ip, long amount) RequireAccIpAmount(ParsedCommand cmd)
        {
            var (acc, ip) = RequireAccIp(cmd);

            if (cmd.Amount is null)
                throw new ArgumentException("Missing amount");

            return (acc, ip, cmd.Amount.Value);
        }
    }
}
