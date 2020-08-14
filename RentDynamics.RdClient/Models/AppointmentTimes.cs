using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using RentDynamics.RdClient.Converters;

namespace RentDynamics.RdClient.Models
{
    [DebuggerDisplay("Count = {Count}")]
    [JsonArray(ItemConverterType = typeof(DateFormatConverter), ItemConverterParameters = new object[] { RentDynamicsDefaultSettings.TimeFormatUs, RentDynamicsDefaultSettings.CultureNameUs })]
    public class AppointmentTimes : List<DateTime>
    {
    }
}