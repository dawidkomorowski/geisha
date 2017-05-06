using Geisha.Engine.Core.Diagnostics;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Diagnostics
{
    [TestFixture]
    public class DiagnosticsInfoTests
    {
        [TestCase("StringName", "Value", "StringName: Value")]
        [TestCase("IntName", 15, "IntName: 15")]
        [TestCase("BoolName", true, "BoolName: True")]
        public void ToString_ShouldReturnCorrectValue(string name, object value, string expected)
        {
            // Arrange
            var diagnosticsInfo = new DiagnosticsInfo {Name = name, Value = value};

            // Act
            var actual = diagnosticsInfo.ToString();

            // Assert
            Assert.That(actual, Is.EqualTo(expected));
        }
    }
}