using Geisha.Common.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Physics.Components;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Physics.Components
{
    [TestFixture]
    public class RectangleColliderComponentSerializationTests : ComponentSerializationTestsBase
    {
        protected override IComponentFactory ComponentFactory => new RectangleColliderComponentFactory();

        [Test]
        public void SerializeAndDeserialize()
        {
            // Arrange
            var component = new RectangleColliderComponent
            {
                Dimension = new Vector2(12.34, 56.78)
            };

            // Act
            var actual = SerializeAndDeserialize(component);

            // Assert
            Assert.That(actual.Dimension, Is.EqualTo(component.Dimension));
            Assert.That(actual.IsColliding, Is.False);
            Assert.That(actual.CollidingEntities, Is.Empty);
        }
    }
}