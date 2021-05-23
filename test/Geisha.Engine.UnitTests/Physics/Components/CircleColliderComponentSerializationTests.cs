using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Components
{
    [TestFixture]
    public class CircleColliderComponentSerializationTests : ComponentSerializationTestsBase
    {
        protected override IComponentFactory ComponentFactory => new CircleColliderComponentFactory();

        [Test]
        public void SerializeAndDeserialize()
        {
            // Arrange
            var component = new CircleColliderComponent
            {
                Radius = 123.456
            };

            // Act
            var actual = SerializeAndDeserialize(component);

            // Assert
            Assert.That(actual.Radius, Is.EqualTo(component.Radius));
            Assert.That(actual.IsColliding, Is.False);
            Assert.That(actual.CollidingEntities, Is.Empty);
        }
    }
}