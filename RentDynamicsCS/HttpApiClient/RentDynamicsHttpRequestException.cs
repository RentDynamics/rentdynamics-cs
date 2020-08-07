using System;
using System.Net.Http;
using RentDynamicsCS.Models;

namespace RentDynamicsCS.HttpApiClient
{
    public class RentDynamicsHttpRequestException : HttpRequestException
    {
        public string? RawResponseBody { get; }
        public ApiError? ApiError { get; }


        public RentDynamicsHttpRequestException(string baseMessage, HttpResponseMessage httpResponseMessage, string? rawResponseBody, ApiError? apiError)
            : base(FormatMessage(baseMessage, httpResponseMessage, apiError))
        {
            RawResponseBody = rawResponseBody;
            ApiError = apiError;
        }

        private static string FormatMessage(string baseMessage, HttpResponseMessage httpResponseMessage, ApiError? apiError)
        {
            baseMessage += $"{httpResponseMessage.ReasonPhrase} ({(int) httpResponseMessage.StatusCode}-{httpResponseMessage.StatusCode})";
            if (apiError == null) return baseMessage;

            string apiErrorMessage = string.Join(Environment.NewLine, apiError.GetErrors());

            if (apiErrorMessage == string.Empty) return baseMessage;

            return $"{baseMessage} - {apiErrorMessage}";
        }
    }
}