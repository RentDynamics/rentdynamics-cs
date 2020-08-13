using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using RentDynamics.RdClient.Converters;

namespace RentDynamics.RdClient.Models
{
    [JsonArray(ItemConverterType = typeof(DateFormatConverter), ItemConverterParameters = new object[] { "MM/dd/yyyy" })]
    public class AppointmentDays : List<DateTime>
    {
    }
}