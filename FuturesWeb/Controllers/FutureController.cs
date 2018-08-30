namespace FuturesWeb.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System.Configuration;
    using System.Threading.Tasks;
    using Futures;
    using Futures.Model;

    public class FutureController : Controller
    {
        // GET: Future
        public ActionResult Index()
        {
            return View();
        }

        public static async Task<FutureDepth> GetFutureDepthAsync(Future futureModel)
        {
            var paras = new Dictionary<string, string>
                            {
                                { "symbol", futureModel.Currency },
                                { "contract_type", futureModel.ContractType },
                                { "size", Convert.ToInt32(ConfigurationManager.AppSettings["MarketDepth"]).ToString() }
                            };
            var url = ConfigurationManager.AppSettings["OkexUrl"] + ConfigurationManager.AppSettings["FutureDepthUrl"];

            Md5.CreateUrl(ref url, paras);

            var request = HttpHelper.CreateGetRequest(url);
            var response = await HttpHelper.GetResponseAsync(request);
            var data = await HttpHelper.ReadResponseAsync(response);

            var multiplier = futureModel.Currency == "btc_usd" ? 10 : 1;
            var futureDepth = new FutureDepth();
            var apiResult = JObject.Parse(data);

            foreach (var itemList in apiResult)
            {
                var reverseAsks = itemList.Value.Reverse();
                double totalCumulative = 0;

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (itemList.Key == "asks")
                {
                    foreach (var item in reverseAsks)
                    {
                        var amount = Math.Round(item.Last.Value<double>() * 10 * multiplier / item.First.Value<double>(), 5);
                        var ask = new FutureDepthDetail
                        {
                            Price = item.First.Value<double>(),
                            Amount = amount,
                            Cumulative = Math.Round(totalCumulative + amount, 5)
                        };

                        totalCumulative = ask.Cumulative;
                        futureDepth.Asks.Add(ask);
                    }
                }
                else if (itemList.Key == "bids")
                {
                    foreach (var item in itemList.Value)
                    {
                        var amount = Math.Round(item.Last.Value<double>() * 10 * multiplier / item.First.Value<double>(), 5);
                        var bid = new FutureDepthDetail
                        {
                            Price = item.First.Value<double>(),
                            Amount = amount,
                            Cumulative = Math.Round(totalCumulative + amount, 5)
                        };

                        totalCumulative = bid.Cumulative;
                        futureDepth.Bids.Add(bid);
                    }
                }
            }

            return futureDepth;
        }

        public static async Task<FuturePosition> FuturePositionAsync(string symbol, string contractType)
        {
            var paras = new Dictionary<string, string>
                            {
                                { "symbol", symbol },
                                { "contract_type", contractType },
                                { "api_key", ConfigurationManager.AppSettings[ "apiPublicKey" ] }
                            };
            var url = ConfigurationManager.AppSettings["OkexUrl"] + ConfigurationManager.AppSettings["FuturePositionUrl"];

            Md5.AddSign(ref paras);

            Md5.CreateUrl(ref url, paras);

            var request = HttpHelper.CreatePostRequest(url);
            var response = await HttpHelper.GetResponseAsync(request);
            var data = await HttpHelper.ReadResponseAsync(response);

            return JsonConvert.DeserializeObject<FuturePosition>(data);
        }
    }
}