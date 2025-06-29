using Newtonsoft.Json;

namespace Umbraco.Commerce.PaymentProviders.Reepay.Api.Models
{
    public class ReepayChargeSessionRequest : ReepaySessionRequestBase
    {
        [JsonProperty("settle")]
        public bool Settle { get; set; }

        [JsonProperty("order")]
        public ReepayOrder Order { get; set; }

        [JsonProperty("recurring")]
        public bool Recurring { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; } = "auto";

    }
}
