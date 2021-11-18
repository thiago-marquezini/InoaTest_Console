using System;

namespace InoaTest_Console
{
    class Program
    {
        private static int SymbolArgCount = 3; /* Argumentos (command-line) por ativo */
        private static B3AtivoController B3AtivosMonitor;

        static void Main(string[] args)
        {
            B3AtivosMonitor = new B3AtivoController();
            
            try
            {
                if ((args.Length % SymbolArgCount) == 0)
                {
                    for (int I = 0; I < (args.Length / SymbolArgCount); I++)
                    {
                        int SymbolIndex = I * SymbolArgCount;

                        string Symbol  = args[SymbolIndex];
                        double RefSell = double.Parse(args[SymbolIndex + 1], System.Globalization.CultureInfo.InvariantCulture);
                        double RefBuy  = double.Parse(args[SymbolIndex + 2], System.Globalization.CultureInfo.InvariantCulture);
                         
                        B3AtivosMonitor.AddSymbol(Symbol, RefSell, RefBuy);
                    }
                }

                B3AtivosMonitor.Run();

            } catch (Exception E)
            {
                Console.WriteLine("Erro: {0}", E.Message);
            }
        }
    }
}
