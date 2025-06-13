using Flurl.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Umbraco.Commerce.PaymentProviders.Reepay.Api.Models;

namespace Umbraco.Commerce.PaymentProviders.Reepay.Api
{
    public class ReepayClient
    {
        private const string ApiSessionUrl = "https://checkout-api.reepay.com/v1/session/";
        private const string BaseApiUrl = "https://api.reepay.com/v1/";

        private readonly ReepayClientConfig _config;
        public ReepayClient(ReepayClientConfig config)
        {
            _config = config;
        }


        public async Task<ReepaySessionResponse> CreateChargeSessionAsync(ReepayChargeSessionRequest data, CancellationToken cancellationToken = default)
        {
            return await $"{ApiSessionUrl}v1/session/charge"
                .WithSettings(x => x.JsonSerializer = new CustomFlurlJsonSerializer(new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
                }))
                .WithHeader("Cache-Control", "no-cache")
                .WithHeader("Content-Type", "application/json")
                .WithHeader("Authorization", _config.PrivateKey)
                .PostAsync(JsonContent.Create(data), cancellationToken: cancellationToken)
                .ReceiveJson<ReepaySessionResponse>().ConfigureAwait(false);
        }

        public async Task<ReepaySessionResponse> CreateRecurringSessionAsync(
          ReepayChargeSessionRequest data, CancellationToken cancellationToken = default)
        {
            return await $"{ApiSessionUrl}/v1/session/recurring"
                    .WithSettings(x => x.JsonSerializer = new CustomFlurlJsonSerializer(new JsonSerializerOptions
                    {
                        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                        PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
                    }))
                    .WithHeader("Cache-Control", "no-cache")
                    .WithHeader("Content-Type", "application/json")
                    .WithHeader("Authorization", _config.PrivateKey)
                    .PostAsync(JsonContent.Create(data), cancellationToken: cancellationToken)
                    .ReceiveJson<ReepaySessionResponse>().ConfigureAwait(false);
        }

        public async Task<ReepayCharge> GetChargeAsync(string handle, CancellationToken cancellationToken = default)
        {
            return await RequestAsync("/v1/charge/" + handle, async (req, ct) => await req
                    .GetAsync(cancellationToken: ct)
                    .ReceiveJson<ReepayCharge>().ConfigureAwait(false),
                    cancellationToken).ConfigureAwait(false);
        }

        public async Task<ReepayCharge> CancelChargeAsync(string handle, CancellationToken cancellationToken = default)
        {
            return await RequestAsync($"/v1/charge/{handle}/cancel", async (req, ct) => await req
                    .PostJsonAsync(null, cancellationToken: ct)
                    .ReceiveJson<ReepayCharge>().ConfigureAwait(false),
                    cancellationToken).ConfigureAwait(false);
        }

        public async Task<ReepayCharge> SettleChargeAsync(string handle, ReepaySettleChargeRequest data, CancellationToken cancellationToken = default)
        {
            return await RequestAsync($"/v1/charge/{handle}/settle", async (req, ct) => await req
                    .PostJsonAsync(data, cancellationToken: ct)
                    .ReceiveJson<ReepayCharge>().ConfigureAwait(false),
                    cancellationToken).ConfigureAwait(false);
        }

        public async Task<ReepayRefund> RefundChargeAsync(CreateRefundRequest data, CancellationToken cancellationToken = default)
        {
            return await RequestAsync("/v1/refund", async (req, ct) => await req
                    .PostJsonAsync(data, cancellationToken: ct)
                    .ReceiveJson<ReepayRefund>().ConfigureAwait(false),
                    cancellationToken).ConfigureAwait(false);
        }

        public async Task<ReepaySubscription> CreateSubscriptionAsync(object data, CancellationToken cancellationToken = default)
        {
            return await RequestAsync("/v1/subscription", async (req, ct) => await req
                    .PostJsonAsync(data, cancellationToken: ct)
                    .ReceiveJson<ReepaySubscription>().ConfigureAwait(false),
                     cancellationToken).ConfigureAwait(false);
        }

        public async Task<ReepaySubscription> GetSubscriptionAsync(string handle, CancellationToken cancellationToken = default)
        {
            return await RequestAsync("/v1/subscription/" + handle, async (req, ct) => await req
                    .GetAsync(cancellationToken: ct)
                    .ReceiveJson<ReepaySubscription>().ConfigureAwait(false),
                    cancellationToken).ConfigureAwait(false);

        }

        public async Task<ReepaySubscription> CancelSubscriptionAsync(string handle, object data, CancellationToken cancellationToken = default)
        {
            return await RequestAsync($"/v1/subscription/{handle}/cancel", async (req, ct) => await req
                    .PostJsonAsync(data, cancellationToken: ct)
                    .ReceiveJson<ReepaySubscription>().ConfigureAwait(false),
                    cancellationToken).ConfigureAwait(false);
        }

        public async Task<ReepaySubscription> UncancelSubscription(string handle, CancellationToken cancellationToken = default)
        {
            return await RequestAsync($"/v1/subscription/{handle}/uncancel", async (req, ct) => await req
                    .PostJsonAsync(null, cancellationToken: ct)
                    .ReceiveJson<ReepaySubscription>().ConfigureAwait(false),
                    cancellationToken).ConfigureAwait(false);
        }

        public async Task<Dictionary<string, object>> GetInvoiceMetaData(string handle, CancellationToken cancellationToken = default)
        {
            return await RequestAsync($"/v1/invoice/{handle}/metadata", async (req, ct) => await req
                    .GetAsync(cancellationToken: ct)
                    .ReceiveJson<Dictionary<string, object>>().ConfigureAwait(false),
                    cancellationToken).ConfigureAwait(false);
        }

        private async Task<TResult> RequestAsync<TResult>(string url, Func<IFlurlRequest, CancellationToken, Task<TResult>> func, CancellationToken cancellationToken = default)
        {
            FlurlRequest req = new FlurlRequest(BaseApiUrl + url)
                .WithSettings(x => x.JsonSerializer = new CustomFlurlJsonSerializer(new JsonSerializerOptions
                {
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                    PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace,
                }))
                .WithHeader("Cache-Control", "no-cache")
                .WithHeader("Content-Type", "application/json")
                .WithHeader("Authorization", _config.PrivateKey);

            return await func.Invoke(req, cancellationToken).ConfigureAwait(false);
        }
    }
}
