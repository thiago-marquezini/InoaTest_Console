using System;
using System.Threading;
using System.Diagnostics;
using System.IO;

using Microsoft.Extensions.Configuration;

using InoaTest_Console.Models;
using InoaTest_Console.Controllers;
using InoaTest_Console.Helpers;

namespace InoaTest_Console
{

    class Program
    {
        private static string WndTitle    = "B3AtivosMonitor";
        private static string SettingFile = "";
        private static int CheckInterval  = 0;
        private static int WeekDayMin     = 0;
        private static int WeekDayMax     = 0;

        private static B3AtivoController B3AtivosMonitor;
        private static SMTPSettings      SMTP = new SMTPSettings();
        private static APISettings       API  = new APISettings();

        private static IConfiguration    Configuration;

        static void Main(string[] args)
        {
            Console.Title = WndTitle;
            SettingFile = Process.GetCurrentProcess().ProcessName + ".json";

            if (File.Exists(SettingFile))
            {
                B3AtivosMonitor = new B3AtivoController();
                try
                {
                    GetSettings(SettingFile);
                    Run();
                    Dispose();

                }
                catch (Exception E)
                {
                    Console.WriteLine("Erro: {0}", E.Message);
                }
            
            } else
            {
                Console.WriteLine("Failed reading settings file. Expected filename: {0}", SettingFile);
            }
        }

        static void GetSettings(string Settings)
        {
            Configuration = new ConfigurationBuilder().AddJsonFile(Settings, optional: false, reloadOnChange: true).Build();

            /* General */
            IConfigurationSection GeneralSection = Configuration.GetSection("General");
            CheckInterval = GeneralSection.GetValue<int>("CheckInterval");
            WeekDayMin    = GeneralSection.GetValue<int>("WeekDayMin");
            WeekDayMax    = GeneralSection.GetValue<int>("WeekDayMax");

            /* API */
            Configuration.GetSection("APISettings").Bind(API);
            B3AtivosMonitor.SetAPI(ref API);

            /* Mail */
            Configuration.GetSection("MailSettings").Bind(SMTP);
            B3AtivosMonitor.SetMail(ref SMTP);

            /* Assets */
            IConfigurationSection AssetsSection = Configuration.GetSection("Assets");
            foreach (IConfigurationSection Asset in AssetsSection.GetChildren())
            {
                SymbolArgs SArgs = new SymbolArgs(Asset.Key, Asset.GetValue<double>("SellPrice"), Asset.GetValue<double>("BuyPrice"));
                B3AtivosMonitor.AddSymbol(ref SArgs);
            }
        }

        static bool ValidWeekDay()
        {
            DateTime Today = DateTime.Now;
            int DayNumber = (int)Today.DayOfWeek;

            if ((DayNumber >= WeekDayMin) && (DayNumber <= WeekDayMax)) 
                return true;
            else
                Console.WriteLine("Dia Ignorado: {0}", Today.DayOfWeek);
                return false;
        }

        static void ProcRamUsage()
        {
            Process cProcess  = Process.GetCurrentProcess();
            float cProcPvtMem = (cProcess.PrivateMemorySize64 / 1024f) / 1024f;
            Console.Title     = WndTitle + " - RAM Usage: " + cProcPvtMem.ToString("0.000") + " MB";
        }

        static void Run()
        {
            while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Escape))
            {
                if (ValidWeekDay()) 
                B3AtivosMonitor.Run(); 

                ProcRamUsage();
                Thread.Sleep(CheckInterval * 1000);
            }

            B3AtivosMonitor.Dispose();
        }

        static void Dispose()
        {
            B3AtivosMonitor = null;
            SMTP            = null;
            API             = null;
            Configuration   = null;

            GC.Collect();
        }
    }
}
