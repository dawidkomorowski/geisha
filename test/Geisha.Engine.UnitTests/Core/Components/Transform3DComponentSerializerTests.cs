using Geisha.Common.Math;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using Geisha.Engine.UnitTests.Core.SceneModel.Serialization;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Components
{
    [TestFixture]
    public class Transform3DComponentSerializerTests : ComponentSerializerTestsBase
    {
        protected override ComponentId ComponentId => Transform3DComponent.Id;
        protected override IComponentFactory ComponentFactory => new Transform3DComponentFactory();
        protected override IComponentSerializer ComponentSerializer => new Transform3DComponentSerializer();

        [Test]
        public void SerializeAndDeserialize()
        {
            // Arrange
            var component = new Transform3DComponent
            {
                Translation = new Vector3(1.23, 4.56, 7.89),
                Rotation = new Vector3(2.31, 5.64, 8.97),
                Scale = new Vector3(3.12, 6.45, 9.78)
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