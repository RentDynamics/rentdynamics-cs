using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.Contrib.HttpClient;
using RentDynamics.RdClient.Resources;

namespace RentDynamics.RdClient.Tests.ResourcesDataSerialization
{
    [TestClass]
    public class AppointmentResourceTests : BaseResourceDataSerializationTest<AppointmentResource>
    {
        [TestMethod]
        public async Task AppointmentTimesRequest_ShouldBeSuccessful()
        {
            const int communityGroupId = 1;
            MockHandler.SetupAnyRequest()
                       .ReturnsResponse("[\"2020-08-06T20:30:00-0600\",\"2020-08-06T20:45:00-0600\"]", MediaTypeNames.Application.Json);


            var appointmentTimes = await Resource.GetAppointmentTimesAsync(communityGroupId, new DateTime(2020, 08, 06), false);

            var utcAppointmentTimes = appointmentTimes.Select(d => d.ToUniversalTime()).ToArray();

            utcAppointmentTimes.Should()
                               .HaveCount(2)
                               .And.ContainInOrder(DateTime.Parse("2020-08-07T02:30:00"), DateTime.Parse("2020-08-07T02:45:00"));
            
            MockHandler.VerifyRequest(message =>
            {
                message.RequestUri.AbsolutePath.Should().Be($"/appointmentTimes/{communityGroupId}");

                message.RequestUri.TryReadQueryAs<Dictionary<string, object>>(out var queryDict).Should().BeTrue();
                
                queryDict.Should().ContainKey("appointmentDate").WhichValue.Should().Be("08/06/2020");
                queryDict.Should().ContainKey("isUTC").WhichValue.Should().BeOfType<string>().Which.Should().ContainEquivalentOf("false");
                
                return true;
            });
        }

        [TestMethod]
        public async Task AppointmentDaysRequest_ShouldBeSuccessful()
        {
            const int communityGroupId = 1;

            MockHandler.SetupAnyRequest()
                       .ReturnsResponse("[\"10/31/2019\",\"11/01/2019\",\"11/02/2019\"]", MediaTypeNames.Application.Json);

            var appointmentDays = await Resource.GetAppointmentDaysAsync(communityGroupId, new DateTime(2019, 10, 31), new DateTime(2019, 11, 02));

            MockHandler.VerifyRequest(message =>
            {
                message.RequestUri.AbsolutePath.Should().Be($"/appointmentDays/{communityGroupId}");
                message.RequestUri.TryReadQueryAs<Dictionary<string, object>>(out var queryDict).Should().BeTrue();

                queryDict.Should().ContainKey("start").WhichValue.Should().Be("10/31/2019");
                queryDict.Should().ContainKey("end").WhichValue.Should().Be("11/02/2019");
                return true;
            });
            
            appointmentDays.Should()
                           .ContainInOrder(new DateTime(2019, 10, 31),
                                           new DateTime(2019, 11, 01),
                                           new DateTime(2019, 11, 02)
                                           );
        }
    }
}