using Geisha.Common;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core
{
    [TestFixture]
    public class PropertyFromExpressionTests
    {
        [Test]
        public void GetPropertyName_ShouldThrowArgumentException_GivenMethodExpression()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => PropertyFromExpression.GetPropertyName<ClassWithProperty, int>(c => c.IntegerMethod()), Throws.ArgumentException);
        }

        [Test]
        public void GetPropertyName_ShouldThrowArgumentException_GivenFieldExpression()
        {
            // Arrange
            // Act
            // Assert
            Assert.That(() => PropertyFromExpression.GetPropertyName<ClassWithProperty, int>(c => c.IntegerField), Throws.ArgumentException);
        }

        [Test]
        public void GetPropertyName_ShouldReturnPropertyName_GivenPropertyExpression()
        {
            // Arrange
            // Act
            var actual = PropertyFromExpression.GetPropertyName<ClassWithProperty, int>(c => c.IntegerProperty);

            // Assert
            Assert.That(actual, Is.EqualTo(nameof(ClassWithProperty.IntegerProperty)));
        }

        private sealed class ClassWithProperty
        {
            public int IntegerField;
            public int IntegerProperty { get; set; }
            public int IntegerMethod() => 0;
        }
    }
}