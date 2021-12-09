using System;
using InoaTest_Console.Controllers;
using InoaTest_Console.Helpers;

namespace InoaTest_Console
{
    /*
      Pontos de melhorias

        - estruturado da forma                                [ MVC? ]
        - dificuldade de ler o código                         [  Ok? ]
        - lendo o config a cada envio (e refazendo o builder) [  Ok  ]
        - a lógica de negócio em um "one liner" pouco legível [  Ok? ]
        - não blindado                                        [  Ok? ]
        - sem dispose                                         [  x   ]
    */
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

                        SymbolArgs SArgs = new SymbolArgs(args[SymbolIndex], 
                                                          double.Parse(args[SymbolIndex + 1], System.Globalization.CultureInfo.InvariantCulture), 
                                                          double.Parse(args[SymbolIndex + 2], System.Globalization.CultureInfo.InvariantCulture));
                         
                        B3AtivosMonitor.AddSymbol(ref SArgs);
                    }
                }

                B3AtivosMonitor.Run();
                B3AtivosMonitor.Dispose();

            } catch (Exception E)
            {
                Console.WriteLine("Erro: {0}", E.Message);
            }
        }
    }
}
