using System;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.Appointment
{
    [PublicAPI]
    public static class AppointmentResourceExtensions
    {
        public static AppointmentTimes GetAppointmentTimes(this AppointmentResource resource, int communityGroupId, DateTime appointmentDate, bool isUtc)
            => resource.GetAppointmentTimesAsync(communityGroupId, appointmentDate, isUtc).GetAwaiter().GetResult();

        public static AppointmentDays GetAppointmentDays(this AppointmentResource resource, int communityGroupId, DateTime startAppointmentDate, DateTime endAppointmentDate)
            => resource.GetAppointmentDaysAsync(communityGroupId, startAppointmentDate, endAppointmentDate).GetAwaiter().GetResult();
    }
}