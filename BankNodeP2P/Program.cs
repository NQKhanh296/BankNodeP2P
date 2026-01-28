using BankNodeP2P.Logging;
using BankNodeP2P.UI;

namespace BankNodeP2P
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            


            var store = new LogStoreJsonl("data/logs.jsonl");
            var logger = new Logger(store);

            var form = new MainForm();
            form.SetLogger(logger);

            logger.Info("App", "Application started");

            Application.Run(form);

        }
    }
}