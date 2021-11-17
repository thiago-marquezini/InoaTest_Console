using System;

namespace InoaTest_Console
{
    class B3AtivoController
    {
        private B3AtivoView  pView = new B3AtivoView();
        private B3AtivoModel pModel;

        public B3AtivoController(string Symbol, double RefSell, double RefBuy)
        {
            pModel = new B3AtivoModel(Symbol, RefSell, RefBuy);
        }

        public void Loop()
        {
            try
            {
                pModel.RESTWork(ref pView);

            } catch (Exception E)
            {
                Console.WriteLine("Erro: {0}", E.Message);
            }
        }
    }
}
