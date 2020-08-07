using System.Linq;
using Newtonsoft.Json.Linq;

namespace RentDynamicsCS
{
    public static class JsonSortHelper
    {
        public static void Sort(JObject jObject)
        {
            var properties = jObject.Properties().ToArray();

            jObject.RemoveAll();

            foreach (JProperty jProperty in properties.OrderBy(a => a.Name))
            {
                jObject.Add(jProperty);
                if (jProperty.Value is JObject propertyObject)
                {
                    Sort(propertyObject);
                }
                //TODO: sorting with arrays
            }
        }
    }
}