using Umbraco.Commerce.Core.PaymentProviders;

namespace Umbraco.Commerce.PaymentProviders.Reepay
{
    public class ReepaySettingsBase
    {
        [PaymentProviderSetting(Label = "Continue URL", Description = "The URL to continue to after this provider has done processing. eg: /continue/", SortOrder = 100)]
        public string ContinueUrl { get; set; }

        [PaymentProviderSetting(Label = "Cancel URL", Description = "The URL to return to if the payment attempt is canceled. eg: /cancel/", SortOrder = 200)]
        public string CancelUrl { get; set; }


        [PaymentProviderSetting(Label = "Error URL", Description = "The URL to return to if the payment attempt errors. eg: /error/", SortOrder = 300)]
        public string ErrorUrl { get; set; }


        [PaymentProviderSetting(Label = "Billing Company Property Alias", Description = "The order property alias containing company of the billing address", SortOrder = 400)]
        public string BillingCompanyPropertyAlias { get; set; }

        [PaymentProviderSetting(Label = "Billing Address (Line 1) Property Alias", Description = "The order property alias containing line 1 of the billing address", SortOrder = 500)]
        public string BillingAddressLine1PropertyAlias { get; set; }

        [PaymentProviderSetting(Label = "Billing Address (Line 2) Property Alias", Description = "The order property alias containing line 2 of the billing address", SortOrder = 600)]
        public string BillingAddressLine2PropertyAlias { get; set; }

        [PaymentProviderSetting(Label = "Billing Address City Property Alias", Description = "The order property alias containing the city of the billing address", SortOrder = 700)]
        public string BillingAddressCityPropertyAlias { get; set; }

        [PaymentProviderSetting(Label = "Billing Address ZipCode Property Alias", Description = "The order property alias containing the zip code of the billing address", SortOrder = 800)]
        public string BillingAddressZipCodePropertyAlias { get; set; }

        [PaymentProviderSetting(Label = "Billing Address State Property Alias", Description = "The order property alias containing the state of the billing address", SortOrder = 900)]
        public string BillingAddressStatePropertyAlias { get; set; }

        [PaymentProviderSetting(Label = "Billing Phone Property Alias", Description = "The order property alias containing the phone of the billing address", SortOrder = 1000)]
        public string BillingPhonePropertyAlias { get; set; }


        [PaymentProviderSetting(Label = "Private Key", Description = "Private Key from the Reepay administration portal.", SortOrder = 1100)]
        public string PrivateKey { get; set; }

        [PaymentProviderSetting(Label = "Webhook Secret", Description = "Webhook Secret from the Reepay administration portal.", SortOrder = 1200)]
        public string WebhookSecret { get; set; }

        [PaymentProviderSetting(Label = "Locale", Description = "The locale of the payment portal to display. Defaults to configuration locale or account locale.", SortOrder = 1300)]
        public string Locale { get; set; }

        [PaymentProviderSetting(Label = "Accepted Payment Methods", Description = "A comma separated list of Payment Methods to accept.", SortOrder = 1400)]
        public string PaymentMethods { get; set; }

        [PaymentProviderSetting(Label = "Test Mode", SortOrder = 10000)]
        public bool TestMode { get; set; }
    }
}
