using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.SceneModel
{
    [TestFixture]
    public class EntityTests
    {
        private static Entity NewEntity => new Entity();

        [Test]
        public void Constructor_ShouldInstantiateEntityWithNoParent()
        {
            // Arrange
            // Act
            var entity = new Entity();

            // Assert
            Assert.That(entity.Parent, Is.Null);
        }

        [Test]
        public void Constructor_ShouldInstantiateEntityWithNoChildren()
        {
            // Arrange
            // Act
            var entity = new Entity();

            // Assert
            Assert.That(entity.Children, Is.Empty);
        }

        [Test]
        public void Constructor_ShouldInstantiateEntityWithNoComponents()
        {
            // Arrange
            // Act
            var entity = new Entity();

            // Assert
            Assert.That(entity.Components, Is.Empty);
        }

        [Test]
        public void Parent_ShouldBeCorrectlySet()
        {
            // Arrange
            var child = NewEntity;
            var parent = NewEntity;

            // Act
            child.Parent = parent;

            // Assert
            Assert.That(child.Parent, Is.EqualTo(parent));
        }

        [Test]
        public void Parent_ShouldAddThisEntityToChildrenOfNewParent_WhenSet()
        {
            // Arrange
            var child = NewEntity;
            var parent = NewEntity;

            // Act
            child.Parent = parent;

            // Assert
            Assert.That(parent.Children.Count, Is.EqualTo(1));
            Assert.That(parent.Children.Single(), Is.EqualTo(child));
        }

        [Test]
        public void Parent_ShouldRemoveThisEntityFromChildrenOfOldParent_WhenSetToNewParent()
        {
            // Arrange
            var child = NewEntity;
            var oldParent = NewEntity;
            var newParent = NewEntity;

            child.Parent = oldParent;

            // Act
            child.Parent = newParent;

            // Assert
            Assert.That(oldParent.Children, Is.Empty);
        }

        [Test]
        public void Parent_ShouldRemoveThisEntityFromChildrenOfParent_WhenSetToNull()
        {
            // Arrange
            var child = NewEntity;
            var parent = NewEntity;

            child.Parent = parent;

            // Act
            child.Parent = null;

            // Assert
            Assert.That(child.Parent, Is.Null);
        }

        [Test]
        public void Parent_ShouldSetSceneOnChild_WhenParentHasScene_AndParentSet()
        {
            // Arrange
            var scene = new Scene();
            var parent = NewEntity;
            var child = NewEntity;

            scene.AddEntity(parent);

            // Act
            child.Parent = parent;

            // Assert
            Assert.That(child.Scene, Is.EqualTo(scene));
        }

        [Test]
        public void Parent_ShouldUnsetSceneOnChild_WhenParentHasScene_AndParentSetToNull()
        {
            // Arrange
            var scene = new Scene();
            var parent = NewEntity;
            var child = NewEntity;

            scene.AddEntity(parent);
            child.Parent = parent;

            // Act
            child.Parent = null;

            // Assert
            Assert.That(child.Scene, Is.Null);
        }

        [Test]
        public void Scene_ShouldSetSceneOnChild_WhenChanged()
        {
            // Arrange
            var scene = new Scene();
            var parent = NewEntity;
            var child = NewEntity;

            child.Parent = parent;

            // Act
            scene.AddEntity(parent);

            // Assert
            Assert.That(child.Scene, Is.EqualTo(scene));
        }

        [Test]
        public void IsRoot_ReturnsFalse_WhenParentIsNotNull()
        {
            // Arrange
            var child = NewEntity;
            var parent = NewEntity;

            child.Parent = parent;

            // Act
            var isRoot = child.IsRoot;

            // Assert
            Assert.That(isRoot, Is.False);
        }

        [Test]
        public void IsRoot_ReturnsTrue_WhenParentIsNull()
        {
            // Arrange
            var root = NewEntity;
            root.Parent = null;

            // Act
            var isRoot = root.IsRoot;

            // Assert
            Assert.That(isRoot, Is.True);
        }

        [Test]
        public void Root_ShouldReturnEntityItself_WhenEntityIsRoot()
        {
            // Arrange
            var root = NewEntity;
            root.Parent = null;

            // Act
            var actual = root.Root;

            // Assert
            Assert.That(actual, Is.EqualTo(root));
        }

        [Test]
        public void Root_ShouldReturnRootEntityOfHierarchy_WhenEntityIsNotRoot()
        {
            // Arrange
            var hierarchy = new EntitiesHierarchy();

            // Act
            var root = hierarchy.Child111.Root;

            // Assert
            Assert.That(root, Is.EqualTo(hierarchy.Root));
        }

        [Test]
        public void AddComponent_ShouldAddComponentToEntity()
        {
            // Arrange
            var entity = NewEntity;
            var componentA = new ComponentA();

            // Act
            entity.AddComponent(componentA);

            // Assert
            Assert.That(entity.Components.Count, Is.EqualTo(1));
            Assert.That(entity.Components.Single(), Is.EqualTo(componentA));
        }

        [Test]
        public void RemoveComponent_ShouldRemoveComponentFromEntity()
        {
            // Arrange
            var entity = NewEntity;
            var componentA = new ComponentA();
            entity.AddComponent(componentA);

            // Act
            entity.RemoveComponent(componentA);

            // Assert
            Assert.That(entity.Components, Is.Empty);
        }

        [Test]
        public void GetComponent_ShouldReturnComponentByTypeFromEntity()
        {
            // Arrange
            var entity = NewEntity;
            var componentA = new ComponentA();
            entity.AddComponent(componentA);

            // Act
            var component = entity.GetComponent<ComponentA>();

            // Assert
            Assert.That(component, Is.EqualTo(componentA));
        }

        [Test]
        public void GetComponent_ShouldReturnOnly_ComponentA_WhenThereAreManyComponentsTypes()
        {
            // Arrange
            var entity = NewEntity;
            var componentA = new ComponentA();
            var componentB = new ComponentB();
            entity.AddComponent(componentA);
            entity.AddComponent(componentB);

            // Act
            var component = entity.GetComponent<ComponentA>();

            // Assert
            Assert.That(component, Is.EqualTo(componentA));
        }

        [Test]
        public void GetComponent_ShouldThrowException_WhenThereAreNoComponents()
        {
            // Arrange
            var entity = NewEntity;

            // Act
            // Assert
            Assert.That(() => entity.GetComponent<ComponentA>(), Throws.InvalidOperationException);
        }

        [Test]
        public void GetComponent_ShouldThrowException_WhenThereIsNoComponentOfRequestedType()
        {
            // Arrange
            var entity = NewEntity;
            entity.AddComponent(new ComponentB());

            // Act
            // Assert
            Assert.That(() => entity.GetComponent<ComponentA>(), Throws.InvalidOperationException);
        }

        [Test]
        public void GetComponent_ShouldThrowException_WhenThereAreMultipleComponentsOfTheSameType()
        {
            // Arrange
            var entity = NewEntity;
            entity.AddComponent(new ComponentA());
            entity.AddComponent(new ComponentA());

            // Act
            // Assert
            Assert.That(() => entity.GetComponent<ComponentA>(), Throws.InvalidOperationException);
        }

        [Test]
        public void GetComponents_ShouldReturnEmptyEnumerable_WhenThereAreNoComponents()
        {
            // Arrange
            var entity = NewEntity;
            entity.AddComponent(new ComponentB());
            entity.AddComponent(new ComponentB());

            // Act
            var actual = entity.GetComponents<ComponentA>();

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GetComponents_ShouldReturnEmptyEnumerable_WhenThereAreNoComponentsOfRequestedType()
        {
            // Arrange
            var entity = NewEntity;

            // Act
            var actual = entity.GetComponents<ComponentA>();

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GetComponents_ShouldReturnEnumerableWithOnly_ComponentA_WhenThereAreManyComponentsTypes()
        {
            // Arrange
            var entity = NewEntity;
            var componentA = new ComponentA();
            var componentB = new ComponentB();
            entity.AddComponent(componentA);
            entity.AddComponent(componentB);

            // Act
            var actual = entity.GetComponents<ComponentA>();

            // Assert
            var components = actual.ToList();
            Assert.That(components.Count, Is.EqualTo(1));
            Assert.That(components.Single(), Is.EqualTo(componentA));
        }

        [Test]
        public void GetComponents_ShouldReturnEnumerableWithAllComponents_WhenThereAreMultipleComponentsOfTheSameType()
        {
            // Arrange
            var entity = NewEntity;
            var component1 = new ComponentA();
            var component2 = new ComponentA();
            entity.AddComponent(component1);
            entity.AddComponent(component2);

            // Act
            var actual = entity.GetComponents<ComponentA>();

            // Assert
            var components = actual.ToList();
            Assert.That(components.Count, Is.EqualTo(2));
            CollectionAssert.AreEquivalent(new[] {component1, component2}, components);
        }

        [Test]
        public void HasComponent_ShouldReturnTrue_WhenAskedFor_ComponentA_and_ThereIs_ComponentA_InEntity()
        {
            // Arrange
            var entity = NewEntity;
            var componentA = new ComponentA();
            entity.AddComponent(componentA);

            // Act
            var actual = entity.HasComponent<ComponentA>();

            // Assert
            Assert.That(actual, Is.True);
        }

        [Test]
        public void HasComponent_ShouldReturnFalse_WhenAskedFor_ComponentA_and_ThereIsNo_ComponentA_InEntity()
        {
            // Arrange
            var entity = NewEntity;

            // Act
            var actual = entity.HasComponent<ComponentA>();

            // Assert
            Assert.That(actual, Is.False);
        }

        [Test]
        public void GetChildrenRecursively_ShouldReturnAllEntitiesInHierarchyExcludingRoot()
        {
            // Arrange
            var entitiesHierarchy = new EntitiesHierarchy();

            // Act
            var allChildren = entitiesHierarchy.Root.GetChildrenRecursively().ToList();

            // Assert
            CollectionAssert.IsNotEmpty(allChildren);
            CollectionAssert.AreEquivalent(
                new List<Entity>
                {
                    entitiesHierarchy.Child1,
                    entitiesHierarchy.Child2,
                    entitiesHierarchy.Child11,
                    entitiesHierarchy.Child12,
                    entitiesHierarchy.Child111
                }, allChildren);
        }

        [Test]
        public void GetChildrenRecursivelyIncludingRoot_ShouldReturnAllEntitiesInHierarchyIncludingRoot()
        {
            // Arrange
            var entitiesHierarchy = new EntitiesHierarchy();

            // Act
            var allChildren = entitiesHierarchy.Root.GetChildrenRecursivelyIncludingRoot().ToList();

            // Assert
            CollectionAssert.IsNotEmpty(allChildren);
            CollectionAssert.AreEquivalent(
                new List<Entity>
                {
                    entitiesHierarchy.Root,
                    entitiesHierarchy.Child1,
                    entitiesHierarchy.Child2,
                    entitiesHierarchy.Child11,
                    entitiesHierarchy.Child12,
                    entitiesHierarchy.Child111
                }, allChildren);
        }

        private class ComponentA : IComponent
        {
        }

        private class ComponentB : IComponent
        {
        }

        private class EntitiesHierarchy
        {
            public Entity Root { get; }
            public Entity Child1 { get; }
            public Entity Child2 { get; }
            public Entity Child11 { get; }
            public Entity Child12 { get; }
            public Entity Child111 { get; }

            public EntitiesHierarchy()
            {
                Root = new Entity {Name = nameof(Root)};
                Child1 = new Entity {Parent = Root, Name = nameof(Child1)};
                Child2 = new Entity {Parent = Root, Name = nameof(Child2)};
                Child11 = new Entity {Parent = Child1, Name = nameof(Child11)};
                Child12 = new Entity {Parent = Child1, Name = nameof(Child12)};
                Child111 = new Entity {Parent = Child11, Name = nameof(Child111)};
            }
        }
    }
}