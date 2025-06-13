namespace Umbraco.Commerce.PaymentProviders.Reepay.Api
{
    public class ReepayClientConfig
    {
        public string BaseUrl { get; set; }

        public string Authorization { get; set; }

        public string WebhookSecret { get; set; }

        public string PrivateKey { get; set; }

    }
}
