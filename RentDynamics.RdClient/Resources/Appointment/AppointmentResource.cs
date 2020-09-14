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

        /// <summary>
        /// Returns a list of available times for a given date
        /// </summary>
        /// <param name="communityGroupId">Id of the community group for which appointment times should be returned</param>
        /// <param name="appointmentDate">A specific date you want for which appointment times should be returned</param>
        /// <param name="asUtc">Default value is false. When <c>false</c>, the response returns appointment times in local time. When <c>true</c>, the response returns appointment times will be in UTC.</param>
        /// <param name="token">The token to monitor for cancellation requests</param>
        /// <returns><see cref="AppointmentTimesVM"/> object that contains appointment times represented as <see cref="DateTime"/> objects.</returns>
        public async Task<AppointmentTimesVM> GetAppointmentTimesAsync(int communityGroupId, DateTime appointmentDate, bool asUtc = false, CancellationToken token = default)
        {
            var parameters = new Dictionary<string, string>
            {
                { "appointmentDate", appointmentDate.Date.ToString(RentDynamicsDefaultSettings.DateFormatShortUs) },
                { "utc", asUtc.ToString() }
            };
            string query = QueryHelpers.AddQueryString($"/appointmentTimes/{communityGroupId}", parameters);
            return await ApiClient.GetAsync<AppointmentTimesVM>(query, token);
        }

        /// <summary>
        /// Returns a list of days that have availability for a specified date range
        /// </summary>
        /// <param name="communityGroupId">Id of the community group for which appointment days should be returned</param>
        /// <param name="startAppointmentDate">Starting date for the search interval (inclusive bound)</param>
        /// <param name="endAppointmentDate">Starting date for the search interval (inclusive bound)</param>
        /// <param name="token">The token to monitor for cancellation requests</param>
        /// <returns><see cref="AppointmentDaysVM"/> object that contains appointment days represented as <see cref="DateTime"/> objects.</returns>
        public async Task<AppointmentDaysVM> GetAppointmentDaysAsync(
            int communityGroupId,
            DateTime startAppointmentDate,
            DateTime endAppointmentDate,
            CancellationToken token = default)
        {
            var parameters = new Dictionary<string, string>
            {
                { "start", startAppointmentDate.Date.ToString(RentDynamicsDefaultSettings.DateFormatShortUs) },
                { "end", endAppointmentDate.Date.ToString(RentDynamicsDefaultSettings.DateFormatShortUs) },
            };
            string query = QueryHelpers.AddQueryString($"/appointmentDays/{communityGroupId}", parameters);
            return await ApiClient.GetAsync<AppointmentDaysVM>(query, token);
        }
    }
}