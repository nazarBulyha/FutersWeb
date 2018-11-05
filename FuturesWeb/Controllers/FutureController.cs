namespace FuturesWeb.Controllers
{
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Diagnostics;
	using System.Linq;
	using System.Threading.Tasks;
	using System.Web.Mvc;
	using Models;
	using UtilHelper;
	using Newtonsoft.Json.Linq;

    public class FutureController : Controller
    {
        // <summary> Return list of Currency model
        // with currency name and 0% filled values. 
        // </summary>
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Title = "Futures Web";

            #region Empty labels data init

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

            #endregion

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

        // <summary> Cumulative - amount that we needed to but or sell
        // divided by lowest price, that depends data(asks or bids).
        // return list of currencies and their values, modified by some logic.
        // </summary>
        [HttpPost]
        public async Task<PartialViewResult> Index(string cumulative)
        {
	        var tasks = new List<Task<FutureDepth>>();
            var currencies = new List<Currency>();
            var futures = new List<Future>
            {
                new Future
                {
                    Currency = new Currency { Name = "btc_usd" },
                    ContractType = "this_week"
                },
                new Future
                {
                    Currency = new Currency { Name = "btc_usd" },
                    ContractType = "quarter"
                },
                new Future
                {
                    Currency = new Currency { Name = "btc_usd" },
                    ContractType = "next_week"
                },

                new Future
                {
                    Currency = new Currency { Name = "ltc_usd" },
                    ContractType = "this_week"
                },
                new Future
                {
                    Currency = new Currency { Name = "ltc_usd" },
                    ContractType = "next_week"
                },
                new Future
                {
                    Currency = new Currency { Name = "ltc_usd" },
                    ContractType = "quarter"
                },

                new Future
                {
                    Currency = new Currency { Name = "eth_usd" },
                    ContractType = "this_week"
                },
                new Future
                {
                    Currency = new Currency { Name = "eth_usd" },
                    ContractType = "next_week"
                },
                new Future
                {
                    Currency = new Currency { Name = "eth_usd" },
                    ContractType = "quarter"
                },

                new Future
                {
                    Currency = new Currency { Name = "etc_usd" },
                    ContractType = "this_week"
                },
                new Future
                {
                    Currency = new Currency { Name = "etc_usd" },
                    ContractType = "next_week"
                },
                new Future
                {
                    Currency = new Currency { Name = "etc_usd" },
                    ContractType = "quarter"
                },

                new Future
                {
                    Currency = new Currency { Name = "bch_usd" },
                    ContractType = "this_week"
                },
                new Future
                {
                    Currency = new Currency { Name = "bch_usd" },
                    ContractType = "next_week"
                },
                new Future
                {
                    Currency = new Currency { Name = "bch_usd" },
                    ContractType = "quarter"
                },

                new Future
                {
                    Currency = new Currency { Name = "btg_usd" },
                    ContractType = "this_week"
                },
                new Future
                {
                    Currency = new Currency { Name = "btg_usd" },
                    ContractType = "next_week"
                },
                new Future
                {
                    Currency = new Currency { Name = "btg_usd" },
                    ContractType = "quarter"
                },

                new Future
                {
                    Currency = new Currency { Name = "xrp_usd" },
                    ContractType = "this_week"
                },
                new Future
                {
                    Currency = new Currency { Name = "xrp_usd" },
                    ContractType = "next_week"
                },
                new Future
                {
                    Currency = new Currency { Name = "xrp_usd" },
                    ContractType = "quarter"
                },

                new Future
                { Currency = new Currency { Name = "eos_usd" },
                    ContractType = "this_week"
                },
                new Future
                {
                    Currency = new Currency { Name = "eos_usd" },
                    ContractType = "next_week"
                },
                new Future
                {
                    Currency = new Currency { Name = "eos_usd" },
                    ContractType = "quarter"
                }
            };
            var i = 0;

            int.TryParse(ConfigurationManager.AppSettings["MarketDepth"], out var marketDepth);
            double.TryParse(cumulative, out var cumul);

            // Fill tasks list
            foreach (var future in futures)
            {
                tasks.Add(GetFutureDepthAsync(future));
            }

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            // Await for result of all tasks
            var futureDepthList = (await Task.WhenAll(tasks)).AsEnumerable();
            stopWatch.Stop();
            Debug.WriteLine(stopWatch.Elapsed);

            // Some logic to get and store value for currency that is displayed
            foreach (var futureResult in futureDepthList)
            {
                // core logic
                var askPrice = futureResult.Asks.FirstOrDefault(z => z.Cumulative >= cumul / z.Price);
                var bidPrice = futureResult.Bids.FirstOrDefault(z => z.Cumulative >= cumul / z.Price);

                futures[i].Currency.Value = askPrice != null && bidPrice != null
                                            ? Math.Round((askPrice.Price - bidPrice.Price) * 100 / askPrice.Price, 2) + "%"
                                            : "0";
                i++;
            }

            // Extract currencies from Future model
            foreach (var future in futures)
            {
                var currency = new Currency
                {
                    Name = future.Currency.Name,
                    Value = future.Currency.Value
                };

                currencies.Add(currency);
            }

            // Group currencies by name
            // example: (btc: List(value1, value2, value3))
            //          (ltc: List(value1, value2, value3))
            var groupedCurrencyList = currencies.GroupBy(f => f.Name)
                                                .Select(c => c.ToList())
                                                .ToDictionary(x => x.First().Name, x => x);

            // Draw grouped list in partial view
            return PartialView("_FutureTable", groupedCurrencyList);
        }

        // <summary> Makes async API call to OKex. Get JSON response
        // of Asks abd Bids for selected currency. Map result to Future model.
        // </summary>
        public async Task<FutureDepth> GetFutureDepthAsync(Future futureModel)
        {
            var paras = new Dictionary<string, string>
            {
                { "symbol", futureModel.Currency.Name },
                { "contract_type", futureModel.ContractType },
                { "size", Convert.ToInt32(ConfigurationManager.AppSettings["MarketDepth"]).ToString() }
            };
            var url = ConfigurationManager.AppSettings["OkexUrl"] + ConfigurationManager.AppSettings["FutureDepthUrl"];

            Md5.CreateUrl(ref url, paras);

            var request = HttpHelper.CreateGetRequest(url);
            var response = await HttpHelper.GetResponseAsync(request);
            var data = HttpHelper.ReadResponse(response);

            var multiplier = futureModel.Currency.Name == "btc_usd" ? 10 : 1;
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
    }
}