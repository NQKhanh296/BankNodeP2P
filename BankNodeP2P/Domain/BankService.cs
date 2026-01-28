using BankNodeP2P.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BankNodeP2P.Logging;
using BankNodeP2P.Persistence;

namespace BankNodeP2P.Domain;

public class BankService
{
    private readonly Dictionary<int, Account> _accounts;
    private readonly BankStore _store;
    private readonly Logger _logger;
    private readonly string _bankIp;
    private int _lastAccount;

    public BankService(
        BankStateDto state,
        BankStore store,
        Logger logger,
        string bankIp)
    {
        _store = store;
        _logger = logger;
        _bankIp = bankIp;

        _lastAccount = state.LastAccountNumber;
        _accounts = state.Accounts
            .ToDictionary(a => a.Number,
                          a => new Account { Number = a.Number, Balance = a.Balance });
    }

    public Account CreateAccount()
    {
        if (_lastAccount < 10000)
            _lastAccount = 10000;
        else
            _lastAccount++;

        var acc = new Account { Number = _lastAccount, Balance = 0 };
        _accounts.Add(acc.Number, acc);

        Save();
        return acc;
    }

    private void Save()
    {
        _store.SaveAtomic(new BankStateDto
        {
            BankIp = _bankIp,
            LastAccountNumber = _lastAccount,
            Accounts = _accounts.Values
                .Select(a => new AccountDto { Number = a.Number, Balance = a.Balance })
                .ToList()
        });
    }
}
