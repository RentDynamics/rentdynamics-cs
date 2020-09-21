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

        /// <param name="communityGroupId">Id of the community group for which appointment times should be returned</param>
        /// <param name="appointmentDate">A specific date you want for which appointment times should be returned</param>
        private static string GetAppointmentTimesQuery(int communityGroupId, DateTime appointmentDate, bool asUtc)
        {
            var parameters = new Dictionary<string, string>
            {
                { "appointmentDate", appointmentDate.Date.ToString(RentDynamicsDefaultSettings.DateFormatShortUs) },
                { "utc", asUtc.ToString() }
            };
            return QueryHelpers.AddQueryString($"/appointmentTimes/{communityGroupId}", parameters);
        }

        /// <summary>
        /// Returns a list of available times as UTC for a given date
        /// </summary>
        /// <inheritdoc cref="GetAppointmentTimesQuery"/>
        /// <returns><see cref="UtcAppointmentTimesVM"/> object that contains appointment times represented as <see cref="DateTimeOffset"/> objects.</returns>
        public Task<UtcAppointmentTimesVM> GetAppointmentTimesAsUtcAsync(
            int communityGroupId,
            DateTime appointmentDate,
            CancellationToken token = default)
        {
            string query = GetAppointmentTimesQuery(communityGroupId, appointmentDate, true);
            return ApiClient.GetAsync<UtcAppointmentTimesVM>(query, token);
        }

        /// <summary>
        /// Returns a list of available times as community local time for a given date 
        /// </summary>
        /// <inheritdoc cref="GetAppointmentTimesQuery"/>
        /// <returns><see cref="LocalAppointmentTimesVM"/> object that contains appointment times represented as <see cref="DateTime"/> objects.</returns>
        public async Task<LocalAppointmentTimesVM> GetAppointmentTimesAsLocalAsync(
            int communityGroupId,
            DateTime appointmentDate,
            CancellationToken token = default)
        {
            string query = GetAppointmentTimesQuery(communityGroupId, appointmentDate, false);
            var response = await ApiClient.GetAsync<LocalAppointmentTimesVM>(query, token);

            var responseWithFixedDates = new LocalAppointmentTimesVM(response.Count);

            foreach (DateTime dateTime in response)
            {
                DateTime fixedDateTime = appointmentDate.Date.Add(dateTime.TimeOfDay);
                responseWithFixedDates.Add(DateTime.SpecifyKind(fixedDateTime, DateTimeKind.Unspecified));
            }

            return responseWithFixedDates;
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