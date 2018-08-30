namespace Futures
{
    using Futures.Model;
    using FuturesWeb.Controllers;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading.Tasks;

    public class Worker
    {
        #region Properties
        public readonly BackgroundWorker myWorker;

        private string Cumulative { get; set; }
        #endregion

        #region Consructor
        public Worker(string cumulative, List<LabelToUpdate> labelsToUpdate)
        {
            myWorker = new BackgroundWorker
            {
                WorkerSupportsCancellation = true,
                WorkerReportsProgress = false
            };
            myWorker.DoWork += (sender, e) => DoWorkAsync(sender, e, labelsToUpdate);
            Cumulative = cumulative;
        }
        #endregion

        #region Methods
        public async void DoWorkAsync(object sender, DoWorkEventArgs e, List<LabelToUpdate> labelsToUpdate)
        {
            var futureDepthResultList = new List<FutureDepth>();
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
            var sw = new Stopwatch();

            int.TryParse(ConfigurationManager.AppSettings["MarketDepth"], out int marketDepth);
            double.TryParse(Cumulative, out var cumul);

            while (true)
            {
                if (myWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                futureList.ForEach(x =>
                {
                    taskList.Add(FutureController.GetFutureDepthAsync(x));
                });

                sw.Start();
                try
                {
                    futureDepthResultList = (await Task.WhenAll(taskList)).ToList();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                sw.Stop();
                Debug.WriteLine(sw.Elapsed);
                sw.Reset();

                var i = 0;
                foreach (var futureResult in futureDepthResultList)
                {
                    var askPrice = futureResult.Asks.FirstOrDefault(z => z.Cumulative >= cumul / futureResult.Asks.First().Price);
                    var bidPrice = futureResult.Bids.FirstOrDefault(z => z.Cumulative >= cumul / futureResult.Bids.First().Price);

                    labelsToUpdate[i].Content = askPrice != null && bidPrice != null
                                ? Math.Round((askPrice.Price - bidPrice.Price) * 100 / askPrice.Price, 5) + "%"
                                : "0";
                    i++;
                }
                taskList.Clear();
                i = 0;

                UpdateLabels(ref labelsToUpdate);
            }
        }

        private void UpdateLabels(ref List<LabelToUpdate> labels)
        {
            foreach (var label in labels)
            {
                // TODO: implement logic to store and update labels.
                /*Application.Current.Dispatcher.Invoke(() => */label.Name = label.Content;
            }
        }
        #endregion
    }
}