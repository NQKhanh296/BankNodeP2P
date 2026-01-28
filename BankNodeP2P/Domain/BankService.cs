using BankNodeP2P.Logging;
using BankNodeP2P.Persistence;
using BankNodeP2P.Protocol;

namespace BankNodeP2P.Domain;

public class BankService : IBankService
{
    private readonly Dictionary<int, Account> _accounts;
    private readonly BankStore _store;
    private readonly Logger _logger;
    private readonly string _bankIp;
    private int _lastAccount;

    private readonly object _lock = new();

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
            .ToDictionary(
                a => a.Number,
                a => new Account { Number = a.Number, Balance = a.Balance }
            );
    }

    public string GetBankIp() => _bankIp;

    public long GetTotalBalance() => GetTotalAmount();

    public int GetAccountCount() => GetNumberOfClients();

    (int accountNumber, string bankIp) IBankService.CreateAccount()
    {
        var acc = CreateAccount();
        return (acc.Number, _bankIp);
    }

    public void Deposit(int accountNumber, string bankIp, long amount)
    {
        RequireLocalBank(bankIp);
        Deposit(accountNumber, amount);
    }

    public void Withdraw(int accountNumber, string bankIp, long amount)
    {
        RequireLocalBank(bankIp);
        Withdraw(accountNumber, amount);
    }

    public long GetAccountBalance(int accountNumber, string bankIp)
    {
        RequireLocalBank(bankIp);
        return GetBalance(accountNumber);
    }

    public void RemoveAccount(int accountNumber, string bankIp)
    {
        RequireLocalBank(bankIp);
        RemoveAccount(accountNumber);
    }

    private void RequireLocalBank(string bankIp)
    {
        if (string.IsNullOrWhiteSpace(bankIp))
            throw new ArgumentException("Invalid bank IP.");

        if (!string.Equals(bankIp, _bankIp, StringComparison.Ordinal))
            throw new InvalidOperationException("Foreign bank code is not supported.");
    }

    public Account CreateAccount()
    {
        lock (_lock)
        {
            if (_lastAccount < 10000)
                _lastAccount = 10000;
            else
                _lastAccount++;

            if (_lastAccount > 99999)
                throw new InvalidOperationException("Account number must be in range 10000–99999.");

            var acc = new Account { Number = _lastAccount, Balance = 0 };
            _accounts.Add(acc.Number, acc);

            SaveLocked();

            _logger.Info("AC", $"Created account {acc.Number}/{_bankIp}");
            return acc;
        }
    }

    public long GetBalance(int accountNumber)
    {
        lock (_lock)
        {
            var acc = GetAccountLocked(accountNumber);
            return acc.Balance;
        }
    }

    public long GetTotalAmount()
    {
        lock (_lock)
        {
            long sum = 0;
            foreach (var a in _accounts.Values)
                sum += a.Balance;
            return sum;
        }
    }


    public int GetNumberOfClients()
    {
        lock (_lock)
        {
            return _accounts.Count;
        }
    }

    public void Deposit(int accountNumber, long amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "The amount must be non-negative.");

        lock (_lock)
        {
            var acc = GetAccountLocked(accountNumber);

            if (acc.Balance > long.MaxValue - amount)
                throw new OverflowException("Account balance exceeds the maximum allowed value.");

            acc.Balance += amount;

            SaveLocked();

            _logger.Info("AD", $"Deposit {amount} to {accountNumber}/{_bankIp}. NewBalance={acc.Balance}");
        }
    }

    public void Withdraw(int accountNumber, long amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "The amount must be non-negative.");

        lock (_lock)
        {
            var acc = GetAccountLocked(accountNumber);

            if (acc.Balance < amount)
                throw new InvalidOperationException("Insufficient funds.");

            acc.Balance -= amount;

            SaveLocked();

            _logger.Info("AW", $"Withdraw {amount} from {accountNumber}/{_bankIp}. NewBalance={acc.Balance}");
        }
    }

    public void RemoveAccount(int accountNumber)
    {
        lock (_lock)
        {
            var acc = GetAccountLocked(accountNumber);

            if (acc.Balance != 0)
                throw new InvalidOperationException("Cannot delete an account with a non-zero balance.");

            _accounts.Remove(accountNumber);

            SaveLocked();

            _logger.Info("AR", $"Removed account {accountNumber}/{_bankIp}");
        }
    }
    private Account GetAccountLocked(int accountNumber)
    {
        if (accountNumber < 10000 || accountNumber > 99999)
            throw new ArgumentOutOfRangeException(nameof(accountNumber), "Account number must be in range 10000–99999.");

        if (!_accounts.TryGetValue(accountNumber, out var acc))
            throw new KeyNotFoundException("The account does not exist.");

        return acc;
    }

    private void SaveLocked()
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
