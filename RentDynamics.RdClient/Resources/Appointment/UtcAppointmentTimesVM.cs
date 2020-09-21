using System;
using System.Collections.Generic;
using System.Diagnostics;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.Appointment
{
    [PublicAPI, UsedImplicitly]
    [DebuggerDisplay("Count = {Count}")]
    public class UtcAppointmentTimesVM : List<DateTimeOffset>
    {
    }
}