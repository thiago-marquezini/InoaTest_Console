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
        private static S ProtoSymbol<S>(S Source) { return (Source is null) ? default : JsonSerializer.Deserialize<S>(JsonSerializer.Serialize(Source)); }
        public APIObjectItem GetSymbol(string Symbol) { return ProtoSymbol(results[Symbol]); }
    }

    public interface IB3AtivoModel
    {
        void RESTWork();
        void RESTDisplay(ref B3AtivoView pView);
    }

    public class B3AtivoModel : IB3AtivoModel
    {
        private RestClient  Client;
        private RestRequest Request;
        private B3AtivoMail Mail;

        private APIObject.APIObjectItem ObjectItem;

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

        public void RESTWork()
        {
            try
            {
                IRestResponse RESTResponse = Client.Get(Request);

                ObjectItem = JsonSerializer.Deserialize<APIObject>(RESTResponse.Content).GetSymbol(Symbol);
                if (!(ObjectItem is null))
                {
                    ObjectItem.Action = ((ObjectItem.price > RefSell) ? B3AtivoAction.Vender : (ObjectItem.price < RefBuy) ? B3AtivoAction.Comprar : B3AtivoAction.Ignorar);

                    switch (ObjectItem.Action)
                    {
                        case B3AtivoAction.Vender:
                            Mail.Send("Recomendação de Venda", string.Format("Recomendamos a venda do ativo {0} ({1}{2})", Symbol, ObjectItem.currency, ObjectItem.price));
                            break;
                        case B3AtivoAction.Comprar:
                            Mail.Send("Recomendação de Compra", string.Format("Recomendamos a compra do ativo {0} ({1}{2})", Symbol, ObjectItem.currency, ObjectItem.price));
                            break;
                        default: 
                            break;
                    }

                } else { throw new ArgumentException("Invalid symbol object"); }

            } catch (Exception E)
            {
               throw new ArgumentException(E.Message);
            }
        }

        public void RESTDisplay(ref B3AtivoView pView)
        {
            pView.Print(ObjectItem);
        }
    }
}
