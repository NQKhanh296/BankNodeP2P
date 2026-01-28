using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.Protocol
{
    /// <summary>
    /// Defines banking operations provided by a bank node.
    /// This abstraction is used by the protocol layer to execute commands
    /// without depending on a specific domain implementation.
    /// </summary>
    public interface IBankService
    {
        string GetBankIp();                 
        long GetTotalBalance();            
        int GetAccountCount();             

        (int accountNumber, string bankIp) CreateAccount();   

        void Deposit(int accountNumber, string bankIp, long amount);    
        void Withdraw(int accountNumber, string bankIp, long amount);   
        long GetAccountBalance(int accountNumber, string bankIp);      
        void RemoveAccount(int accountNumber, string bankIp);
    }
}
