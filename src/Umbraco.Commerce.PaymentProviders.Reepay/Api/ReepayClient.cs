using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Umbraco.Commerce.Common.Logging;
using Umbraco.Commerce.PaymentProviders.Reepay.Api.Exceptions;
using Umbraco.Commerce.PaymentProviders.Reepay.Api.Models;

namespace Umbraco.Commerce.PaymentProviders.Reepay.Api
{
    public class ReepayClient
    {
        private const string SessionApiUrl = "https://checkout-api.reepay.com/v1/session/";
        private const string BaseApiUrl = "https://api.reepay.com/v1/";
        private const string ChargeUrl = SessionApiUrl + "charge";
        private const string RecurringUrl = SessionApiUrl + "recurring";

        private readonly HttpClient _client;
        private readonly JsonSerializerSettings _jsonSerializerSettings;
        private readonly ILogger<ReepayClient> _logger;

        public ReepayClient(ReepayClientConfig config, ILogger<ReepayClient> logger)
        {
            _logger = logger;

            _client = new HttpClient();
            _client.Timeout = new TimeSpan(0, 5, 0);
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore
            };

            var credentialBase64 = Convert.ToBase64String(System.Text.Encoding.GetEncoding("UTF-8").GetBytes(config.PrivateKey + ":"));
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentialBase64);
        }


        public async Task<ReepaySessionResponse> CreateChargeSessionAsync(ReepayChargeSessionRequest data, CancellationToken cancellationToken = default)
        {
            HttpContent body = new StringContent(JsonConvert.SerializeObject(data));
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await _client.PostAsync(ChargeUrl, body, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var resErr = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken).ConfigureAwait(false);
                _logger.Error($"Unable to create session, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {ChargeUrl}, input data: {JsonConvert.SerializeObject(data)}");
                throw new ReepayException(resErr);
            }

            return await response.Content.ReadFromJsonAsync<ReepaySessionResponse>(cancellationToken).ConfigureAwait(false);
        }

        public async Task<ReepaySessionResponse> CreateRecurringSessionAsync(
          ReepayChargeSessionRequest data, CancellationToken cancellationToken = default)
        {
            HttpContent body = new StringContent(JsonConvert.SerializeObject(data));
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await _client.PostAsync(RecurringUrl, body, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var resErr = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken).ConfigureAwait(false);
                _logger.Error($"Unable to recurring create session, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {RecurringUrl}, input data: {JsonConvert.SerializeObject(data)}");
                throw new ReepayException(resErr);
            }

            return await response.Content.ReadFromJsonAsync<ReepaySessionResponse>(cancellationToken).ConfigureAwait(false);
        }

        public async Task<ReepayCharge> GetChargeAsync(string handle, CancellationToken cancellationToken = default)
        {
            var url = $"{ChargeUrl}/{handle}";
            HttpResponseMessage response = await _client.GetAsync(url).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var resErr = await response.Content.ReadFromJsonAsync<ErrorResponse>().ConfigureAwait(false);
                _logger.Error($"Unable to get charge object, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}");
                throw new ReepayException(resErr);
            }

            return await response.Content.ReadFromJsonAsync<ReepayCharge>().ConfigureAwait(false);
        }

        public async Task<ReepayCharge> CancelChargeAsync(string handle, CancellationToken cancellationToken = default)
        {
            var url = $"{ChargeUrl}/{handle}/cancel";
            HttpContent body = new StringContent("");
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await _client.PostAsync(url, body).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var resErr = await response.Content.ReadFromJsonAsync<ErrorResponse>().ConfigureAwait(false);
                _logger.Error($"Unable to cancel charge, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}");
                throw new ReepayException(resErr);
            }

            return await response.Content.ReadFromJsonAsync<ReepayCharge>().ConfigureAwait(false);
        }

        public async Task<ReepayCharge> SettleChargeAsync(string handle, ReepaySettleChargeRequest data, CancellationToken cancellationToken = default)
        {
            var url = $"{ChargeUrl}/{handle}/settle";
            var stringData = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
            HttpContent body = new StringContent(stringData);
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await _client.PostAsync(url, body).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var resErr = await response.Content.ReadFromJsonAsync<ErrorResponse>().ConfigureAwait(false);
                _logger.Error($"Unable to settle charge, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}, input data: {stringData}");
                throw new ReepayException(resErr);
            }

            return await response.Content.ReadFromJsonAsync<ReepayCharge>().ConfigureAwait(false);

        }

        public async Task<ReepayRefund> RefundChargeAsync(CreateRefundRequest data, CancellationToken cancellationToken = default)
        {
            var url = $"{BaseApiUrl}refund";
            var stringData = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
            HttpContent body = new StringContent(stringData);
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await _client.PostAsync(url, body).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var resErr = await response.Content.ReadFromJsonAsync<ErrorResponse>().ConfigureAwait(false);
                _logger.Error($"Unable to create refund, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}, input data: {stringData}");
                throw new ReepayException(resErr);
            }

            return await response.Content.ReadFromJsonAsync<ReepayRefund>().ConfigureAwait(false);
        }

        public async Task<ReepaySubscription> CreateSubscriptionAsync(object data, CancellationToken cancellationToken = default)
        {
            var url = $"{BaseApiUrl}subscription";
            var stringData = JsonConvert.SerializeObject(data, _jsonSerializerSettings);
            HttpContent body = new StringContent(stringData);
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await _client.PostAsync(url, body).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var resErr = await response.Content.ReadFromJsonAsync<ErrorResponse>().ConfigureAwait(false);
                _logger.Error($"Unable to create refund, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}, input data: {stringData}");
                throw new ReepayException(resErr);
            }

            return await response.Content.ReadFromJsonAsync<ReepaySubscription>().ConfigureAwait(false);
        }

        public async Task<ReepaySubscription> GetSubscriptionAsync(string handle, CancellationToken cancellationToken = default)
        {
            var url = $"{BaseApiUrl}subscription/{handle}";
            HttpResponseMessage response = await _client.GetAsync(url).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var resErr = await response.Content.ReadFromJsonAsync<ErrorResponse>().ConfigureAwait(false);
                _logger.Error($"Unable to get subscription, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}, input data: {handle}");
                throw new ReepayException(resErr);
            }

            return await response.Content.ReadFromJsonAsync<ReepaySubscription>().ConfigureAwait(false);
        }

        public async Task<ReepaySubscription> CancelSubscriptionAsync(string handle, CancellationToken cancellationToken = default)
        {
            var url = $"{BaseApiUrl}subscription/{handle}/cancel";

            var stringData = JsonConvert.SerializeObject(new { }, _jsonSerializerSettings);
            HttpContent body = new StringContent(stringData);
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await _client.PostAsync(url, body, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var resErr = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken).ConfigureAwait(false);
                _logger.Error($"Unable to cancel subscription, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}, handle: {handle}");
                throw new ReepayException(resErr);
            }

            return await response.Content.ReadFromJsonAsync<ReepaySubscription>(cancellationToken).ConfigureAwait(false);
        }

        public async Task<ReepaySubscription> UncancelSubscription(string handle, CancellationToken cancellationToken = default)
        {
            var url = $"{BaseApiUrl}subscription/{handle}/uncancel";

            var stringData = JsonConvert.SerializeObject(new { }, _jsonSerializerSettings);
            HttpContent body = new StringContent(stringData);
            body.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            HttpResponseMessage response = await _client.PostAsync(url, body, cancellationToken).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                var resErr = await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken).ConfigureAwait(false);
                _logger.Error($"Unable to uncancel subscription, error: {resErr.Code} {resErr.Message} {resErr.Error} {resErr.Transaction_error} {resErr.Http_status} {resErr.Http_reason} {resErr.Path} {resErr.Request_id} {resErr.TimeStamp}, url: {url}, handle: {handle}");
                throw new ReepayException(resErr);
            }

            return await response.Content.ReadFromJsonAsync<ReepaySubscription>(cancellationToken).ConfigureAwait(false);
        }
    }
}
