using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.SceneModel.Serialization;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel.Serialization
{
    [TestFixture]
    public class SerializableEntityMapperTests
    {
        private ISerializableComponentMapper _serializableComponentMapper = null!;
        private ISerializableComponentMapperProvider _serializableComponentMapperProvider = null!;
        private SerializableEntityMapper _serializableEntityMapper = null!;

        [SetUp]
        public void SetUp()
        {
            _serializableComponentMapper = Substitute.For<ISerializableComponentMapper>();
            _serializableComponentMapperProvider = Substitute.For<ISerializableComponentMapperProvider>();
            _serializableEntityMapper = new SerializableEntityMapper(_serializableComponentMapperProvider);
        }

        #region MapToSerializable

        [Test]
        public void MapToSerializable_ShouldReturnSerializableEntityWithNoChildren_GivenEntityWithNoChildren()
        {
            // Arrange
            var entity = new Entity();

            // Act
            var actual = _serializableEntityMapper.MapToSerializable(entity);

            // Assert
            Assert.That(actual.Children, Has.Count.Zero);
        }

        [Test]
        public void MapToSerializable_ShouldReturnSerializableEntityWithChildren_GivenEntityWithChildren()
        {
            // Arrange
            var entity = new Entity();
            entity.AddChild(new Entity());
            entity.AddChild(new Entity());
            entity.AddChild(new Entity());

            // Act
            var actual = _serializableEntityMapper.MapToSerializable(entity);

            // Assert
            Assert.That(actual.Children, Has.Count.EqualTo(3));
        }

        [Test]
        public void MapToSerializable_ShouldReturnSerializableEntityGraph_GivenEntityGraph()
        {
            // Arrange
            var root = new Entity();
            var child1 = new Entity {Parent = root};
            var child2 = new Entity {Parent = root};
            _ = new Entity {Parent = root};

            child1.AddChild(new Entity());
            child1.AddChild(new Entity());
            child2.AddChild(new Entity());

            // Act
            var actual = _serializableEntityMapper.MapToSerializable(root);

            // Assert
            Assert.That(actual.Children, Has.Count.EqualTo(3));
            Assert.That(actual.Children.ElementAt(0).Children, Has.Count.EqualTo(2));
            Assert.That(actual.Children.ElementAt(1).Children, Has.Count.EqualTo(1));
            Assert.That(actual.Children.ElementAt(2).Children, Has.Count.Zero);
        }

        [Test]
        public void MapToSerializable_ShouldReturnSerializableEntityWithName_GivenEntityWithName()
        {
            // Arrange
            var entity = new Entity
            {
                Name = "Some entity"
            };

            // Act
            var actual = _serializableEntityMapper.MapToSerializable(entity);

            // Assert
            Assert.That(actual.Name, Is.EqualTo(entity.Name));
        }

        [Test]
        public void MapToSerializable_ShouldReturnSerializableEntityWithNoComponents_GivenEntityWithNoComponents()
        {
            // Arrange
            var entity = new Entity();

            // Act
            var actual = _serializableEntityMapper.MapToSerializable(entity);

            // Assert
            Assert.That(actual.Components, Has.Count.Zero);
        }

        [Test]
        public void MapToSerializable_ShouldReturnSerializableEntityWithComponents_GivenEntityWithComponents()
        {
            // Arrange
            var entity = new Entity();

            var component1 = new TestComponent();
            var component2 = new TestComponent();
            var component3 = new TestComponent();
            entity.AddComponent(component1);
            entity.AddComponent(component2);
            entity.AddComponent(component3);

            var serializableComponent1 = new SerializableTestComponent();
            var serializableComponent2 = new SerializableTestComponent();
            var serializableComponent3 = new SerializableTestComponent();

            _serializableComponentMapperProvider.GetMapperFor(component1).Returns(_serializableComponentMapper);
            _serializableComponentMapperProvider.GetMapperFor(component2).Returns(_serializableComponentMapper);
            _serializableComponentMapperProvider.GetMapperFor(component3).Returns(_serializableComponentMapper);

            _serializableComponentMapper.MapToSerializable(component1).Returns(serializableComponent1);
            _serializableComponentMapper.MapToSerializable(component2).Returns(serializableComponent2);
            _serializableComponentMapper.MapToSerializable(component3).Returns(serializableComponent3);

            // Act
            var actual = _serializableEntityMapper.MapToSerializable(entity);

            // Assert
            Assert.That(actual.Components, Has.Count.EqualTo(3));
            Assert.That(actual.Components.ElementAt(0), Is.EqualTo(serializableComponent1));
            Assert.That(actual.Components.ElementAt(1), Is.EqualTo(serializableComponent2));
            Assert.That(actual.Components.ElementAt(2), Is.EqualTo(serializableComponent3));
        }

        #endregion

        #region MapFromSerializable

        [Test]
        public void MapFromSerializable_ShouldReturnEntityWithNoChildren_GivenSerializableEntityWithNoChildren()
        {
            // Arrange
            var serializableEntity = GetSerializableEntity();

            // Act
            var actual = _serializableEntityMapper.MapFromSerializable(serializableEntity);

            // Assert
            Assert.That(actual.Children, Has.Count.Zero);
        }

        [Test]
        public void MapFromSerializable_ShouldReturnEntityWithChildren_GivenSerializableEntityWithChildren()
        {
            // Arrange
            var serializableEntity = GetSerializableEntity(
                GetSerializableEntity(),
                GetSerializableEntity(),
                GetSerializableEntity()
            );

            // Act
            var actual = _serializableEntityMapper.MapFromSerializable(serializableEntity);

            // Assert
            Assert.That(actual.Children, Has.Count.EqualTo(3));
        }

        [Test]
        public void MapFromSerializable_ShouldReturnEntityGraph_GivenSerializableEntityGraph()
        {
            // Arrange
            var serializableEntity = GetSerializableEntity(
                GetSerializableEntity(
                    GetSerializableEntity(),
                    GetSerializableEntity()
                ),
                GetSerializableEntity(
                    GetSerializableEntity()
                ),
                GetSerializableEntity()
            );

            // Act
            var actual = _serializableEntityMapper.MapFromSerializable(serializableEntity);

            // Assert
            Assert.That(actual.Children, Has.Count.EqualTo(3));
            Assert.That(actual.Children.ElementAt(0).Children, Has.Count.EqualTo(2));
            Assert.That(actual.Children.ElementAt(1).Children, Has.Count.EqualTo(1));
            Assert.That(actual.Children.ElementAt(2).Children, Has.Count.EqualTo(0));
        }

        [Test]
        public void MapFromSerializable_ShouldReturnEntityWithName_GivenSerializableEntityWithName()
        {
            // Arrange
            var serializableEntity = GetSerializableEntity();
            serializableEntity.Name = "Some entity";

            // Act
            var actual = _serializableEntityMapper.MapFromSerializable(serializableEntity);

            // Assert
            Assert.That(actual.Name, Is.EqualTo(serializableEntity.Name));
        }

        [Test]
        public void MapFromSerializable_ShouldReturnEntityWithNoComponents_GivenSerializableEntityWithNoComponents()
        {
            // Arrange
            var serializableEntity = GetSerializableEntity();

            // Act
            var actual = _serializableEntityMapper.MapFromSerializable(serializableEntity);

            // Assert
            Assert.That(actual.Components, Has.Count.Zero);
        }

        [Test]
        public void MapFromSerializable_ShouldReturnEntityWithComponents_GivenSerializableEntityWithComponents()
        {
            // Arrange
            var serializableEntity = GetSerializableEntity();

            var serializableComponent1 = new SerializableTestComponent();
            var serializableComponent2 = new SerializableTestComponent();
            var serializableComponent3 = new SerializableTestComponent();

            serializableEntity.Components = new List<ISerializableComponent>
            {
                serializableComponent1,
                serializableComponent2,
                serializableComponent3
            };

            var component1 = new TestComponent();
            var component2 = new TestComponent();
            var component3 = new TestComponent();

            _serializableComponentMapperProvider.GetMapperFor(serializableComponent1).Returns(_serializableComponentMapper);
            _serializableComponentMapperProvider.GetMapperFor(serializableComponent2).Returns(_serializableComponentMapper);
            _serializableComponentMapperProvider.GetMapperFor(serializableComponent3).Returns(_serializableComponentMapper);

            _serializableComponentMapper.MapFromSerializable(serializableComponent1).Returns(component1);
            _serializableComponentMapper.MapFromSerializable(serializableComponent2).Returns(component2);
            _serializableComponentMapper.MapFromSerializable(serializableComponent3).Returns(component3);

            // Act
            var actual = _serializableEntityMapper.MapFromSerializable(serializableEntity);

            // Assert
            Assert.That(actual.Components, Has.Count.EqualTo(3));
            Assert.That(actual.Components.ElementAt(0), Is.EqualTo(component1));
            Assert.That(actual.Components.ElementAt(1), Is.EqualTo(component2));
            Assert.That(actual.Components.ElementAt(2), Is.EqualTo(component3));
        }

        #endregion

        #region Helpers

        private static SerializableEntity GetSerializableEntity(params SerializableEntity[] children)
        {
            return new SerializableEntity
            {
                Children = children.ToList(),
                Components = new List<ISerializableComponent>()
            };
        }

        private class TestComponent : IComponent
        {
        }

        private class SerializableTestComponent : ISerializableComponent
        {
        }

        #endregion
    }
}