using System;
using RentDynamics.RdClient.Models;

namespace RentDynamics.RdClient.Resources
{
    public static class AppointmentResourceSyncMethodExtensions
    {
        public static AppointmentTimes GetAppointmentTimes(this AppointmentResource resource, int communityGroupId, DateTime appointmentDate, bool isUtc)
            => resource.GetAppointmentTimesAsync(communityGroupId, appointmentDate, isUtc).GetAwaiter().GetResult();

        public static AppointmentDays GetAppointmentDays(this AppointmentResource resource, int communityGroupId, DateTime startAppointmentDate, DateTime endAppointmentDate)
            => resource.GetAppointmentDaysAsync(communityGroupId, startAppointmentDate, endAppointmentDate).GetAwaiter().GetResult();
    }
}