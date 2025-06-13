using Umbraco.Commerce.Core.PaymentProviders;

namespace Umbraco.Commerce.PaymentProviders.Reepay
{
    public class ReepayCheckoutSettings : ReepaySettingsBase
    {
        [PaymentProviderSetting(Label = "Auto Capture", Description = "Flag indicating whether to immediately capture the payment, or whether to just authorize the payment for later (manual) capture.", SortOrder = 1500)]
        public bool Capture { get; set; }

        [PaymentProviderSetting(Label = "Button Text", Description = "Optional alternative button text. Maximum length 32 characters.", SortOrder = 1600)]
        public string ButtonText { get; set; }
    }
}
