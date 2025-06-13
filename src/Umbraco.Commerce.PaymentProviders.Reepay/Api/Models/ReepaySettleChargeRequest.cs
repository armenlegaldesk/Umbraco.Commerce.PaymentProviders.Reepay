using Newtonsoft.Json;

namespace Umbraco.Commerce.PaymentProviders.Reepay.Api.Models
{
    public class ReepaySettleChargeRequest
    {
        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("ordertext")]
        public string Ordertext { get; set; }

        [JsonProperty("order_lines")]
        public IList<CreateOrderLine> Order_lines { get; set; }
    }

}
