using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Rendering.Components;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components
{
    [TestFixture]
    public class EllipseRendererComponentTests
    {
        private Entity Entity { get; set; } = null!;

        [SetUp]
        public void SetUp()
        {
            var scene = TestSceneFactory.Create();
            Entity = scene.CreateEntity();
        }

        [Test]
        public void Radius_ConvertsEllipseToCircle_WhenSet()
        {
            // Arrange
            const double radius = 1.23;

            var ellipseRendererComponent = Entity.CreateComponent<EllipseRendererComponent>();
            ellipseRendererComponent.RadiusX = 4.56;
            ellipseRendererComponent.RadiusY = 7.89;


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
            var ellipseRendererComponent = Entity.CreateComponent<EllipseRendererComponent>();
            ellipseRendererComponent.RadiusX = 1.23;
            ellipseRendererComponent.RadiusY = 1.23;

            // Act
            var actual = ellipseRendererComponent.Radius;

            // Assert
            Assert.That(actual, Is.EqualTo(1.23));
        }

        [Test]
        public void Radius_ThrowsException_WhenEllipseIsNotCircle()
        {
            // Arrange
            var ellipseRendererComponent = Entity.CreateComponent<EllipseRendererComponent>();
            ellipseRendererComponent.RadiusX = 4.56;
            ellipseRendererComponent.RadiusY = 7.89;

            // Act
            // Assert
            Assert.That(() => { _ = ellipseRendererComponent.Radius; }, Throws.TypeOf<EllipseIsNotCircleException>());
        }
    }
}