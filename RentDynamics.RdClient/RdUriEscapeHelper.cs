namespace RentDynamics.RdClient
{
    public static class RdUriEscapeHelper
    {
        public static string UnescapeSpecialRdApiCharacters(string url)
        {
            //Pipe '|' character is treated specially in RD api. Details: https://github.com/Skylude/django-rest-framework-signature/blob/master/rest_framework_signature/authentication.py#L132
            return url.Replace("%7C", "|");
        }
    }
}