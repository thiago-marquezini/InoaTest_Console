using System;
using InoaTest_Console.Models;
using InoaTest_Console.Views;
using InoaTest_Console.Helpers;

namespace InoaTest_Console.Controllers
{
    interface IB3AtivoController
    {
        void AddSymbol(ref SymbolArgs SArgs);
        void Run();
        void Dispose();
    }

    class B3AtivoController : IB3AtivoController, IDisposable
    {
        private static B3AtivoView    SymbolView;
        private static B3AtivoMail    SymbolMail;
        private static Collection     SymbolCollection;
        private static Iterator       SymbolIterator;

        public B3AtivoController()
        {
            SymbolCollection = new Collection();
            SymbolView       = new B3AtivoView();
        }

        public void SetMail(SMTPSettings refSMTP)
        {
            SymbolMail = new B3AtivoMail(refSMTP);
        }

        public void AddSymbol(ref SymbolArgs SArgs)
        {
            SymbolCollection[SymbolCollection.Count] = new SymbolArgs(SArgs.Symbol, SArgs.RefSell, SArgs.RefBuy);
        }

        public void Run()
        {
            SymbolIterator = SymbolCollection.SetupIterator();

            for (SymbolArgs Arg = SymbolIterator.First(); !SymbolIterator.Finished; Arg = SymbolIterator.Next())
            {
                try
                {
                    B3AtivoModel SymbolModel = new B3AtivoModel(ref Arg, ref SymbolMail, ref SymbolView);

                    SymbolModel.GetAssetData();
                    SymbolModel.PrintAsset();
                    SymbolModel.Dispose();

                } catch (Exception E)
                {
                    throw new ArgumentException("Controller: " + E.Message);
                }
            }
        }

        public void Dispose()
        {
            SymbolView       = null;
            SymbolMail       = null;
            SymbolCollection = null;
            SymbolIterator   = null;

            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }
}
