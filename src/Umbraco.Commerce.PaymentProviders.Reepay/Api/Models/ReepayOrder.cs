﻿using Newtonsoft.Json;

namespace Umbraco.Commerce.PaymentProviders.Reepay.Api.Models
{
    public class ReepayOrder
    {
        [JsonProperty("handle")]
        public string Handle { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("customer")]
        public ReepayCustomer Customer { get; set; }

        [JsonProperty("billing_address")]
        public ReepayAddress BillingAddress { get; set; }

        [JsonProperty("shipping_address")]
        public ReepayAddress ShippingAddress { get; set; }

        [JsonProperty("metadata")]
        public Dictionary<string, string> MetaData { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; } = "auto";
    }

}
