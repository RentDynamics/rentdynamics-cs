using FluentAssertions;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RentDynamics.RdClient.Tests;

[TestClass]
public class NonceCalculatorTests
{
    [TestMethod]
    public async Task JArrays_ShouldBeSorted()
    {
        // Arrange
        string unsortedPayload =
            """
            [{"c":3,"a":1,"b":2},{"y":3,"z":1,"x":2}]
            """;
        TextReader unsortedJsonReader = new StringReader(unsortedPayload);

        // Act
        string sortedJson = await NonceCalculator.GetSortedJsonAsync(unsortedJsonReader);

        // Assert
        string sortedPayload =
            """
            [{"a":1,"b":2,"c":3},{"x":2,"y":3,"z":1}]
            """;
        sortedJson.Should().Be(sortedPayload);
    }

    [TestMethod]
    public async Task JObjects_ShouldBeSorted()
    {
        // Arrange
        string unsortedPayload =
            """
            {"c":3,"a":1,"b":2}
            """;
        TextReader unsortedJsonReader = new StringReader(unsortedPayload);

        // Act
        string sortedJson = await NonceCalculator.GetSortedJsonAsync(unsortedJsonReader);

        // Assert
        string sortedPayload =
            """
            {"a":1,"b":2,"c":3}
            """;
        sortedJson.Should().Be(sortedPayload);
    }
}