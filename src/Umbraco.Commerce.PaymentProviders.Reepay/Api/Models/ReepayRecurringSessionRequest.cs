using Newtonsoft.Json;

namespace Umbraco.Commerce.PaymentProviders.Reepay.Api.Models
{
    public class ReepayRecurringSessionRequest : ReepaySessionRequestBase
    {
        [JsonProperty("customer")]
        public string Customer { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("create_customer")]
        public ReepayCustomer CreateCustomer { get; set; }
    }
}
