using System;
using System.Threading;
using System.Diagnostics;
using InoaTest_Console.Controllers;
using InoaTest_Console.Helpers;
using Microsoft.Extensions.Configuration;

namespace InoaTest_Console
{

    class Program
    {
        private static int CheckInterval  = 0;

        private static B3AtivoController B3AtivosMonitor;
        private static IConfiguration    Configuration;
        private static SMTPSettings      SMTP = new SMTPSettings();

        static void Main(string[] args)
        {
            B3AtivosMonitor = new B3AtivoController();

            try
            {
                GetSettings();
                Run();
                Dispose();

            } catch (Exception E)
            {
                Console.WriteLine("Erro: {0}", E.Message);
            }
        }

        static void GetSettings()
        {
            Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).Build();

            /* General */
            IConfigurationSection GeneralSection = Configuration.GetSection("General");
            CheckInterval = GeneralSection.GetValue<int>("CheckInterval");

            /* Mail */
            Configuration.GetSection("MailSettings").Bind(SMTP);
            B3AtivosMonitor.SetMail(SMTP);

            /* Assets */
            IConfigurationSection AssetsSection = Configuration.GetSection("Assets");
            foreach (IConfigurationSection Asset in AssetsSection.GetChildren())
            {
                SymbolArgs SArgs = new SymbolArgs(Asset.Key, Asset.GetValue<double>("SellPrice"), Asset.GetValue<double>("BuyPrice"));
                B3AtivosMonitor.AddSymbol(ref SArgs);
            }
        }

        static void Run()
        {
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                B3AtivosMonitor.Run();

                Process cProcess = Process.GetCurrentProcess();
                float cProcPvtMem = (cProcess.PrivateMemorySize64 / 1024f) / 1024f;
                Console.Title = "InoaTest - RAM Usage: " + cProcPvtMem.ToString("0.00") + "MB";

                Thread.Sleep(CheckInterval * 1000);
            }
        }

        static void Dispose()
        {
            B3AtivosMonitor.Dispose();

            B3AtivosMonitor = null;
            Configuration = null;
            SMTP = null;
        }
    }
}
