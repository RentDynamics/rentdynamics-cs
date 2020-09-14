using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.WebUtilities;

namespace RentDynamics.RdClient.Resources.Appointment
{
    [PublicAPI]
    public class AppointmentResource : BaseRentDynamicsResource
    {
        [UsedImplicitly]
        public AppointmentResource(IRentDynamicsApiClient apiClient) : base(apiClient)
        {
        }

        public AppointmentTimesVM GetAppointmentTimes(AppointmentResource resource, int communityGroupId, DateTime appointmentDate, bool isUtc)
            => resource.GetAppointmentTimesAsync(communityGroupId, appointmentDate, isUtc).GetAwaiter().GetResult();

        /// <summary>
        /// Access to appointment times
        /// </summary>
        /// <param name="communityGroupId">Id of the community group for which appointment times should be returned</param>
        /// <param name="appointmentDate">A specific date you want for which appointment times should be returned</param>
        /// <param name="isUtc">Return appointment times as UTC values. If the parameter is set to <c>false</c>, the response will be in MDT timezone.</param>
        /// <param name="token">The token to monitor for cancellation requests</param>
        /// <returns><see cref="AppointmentTimesVM"/> object that contains appointment times represented as <see cref="DateTime"/> objects.</returns>
        public async Task<AppointmentTimesVM> GetAppointmentTimesAsync(int communityGroupId, DateTime appointmentDate, bool isUtc, CancellationToken token = default)
        {
            var parameters = new Dictionary<string, string>
            {
                { "appointmentDate", appointmentDate.Date.ToString(RentDynamicsDefaultSettings.DateFormatShortUs) },
                { "utc", isUtc.ToString() }
            };
            string query = QueryHelpers.AddQueryString($"/appointmentTimes/{communityGroupId}", parameters);
            return await ApiClient.GetAsync<AppointmentTimesVM>(query, token);
        }

        public async Task<AppointmentDaysVM> GetAppointmentDaysAsync(int communityGroupId, DateTime startAppointmentDate, DateTime endAppointmentDate, CancellationToken token = default)
        {
            var parameters = new Dictionary<string, string>
            {
                { "start", startAppointmentDate.Date.ToString(RentDynamicsDefaultSettings.DateFormatShortUs) },
                { "end", endAppointmentDate.Date.ToString(RentDynamicsDefaultSettings.DateFormatShortUs) },
            };
            string query = QueryHelpers.AddQueryString($"/appointmentDays/{communityGroupId}", parameters);
            return await ApiClient.GetAsync<AppointmentDaysVM>(query);
        }
    }
}