namespace RentDynamics.RdClient
{
    public static class RdUriEscapeHelper
    {
        public static string UnescapeSpecialRdApiCharacters(string url)
        {
            //Nonce calculator in python RD-api decodes only %7C (pipe - '|') and %20 (whitespace - ' '), everything else remains URL encoded. 
            //Details: https://github.com/Skylude/django-rest-framework-signature/blob/master/rest_framework_signature/authentication.py#L132
            return url.Replace("%7C", "|")
                      .Replace("%20", " ");
        }
    }
}