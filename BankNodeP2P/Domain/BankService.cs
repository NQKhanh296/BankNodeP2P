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

    // AC
    public Account CreateAccount()
    {
        lock (_lock)
        {
            if (_lastAccount < 10000)
                _lastAccount = 10000;
            else
                _lastAccount++;

            if (_lastAccount > 99999)
                throw new InvalidOperationException("Došel rozsah čísel účtů (10000-99999).");

            var acc = new Account { Number = _lastAccount, Balance = 0 };
            _accounts.Add(acc.Number, acc);

            SaveLocked();

            _logger.Info("AC", $"Created account {acc.Number}/{_bankIp}");
            return acc;
        }
    }

    // AB
    public long GetBalance(int accountNumber)
    {
        lock (_lock)
        {
            var acc = GetAccountLocked(accountNumber);
            return acc.Balance;
        }
    }

    // BA
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

    // BN
    public int GetNumberOfClients()
    {
        lock (_lock)
        {
            return _accounts.Count;
        }
    }

    // AD
    public void Deposit(int accountNumber, long amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Částka musí být nezáporná.");

        lock (_lock)
        {
            var acc = GetAccountLocked(accountNumber);

            // ochrana proti přetečení long
            if (acc.Balance > long.MaxValue - amount)
                throw new OverflowException("Přetečení zůstatku účtu.");

            acc.Balance += amount;

            SaveLocked();

            _logger.Info("AD", $"Deposit {amount} to {accountNumber}/{_bankIp}. NewBalance={acc.Balance}");
        }
    }

    // AW
    public void Withdraw(int accountNumber, long amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Částka musí být nezáporná.");

        lock (_lock)
        {
            var acc = GetAccountLocked(accountNumber);

            if (acc.Balance < amount)
                throw new InvalidOperationException("Není dostatek finančních prostředků.");

            acc.Balance -= amount;

            SaveLocked();

            _logger.Info("AW", $"Withdraw {amount} from {accountNumber}/{_bankIp}. NewBalance={acc.Balance}");
        }
    }

    // AR
    public void RemoveAccount(int accountNumber)
    {
        lock (_lock)
        {
            var acc = GetAccountLocked(accountNumber);

            if (acc.Balance != 0)
                throw new InvalidOperationException("Nelze smazat bankovní účet na kterém jsou finance.");

            _accounts.Remove(accountNumber);

            SaveLocked();

            _logger.Info("AR", $"Removed account {accountNumber}/{_bankIp}");
        }
    }

    // --- helpers ---
    private Account GetAccountLocked(int accountNumber)
    {
        if (accountNumber < 10000 || accountNumber > 99999)
            throw new ArgumentOutOfRangeException(nameof(accountNumber), "Číslo účtu musí být v rozsahu 10000-99999.");

        if (!_accounts.TryGetValue(accountNumber, out var acc))
            throw new KeyNotFoundException("Účet neexistuje.");

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
