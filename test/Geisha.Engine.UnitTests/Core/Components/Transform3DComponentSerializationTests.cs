using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.Math;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Components
{
    [TestFixture]
    public class Transform3DComponentSerializationTests : ComponentSerializationTestsBase
    {
        [Test]
        public void SerializeAndDeserialize()
        {
            // Arrange
            var translation = new Vector3(1.23, 4.56, 7.89);
            var rotation = new Vector3(2.31, 5.64, 8.97);
            var scale = new Vector3(3.12, 6.45, 9.78);

            // Act
            var actual = SerializeAndDeserialize<Transform3DComponent>(component =>
            {
                component.Translation = translation;
                component.Rotation = rotation;
                component.Scale = scale;
            });

            // Assert
            Assert.That(actual.Translation, Is.EqualTo(translation));
            Assert.That(actual.Rotation, Is.EqualTo(rotation));
            Assert.That(actual.Scale, Is.EqualTo(scale));
        }
    }
}