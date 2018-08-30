namespace Futures
{
    using System.Collections.Generic;
    using Futures.Model;
    using Newtonsoft.Json;

    public class FutureDepth
    {
        [JsonProperty(PropertyName = "asks")]
        public List<FutureDepthDetail> Asks { get; set; } = new List<FutureDepthDetail>();

        [JsonProperty(PropertyName = "bids")]
        public List<FutureDepthDetail> Bids { get; set; } = new List<FutureDepthDetail>();
    }
}