using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BankNodeP2P.Protocol
{
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
