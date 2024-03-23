using System;
using System.Linq;
using Geisha.Engine.Core.Collections;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Collections
{
    [TestFixture]
    public class SingleOrEmptyTests
    {
        [Test]
        public void Empty_ShouldCreateSingleOrEmptyWithNoElements()
        {
            // Arrange
            // Act
            var actual = SingleOrEmpty.Empty<int>();

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void Single_ShouldCreateSingleOrEmptyWithSingleElement()
        {
            // Arrange
            var element = Guid.NewGuid();

            // Act
            var actual = SingleOrEmpty.Single(element);

            // Assert
            Assert.That(actual, Has.One.Items);
            Assert.That(actual.Single(), Is.EqualTo(element));
        }
    }
}