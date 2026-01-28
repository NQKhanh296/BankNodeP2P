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

        public CommandHandler(IBankService bank)
        {
            this.bank = bank;
        }
        public string Execute(ParsedCommand cmd)
        {
            try
            {
                switch (cmd.Code)
                {
                    case CommandCodeEnum.BC:
                        return $"BC {bank.GetBankIp()}";

                    case CommandCodeEnum.AC:
                        {
                            var (acc, ip) = bank.CreateAccount();
                            return $"AC {acc}/{ip}";
                        }

                    case CommandCodeEnum.AD:
                        {
                            var (acc, ip, amount) = RequireAccIpAmount(cmd);
                            bank.Deposit(acc, ip, amount);
                            return "AD";
                        }

                    case CommandCodeEnum.AW:
                        {
                            var (acc, ip, amount) = RequireAccIpAmount(cmd);
                            bank.Withdraw(acc, ip, amount);
                            return "AW";
                        }

                    case CommandCodeEnum.AB:
                        {
                            var (acc, ip) = RequireAccIp(cmd);
                            var bal = bank.GetAccountBalance(acc, ip);
                            return $"AB {bal.ToString(CultureInfo.InvariantCulture)}";
                        }

                    case CommandCodeEnum.AR:
                        {
                            var (acc, ip) = RequireAccIp(cmd);
                            bank.RemoveAccount(acc, ip);
                            return "AR";
                        }

                    case CommandCodeEnum.BA:
                        {
                            var total = bank.GetTotalBalance();
                            return $"BA {total.ToString(CultureInfo.InvariantCulture)}";
                        }

                    case CommandCodeEnum.BN:
                        {
                            var count = bank.GetAccountCount();
                            return $"BN {count.ToString(CultureInfo.InvariantCulture)}";
                        }

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

            var amount = cmd.Amount.Value;
            return (acc, ip, amount);
        }
    }
}
