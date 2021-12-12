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
        void GetAssetData();
        void PrintAsset();
        void Dispose();
    }

    class B3AtivoModel : IB3AtivoModel, IDisposable
    {
        private RestClient  Client;
        private RestRequest Request;

        private SymbolArgs  Args;
        private B3AtivoMail Mail;
        private B3AtivoView View;

        private APIObject ObjectItem;

        public B3AtivoModel(ref SymbolArgs Args, ref B3AtivoMail Mail, ref B3AtivoView View)
        {
            this.Args = Args;
            this.Mail = Mail;
            this.View = View;
        }

        public void GetAssetData()
        {
            Client  = new RestClient("https://api.hgbrasil.com/finance/");
            Request = new RestRequest("stock_price?key=e33c0784&symbol=" + Args.Symbol);

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
                    throw new ArgumentException("Model: Invalid symbol object"); 
                }

            } catch (Exception E)
            {
               throw new ArgumentException("Model: " + E.Message);
            }
        }

        public void PrintAsset()
        {
            View.Print(ObjectItem.results[Args.Symbol]);
        }

        public void Dispose()
        {
            Client  = null;
            Request = null;
            Args    = null;
            Mail    = null;
            View    = null;

            ObjectItem = null;

            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }
}
