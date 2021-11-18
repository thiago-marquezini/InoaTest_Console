using System;
using System.Threading;

namespace InoaTest_Console
{
    public interface IB3AtivoController
    {
        void AddSymbol(string Symbol, double RefSell, double RefBuy);
        void Run();
    }

    public class B3AtivoController : IB3AtivoController
    {
        private const int CheckInterval = 5; /* Intervalo de atualizacao em segundos(s) */

        private static B3AtivoView    SymbolView;
        private static B3AtivoModel[] SymbolModel;
        private static Collection     SymbolCollection;
        private static Iterator       SymbolIterator;

        public B3AtivoController()
        {
            SymbolCollection = new Collection();
            SymbolView       = new B3AtivoView();
        }

        public void AddSymbol(string Symbol, double RefSell, double RefBuy)
        {
            SymbolCollection[SymbolCollection.Count] = new SymbolArgs(Symbol, RefSell, RefBuy);
        }

        public void Run()
        {
            SymbolModel    = new B3AtivoModel[SymbolCollection.Count];
            SymbolIterator = SymbolCollection.SetupIterator();

            for (SymbolArgs Arg = SymbolIterator.First(); !SymbolIterator.Finished; Arg = SymbolIterator.Next())
            {
                SymbolModel[SymbolIterator.Index] = new B3AtivoModel(Arg.Symbol, Arg.RefSell, Arg.RefBuy);
            }

            while (true)
            {
                try
                {
                    SymbolIterator.First();
                    while (!SymbolIterator.Finished)
                    {
                        SymbolModel[SymbolIterator.Index].RESTWork();
                        SymbolModel[SymbolIterator.Index].RESTDisplay(ref SymbolView);

                        SymbolIterator.Next();
                    }

                } catch (Exception E)
                {
                    throw new ArgumentException(E.Message);
                }

                Thread.Sleep(CheckInterval * 1000);
            }
        }
    }
}
