using System;
using System.Threading;

namespace InoaTest_Console
{
    class B3AtivoController
    {
        private static int CheckInterval = 1; /* Intervalo de atualizacao em minuto(s) */
        private static Collection SymbolCollection = new Collection();
        private static Iterator AtivoIterator;
        private static B3AtivoView  pView = new B3AtivoView();
        private static B3AtivoModel[] pModel;

        public void AddSymbol(string Symbol, double RefSell, double RefBuy)
        {
            SymbolCollection[SymbolCollection.Count] = new AtivoArgs(Symbol, RefSell, RefBuy);
        }

        public void Run()
        {
            AtivoIterator = SymbolCollection.SetupIterator();

            for (AtivoArgs Arg = AtivoIterator.First(); !AtivoIterator.Finished; Arg = AtivoIterator.Next())
            {
                pModel[AtivoIterator.Index] = new B3AtivoModel(Arg.Symbol, Arg.RefSell, Arg.RefBuy);
            }

            while (true)
            {
                try
                {
                    AtivoIterator.First();
                    while (!AtivoIterator.Finished)
                    {
                        pModel[AtivoIterator.Index].RESTWork(ref pView);
                        AtivoIterator.Next();
                    }

                } catch (Exception E)
                {
                    Console.WriteLine("Erro: {0}", E.Message);
                }

                Thread.Sleep(CheckInterval * 60000);
            }
        }
    }
}
