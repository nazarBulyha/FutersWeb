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
            ViewBag.Title = "Futures Web";
            var labelsToUpdate1 = new List<LabelToUpdate>
            {
                new LabelToUpdate { Content = "0%", Name = "BTCTW" },
                new LabelToUpdate { Content = "0%", Name = "BTCNW" },
                new LabelToUpdate { Content = "0%", Name = "BTCQ" }
            };
            var labelsToUpdate2 = new List<LabelToUpdate>
            {
                new LabelToUpdate { Content = "0%", Name = "LTCTW" },
                new LabelToUpdate { Content = "0%", Name = "LTCNW" },
                new LabelToUpdate { Content = "0%", Name = "LTCQ" }
            };
            var labelsToUpdate3 = new List<LabelToUpdate>
            {
                new LabelToUpdate { Content = "0%", Name = "ETHTW" },
                new LabelToUpdate { Content = "0%", Name = "ETHNW" },
                new LabelToUpdate { Content = "0%", Name = "ETHQ" }
            };
            var labelsToUpdate4 = new List<LabelToUpdate>
            {
                new LabelToUpdate { Content = "0%", Name = "ETCTW" },
                new LabelToUpdate { Content = "0%", Name = "ETCNW" },
                new LabelToUpdate { Content = "0%", Name = "ETCQ" }
            };
            var labelsToUpdate5 = new List<LabelToUpdate>
            {
                new LabelToUpdate { Content = "0%", Name = "BCHTW" },
                new LabelToUpdate { Content = "0%", Name = "BCHNW" },
                new LabelToUpdate { Content = "0%", Name = "BCHQ" }
            };
            var labelsToUpdate6 = new List<LabelToUpdate>
            {
                new LabelToUpdate { Content = "0%", Name = "BTGTW" },
                new LabelToUpdate { Content = "0%", Name = "BTGNW" },
                new LabelToUpdate { Content = "0%", Name = "BTGQ" }
            };
            var labelsToUpdate7 = new List<LabelToUpdate>
            {
                new LabelToUpdate { Content = "0%", Name = "XRPTW" },
                new LabelToUpdate { Content = "0%", Name = "XRPNW" },
                new LabelToUpdate { Content = "0%", Name = "XRPQ" }
            };
            var labelsToUpdate8 = new List<LabelToUpdate>
            {
                new LabelToUpdate { Content = "0%", Name = "EOSTW" },
                new LabelToUpdate { Content = "0%", Name = "EOSNW" },
                new LabelToUpdate { Content = "0%", Name = "EOSQ" }
            };

            var result = new Dictionary<string, List<LabelToUpdate>>
            {
                { "BTC", labelsToUpdate1 },
                { "LTC", labelsToUpdate2 },
                { "ETH", labelsToUpdate3 },
                { "ETC", labelsToUpdate4 },
                { "BCH", labelsToUpdate5 },
                { "BTG", labelsToUpdate6 },
                { "XRP", labelsToUpdate7 },
                { "EOS", labelsToUpdate8}
            };

            return View(result);
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

        private static async Task<FuturePosition> FuturePositionAsync(string symbol, string contractType)
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