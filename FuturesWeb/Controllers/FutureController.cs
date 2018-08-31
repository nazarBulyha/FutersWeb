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
    using System.Diagnostics;

    public class FutureController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Futures Web";
            var labelsToUpdate1 = new List<Currency>
            {
                new Currency { Value = "0%", Name = "BTC" },
                new Currency { Value = "0%", Name = "BTC" },
                new Currency { Value = "0%", Name = "BTC" }
            };
            var labelsToUpdate2 = new List<Currency>
            {
                new Currency { Value = "0%", Name = "LTC" },
                new Currency { Value = "0%", Name = "LTC" },
                new Currency { Value = "0%", Name = "LTC" }
            };
            var labelsToUpdate3 = new List<Currency>
            {
                new Currency { Value = "0%", Name = "ETH" },
                new Currency { Value = "0%", Name = "ETH" },
                new Currency { Value = "0%", Name = "ETH" }
            };
            var labelsToUpdate4 = new List<Currency>
            {
                new Currency { Value = "0%", Name = "ETC" },
                new Currency { Value = "0%", Name = "ETC" },
                new Currency { Value = "0%", Name = "ETC" }
            };
            var labelsToUpdate5 = new List<Currency>
            {
                new Currency { Value = "0%", Name = "BCH" },
                new Currency { Value = "0%", Name = "BCH" },
                new Currency { Value = "0%", Name = "BCH" }
            };
            var labelsToUpdate6 = new List<Currency>
            {
                new Currency { Value = "0%", Name = "BTG" },
                new Currency { Value = "0%", Name = "BTG" },
                new Currency { Value = "0%", Name = "BTG" }
            };
            var labelsToUpdate7 = new List<Currency>
            {
                new Currency { Value = "0%", Name = "XRP" },
                new Currency { Value = "0%", Name = "XRP" },
                new Currency { Value = "0%", Name = "XRP" }
            };
            var labelsToUpdate8 = new List<Currency>
            {
                new Currency { Value = "0%", Name = "EOS" },
                new Currency { Value = "0%", Name = "EOS" },
                new Currency { Value = "0%", Name = "EOS" }
            };

            var result = new Dictionary<string, List<Currency>>
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

        [HttpGet]
        public async Task<ActionResult> GetFuturesAsync(string Cumulative)
        {
            // TODO: get dictionary result
            var futureDepthList = new List<FutureDepth>();
            var taskList = new List<Task<FutureDepth>>();
            var futureList = new List<Future>
                {
                    new Future { Currency = "btc_usd", ContractType = "this_week" },
                    new Future { Currency = "btc_usd", ContractType = "next_week" },
                    new Future { Currency = "btc_usd", ContractType = "quarter" },

                    new Future { Currency = "ltc_usd", ContractType = "this_week" },
                    new Future { Currency = "ltc_usd", ContractType = "next_week" },
                    new Future { Currency = "ltc_usd", ContractType = "quarter" },

                    new Future { Currency = "eth_usd", ContractType = "this_week" },
                    new Future { Currency = "eth_usd", ContractType = "next_week" },
                    new Future { Currency = "eth_usd", ContractType = "quarter" },

                    new Future { Currency = "etc_usd", ContractType = "this_week" },
                    new Future { Currency = "etc_usd", ContractType = "next_week" },
                    new Future { Currency = "etc_usd", ContractType = "quarter" },

                    new Future { Currency = "bch_usd", ContractType = "this_week" },
                    new Future { Currency = "bch_usd", ContractType = "next_week" },
                    new Future { Currency = "bch_usd", ContractType = "quarter" },

                    new Future { Currency = "btg_usd", ContractType = "this_week" },
                    new Future { Currency = "btg_usd", ContractType = "next_week" },
                    new Future { Currency = "btg_usd", ContractType = "quarter" },

                    new Future { Currency = "xrp_usd", ContractType = "this_week" },
                    new Future { Currency = "xrp_usd", ContractType = "next_week" },
                    new Future { Currency = "xrp_usd", ContractType = "quarter" },

                    new Future { Currency = "eos_usd", ContractType = "this_week" },
                    new Future { Currency = "eos_usd", ContractType = "next_week" },
                    new Future { Currency = "eos_usd", ContractType = "quarter" }
                };

            int.TryParse(ConfigurationManager.AppSettings["MarketDepth"], out var marketDepth);
            double.TryParse(Cumulative, out var cumul);
            futureList.ForEach(x =>
            {
                taskList.Add(FutureController.GetFutureDepthAsync(x));
            });

            futureDepthList = (await Task.WhenAll(taskList)).ToList();

            var i = 0;
            foreach (var futureResult in futureDepthList)
            {
                var askPrice = futureResult.Asks.FirstOrDefault(z => z.Cumulative >= cumul / futureResult.Asks.First().Price);
                var bidPrice = futureResult.Bids.FirstOrDefault(z => z.Cumulative >= cumul / futureResult.Bids.First().Price);

                // rewrite existing list by our needs to group it by currency later
                futureList[i].ContractType = askPrice != null && bidPrice != null
                            ? Math.Round((askPrice.Price - bidPrice.Price) * 100 / askPrice.Price, 5) + "%"
                            : "0";
                i++;
            }

            var groupedFutureList = futureList
                .GroupBy(f => f.Currency)
                .Select(c => c.ToList())
                .ToDictionary(x => x.First().Currency, x => x);

            //taskList.Clear();
            //i = 0;

            return PartialView("_FutureTable", groupedFutureList);
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