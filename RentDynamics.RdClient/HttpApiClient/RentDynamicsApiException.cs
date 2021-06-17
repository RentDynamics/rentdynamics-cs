using System;
using System.Net.Http;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.HttpApiClient
{
    [PublicAPI]
    public class RentDynamicsApiException : Exception
    {
        public string? RawResponseBody { get; }
        public ApiError? ApiError { get; }
        public int StatusCode { get; }


        public RentDynamicsApiException(string baseMessage, HttpResponseMessage httpResponseMessage, string? rawResponseBody, ApiError? apiError)
            : base(FormatMessage(baseMessage, httpResponseMessage, apiError))
        {
            RawResponseBody = rawResponseBody;
            ApiError = apiError;
            StatusCode = (int) httpResponseMessage.StatusCode;
        }

        private static string FormatMessage(string baseMessage, HttpResponseMessage httpResponseMessage, ApiError? apiError)
        {
            baseMessage += $"{httpResponseMessage.ReasonPhrase} ({(int) httpResponseMessage.StatusCode}-{httpResponseMessage.StatusCode})";
            if (apiError == null) return baseMessage;

            string apiErrorMessage = string.Join(Environment.NewLine, apiError);

            if (apiErrorMessage == string.Empty) return baseMessage;

            return $"{baseMessage} - {apiErrorMessage}";
        }
    }
}