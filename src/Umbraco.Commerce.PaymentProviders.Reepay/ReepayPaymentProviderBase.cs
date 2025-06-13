using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Text;
using Umbraco.Commerce.Common.Logging;
using Umbraco.Commerce.Core.Api;
using Umbraco.Commerce.Core.Models;
using Umbraco.Commerce.Core.PaymentProviders;
using Umbraco.Commerce.Extensions;
using Umbraco.Commerce.PaymentProviders.Reepay.Api;
using Umbraco.Commerce.PaymentProviders.Reepay.Api.Models;

namespace Umbraco.Commerce.PaymentProviders.Reepay
{
    public abstract class ReepayPaymentProviderBase<TSelf, TSettings> : PaymentProviderBase<TSettings>
          where TSelf : ReepayPaymentProviderBase<TSelf, TSettings>
          where TSettings : ReepaySettingsBase, new()
    {
        protected readonly ILogger<TSelf> _logger;

        public ReepayPaymentProviderBase(UmbracoCommerceContext umbracoCommerce, ILogger<TSelf> logger)
            : base(umbracoCommerce)
        {
            _logger = logger;
        }

        public override string GetContinueUrl(PaymentProviderContext<TSettings> ctx)
        {
            ctx.Settings.MustNotBeNull("ctx.Settings");
            ctx.Settings.ContinueUrl.MustNotBeNull("ctx.Settings.ContinueUrl");

            return ctx.Settings.ContinueUrl;
        }

        public override string GetCancelUrl(PaymentProviderContext<TSettings> ctx)
        {
            ctx.Settings.MustNotBeNull("ctx.Settings");
            ctx.Settings.CancelUrl.MustNotBeNull("ctx.Settings.CancelUrl");

            return ctx.Settings.CancelUrl;
        }

        public override string GetErrorUrl(PaymentProviderContext<TSettings> ctx)
        {
            ctx.Settings.MustNotBeNull("ctx.Settings");
            ctx.Settings.ErrorUrl.MustNotBeNull("ctx.Settings.ErrorUrl");

            return ctx.Settings.ErrorUrl;
        }


        public virtual IEnumerable<TransactionMetaDataDefinition> TransactionMetaDataDefinitions => new[] {
                    new TransactionMetaDataDefinition("reepaySessionId", "Reepay Session ID", (string) null),
                    new TransactionMetaDataDefinition("reepayCustomerHandle", "Reepay Customer Handle", (string) null)
        };

        protected ReepayClientConfig GetReepayClientConfig(ReepaySettingsBase settings)
        {
            var basicAuth = Base64Encode(settings.PrivateKey + ":");

            return new ReepayClientConfig
            {
                BaseUrl = "https://api.reepay.com",
                PrivateKey = settings.PrivateKey,
                Authorization = "Basic " + basicAuth,
                WebhookSecret = settings.WebhookSecret
            };
        }

        protected PaymentStatus GetPaymentStatus(ReepayCharge charge)
        {
            // Possible Charge statuses:
            // - authorized
            // - settled
            // - failed
            // - cancelled
            // - pending

            if (charge.State == "authorized")
                return PaymentStatus.Authorized;

            if (charge.State == "settled")
            {
                if (charge.RefundedAmount > 0)
                    return PaymentStatus.Refunded;

                return PaymentStatus.Captured;
            }

            if (charge.State == "failed")
                return PaymentStatus.Error;

            if (charge.State == "cancelled")
                return PaymentStatus.Cancelled;

            if (charge.State == "pending")
                return PaymentStatus.PendingExternalSystem;

            return PaymentStatus.Initialized;
        }

        protected PaymentStatus GetPaymentStatus(ReepayRefund refund)
        {
            // Possible Refund statuses:
            // - refunded
            // - failed
            // - processing

            if (refund.State == "refunded")
                return PaymentStatus.Refunded;

            if (refund.State == "failed")
                return PaymentStatus.Error;

            if (refund.State == "processing")
                return PaymentStatus.PendingExternalSystem;

            return PaymentStatus.Authorized;
        }

        protected async Task<ReepayWebhookEvent> GetReepayWebhookEventAsync(PaymentProviderContext<ReepayCheckoutSettings> ctx)
        {
            ReepayWebhookEvent reepayEvent = null;

            if (ctx.AdditionalData.ContainsKey("UmbracoCommerce_ReepayEvent"))
            {
                reepayEvent = (ReepayWebhookEvent)ctx.AdditionalData["UmbracoCommerce_ReepayEvent"];
            }
            else
            {
                try
                {
                    using (Stream stream = ctx.HttpContext.Request.Body)
                    {
                        if (stream.CanSeek)
                            stream.Seek(0, SeekOrigin.Begin);

                        using (var sr = new StreamReader(stream))
                        using (var jr = new JsonTextReader(sr) { DateParseHandling = DateParseHandling.None })
                        {
                            while (jr.Read())
                            {
                                JObject obj = (JObject)JToken.ReadFrom(jr);

                                if (obj != null)
                                {
                                    if (obj.TryGetValue("signature", out JToken signature) &&
                                        obj.TryGetValue("timestamp", out JToken timestamp) &&
                                        obj.TryGetValue("id", out JToken id))
                                    {
                                        // Validate the webhook signature: https://reference.reepay.com/api/#webhooks
                                        var calcSignature = CalculateSignature(ctx.Settings.WebhookSecret, timestamp.Value<string>(), id.Value<string>());

                                        if (signature.Value<string>() == calcSignature)
                                        {
                                            var json = obj.ToString(Formatting.None);

                                            reepayEvent = JsonConvert.DeserializeObject<ReepayWebhookEvent>(json);

                                            ctx.AdditionalData.Add("UmbracoCommerce_ReepayEvent", reepayEvent);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Reepay - GetReepayWebhookEvent");
                }
            }

            return reepayEvent;
        }


        private string CalculateSignature(string webhookSecret, string timestamp, string id)
        {
            // signature = hexencode(hmac_sha_256(webhook_secret, timestamp + id))

            var signature = ComputeSignature(webhookSecret, timestamp, id);

            return signature;
        }

        private string ComputeSignature(string secret, string timestamp, string id)
        {
            using (var cryptographer = new HMACSHA256(Encoding.UTF8.GetBytes(secret)))
            {
                byte[] buffer = Encoding.UTF8.GetBytes(timestamp + id);
                var hash = cryptographer.ComputeHash(buffer);
                return HexEncode(hash).ToLowerInvariant();
            }
        }

        private string HexEncode(byte[] data)
        {
            return BitConverter.ToString(data).Replace("-", string.Empty);
        }

    }
}

