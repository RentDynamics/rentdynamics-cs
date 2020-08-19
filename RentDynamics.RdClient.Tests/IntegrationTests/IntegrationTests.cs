using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RentDynamics.RdClient.Models;
using RentDynamics.RdClient.Resources;
using RentDynamics.RdClient.Resources.LeadCards;

namespace RentDynamics.RdClient.Tests.IntegrationTests
{
    [TestClass]
    public class IntegrationTests : BaseIntegrationTest
    {
        [TestMethod]
        public async Task LeadCardRequest()
        {
            int communityId = 1;

            var apiClient = CreateApiClient();
            var resource = new LeadCardsResource(apiClient);
            var leadCardRequest = new LeadCardRequest("valery_petrov@somedomain.com",
                                                      "Valery",
                                                      "Petrov",
                                                      1234,
                                                      "5555555555",
                                                      DateTime.UtcNow,
                                                      2,
                                                      "my test valery",
                                                      appointmentDate: DateTime.Today.AddMinutes(15).AddHours(15)
                                                     );
            var res = await resource.CreateCommunityLeadCardAsync(communityId, leadCardRequest);
        }

        [TestMethod]
        public async Task AddressEndpoint()
        {
            var apiClient = CreateApiClient();

            var newAddress = new Dictionary<string, object>
            {
                { "addressLine_1", "123 Main Street" },
                { "city", "LA" },
                { "stateId", 5 },
                { "zip", "95800" },
                { "addressTypeId", 1 },
            };

            var createdAddress = await apiClient.PostAsync<object, Dictionary<string, object>>("/addresses", newAddress);

            string addressId = createdAddress["id"].ToString()!;

            var getAddress = await apiClient.GetAsync<Dictionary<string, object>>($"/addresses/{addressId}");

            var updateAddress = new Dictionary<string, object>
            {
                { "city", "New city" }
            };
            var putAddress = await apiClient.PutAsync<object, Dictionary<string, object>>($"addresses/{addressId}", updateAddress);

            var deleted = await apiClient.DeleteAsync<Dictionary<string, object>>($"addresses/{addressId}");
        }

        [TestMethod]
        public async Task AppointmentTimesEndpoint()
        {
            var client = CreateApiClient();

            var appointmentResource = new AppointmentResource(client);

            int communityGroupId = AvailableCommunityGroupId;
            var appointmentTimes = await appointmentResource.GetAppointmentTimesAsync(communityGroupId, new DateTime(2020, 08, 01), true);

            Assert.IsNotNull(appointmentTimes);
        }

        [TestMethod]
        public async Task AppointmentDaysEndpoint()
        {
            var client = CreateApiClient();

            var appointmentResource = new AppointmentResource(client);

            int communityGroupId = AvailableCommunityGroupId;
            var appointmentDays = await appointmentResource.GetAppointmentDaysAsync(communityGroupId,
                                                                                    new DateTime(2019, 10, 31),
                                                                                    new DateTime(2019, 11, 12));

            Assert.IsNotNull(appointmentDays);
        }

        [TestMethod]
        public void Get_SmsConversations()
        {
            int communityGroupId = AvailableCommunityGroupId;
            string leadPhoneNumber = "123";

            var client = CreateApiClient();


            var url = $"/smsConversations?filters=community_group_id={communityGroupId}|phone_number={leadPhoneNumber}";
            var existingConversations = client.Get<List<object>>(url);

            if (existingConversations.Count == 0)
            {
                Assert.Inconclusive("No conversations available");
            }
        }
    }
}