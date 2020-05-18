using Geisha.Engine.Rendering.Components;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components
{
    [TestFixture]
    public class EllipseRendererComponentTests
    {
        [Test]
        public void Radius_ConvertsEllipseToCircle_WhenSet()
        {
            // Arrange
            const double radius = 1.23;

            var ellipseRendererComponent = new EllipseRendererComponent
            {
                RadiusX = 4.56,
                RadiusY = 7.89
            };

            // Act
            ellipseRendererComponent.Radius = radius;

            // Assert
            Assert.That(ellipseRendererComponent.Radius, Is.EqualTo(radius));
            Assert.That(ellipseRendererComponent.RadiusX, Is.EqualTo(radius));
            Assert.That(ellipseRendererComponent.RadiusY, Is.EqualTo(radius));
        }

        [Test]
        public void Radius_ReturnsValue_WhenEllipseIsCircle()
        {
            // Arrange
            var ellipseRendererComponent = new EllipseRendererComponent
            {
                RadiusX = 1.23,
                RadiusY = 1.23
            };

            // Act
            var actual = ellipseRendererComponent.Radius;

            // Assert
            Assert.That(actual, Is.EqualTo(1.23));
        }

        [Test]
        public void Radius_ThrowsException_WhenEllipseIsNotCircle()
        {
            // Arrange
            var ellipseRendererComponent = new EllipseRendererComponent
            {
                RadiusX = 4.56,
                RadiusY = 7.89
            };

            // Act
            // Assert
            Assert.That(() => { _ = ellipseRendererComponent.Radius; }, Throws.TypeOf<EllipseIsNotCircleException>());
        }
    }
}