using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RentDynamics.RdClient.Models;
using RentDynamics.RdClient.Resources;
using RentDynamics.RdClient.Resources.LeadCards;

namespace RentDynamics.RdClient.Tests.IntegrationTests
{
    [TestClass]
    public class RdApiIntegrationTests : BaseRdApiIntegrationTest
    {
        private void ShouldBeSame<TTarget, TProperty>(TTarget o1, TTarget o2, Func<TTarget, TProperty> selector)
        {
            selector(o2).Should().Be(selector(o1));
        }

        [TestMethod]
        public async Task LeadCardRequest()
        {
            int communityId = 1;

            var apiClient = CreateApiClient();
            var resource = new LeadCardsResource(apiClient);

            var req = new LeadCard
            {
                FirstName = "Valeriy",
                LastName = "Petrov",
                PhoneNumber = "8005553535",
                Email = "valery_petrov@somedomain.com",
                Bathrooms = (decimal?) 1.5,
                Bedrooms = 2,
                Note = "My note",
                AppointmentDate = DateTime.UtcNow.AddDays(3),
                MoveDate = DateTime.UtcNow.AddDays(-2),
                CommunicationTypeId = 1,
                TourTypeId = 1,
                UnqualifiedReasonTypeId = 1,
                EndFollowUpReasonTypeId = 1,
                PreferredCommunicationTypeId = 1,
                NoAppointmentReasonTypeId = 1,
                SecondaryPreferredCommunicationTypeId = 1,
                LeaseTerm = 1,
                ReferrerSourceId = 1,
            };

            req.Amenities.Add(22);

            var occupant = new Occupant
            {
                FirstName = "Pedro",
                RelationshipTypeId = 1,
                PhoneNumber = "1111111111",
                EmailAddress = "pedro_petrov@somedomain.com"
            };
            req.Occupants.Add(occupant);

            var address = new Address
            {
                AddressLine1 = "123 Main Street",
                AddressLine2 = "-",
                Country = "Russia",
                Zip = "7777",
                City = "MagicalLand",
                State = "Alaska",
                AddressTypeId = 1
            };

            req.Address = address;

            var pet = new Pet
            {
                Breed = "Bulldog",
                PetName = "Anton"
            };

            req.Pets.Add(pet);

            // MoveDate = DateTime.Today.AddHours(2).AddDays(-2).AddMinutes(33).AddSeconds(21).AddMilliseconds(11)
            var res = await resource.CreateCommunityLeadCardAsync(communityId, req);

            Console.WriteLine(JsonConvert.SerializeObject(res, Formatting.Indented));


            ShouldBeSame(req, res, a => a.FirstName);
            ShouldBeSame(req, res, a => a.LastName);
            ShouldBeSame(req, res, a => a.Email);
            ShouldBeSame(req, res, a => a.PhoneNumber);
            ShouldBeSame(req, res, a => a.AdSourceId);
            ShouldBeSame(req, res, a => a.Bathrooms);
            ShouldBeSame(req, res, a => a.Bedrooms);
            ShouldBeSame(req, res, a => a.Note);
            ShouldBeSame(req, res, a => a.CommunicationTypeId);
            ShouldBeSame(req, res, a => a.TourTypeId);
            ShouldBeSame(req, res, a => a.MoveReasonTypeId);
            // ShouldBeSame(req, res, a => a.UnqualifiedReasonTypeId);
            // ShouldBeSame(req, res, a => a.EndFollowUpReasonTypeId);
            // ShouldBeSame(req, res, a => a.PreferredCommunicationTypeId);
            // ShouldBeSame(req, res, a => a.NoAppointmentReasonTypeId);
            // ShouldBeSame(req, res, a => a.SecondaryPreferredCommunicationTypeId);
            ShouldBeSame(req, res, a => a.LeaseTerm);
            ShouldBeSame(req, res, a => a.ReferrerSourceId);
            ShouldBeSame(req, res, a => a.SecondaryAdSourceId);
            // ShouldBeSame(req, res, a => a.FollowUpDate);
            res.AppointmentDate.Should().BeCloseTo(req.AppointmentDate!.Value);
            res.MoveDate.Should().BeCloseTo(req.MoveDate!.Value);
            res.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

            res.Amenities.Should().ContainInOrder(req.Amenities);
            res.Occupants.Should()
               .HaveCount(2)
               .And.Contain(oc => oc.EmailAddress == occupant.EmailAddress &&
                                  oc.FirstName == occupant.FirstName &&
                                  oc.RelationshipTypeId == occupant.RelationshipTypeId &&
                                  oc.PhoneNumber == occupant.PhoneNumber
                           );

            ShouldBeSame(req.Address, res.Address, a => a.City);
            ShouldBeSame(req.Address!, res.Address, a => a.Country);
            ShouldBeSame(req.Address!, res.Address, a => a.State);
            ShouldBeSame(req.Address!, res.Address, a => a.AddressLine1);
            ShouldBeSame(req.Address!, res.Address, a => a.AddressLine2);
            res.Address!.Created.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
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