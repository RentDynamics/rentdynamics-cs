using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using RentDynamicsCS.Models;

namespace RentDynamicsCS.Resources
{
    public class AppointmentResource : BaseRentDynamicsResource
    {
        public AppointmentResource(IRentDynamicsApiClient apiClient) : base(apiClient)
        {
        }

        public async Task<List<DateTime>> GetAppointmentTimesAsync(int communityGroupId, DateTime appointmentDate, bool isUtc)
        {
            var parameters = new Dictionary<string, string>
            {
                { "appointmentDate", appointmentDate.Date.ToString("MM/dd/yyyy") },
                { "isUTC", isUtc.ToString() }
            };
            string query = QueryHelpers.AddQueryString($"/appointmentTimes/{communityGroupId}", parameters);
            return await ApiClient.GetAsync<List<DateTime>>(query);
        }

        public async Task<AppointmentDays> GetAppointmentDaysAsync(int communityGroupId, DateTime startAppointmentDate, DateTime endAppointmentDate)
        {
            var parameters = new Dictionary<string, string>
            {
                { "start", startAppointmentDate.Date.ToString("MM/dd/yyyy") },
                { "end", endAppointmentDate.Date.ToString("MM/dd/yyyy") },
            };
            string query = QueryHelpers.AddQueryString($"/appointmentDays/{communityGroupId}", parameters);
            return await ApiClient.GetAsync<AppointmentDays>(query);
        }
    }
}