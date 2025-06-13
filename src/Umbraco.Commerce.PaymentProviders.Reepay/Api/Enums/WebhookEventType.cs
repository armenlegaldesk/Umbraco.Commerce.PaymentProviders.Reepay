using System.Runtime.Serialization;

namespace Umbraco.Commerce.PaymentProviders.Reepay.Api.Enums
{
    public enum WebhookEventType
    {
        [EnumMember(Value = "invoice_created")] InvoiceCreated,
        [EnumMember(Value = "invoice_settled")] InvoiceSettled,
        [EnumMember(Value = "invoice_authorized")] InvoiceAuthorized,
        [EnumMember(Value = "invoice_dunning")] InvoiceDunning,
        [EnumMember(Value = "invoice_dunning_notification")] InvoiceDunningNotification,
        [EnumMember(Value = "invoice_dunning_cancelled")] InvoiceDunningCancelled,
        [EnumMember(Value = "invoice_failed")] InvoiceFailed,
        [EnumMember(Value = "invoice_refund")] InvoiceRefund,
        [EnumMember(Value = "invoice_refund_failed")] InvoiceRefundFailed,
        [EnumMember(Value = "invoice_reactivate")] InvoiceReactivate,
        [EnumMember(Value = "invoice_cancelled")] InvoiceCancelled,
        [EnumMember(Value = "invoice_changed")] InvoiceChanged,
        [EnumMember(Value = "invoice_credited")] InvoiceCredited,
        [EnumMember(Value = "subscription_created")] SubscriptionCreated,
        [EnumMember(Value = "subscription_payment_method_added")] SubscriptionPaymentMethodAdded,
        [EnumMember(Value = "subscription_payment_method_changed")] SubscriptionPaymentMethodChanged,
        [EnumMember(Value = "subscription_trial_end")] SubscriptionTrialEnd,
        [EnumMember(Value = "subscription_renewal")] SubscriptionRenewal,
        [EnumMember(Value = "subscription_cancelled")] SubscriptionCancelled,
        [EnumMember(Value = "subscription_uncancelled")] SubscriptionUncancelled,
        [EnumMember(Value = "subscription_on_hold")] SubscriptionOnHold,
        [EnumMember(Value = "subscription_on_hold_dunning")] SubscriptionOnHoldDunning,
        [EnumMember(Value = "subscription_reactivated")] SubscriptionReactivated,
        [EnumMember(Value = "subscription_expired")] SubscriptionExpired,
        [EnumMember(Value = "subscription_expired_dunning")] SubscriptionExpiredDunning,
        [EnumMember(Value = "subscription_changed")] SubscriptionChanged,
        [EnumMember(Value = "customer_created")] CustomerCreated,
        [EnumMember(Value = "customer_payment_method_added")] CustomerPaymentMethodAdded,
        [EnumMember(Value = "customer_changed")] CustomerChanged,
        [EnumMember(Value = "customer_deleted")] CustomerDeleted,
    }

}
