using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RentDynamicsCS.Converters;

namespace RentDynamicsCS.Models
{
    [JsonArray(ItemConverterType = typeof(DateFormatConverter), ItemConverterParameters = new object[] { "MM/dd/yyyy" })]
    public class AppointmentDays : List<DateTime>
    {
    }
}