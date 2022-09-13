using System;
using Geisha.Common;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core
{
    [TestFixture]
    public class DateTimeProviderTests
    {
        [Test]
        public void Now_ShouldReturnValueOfDateTimeNow()
        {
            // Arrange
            var dateTimeProvider = new DateTimeProvider();

            // Act
            var dateTimeNowBefore = DateTime.Now;
            var actual = dateTimeProvider.Now();
            var dateTimeNowAfter = DateTime.Now;

            // Assert
            Assert.That(actual, Is.GreaterThanOrEqualTo(dateTimeNowBefore));
            Assert.That(actual, Is.LessThanOrEqualTo(dateTimeNowAfter));
        }
    }
}