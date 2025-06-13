using Newtonsoft.Json;

namespace Umbraco.Commerce.PaymentProviders.Reepay.Api.Models
{
    public class AddCardPaymentMethodRequest
    {
        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("card_token")]
        public string Card_token { get; set; }
    }
}
