using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.SceneModel;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.SceneModel
{
    [TestFixture]
    public class EntityDefinitionMapperTests
    {
        private IComponentDefinitionMapper _componentDefinitionMapper;
        private IComponentDefinitionMapperProvider _componentDefinitionMapperProvider;
        private EntityDefinitionMapper _entityDefinitionMapper;

        [SetUp]
        public void SetUp()
        {
            _componentDefinitionMapper = Substitute.For<IComponentDefinitionMapper>();
            _componentDefinitionMapperProvider = Substitute.For<IComponentDefinitionMapperProvider>();
            _entityDefinitionMapper = new EntityDefinitionMapper(_componentDefinitionMapperProvider);
        }

        #region ToDefinition

        [Test]
        public void ToDefinition_ShouldReturnEntityDefinitionWithNoChildren_GivenEntityWithNoChildren()
        {
            // Arrange
            var entity = new Entity();

            // Act
            var actual = _entityDefinitionMapper.ToDefinition(entity);

            // Assert
            Assert.That(actual.Children, Has.Count.Zero);
        }

        [Test]
        public void ToDefinition_ShouldReturnEntityDefinitionWithChildren_GivenEntityWithChildren()
        {
            // Arrange
            var entity = new Entity();
            entity.AddChild(new Entity());
            entity.AddChild(new Entity());
            entity.AddChild(new Entity());

            // Act
            var actual = _entityDefinitionMapper.ToDefinition(entity);

            // Assert
            Assert.That(actual.Children, Has.Count.EqualTo(3));
        }

        [Test]
        public void ToDefinition_ShouldReturnEntityDefinitionGraph_GivenEntityGraph()
        {
            // Arrange
            var root = new Entity();
            var child1 = new Entity {Parent = root};
            var child2 = new Entity {Parent = root};
            var child3 = new Entity {Parent = root};

            child1.AddChild(new Entity());
            child1.AddChild(new Entity());
            child2.AddChild(new Entity());

            // Act
            var actual = _entityDefinitionMapper.ToDefinition(root);

            // Assert
            Assert.That(actual.Children, Has.Count.EqualTo(3));
            Assert.That(actual.Children.ElementAt(0).Children, Has.Count.EqualTo(2));
            Assert.That(actual.Children.ElementAt(1).Children, Has.Count.EqualTo(1));
            Assert.That(actual.Children.ElementAt(2).Children, Has.Count.Zero);
        }

        [Test]
        public void ToDefinition_ShouldReturnEntityDefinitionWithName_GivenEntityWithName()
        {
            // Arrange
            var entity = new Entity
            {
                Name = "Some entity"
            };

            // Act
            var actual = _entityDefinitionMapper.ToDefinition(entity);

            // Assert
            Assert.That(actual.Name, Is.EqualTo(entity.Name));
        }

        [Test]
        public void ToDefinition_ShouldReturnEntityDefinitionWithNoComponents_GivenEntityWithNoComponents()
        {
            // Arrange
            var entity = new Entity();

            // Act
            var actual = _entityDefinitionMapper.ToDefinition(entity);

            // Assert
            Assert.That(actual.Components, Has.Count.Zero);
        }

        [Test]
        public void ToDefinition_ShouldReturnEntityDefinitionWithComponents_GivenEntityWithComponents()
        {
            // Arrange
            var entity = new Entity();

            var component1 = new TestComponent();
            var component2 = new TestComponent();
            var component3 = new TestComponent();
            entity.AddComponent(component1);
            entity.AddComponent(component2);
            entity.AddComponent(component3);

            var componentDefinition1 = new TestComponentDefinition();
            var componentDefinition2 = new TestComponentDefinition();
            var componentDefinition3 = new TestComponentDefinition();

            _componentDefinitionMapperProvider.GetMapperFor(component1).Returns(_componentDefinitionMapper);
            _componentDefinitionMapperProvider.GetMapperFor(component2).Returns(_componentDefinitionMapper);
            _componentDefinitionMapperProvider.GetMapperFor(component3).Returns(_componentDefinitionMapper);

            _componentDefinitionMapper.ToDefinition(component1).Returns(componentDefinition1);
            _componentDefinitionMapper.ToDefinition(component2).Returns(componentDefinition2);
            _componentDefinitionMapper.ToDefinition(component3).Returns(componentDefinition3);

            // Act
            var actual = _entityDefinitionMapper.ToDefinition(entity);

            // Assert
            Assert.That(actual.Components, Has.Count.EqualTo(3));
            Assert.That(actual.Components.ElementAt(0), Is.EqualTo(componentDefinition1));
            Assert.That(actual.Components.ElementAt(1), Is.EqualTo(componentDefinition2));
            Assert.That(actual.Components.ElementAt(2), Is.EqualTo(componentDefinition3));
        }

        #endregion

        #region FromDefinition

        [Test]
        public void FromDefinition_ShouldReturnEntityWithNoChildren_GivenEntityDefinitionWithNoChildren()
        {
            // Arrange
            var entityDefinition = GetEntityDefinition();

            // Act
            var actual = _entityDefinitionMapper.FromDefinition(entityDefinition);

            // Assert
            Assert.That(actual.Children, Has.Count.Zero);
        }

        [Test]
        public void FromDefinition_ShouldReturnEntityWithChildren_GivenEntityDefinitionWithChildren()
        {
            // Arrange
            var entityDefinition = GetEntityDefinition(
                GetEntityDefinition(),
                GetEntityDefinition(),
                GetEntityDefinition()
            );

            // Act
            var actual = _entityDefinitionMapper.FromDefinition(entityDefinition);

            // Assert
            Assert.That(actual.Children, Has.Count.EqualTo(3));
        }

        [Test]
        public void FromDefinition_ShouldReturnEntityGraph_GivenEntityDefinitionGraph()
        {
            // Arrange
            var entityDefinition = GetEntityDefinition(
                GetEntityDefinition(
                    GetEntityDefinition(),
                    GetEntityDefinition()
                ),
                GetEntityDefinition(
                    GetEntityDefinition()
                ),
                GetEntityDefinition()
            );

            // Act
            var actual = _entityDefinitionMapper.FromDefinition(entityDefinition);

            // Assert
            Assert.That(actual.Children, Has.Count.EqualTo(3));
            Assert.That(actual.Children.ElementAt(0).Children, Has.Count.EqualTo(2));
            Assert.That(actual.Children.ElementAt(1).Children, Has.Count.EqualTo(1));
            Assert.That(actual.Children.ElementAt(2).Children, Has.Count.EqualTo(0));
        }

        [Test]
        public void FromDefinition_ShouldReturnEntityWithName_GivenEntityDefinitionWithName()
        {
            // Arrange
            var entityDefinition = GetEntityDefinition();
            entityDefinition.Name = "Some entity";

            // Act
            var actual = _entityDefinitionMapper.FromDefinition(entityDefinition);

            // Assert
            Assert.That(actual.Name, Is.EqualTo(entityDefinition.Name));
        }

        [Test]
        public void FromDefinition_ShouldReturnEntityWithNoComponents_GivenEntityDefinitionWithNoComponents()
        {
            // Arrange
            var entityDefinition = GetEntityDefinition();

            // Act
            var actual = _entityDefinitionMapper.FromDefinition(entityDefinition);

            // Assert
            Assert.That(actual.Components, Has.Count.Zero);
        }

        [Test]
        public void FromDefinition_ShouldReturnEntityWithComponents_GivenEntityDefinitionWithComponents()
        {
            // Arrange
            var entityDefinition = GetEntityDefinition();

            var componentDefinition1 = new TestComponentDefinition();
            var componentDefinition2 = new TestComponentDefinition();
            var componentDefinition3 = new TestComponentDefinition();

            entityDefinition.Components = new List<IComponentDefinition>
            {
                componentDefinition1,
                componentDefinition2,
                componentDefinition3
            };

            var component1 = new TestComponent();
            var component2 = new TestComponent();
            var component3 = new TestComponent();

            _componentDefinitionMapperProvider.GetMapperFor(componentDefinition1).Returns(_componentDefinitionMapper);
            _componentDefinitionMapperProvider.GetMapperFor(componentDefinition2).Returns(_componentDefinitionMapper);
            _componentDefinitionMapperProvider.GetMapperFor(componentDefinition3).Returns(_componentDefinitionMapper);

            _componentDefinitionMapper.FromDefinition(componentDefinition1).Returns(component1);
            _componentDefinitionMapper.FromDefinition(componentDefinition2).Returns(component2);
            _componentDefinitionMapper.FromDefinition(componentDefinition3).Returns(component3);

            // Act
            var actual = _entityDefinitionMapper.FromDefinition(entityDefinition);

            // Assert
            Assert.That(actual.Components, Has.Count.EqualTo(3));
            Assert.That(actual.Components.ElementAt(0), Is.EqualTo(component1));
            Assert.That(actual.Components.ElementAt(1), Is.EqualTo(component2));
            Assert.That(actual.Components.ElementAt(2), Is.EqualTo(component3));
        }

        #endregion

        #region Helpers

        private EntityDefinition GetEntityDefinition(params EntityDefinition[] children)
        {
            return new EntityDefinition
            {
                Children = children.ToList(),
                Components = new List<IComponentDefinition>()
            };
        }

        private class TestComponent : IComponent
        {
        }

        private class TestComponentDefinition : IComponentDefinition
        {
        }

        #endregion
    }
}