using Geisha.Common.Math;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Components
{
    [TestFixture]
    public class Transform2DComponentSerializerTests : ComponentSerializerTestsBase
    {
        protected override IComponentFactory ComponentFactory => new Transform2DComponentFactory();

        [Test]
        public void SerializeAndDeserialize()
        {
            // Arrange
            var component = new Transform2DComponent
            {
                Translation = new Vector2(12.34, 56.78),
                Rotation = 123.456,
                Scale = new Vector2(87.65, 43.21)
            };

            // Act
            var actual = SerializeAndDeserialize(component);

            // Assert
            Assert.That(actual.Translation, Is.EqualTo(component.Translation));
            Assert.That(actual.Rotation, Is.EqualTo(component.Rotation));
            Assert.That(actual.Scale, Is.EqualTo(component.Scale));
        }
    }
}