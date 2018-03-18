using System.Linq;
using Geisha.Engine.Core.SceneModel;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.SceneModel
{
    [TestFixture]
    public class EntityDefinitionMapperTests
    {
        #region ToDefinition

        [Test]
        public void ToDefinition_ShouldReturnEntityDefinitionWithNoChildren_GivenEntityWithNoChildren()
        {
            // Arrange
            var entityDefinitionMapper = new EntityDefinitionMapper();
            var entity = new Entity();

            // Act
            var actual = entityDefinitionMapper.ToDefinition(entity);

            // Assert
            Assert.That(actual.Children, Has.Count.Zero);
        }

        [Test]
        public void ToDefinition_ShouldReturnEntityDefinitionWithChildren_GivenEntityWithChildren()
        {
            // Arrange
            var entityDefinitionMapper = new EntityDefinitionMapper();
            var entity = new Entity();
            entity.AddChild(new Entity());
            entity.AddChild(new Entity());
            entity.AddChild(new Entity());

            // Act
            var actual = entityDefinitionMapper.ToDefinition(entity);

            // Assert
            Assert.That(actual.Children, Has.Count.EqualTo(3));
        }

        [Test]
        public void ToDefinition_ShouldReturnEntityDefinitionGraph_GivenEntityGraph()
        {
            // Arrange
            var entityDefinitionMapper = new EntityDefinitionMapper();
            var root = new Entity();
            var child1 = new Entity {Parent = root};
            var child2 = new Entity {Parent = root};
            var child3 = new Entity {Parent = root};

            child1.AddChild(new Entity());
            child1.AddChild(new Entity());
            child2.AddChild(new Entity());

            // Act
            var actual = entityDefinitionMapper.ToDefinition(root);

            // Assert
            Assert.That(actual.Children, Has.Count.EqualTo(3));
            Assert.That(actual.Children.ElementAt(0).Children, Has.Count.EqualTo(2));
            Assert.That(actual.Children.ElementAt(1).Children, Has.Count.EqualTo(1));
            Assert.That(actual.Children.ElementAt(2).Children, Has.Count.Zero);
        }

        #endregion

        #region FromDefinition

        [Test]
        public void FromDefinition_ShouldReturnEntityWithNoChildren_GivenEntityDefinitionWithNoChildren()
        {
            // Arrange
            var entityDefinitionMapper = new EntityDefinitionMapper();
            var entityDefinition = GetEntityDefinitionWithChildren();

            // Act
            var actual = entityDefinitionMapper.FromDefinition(entityDefinition);

            // Assert
            Assert.That(actual.Children, Has.Count.Zero);
        }

        [Test]
        public void FromDefinition_ShouldReturnEntityWithChildren_GivenEntityDefinitionWithChildren()
        {
            // Arrange
            var entityDefinitionMapper = new EntityDefinitionMapper();
            var entityDefinition = GetEntityDefinitionWithChildren(
                GetEntityDefinitionWithChildren(),
                GetEntityDefinitionWithChildren(),
                GetEntityDefinitionWithChildren()
            );

            // Act
            var actual = entityDefinitionMapper.FromDefinition(entityDefinition);

            // Assert
            Assert.That(actual.Children, Has.Count.EqualTo(3));
        }

        [Test]
        public void FromDefinition_ShouldReturnEntityGraph_GivenEntityDefinitionGraph()
        {
            // Arrange
            var entityDefinitionMapper = new EntityDefinitionMapper();
            var entityDefinition = GetEntityDefinitionWithChildren(
                GetEntityDefinitionWithChildren(
                    GetEntityDefinitionWithChildren(),
                    GetEntityDefinitionWithChildren()
                ),
                GetEntityDefinitionWithChildren(
                    GetEntityDefinitionWithChildren()
                ),
                GetEntityDefinitionWithChildren()
            );

            // Act
            var actual = entityDefinitionMapper.FromDefinition(entityDefinition);

            // Assert
            Assert.That(actual.Children, Has.Count.EqualTo(3));
            Assert.That(actual.Children.ElementAt(0).Children, Has.Count.EqualTo(2));
            Assert.That(actual.Children.ElementAt(1).Children, Has.Count.EqualTo(1));
            Assert.That(actual.Children.ElementAt(2).Children, Has.Count.EqualTo(0));
        }

        #endregion

        #region Helpers

        private EntityDefinition GetEntityDefinitionWithChildren(params EntityDefinition[] entityDefinitions)
        {
            return new EntityDefinition
            {
                Children = entityDefinitions.ToList()
            };
        }

        #endregion
    }
}