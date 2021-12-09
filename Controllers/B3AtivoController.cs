using System;
using System.Threading;
using InoaTest_Console.Models;
using InoaTest_Console.Views;
using InoaTest_Console.Helpers;

namespace InoaTest_Console.Controllers
{
    interface IB3AtivoController
    {
        void AddSymbol(ref SymbolArgs SArgs);
        void Run();
    }

    class B3AtivoController : IB3AtivoController, IDisposable
    {
        private int CheckInterval = 5; /* Intervalo de atualizacao em segundos(s) */

        private static B3AtivoView    SymbolView;
        private static B3AtivoMail    Mail;
        private static Collection     SymbolCollection;
        private static Iterator       SymbolIterator;

        public B3AtivoController()
        {
            SymbolCollection = new Collection();
            SymbolView       = new B3AtivoView();
            Mail             = new B3AtivoMail("appsettings.json", "MailSettings");
        }

        ~B3AtivoController()
        {
            Dispose();
        }

        public void AddSymbol(ref SymbolArgs SArgs)
        {
            SymbolCollection[SymbolCollection.Count] = new SymbolArgs(SArgs.Symbol, SArgs.RefSell, SArgs.RefBuy);
        }

        public void Run()
        {
            SymbolIterator = SymbolCollection.SetupIterator();

            while (true)
            {
                for (SymbolArgs Arg = SymbolIterator.First(); !SymbolIterator.Finished; Arg = SymbolIterator.Next())
                {
                    try
                    {
                        B3AtivoModel SBM = new B3AtivoModel(ref Arg, ref Mail);

                        SBM.RESTWork();
                        SBM.RESTDisplay(ref SymbolView);

                        SBM.Dispose();

                    } catch (Exception E)
                    {
                        throw new ArgumentException("Controller: " + E.Message);
                    }
                }

                Thread.Sleep(CheckInterval * 1000);
            }
        }

        public void Dispose()
        {
            CheckInterval = 0;

            SymbolView       = null;
            Mail             = null;
            SymbolCollection = null;
            SymbolIterator   = null;

            GC.SuppressFinalize(this);
        }
    }
}
