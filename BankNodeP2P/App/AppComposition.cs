using BankNodeP2P.Domain;
using BankNodeP2P.Logging;
using BankNodeP2P.Persistence;

namespace BankNodeP2P.App
{
    /// <summary>
    /// Composes and initializes core application components.
    /// Responsible for wiring configuration, logging,
    /// persistence and domain services together.
    /// </summary>
    public static class AppComposition
    {
        /// <summary>
        /// Builds and initializes the main application components.
        /// Creates configuration, logging, persistence storage
        /// and the bank service.
        /// </summary>
        /// <returns>
        /// Tuple containing application configuration,
        /// logger, bank store and bank service.
        /// </returns>
        public static (AppConfig config, Logger logger, BankStore bankStore, BankService bankService) Build()
        {
            var baseDir = AppContext.BaseDirectory;
            var dataDir = Path.Combine(baseDir, "data");
            Directory.CreateDirectory(dataDir);

            var configPath = Path.Combine(dataDir, "appsettings.json");
            var config = AppConfig.LoadOrCreate(configPath);

            var logPath = Path.Combine(dataDir, "logs.jsonl");
            var bankStatePath = Path.Combine(dataDir, "bank-state.json");

            var logStore = new LogStoreJsonl(logPath);
            var logger = new Logger(logStore);

            var bankStore = new BankStore(bankStatePath);
            var state = bankStore.Load();

            if (string.IsNullOrWhiteSpace(state.BankIp))
                state.BankIp = config.BankIp;

            var bankService = new BankService(state, bankStore, logger, config.BankIp);

            logger.Info("App", $"Composition built. BankIp={config.BankIp} Port={config.Port}");
            logger.Info("App", $"Timeouts: cmd={config.CommandTimeoutMs}ms idle={config.ClientIdleTimeoutMs}ms");
            logger.Info("App", $"Paths: cfg={configPath} logs={logPath} state={bankStatePath}");

            return (config, logger, bankStore, bankService);
        }
    }
}
