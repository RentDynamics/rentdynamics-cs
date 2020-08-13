using Newtonsoft.Json.Converters;

namespace RentDynamics.RdClient.Converters
{
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
}