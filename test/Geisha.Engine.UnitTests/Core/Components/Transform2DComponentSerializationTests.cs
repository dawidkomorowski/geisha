using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Components
{
    [TestFixture]
    public class Transform2DComponentSerializationTests : ComponentSerializationTestsBase
    {
        [Test]
        public void SerializeAndDeserialize()
        {
            // Arrange
            var translation = new Vector2(12.34, 56.78);
            const double rotation = 123.456;
            var scale = new Vector2(87.65, 43.21);
            const bool isInterpolated = true;

            // Act
            var actual = SerializeAndDeserialize<Transform2DComponent>(component =>
            {
                component.Translation = translation;
                component.Rotation = rotation;
                component.Scale = scale;
                component.IsInterpolated = isInterpolated;
            });

            // Assert
            Assert.That(actual.Translation, Is.EqualTo(translation));
            Assert.That(actual.Rotation, Is.EqualTo(rotation));
            Assert.That(actual.Scale, Is.EqualTo(scale));
            Assert.That(actual.IsInterpolated, Is.EqualTo(isInterpolated));
        }
    }
}