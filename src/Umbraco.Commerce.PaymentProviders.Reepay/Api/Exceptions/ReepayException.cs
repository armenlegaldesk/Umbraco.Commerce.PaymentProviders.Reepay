using Umbraco.Commerce.PaymentProviders.Reepay.Api.Models;

namespace Umbraco.Commerce.PaymentProviders.Reepay.Api.Exceptions
{
    public class ReepayException : Exception
    {
        public ErrorResponse ReepayError { get; set; }

        public ReepayException() : base()
        {
        }

        public ReepayException(ErrorResponse errorResponse) : base()
        {
            ReepayError = errorResponse;
        }

        public ReepayException(string message, ErrorResponse errorResponse) : base(message)
        {
            ReepayError = errorResponse;
        }
    }
}
