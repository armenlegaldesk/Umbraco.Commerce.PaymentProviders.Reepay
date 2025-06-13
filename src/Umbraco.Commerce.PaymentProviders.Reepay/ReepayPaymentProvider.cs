using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Umbraco.Commerce.Common.Logging;
using Umbraco.Commerce.Core.Api;
using Umbraco.Commerce.Core.Models;
using Umbraco.Commerce.Core.PaymentProviders;
using Umbraco.Commerce.Extensions;
using Umbraco.Commerce.PaymentProviders.Reepay.Api;
using Umbraco.Commerce.PaymentProviders.Reepay.Api.Enums;
using Umbraco.Commerce.PaymentProviders.Reepay.Api.Models;

namespace Umbraco.Commerce.PaymentProviders.Reepay
{
    [PaymentProvider("reepay-checkout", "Reepay Checkout", "Reepay payment provider")]
    public class ReepayCheckoutPaymentProvider : ReepayPaymentProviderBase<ReepayCheckoutPaymentProvider, ReepayCheckoutSettings>

    {
        public virtual bool CanCancelPayments => true;

        public virtual bool CanCapturePayments => true;

        public virtual bool CanRefundPayments => true;

        public virtual bool CanFetchPaymentStatus => true;

        public virtual bool FinalizeAtContinueUrl => false;

        protected readonly ILogger<ReepayCheckoutPaymentProvider> _logger;

