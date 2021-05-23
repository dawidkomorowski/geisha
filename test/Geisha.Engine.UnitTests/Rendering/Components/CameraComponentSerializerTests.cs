using Geisha.Common.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.Rendering.Components;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Rendering.Components
{
    [TestFixture]
    public class CameraComponentSerializerTests : ComponentSerializerTestsBase
    {
        protected override IComponentFactory ComponentFactory => new CameraComponentFactory();
        protected override IComponentSerializer ComponentSerializer => new CameraComponentSerializer();

        [TestCase(AspectRatioBehavior.Overscan)]
        [TestCase(AspectRatioBehavior.Underscan)]
        public void SerializeAndDeserialize(AspectRatioBehavior aspectRatioBehavior)
        {
            // Arrange
            var component = new CameraComponent
            {
                AspectRatioBehavior = aspectRatioBehavior,
                ViewRectangle = new Vector2(12.34, 56.78)
            };

            // Act
            var actual = SerializeAndDeserialize(component);

            // Assert
            Assert.That(actual.AspectRatioBehavior, Is.EqualTo(component.AspectRatioBehavior));
            Assert.That(actual.ViewRectangle, Is.EqualTo(component.ViewRectangle));
        }
    }
}