using BankNodeP2P.Domain;
using BankNodeP2P.Logging;
using BankNodeP2P.Persistence;

namespace BankNodeP2P.App
{
    /// <summary>
    /// Composes and initializes core application components.
    /// Responsible for wiring logging and persistence together.
    /// Runtime network configuration (IP/port/timeouts) is provided by the UI.
    /// </summary>
    public static class AppComposition
    {
        /// <summary>
        /// Creates core infrastructure components (logger and bank store),
        /// ensures the data directory exists and loads the persisted bank state.
        /// </summary>
        /// <returns>
        /// Tuple containing logger, bank store and loaded bank state.
        /// </returns>
        public static (Logger logger, BankStore bankStore, BankStateDto state) BuildCore()
        {
            var baseDir = AppContext.BaseDirectory;
            var dataDir = Path.Combine(baseDir, "data");
            Directory.CreateDirectory(dataDir);

            var logPath = Path.Combine(dataDir, "logs.jsonl");
            var bankStatePath = Path.Combine(dataDir, "bank-state.json");

            var logStore = new LogStoreJsonl(logPath);
            var logger = new Logger(logStore);

            var bankStore = new BankStore(bankStatePath);
            var state = bankStore.Load();

            logger.Info("App", "Core composition built.");
            logger.Info("App", $"Paths: logs={logPath} state={bankStatePath}");

            return (logger, bankStore, state);
        }

        /// <summary>
        /// Creates the bank domain service using a loaded state and runtime bank IP.
        /// </summary>
        /// <param name="state">Loaded persistent bank state.</param>
        /// <param name="bankStore">Persistence store instance.</param>
        /// <param name="logger">Logger instance.</param>
        /// <param name="bankIp">Runtime bank IP address (provided by UI).</param>
        /// <returns>Initialized bank service.</returns>
        public static BankService BuildBankService(BankStateDto state, BankStore bankStore, Logger logger, string bankIp)
        {
            if (string.IsNullOrWhiteSpace(state.BankIp))
                state.BankIp = bankIp;

            logger.Info("App", $"Bank service built. BankIp={bankIp}");

            return new BankService(state, bankStore, logger, bankIp);
        }
    }
}
