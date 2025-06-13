using Newtonsoft.Json;
using Umbraco.Commerce.Core.Models;

namespace Umbraco.Commerce.PaymentProviders.Reepay.Api.Models
{
    public class ReepayCharge
    {
        [JsonProperty("handle")]
        public string Handle { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("customer")]
        public string Customer { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("authorized")]
        public DateTime? Authorized { get; set; }

        [JsonProperty("settled")]
        public DateTime? Settled { get; set; }

        [JsonProperty("cancelled")]
        public DateTime? Cancelled { get; set; }

        [JsonProperty("created")]
        public DateTime Created { get; set; }

        [JsonProperty("transaction")]
        public string Transaction { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_state")]
        public string ErrorState { get; set; }

        [JsonProperty("processing")]
        public bool Processing { get; set; }

        [JsonProperty("refunded_amount")]
        public int RefundedAmount { get; set; }

        [JsonProperty("authorized_amount")]
        public int AuthorizedAmount { get; set; }

        [JsonProperty("source")]
        public ReepaySource Source { get; set; }

        [JsonProperty("billing_address")]
        public ReepayAddress BillingAddress { get; set; }

        [JsonProperty("shipping_address")]
        public ReepayAddress ShippingAddress { get; set; }



        [JsonProperty("order_lines")]
        public List<dynamic> Order_lines { get; set; }

        [JsonProperty("refunded_amount")]
        public int Refunded_amount { get; set; }

        [JsonProperty("authorized_amount")]
        public int Authorized_amount { get; set; }

        [JsonProperty("error_state")]
        public string Error_state { get; set; }

        [JsonProperty("recurring_payment_method")]
        public string Recurring_payment_method { get; set; }

        [JsonProperty("billing_address")]
        public Address Billing_address { get; set; }

        [JsonProperty("shipping_address")]
        public Address Shipping_address { get; set; }
    }
}
