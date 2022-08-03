using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq.Contrib.HttpClient;
using Newtonsoft.Json.Linq;
using RentDynamics.RdClient.Resources;

namespace RentDynamics.RdClient.Tests.ResourcesDataSerialization
{
    [TestClass]
    public class DeserializationTests : BaseResourceDataSerializationTest<BaseRentDynamicsResource>
    {
        public class MyModel
        {
            public int ID { get; set; }
            public string? NameProperty { get; set; }
        }

        [TestMethod]
        public async Task Model_WithPascalCaseProperties_ShouldBeDeserialized_WhenResponseContainsCamelCaseProperties()
        {
            MockHandler.SetupAnyRequest()
                       .ReturnsResponse(HttpStatusCode.OK, "{\"id\":100500, \"nameProperty\":\"MyTestName\"}", MediaTypeNames.Application.Json);


            var myModel = await CreateRdClient().GetAsync<MyModel>("");

            myModel.ID.Should().Be(100500);
            myModel.NameProperty.Should().Be("MyTestName");
        }

        [TestMethod]
        public async Task Model_WithPascalCaseProperties_ShouldBeSerialized_IntoCamelCaseJson()
        {
            MockHandler.SetupAnyRequest()
                       .ReturnsResponse(HttpStatusCode.NoContent, Stream.Null);

            var myModel = new MyModel
            {
                ID = 100500,
                NameProperty = "MyTestName"
            };

            await CreateRdClient().PostAsync<MyModel, object>("", myModel);

            MockHandler.VerifyRequest(async message =>
            {
                var jContent = await message.Content.ReadAsAsync<JObject>();

                jContent.Should().ContainKey("id").WhoseValue.Value<int>().Should().Be(100500);
                jContent.Should().ContainKey("nameProperty").WhoseValue.Value<string>().Should().Be("MyTestName");

                return true;
            });
        }
    }
}