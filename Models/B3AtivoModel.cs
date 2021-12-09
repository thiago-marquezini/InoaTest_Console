using System;
using System.Collections.Generic;
using System.Text.Json;
using RestSharp;
using InoaTest_Console.Views;
using InoaTest_Console.Helpers;

namespace InoaTest_Console.Models
{
    enum B3AtivoAction { Vender, Comprar, Ignorar }

    class APIObjectItem
    {
        public string symbol { get; set; }
        public string currency { get; set; }
        public double price { get; set; }
        public double change_percent { get; set; }
        public string updated_at { get; set; }
        public B3AtivoAction Action { get; set; }
    }

    class APIObject
    {
        public Dictionary<string, APIObjectItem> results 
        { 
            get; set;
        }
    }

    interface IB3AtivoModel
    {
        void RESTWork();
        void RESTDisplay(ref B3AtivoView pView);
    }

    class B3AtivoModel : IB3AtivoModel, IDisposable
    {
        private RestClient  Client;
        private RestRequest Request;

        private B3AtivoMail Mail;
        private SymbolArgs  Args;

        private APIObject ObjectItem;

        private string BaseAPI = "https://api.hgbrasil.com/finance/";
        private string APIRes  = "stock_price?key=679d092c&symbol=";

        public B3AtivoModel(ref SymbolArgs Args, ref B3AtivoMail Mail)
        {
            this.Args = Args;
            this.Mail = Mail;
        }

        public void RESTWork()
        {
            Client = new RestClient(BaseAPI);
            Request = new RestRequest(APIRes + Args.Symbol);

            try
            {
                IRestResponse RESTResponse = Client.Get(Request);

                ObjectItem = JsonSerializer.Deserialize<APIObject>(RESTResponse.Content);
                if (!(ObjectItem.results[Args.Symbol] is null))
                {
                    ObjectItem.results[Args.Symbol].Action = ((ObjectItem.results[Args.Symbol].price > Args.RefSell) ? B3AtivoAction.Vender : (ObjectItem.results[Args.Symbol].price < Args.RefBuy) ? B3AtivoAction.Comprar : B3AtivoAction.Ignorar);

                    switch (ObjectItem.results[Args.Symbol].Action)
                    {
                        case B3AtivoAction.Vender:
                            Mail.Send("Recomendação de Venda", string.Format("Recomendamos a venda do ativo {0} ({1}{2})", Args.Symbol, ObjectItem.results[Args.Symbol].currency, ObjectItem.results[Args.Symbol].price));
                            break;
                        case B3AtivoAction.Comprar:
                            Mail.Send("Recomendação de Compra", string.Format("Recomendamos a compra do ativo {0} ({1}{2})", Args.Symbol, ObjectItem.results[Args.Symbol].currency, ObjectItem.results[Args.Symbol].price));
                            break;
                        default: 
                            break;
                    }

                } else 
                { 
                    throw new ArgumentException("Invalid symbol object"); 
                }

            } catch (Exception E)
            {
               throw new ArgumentException("Model: " + E.Message);
            }
        }

        public void RESTDisplay(ref B3AtivoView pView)
        {
            pView.Print(ObjectItem.results[Args.Symbol]);
        }

        public void Dispose()
        {
            Client  = null;
            Request = null;
            Mail    = null;
            Args    = null;

            ObjectItem = null;

            BaseAPI = null;
            APIRes  = null;

            GC.SuppressFinalize(this);
        }
    }
}
