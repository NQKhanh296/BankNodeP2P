using BankNodeP2P.Logging;
using BankNodeP2P.Persistence;
using BankNodeP2P.Protocol;

namespace BankNodeP2P.Domain
{
    /// <summary>
    /// Implements the core banking logic of the bank node.
    /// Manages accounts, balances and persistence,
    /// and provides operations defined by the bank protocol.
    /// </summary>
    public class BankService : IBankService
    {
        private readonly Dictionary<int, Account> _accounts;
        private readonly BankStore _store;
        private readonly Logger _logger;
        private readonly string _bankIp;
        private int _lastAccount;

        private readonly object _lock = new();

        /// <summary>
        /// Initializes the bank service using persisted state.
        /// </summary>
        /// <param name="state">Previously stored bank state.</param>
        /// <param name="store">Persistence storage service.</param>
        /// <param name="logger">Logging service.</param>
        /// <param name="bankIp">IP address of this bank node.</param>
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

        /// <summary>
        /// Returns the IP address of this bank.
        /// </summary>
        public string GetBankIp() => _bankIp;

        /// <summary>
        /// Returns the total amount of money stored in this bank.
        /// </summary>
        public long GetTotalBalance() => GetTotalAmount();

        /// <summary>
        /// Returns the number of client accounts in this bank.
        /// </summary>
        public int GetAccountCount() => GetNumberOfClients();

        /// <summary>
        /// Creates a new account and returns its number together with the bank IP.
        /// Explicit implementation of IBankService.
        /// </summary>
        (int accountNumber, string bankIp) IBankService.CreateAccount()
        {
            var acc = CreateAccount();
            return (acc.Number, _bankIp);
        }

        /// <summary>
        /// Deposits money to a local account.
        /// </summary>
        public void Deposit(int accountNumber, string bankIp, long amount)
        {
            RequireLocalBank(bankIp);
            Deposit(accountNumber, amount);
        }

        /// <summary>
        /// Withdraws money from a local account.
        /// </summary>
        public void Withdraw(int accountNumber, string bankIp, long amount)
        {
            RequireLocalBank(bankIp);
            Withdraw(accountNumber, amount);
        }

        /// <summary>
        /// Returns the balance of a local account.
        /// </summary>
        public long GetAccountBalance(int accountNumber, string bankIp)
        {
            RequireLocalBank(bankIp);
            return GetBalance(accountNumber);
        }

        /// <summary>
        /// Removes an account from the local bank.
        /// </summary>
        public void RemoveAccount(int accountNumber, string bankIp)
        {
            RequireLocalBank(bankIp);
            RemoveAccount(accountNumber);
        }

        /// <summary>
        /// Verifies that the operation targets this local bank.
        /// </summary>
        private void RequireLocalBank(string bankIp)
        {
            if (string.IsNullOrWhiteSpace(bankIp))
                throw new ArgumentException("Invalid bank IP.");

            if (!string.Equals(bankIp, _bankIp, StringComparison.Ordinal))
                throw new InvalidOperationException("Foreign bank code is not supported.");
        }

        /// <summary>
        /// Creates a new local bank account.
        /// </summary>
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

        /// <summary>
        /// Returns the balance of the specified account.
        /// </summary>
        public long GetBalance(int accountNumber)
        {
            lock (_lock)
            {
                var acc = GetAccountLocked(accountNumber);
                return acc.Balance;
            }
        }

        /// <summary>
        /// Returns the total balance of all accounts.
        /// </summary>
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

        /// <summary>
        /// Returns the number of existing accounts.
        /// </summary>
        public int GetNumberOfClients()
        {
            lock (_lock)
            {
                return _accounts.Count;
            }
        }

        /// <summary>
        /// Deposits money into an account.
        /// </summary>
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

        /// <summary>
        /// Withdraws money from an account.
        /// </summary>
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

        /// <summary>
        /// Removes an account with zero balance.
        /// </summary>
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

        /// <summary>
        /// Returns an account instance or throws if it does not exist.
        /// Must be called under a lock.
        private Account GetAccountLocked(int accountNumber)
        {
            if (accountNumber < 10000 || accountNumber > 99999)
                throw new ArgumentOutOfRangeException(nameof(accountNumber), "Account number must be in range 10000–99999.");

            if (!_accounts.TryGetValue(accountNumber, out var acc))
                throw new KeyNotFoundException("The account does not exist.");

            return acc;
        }

        /// <summary>
        /// Saves the current bank state to persistent storage.
        /// Must be called under a lock.
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
}
