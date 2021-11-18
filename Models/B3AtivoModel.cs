using System;
using System.Collections.Generic;
using System.Text.Json;
using RestSharp;

namespace InoaTest_Console
{
    public enum B3AtivoAction { Vender, Comprar, Ignorar }

    public class APIObject
    {
        public class APIObjectItem
        {
            public string symbol { get; set; }
            public string currency { get; set; }
            public double price { get; set; }
            public double change_percent { get; set; }
            public string updated_at { get; set; }
            public B3AtivoAction Action { get; set; }
        }

        public Dictionary<string, APIObjectItem> results { get; set; }
    }

    public interface IB3AtivoModel
    {
        void RESTWork(ref B3AtivoView pView);
    }

    public class B3AtivoModel : IB3AtivoModel
    {
        private RestClient  Client;
        private RestRequest Request;
        private B3AtivoMail Mail;

        private readonly string BaseAPI = "https://api.hgbrasil.com/finance/";
        private readonly string APIRes  = "stock_price?key=679d092c&symbol=";

        private string Symbol  = "PETR4";
        private double RefSell = 0.00;
        private double RefBuy  = 0.00;

        public B3AtivoModel(string RefSymbol, double RefSell, double RefBuy)
        {
            this.Symbol  = RefSymbol;
            this.RefSell = RefSell;
            this.RefBuy  = RefBuy;

            Client  = new RestClient(BaseAPI);
            Request = new RestRequest(APIRes + Symbol);
            Mail    = new B3AtivoMail("appsettings.json", "MailSettings");
        }

        public void RESTWork(ref B3AtivoView pView)
        {
            try
            {
                IRestResponse RESTResponse = Client.Get(Request);
                APIObject Object = JsonSerializer.Deserialize<APIObject>(RESTResponse.Content);

                if (Object.results.ContainsKey(Symbol))
                {
                    Object.results[Symbol].Action = ((Object.results[Symbol].price > RefSell) ? B3AtivoAction.Vender : (Object.results[Symbol].price < RefBuy) ? B3AtivoAction.Comprar : B3AtivoAction.Ignorar);
                    pView.Print(Object.results[Symbol]);

                    switch (Object.results[Symbol].Action)
                    {
                        case B3AtivoAction.Vender:
                            Mail.Send("Recomendação de Venda", String.Format("Recomendamos a venda do ativo {0} ({1}{2})", Symbol, Object.results[Symbol].currency, Object.results[Symbol].price));
                            break;
                        case B3AtivoAction.Comprar:
                            Mail.Send("Recomendação de Compra", String.Format("Recomendamos a compra do ativo {0} ({1}{2})", Symbol, Object.results[Symbol].currency, Object.results[Symbol].price));
                            break;
                        default: 
                            break;
                    }
                }

            } catch (Exception E)
            {
               throw new ArgumentException(E.Message);
            }
        }
    }
}
