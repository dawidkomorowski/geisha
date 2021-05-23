using System;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel.Serialization
{
    [TestFixture]
    public class ComponentSerializerTests
    {
        [Test]
        public void Constructor_ShouldSetComponentId()
        {
            // Arrange
            // Act
            var serializer = new TestComponentSerializer();

            // Assert
            Assert.That(serializer.ComponentId, Is.EqualTo(new ComponentId("Bugged")));
        }

        [Test]
        public void Serialize_ShouldCastComponentAndPassComponentDataWriter()
        {
            // Arrange
            var component = new TestComponent();
            var componentDataWriter = Substitute.For<IComponentDataWriter>();

            var serializer = new TestComponentSerializer();

            // Act
            serializer.Serialize(component, componentDataWriter);

            // Assert
            Assert.That(serializer.ComponentInSerialize, Is.EqualTo(component));
            Assert.That(serializer.ComponentDataWriter, Is.EqualTo(componentDataWriter));
        }

        [Test]
        public void Deserialize_ShouldCastComponentAndPassComponentDataReader()
        {
            // Arrange
            var component = new TestComponent();
            var componentDataReader = Substitute.For<IComponentDataReader>();

            var serializer = new TestComponentSerializer();

            // Act
            serializer.Deserialize(component, componentDataReader);

            // Assert
            Assert.That(serializer.ComponentInDeserialize, Is.EqualTo(component));
            Assert.That(serializer.ComponentDataReader, Is.EqualTo(componentDataReader));
        }

        #region Helpers

        private sealed class TestComponent : Component
        {
        }

        private sealed class TestComponentSerializer : ComponentSerializer<TestComponent>
        {
            public TestComponentSerializer() : base(new ComponentId())
            {
                throw new NotImplementedException();
            }

            public TestComponent? ComponentInSerialize { get; private set; }
            public TestComponent? ComponentInDeserialize { get; private set; }
            public IComponentDataWriter? ComponentDataWriter { get; private set; }
            public IComponentDataReader? ComponentDataReader { get; private set; }

            protected override void Serialize(TestComponent component, IComponentDataWriter componentDataWriter)
            {
                ComponentInSerialize = component;
                ComponentDataWriter = componentDataWriter;
            }

            protected override void Deserialize(TestComponent component, IComponentDataReader componentDataReader)
            {
                ComponentInDeserialize = component;
                ComponentDataReader = componentDataReader;
            }
        }

        #endregion
    }
}