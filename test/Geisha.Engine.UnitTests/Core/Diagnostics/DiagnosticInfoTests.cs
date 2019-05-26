using Geisha.Engine.Core.Diagnostics;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Diagnostics
{
    [TestFixture]
    public class DiagnosticInfoTests
    {
        [Test]
        public void ToString_ShouldReturnCorrectValue()
        {
            // Arrange
            var diagnosticInfo = new DiagnosticInfo("Name", "Value");

            // Act
            var actual = diagnosticInfo.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo("Name: Value"));
        }
    }
}