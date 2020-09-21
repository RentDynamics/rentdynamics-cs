using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;
using Newtonsoft.Json;
using RentDynamics.RdClient.Converters;

namespace RentDynamics.RdClient.Resources.Appointment
{
    [PublicAPI, UsedImplicitly]
    [DebuggerDisplay("Count = {Count}")]
    [JsonArray(ItemConverterType = typeof(DateFormatConverter), ItemConverterParameters = new object[] { RentDynamicsDefaultSettings.DateFormatShortUs })]
    public class AppointmentDaysVM : List<DateTime>
    {
    }
}