        public ReepayCheckoutPaymentProvider(UmbracoCommerceContext umbracoCommerce, ILogger<ReepayCheckoutPaymentProvider> logger)
            : base(umbracoCommerce, logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Will redirect the customer to the given payment gateway payment form.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async Task<PaymentFormResult> GenerateFormAsync(PaymentProviderContext<ReepayCheckoutSettings> ctx, CancellationToken cancellationToken = default)
        {
            Debug.WriteLine($"{nameof(GenerateFormAsync)} hits. " +
                $"ContinueUrl: {ctx.Urls.ContinueUrl}" +
                $"CancelUrl: {ctx.Urls.CancelUrl}" +
                $"ErrorUrl: {ctx.Urls.ErrorUrl}"
                );

            var currency = await Context.Services.CurrencyService.GetCurrencyAsync(ctx.Order.CurrencyId);

            var currencyCode = currency.Code.ToUpperInvariant();

            // Ensure currency has valid ISO 4217 code
            if (!Iso4217.CurrencyCodes.ContainsKey(currencyCode))
            {
                throw new Exception("Currency must be a valid ISO 4217 currency code: " + currency.Name);
            }

            var billingCountry = ctx.Order.PaymentInfo.CountryId.HasValue
                ? await Context.Services.CountryService.GetCountryAsync(ctx.Order.PaymentInfo.CountryId.Value)
                : null;

            var orderAmount = AmountToMinorUnits(ctx.Order.TransactionAmount.Value);

            var paymentMethods = ctx.Settings.PaymentMethods?.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                   .Where(x => !string.IsNullOrWhiteSpace(x))
                   .Select(s => s.Trim())
                   .ToArray();

            var customerHandle = !string.IsNullOrEmpty(ctx.Order.CustomerInfo.CustomerReference)
                                        ? ctx.Order.CustomerInfo.CustomerReference
                                        : Guid.NewGuid().ToString();

            string paymentFormLink = string.Empty;

            var sessionId = ctx.Order.Properties["reepaySessionId"]?.Value;

            // https://docs.reepay.com/docs/new-web-shop

            try
            {
                var metaData = new Dictionary<string, string>()
                {
                    { "orderReference", ctx.Order.GenerateOrderReference() },
                    { "orderId", ctx.Order.Id.ToString("D") },
                    { "orderNumber", ctx.Order.OrderNumber }
                };

                var checkoutSessionRequest = new ReepayChargeSessionRequest
                {
                    Order = new ReepayOrder
                    {
                        Key = ctx.Order.GenerateOrderReference(),
                        Handle = ctx.Order.OrderNumber,
                        Amount = (int)orderAmount,
                        Currency = currencyCode,
                        Customer = new ReepayCustomer
                        {
                            Email = ctx.Order.CustomerInfo.Email,
                            Handle = customerHandle,
                            FirstName = ctx.Order.CustomerInfo.FirstName,
                            LastName = ctx.Order.CustomerInfo.LastName,
                            Company = !string.IsNullOrWhiteSpace(ctx.Settings.BillingCompanyPropertyAlias)
                                ? ctx.Order.Properties[ctx.Settings.BillingCompanyPropertyAlias] : null,
                            Address = !string.IsNullOrWhiteSpace(ctx.Settings.BillingAddressLine1PropertyAlias)
                                ? ctx.Order.Properties[ctx.Settings.BillingAddressLine1PropertyAlias] : null,
                            Address2 = !string.IsNullOrWhiteSpace(ctx.Settings.BillingAddressLine2PropertyAlias)
                                ? ctx.Order.Properties[ctx.Settings.BillingAddressLine2PropertyAlias] : null,
                            PostalCode = !string.IsNullOrWhiteSpace(ctx.Settings.BillingAddressZipCodePropertyAlias)
                                ? ctx.Order.Properties[ctx.Settings.BillingAddressZipCodePropertyAlias] : null,
                            City = !string.IsNullOrWhiteSpace(ctx.Settings.BillingAddressCityPropertyAlias)
                                ? ctx.Order.Properties[ctx.Settings.BillingAddressCityPropertyAlias] : null,
                            Phone = !string.IsNullOrWhiteSpace(ctx.Settings.BillingPhonePropertyAlias)
                                ? ctx.Order.Properties[ctx.Settings.BillingPhonePropertyAlias] : null,
                            Country = billingCountry?.Code,
                            GenerateHandle = string.IsNullOrEmpty(customerHandle)
                        },
                        BillingAddress = new ReepayAddress
                        {
                            FirstName = ctx.Order.CustomerInfo.FirstName,
                            LastName = ctx.Order.CustomerInfo.LastName,
                            Company = !string.IsNullOrWhiteSpace(ctx.Settings.BillingCompanyPropertyAlias)
                                ? ctx.Order.Properties[ctx.Settings.BillingCompanyPropertyAlias] : null,
                            Address = !string.IsNullOrWhiteSpace(ctx.Settings.BillingAddressLine1PropertyAlias)
                                ? ctx.Order.Properties[ctx.Settings.BillingAddressLine1PropertyAlias] : null,
                            Address2 = !string.IsNullOrWhiteSpace(ctx.Settings.BillingAddressLine2PropertyAlias)
                                ? ctx.Order.Properties[ctx.Settings.BillingAddressLine2PropertyAlias] : null,
                            PostalCode = !string.IsNullOrWhiteSpace(ctx.Settings.BillingAddressZipCodePropertyAlias)
                                ? ctx.Order.Properties[ctx.Settings.BillingAddressZipCodePropertyAlias] : null,
                            City = !string.IsNullOrWhiteSpace(ctx.Settings.BillingAddressCityPropertyAlias)
                                ? ctx.Order.Properties[ctx.Settings.BillingAddressCityPropertyAlias] : null,
                            Phone = !string.IsNullOrWhiteSpace(ctx.Settings.BillingPhonePropertyAlias)
                                ? ctx.Order.Properties[ctx.Settings.BillingPhonePropertyAlias] : null,
                            Country = billingCountry?.Code
                        },
                        MetaData = metaData
                    },
                    Settle = ctx.Settings.Capture,
                    AcceptUrl = ctx.Urls.ContinueUrl,
                    CancelUrl = ctx.Urls.CancelUrl
                };

                if (!string.IsNullOrWhiteSpace(ctx.Settings.Locale))
                {
                    checkoutSessionRequest.Locale = ctx.Settings.Locale;
                }

                if (paymentMethods?.Length > 0)
                {
                    // Set payment methods if any exists otherwise omit.
                    checkoutSessionRequest.PaymentMethods = paymentMethods;
                }

                var clientConfig = GetReepayClientConfig(ctx.Settings);
                var client = new ReepayClient(clientConfig);

                // Create checkout session
                var checkoutSession = await client.CreateChargeSessionAsync(checkoutSessionRequest);
                if (checkoutSession != null)
                {
                    // Get session id
                    sessionId = checkoutSession.Id;

                    // Get session url
                    paymentFormLink = checkoutSession.Url;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Reepay - error creating payment.");
            }

            return new PaymentFormResult()
            {
                MetaData = new Dictionary<string, string>
                {
                    { "reepaySessionId", sessionId },
                    { "reepayCustomerHandle", customerHandle }
                },
                Form = new PaymentForm(paymentFormLink, PaymentFormMethod.Get)
                            .WithJsFile("https://checkout.reepay.com/checkout.js")
                            .WithJs(@"var rp = new Reepay.WindowCheckout('" + sessionId + "');")
            };
        }

        public override async Task<CallbackResult> ProcessCallbackAsync(PaymentProviderContext<ReepayCheckoutSettings> ctx, CancellationToken cancellationToken = default)
        {
            try
            {
                // Process callback

                var reepayEvent = await GetReepayWebhookEventAsync(ctx);

                if (reepayEvent != null && (reepayEvent.EventType == WebhookEventType.InvoiceAuthorized || reepayEvent.EventType == WebhookEventType.InvoiceSettled))
                {
                    return CallbackResult.Ok(new TransactionInfo
                    {
                        TransactionId = reepayEvent.Transaction,
                        AmountAuthorized = ctx.Order.TransactionAmount.Value,
                        PaymentStatus = reepayEvent.EventType == WebhookEventType.InvoiceSettled ? PaymentStatus.Captured : PaymentStatus.Authorized
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Reepay - ProcessCallback");
            }

            return CallbackResult.BadRequest();
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

            if (ctx.AdditionalData.ContainsKey("Vendr_ReepayEvent"))
            {
                reepayEvent = (ReepayWebhookEvent)ctx.AdditionalData["Vendr_ReepayEvent"];
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

                                            ctx.AdditionalData.Add("Vendr_ReepayEvent", reepayEvent);
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

        public override string GetContinueUrl(PaymentProviderContext context)
        {
            return base.GetContinueUrl(context);
        }

        public override string GetCancelUrl(PaymentProviderContext context)
        {
            return base.GetCancelUrl(context);
        }

        public override string GetErrorUrl(PaymentProviderContext context)
        {
            return base.GetErrorUrl(context);
        }
    }
}

