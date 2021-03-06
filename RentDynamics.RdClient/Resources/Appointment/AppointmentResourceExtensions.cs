using System;
using JetBrains.Annotations;

namespace RentDynamics.RdClient.Resources.Appointment
{
    [PublicAPI]
    public static class AppointmentResourceExtensions
    {
        /// <inheritdoc cref="AppointmentResource.GetAppointmentTimesAsUtcAsync"/>
        public static UtcAppointmentTimesVM GetAppointmentTimesAsUtc(this AppointmentResource resource, int communityGroupId, DateTime appointmentDate)
            => resource.GetAppointmentTimesAsUtcAsync(communityGroupId, appointmentDate).GetAwaiter().GetResult();

        /// <inheritdoc cref="AppointmentResource.GetAppointmentTimesAsLocalAsync"/>
        public static LocalAppointmentTimesVM GetAppointmentTimesAsLocalAsync(this AppointmentResource resource, int communityGroupId, DateTime appointmentDate)
            => resource.GetAppointmentTimesAsLocalAsync(communityGroupId, appointmentDate).GetAwaiter().GetResult();

        /// <inheritdoc cref="AppointmentResource.GetAppointmentDaysAsync"/>
        public static AppointmentDaysVM GetAppointmentDays(this AppointmentResource resource, int communityGroupId, DateTime startAppointmentDate, DateTime endAppointmentDate)
            => resource.GetAppointmentDaysAsync(communityGroupId, startAppointmentDate, endAppointmentDate).GetAwaiter().GetResult();
    }
}