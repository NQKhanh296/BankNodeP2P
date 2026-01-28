using BankNodeP2P.Domain;
using BankNodeP2P.Logging;
using BankNodeP2P.Persistence;

namespace BankNodeP2P.App;

public static class AppComposition
{
    public static (Logger logger, BankStore bankStore, BankService bankService) Build(string bankIp)
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

        // pokud je v uloženém stavu prázdný bankIp, doplníme
        if (string.IsNullOrWhiteSpace(state.BankIp))
            state.BankIp = bankIp;

        var bankService = new BankService(state, bankStore, logger, bankIp);

        logger.Info("App", $"Composition built. BankIp={bankIp}");
        logger.Info("App", $"Paths: logs={logPath} state={bankStatePath}");

        return (logger, bankStore, bankService);
    }
}
