using System.Linq;
using Newtonsoft.Json.Linq;

namespace RentDynamics.RdClient
{
    public static class JsonSortHelper
    {
        private static void SortJArray(JArray jArray)
        {
            foreach (JToken jToken in jArray)
            {
                Sort(jToken);
            }
        }

        private static void SortJObject(JObject jObject)
        {
            var properties = jObject.Properties().ToArray();

            foreach (JProperty jProperty in properties)
            {
                jProperty.Remove();
            }

            foreach (JProperty jProperty in properties.OrderBy(a => a.Name))
            {
                jObject.Add(jProperty);
                SortJProperty(jProperty);
            }
        }

        private static void SortJProperty(JProperty jProperty)
        {
            Sort(jProperty.Value);
        }

        public static void Sort(JToken jToken)
        {
            switch (jToken)
            {
                case JArray jArray:
                    SortJArray(jArray);
                    break;
                case JObject jObject:
                    SortJObject(jObject);
                    break;
                case JProperty jProperty:
                    SortJProperty(jProperty);
                    break;
            }
        }
    }
}