using System;
using System.Collections.Generic;
using System.Text.Json;
using RestSharp;

using InoaTest_Console.Views;
using InoaTest_Console.Helpers;

namespace InoaTest_Console.Models
{
    enum B3AtivoAction { Vender, Comprar, Ignorar }

    class APISettings
    {
        public string BaseURL { get; set; }
        public string Resource { get; set; }
    }

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
        private IRestResponse Response;

        private APISettings API;
        private SymbolArgs  Args;
        private B3AtivoMail Mail;
        private B3AtivoView View;

        private APIObject ObjItem;

        public B3AtivoModel(ref SymbolArgs Args, ref B3AtivoMail Mail, ref B3AtivoView View, ref APISettings API)
        {
            this.API  = API;
            this.Args = Args;
            this.Mail = Mail;
            this.View = View;
        }

        public void GetAssetData()
        {
            Client  = new RestClient(API.BaseURL);
            Request = new RestRequest(API.Resource + Args.Symbol);

            try
            {
                Response = Client.Get(Request);

                ObjItem = JsonSerializer.Deserialize<APIObject>(Response.Content);
                if (!(ObjItem.results[Args.Symbol] is null))
                {
                    ObjItem.results[Args.Symbol].Action = ((ObjItem.results[Args.Symbol].price > Args.RefSell) ? B3AtivoAction.Vender : (ObjItem.results[Args.Symbol].price < Args.RefBuy) ? B3AtivoAction.Comprar : B3AtivoAction.Ignorar);

                    switch (ObjItem.results[Args.Symbol].Action)
                    {
                        case B3AtivoAction.Vender:
                            Mail.Send("Recomendação de Venda", string.Format("Recomendamos a venda do ativo {0} ({1}{2})", Args.Symbol, ObjItem.results[Args.Symbol].currency, ObjItem.results[Args.Symbol].price));
                            break;
                        case B3AtivoAction.Comprar:
                            Mail.Send("Recomendação de Compra", string.Format("Recomendamos a compra do ativo {0} ({1}{2})", Args.Symbol, ObjItem.results[Args.Symbol].currency, ObjItem.results[Args.Symbol].price));
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
            View.Print(ObjItem.results[Args.Symbol]);
        }

        public void Dispose()
        {
            Client   = null;
            Request  = null;
            Response = null;
            API      = null;
            Args     = null;
            Mail     = null;
            View     = null;

            ObjItem  = null;

            GC.Collect();
            GC.SuppressFinalize(this);
        }
    }
}
