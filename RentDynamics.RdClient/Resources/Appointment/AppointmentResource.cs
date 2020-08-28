using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;

namespace RentDynamics.RdClient.Resources.Appointment
{
    public class AppointmentResource : BaseRentDynamicsResource
    {
        public AppointmentResource(IRentDynamicsApiClient apiClient) : base(apiClient)
        {
        }

        public async Task<AppointmentTimes> GetAppointmentTimesAsync(int communityGroupId, DateTime appointmentDate, bool isUtc)
        {
            var parameters = new Dictionary<string, string>
            {
                { "appointmentDate", appointmentDate.Date.ToString(RentDynamicsDefaultSettings.DateFormatShortUs) },
                { "isUTC", isUtc.ToString() }
            };
            string query = QueryHelpers.AddQueryString($"/appointmentTimes/{communityGroupId}", parameters);
            return await ApiClient.GetAsync<AppointmentTimes>(query);
        }

        public async Task<AppointmentDays> GetAppointmentDaysAsync(int communityGroupId, DateTime startAppointmentDate, DateTime endAppointmentDate)
        {
            var parameters = new Dictionary<string, string>
            {
                { "start", startAppointmentDate.Date.ToString(RentDynamicsDefaultSettings.DateFormatShortUs) },
                { "end", endAppointmentDate.Date.ToString(RentDynamicsDefaultSettings.DateFormatShortUs) },
            };
            string query = QueryHelpers.AddQueryString($"/appointmentDays/{communityGroupId}", parameters);
            return await ApiClient.GetAsync<AppointmentDays>(query);
        }
    }
}