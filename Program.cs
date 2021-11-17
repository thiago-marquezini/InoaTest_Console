using System;
using System.Threading;

namespace InoaTest_Console
{
    class Program
    {
        private static int SymbolArgCount = 3; /* Argumentos (command-line) por ativo */
        private static int CheckInterval  = 1; /* Intervalo de atualizacao em minuto(s) */

        private static B3AtivoController[] B3AtivosMonitor;
        private static Collection SymbolCollection = new Collection();
        private static Iterator ArgIterator;

        static void Main(string[] args)
        {
            try
            {
                if ((args.Length % SymbolArgCount) == 0)
                {
                    for (int I = 0; I < (args.Length / SymbolArgCount); I++)
                    {
                        int SymbolIndex  = I * SymbolArgCount;

                        string Symbol    = args[SymbolIndex];
                        double RefSell   = double.Parse(args[SymbolIndex + 1], System.Globalization.CultureInfo.InvariantCulture);
                        double RefBuy    = double.Parse(args[SymbolIndex + 2], System.Globalization.CultureInfo.InvariantCulture);

                        SymbolCollection[I] = new AtivoArgs(Symbol, RefSell, RefBuy);
                    }
                }

                Setup();
                Run();

            } catch
            {
                Console.WriteLine("[ ERRO ] Argumentos invalidos! Uso: InoaTest_Console.exe [ATIVO0 VENDA0 COMPRA0 ATIVO1 VENDA1 COMPRA1 ...]");
                Environment.Exit(0);
            }
        }

        static void Setup()
        {
            B3AtivosMonitor = new B3AtivoController[SymbolCollection.Count];
            ArgIterator = SymbolCollection.SetupIterator();

            for (AtivoArgs Arg = ArgIterator.First(); !ArgIterator.Finished; Arg = ArgIterator.Next())
            {
                B3AtivosMonitor[ArgIterator.Index] = new B3AtivoController(Arg.Symbol, Arg.RefSell, Arg.RefBuy);
            }
        }

        static void Run()
        {
            while (true)
            {
                ArgIterator.First();
                while (!ArgIterator.Finished)
                {
                    B3AtivosMonitor[ArgIterator.Index].Loop();
                    ArgIterator.Next();
                }

                Thread.Sleep(CheckInterval * 60000);
            }
        }
    }
}
