using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RentDynamics.Client.Converters;

namespace RentDynamics.Client.Models
{
    [JsonArray(ItemConverterType = typeof(DateFormatConverter), ItemConverterParameters = new object[] { "MM/dd/yyyy" })]
    public class AppointmentDays : List<DateTime>
    {
    }
}