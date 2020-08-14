using System.Globalization;
using JetBrains.Annotations;
using Newtonsoft.Json.Converters;

namespace RentDynamics.RdClient.Converters
{
    public class DateFormatConverter : IsoDateTimeConverter
    {
        [UsedImplicitly]
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }

        [UsedImplicitly]
        public DateFormatConverter(string format, string cultureName)
            : this(format)
        {
            Culture = CultureInfo.CreateSpecificCulture(cultureName);
        }
    }
}