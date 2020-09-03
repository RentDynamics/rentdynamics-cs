using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RentDynamics.RdClient.Resources.LeadCards;

namespace RentDynamics.RdClient.Tests.ResourcesDataSerialization
{
    [TestClass]
    public class LeadCardSerializationTests
    {
        [TestMethod]
        public void LeadCard_ShouldBeDeserialized_WhenEmailAndPhoneNumber_AreNull()
        {
            const string json = "{\"phoneNumber\":null, \"email\":null}";
            var leadCard = JsonConvert.DeserializeObject<LeadCardVM>(json, RentDynamicsDefaultSettings.DefaultSerializerSettings);

            leadCard.PhoneNumber.Should().BeNull();
            leadCard.Email.Should().BeNull();
        }
    }
}