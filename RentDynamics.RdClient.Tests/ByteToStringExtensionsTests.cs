using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RentDynamics.RdClient.Tests
{
    [TestClass]
    public class ByteToStringExtensionsTests
    {
        [TestMethod]
        public void FormatMethod_ShouldThrow_WhenArgumentIsNull()
        {
            byte[]? nullVariable = null;
            Assert.ThrowsException<ArgumentNullException>(() => nullVariable!.ToHexString());
        }

        [TestMethod]
        public void FormatMethod_ShouldFormatBytes()
        {
            byte[] bytes = { 0x00, 0x01, 0x10, 0xAA, 0xFF };

            string formatted = bytes.ToHexString();

            formatted.Should().Be("000110AAFF");
        }
    }
}