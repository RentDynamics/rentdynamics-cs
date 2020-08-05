using System.Net.Http;
using RentDynamicsCS.Models;

namespace RentDynamicsCS
{
    public class CustomHttpRequestException : HttpRequestException
    {
        public string? RawResponseBody { get; }
        public ApiError? ApiError { get; }


        public CustomHttpRequestException(string message, string? rawResponseBody, ApiError? apiError)
            : base(FormatMessage(message, apiError))
        {
            RawResponseBody = rawResponseBody;
            ApiError = apiError;
        }

        private static string FormatMessage(string baseMessage, ApiError? apiError)
        {
            if (apiError == null) return baseMessage;

            return $"{baseMessage} - {apiError.ErrorMessage}";
        }
    }
}