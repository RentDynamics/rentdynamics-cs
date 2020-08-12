using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.Contrib.HttpClient;
using RentDynamics.Client.Resources;

namespace RentDynamics.Client.Tests.ResourcesDataSerialization
{
    [TestClass]
    public class AppointmentResourceTests : BaseResourceDataSerializationTest<AppointmentResource>
    {
        [TestMethod]
        public async Task AppointmentTimesMoqEndpoint()
        {
            MockHandler.SetupAnyRequest()
                       .ReturnsResponse("[\"2020-08-06T20:30:00-0600\",\"2020-08-06T20:45:00-0600\"]", MediaTypeNames.Application.Json);
            

            var appointmentTimes = await Resource.GetAppointmentTimesAsync(1, DateTime.Now, false);

            var utcAppointmentTimes = appointmentTimes.Select(d => d.ToUniversalTime()).ToArray();

            utcAppointmentTimes.Should()
                               .HaveCount(2)
                               .And.ContainInOrder(DateTime.Parse("2020-08-07T02:30:00"), DateTime.Parse("2020-08-07T02:45:00"));
        }

        [TestMethod]
        public async Task AppointmentDaysMoqEndpoint()
        {
            MockHandler.SetupAnyRequest()
                       .ReturnsResponse("[\"10/31/2019\",\"11/01/2019\",\"11/02/2019\"]", MediaTypeNames.Application.Json);

            var appointmentDays = await Resource.GetAppointmentDaysAsync(1, DateTime.Today.AddDays(-1), DateTime.Today);

            appointmentDays.Should()
                           .ContainInOrder(new DateTime(2019, 10, 31),
                                           new DateTime(2019, 11, 01),
                                           new DateTime(2019, 11, 02)
                                           );
        }
    }
}