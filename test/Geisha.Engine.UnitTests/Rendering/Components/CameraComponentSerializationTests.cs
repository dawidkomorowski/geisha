using Geisha.Engine.Core.Math;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components
{
    [TestFixture]
    public class CameraComponentSerializationTests : ComponentSerializationTestsBase
    {
        [TestCase(AspectRatioBehavior.Overscan)]
        [TestCase(AspectRatioBehavior.Underscan)]
        public void SerializeAndDeserialize(AspectRatioBehavior aspectRatioBehavior)
        {
            // Arrange
            var viewRectangle = new Vector2(12.34, 56.78);

            // Act
            var actual = SerializeAndDeserialize<CameraComponent>(component =>
            {
                component.AspectRatioBehavior = aspectRatioBehavior;
                component.ViewRectangle = viewRectangle;
            });

            // Assert
            Assert.That(actual.AspectRatioBehavior, Is.EqualTo(aspectRatioBehavior));
            Assert.That(actual.ViewRectangle, Is.EqualTo(viewRectangle));
        }
    }
}