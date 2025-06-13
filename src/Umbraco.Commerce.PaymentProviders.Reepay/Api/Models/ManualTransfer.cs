using Newtonsoft.Json;

namespace Umbraco.Commerce.PaymentProviders.Reepay.Api.Models
{
    public class ManualTransfer
    {
        [JsonProperty("comment")]
        public string Comment { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }

        [JsonProperty("payment_date")]
        public string Payment_date { get; set; }
    }

}
