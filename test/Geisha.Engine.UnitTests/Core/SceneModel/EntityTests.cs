﻿using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel
{
    [TestFixture]
    public class EntityTests
    {
        private Scene Scene { get; set; } = null!;

        [SetUp]
        public void SetUp()
        {
            Scene = TestSceneFactory.Create(new IComponentFactory[] { new ComponentAFactory(), new ComponentBFactory() });
        }

        #region Parent

        [Test]
        public void Parent_ShouldThrowException_WhenSetToEntityItself()
        {
            // Arrange
            var entity = Scene.CreateEntity();

            // Act
            // Assert
            Assert.That(() => entity.Parent = entity, Throws.ArgumentException);
        }

        [Test]
        public void Parent_ShouldThrowException_WhenSetToEntityCreatedByAnotherScene()
        {
            // Arrange
            var scene1 = TestSceneFactory.Create();
            var scene2 = TestSceneFactory.Create();

            var entityInScene1 = scene1.CreateEntity();
            var entityInScene2 = scene2.CreateEntity();

            // Act
            // Assert
            Assert.That(() => entityInScene1.Parent = entityInScene2, Throws.ArgumentException);
        }

        [Test]
        public void Parent_ShouldThrowException_WhenSetOnEntityRemovedFromTheScene()
        {
            // Arrange
            var child = Scene.CreateEntity();
            var parent = Scene.CreateEntity();

            Scene.RemoveEntity(child);

            // Act
            // Assert
            Assert.That(() => child.Parent = parent, Throws.InvalidOperationException);
        }

        [Test]
        public void Parent_ShouldBeCorrectlySet()
        {
            // Arrange
            var child = Scene.CreateEntity();
            var parent = Scene.CreateEntity();

            // Act
            child.Parent = parent;

            // Assert
            Assert.That(child.Parent, Is.EqualTo(parent));
        }

        [Test]
        public void Parent_ShouldAddThisEntityToChildrenOfNewParent_WhenSet()
        {
            // Arrange
            var child = Scene.CreateEntity();
            var parent = Scene.CreateEntity();

            // Act
            child.Parent = parent;

            // Assert
            Assert.That(parent.Children.Single(), Is.EqualTo(child));
        }

        [Test]
        public void Parent_ShouldRemoveThisEntityFromChildrenOfOldParent_WhenSetToNewParent()
        {
            // Arrange
            var child = Scene.CreateEntity();
            var oldParent = Scene.CreateEntity();
            var newParent = Scene.CreateEntity();

            child.Parent = oldParent;

            // Act
            child.Parent = newParent;

            // Assert
            Assert.That(oldParent.Children, Is.Empty);
        }

        [Test]
        public void Parent_ShouldRemoveThisEntityFromSceneRootEntities_WhenSet()
        {
            // Arrange
            var child = Scene.CreateEntity();
            var parent = Scene.CreateEntity();

            // Act
            child.Parent = parent;

            // Assert
            Assert.That(Scene.RootEntities, Does.Not.Contain(child));
        }

        [Test]
        public void Parent_ShouldRemoveThisEntityFromChildrenOfParent_WhenSetToNull()
        {
            // Arrange
            var child = Scene.CreateEntity();
            var parent = Scene.CreateEntity();

            child.Parent = parent;

            // Act
            child.Parent = null;

            // Assert
            Assert.That(child.Parent, Is.Null);
        }

        [Test]
        public void Parent_ShouldAddThisEntityToSceneRootEntities_WhenSetToNull()
        {
            // Arrange
            var child = Scene.CreateEntity();
            var parent = Scene.CreateEntity();

            child.Parent = parent;

            // Act
            child.Parent = null;

            // Assert
            Assert.That(Scene.RootEntities, Contains.Item(child));
        }

        [Test]
        public void Parent_ShouldCall_OnEntityParentChanged_WhenRootEntityBecomesChild()
        {
            // Arrange
            var observer1 = Substitute.For<ISceneObserver>();
            var observer2 = Substitute.For<ISceneObserver>();

            Scene.AddObserver(observer1);
            Scene.AddObserver(observer2);

            var root = Scene.CreateEntity();
            var child = Scene.CreateEntity();

            observer1.ClearReceivedCalls();
            observer2.ClearReceivedCalls();

            // Act
            child.Parent = root;

            // Assert
            observer1.Received(1).OnEntityParentChanged(child, null, root);
            observer2.Received(1).OnEntityParentChanged(child, null, root);
        }

        [Test]
        public void Parent_ShouldCall_OnEntityParentChanged_WhenChildEntityBecomesRoot()
        {
            // Arrange
            var observer1 = Substitute.For<ISceneObserver>();
            var observer2 = Substitute.For<ISceneObserver>();

            Scene.AddObserver(observer1);
            Scene.AddObserver(observer2);

            var root = Scene.CreateEntity();
            var child = Scene.CreateEntity();
            child.Parent = root;

            observer1.ClearReceivedCalls();
            observer2.ClearReceivedCalls();

            // Act
            child.Parent = null;

            // Assert
            observer1.Received(1).OnEntityParentChanged(child, root, null);
            observer2.Received(1).OnEntityParentChanged(child, root, null);
        }

        [Test]
        public void Parent_ShouldCall_OnEntityParentChanged_WhenChildEntityBecomesChildOfNewParent()
        {
            // Arrange
            var observer1 = Substitute.For<ISceneObserver>();
            var observer2 = Substitute.For<ISceneObserver>();

            Scene.AddObserver(observer1);
            Scene.AddObserver(observer2);

            var oldParent = Scene.CreateEntity();
            var newParent = Scene.CreateEntity();
            var child = Scene.CreateEntity();
            child.Parent = oldParent;

            observer1.ClearReceivedCalls();
            observer2.ClearReceivedCalls();

            // Act
            child.Parent = newParent;

            // Assert
            observer1.Received(1).OnEntityParentChanged(child, oldParent, newParent);
            observer2.Received(1).OnEntityParentChanged(child, oldParent, newParent);
        }

        #endregion

        #region IsRoot

        [Test]
        public void IsRoot_ReturnsFalse_WhenParentIsNotNull()
        {
            // Arrange
            var child = Scene.CreateEntity();
            var parent = Scene.CreateEntity();

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
            var root = Scene.CreateEntity();
            root.Parent = null;

            // Act
            var isRoot = root.IsRoot;

            // Assert
            Assert.That(isRoot, Is.True);
        }

        #endregion

        #region Root

        [Test]
        public void Root_ShouldReturnEntityItself_WhenEntityIsRoot()
        {
            // Arrange
            var root = Scene.CreateEntity();
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
            var hierarchy = new EntitiesHierarchy(Scene);

            // Act
            var root = hierarchy.Child111.Root;

            // Assert
            Assert.That(root, Is.EqualTo(hierarchy.Root));
        }

        #endregion

        #region CreateChildEntity

        [Test]
        public void CreateChildEntity_ShouldThrowException_WhenUsedOnEntityRemovedFromTheScene()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            Scene.RemoveEntity(entity);

            // Act
            // Assert
            Assert.That(() => entity.CreateChildEntity(), Throws.InvalidOperationException);
        }

        [Test]
        public void CreateChildEntity_ShouldSetParentOfChildEntity()
        {
            // Arrange
            var parent = Scene.CreateEntity();

            // Act
            var child = parent.CreateChildEntity();

            // Assert
            Assert.That(child.Parent, Is.EqualTo(parent));
        }

        [Test]
        public void CreateChildEntity_ShouldCreateEntityAsChildOfAnotherEntity()
        {
            // Arrange
            var parent = Scene.CreateEntity();

            // Act
            var child = parent.CreateChildEntity();

            // Assert
            Assert.That(parent.Children.Single(), Is.EqualTo(child));
        }

        [Test]
        public void CreateChildEntity_ShouldSetSceneOfChildEntityTheSameAsOfParent()
        {
            // Arrange
            var parent = Scene.CreateEntity();

            // Act
            var child = parent.CreateChildEntity();

            // Assert
            Assert.That(child.Scene, Is.EqualTo(parent.Scene));
        }

        [Test]
        public void CreateChildEntity_ShouldAddNewEntityToSceneAllEntities()
        {
            // Arrange
            var parent = Scene.CreateEntity();

            // Act
            var child = parent.CreateChildEntity();

            // Assert
            Assert.That(Scene.AllEntities, Contains.Item(child));
        }

        [Test]
        public void CreateChildEntity_ShouldNotAddNewEntityToSceneRootEntities()
        {
            // Arrange
            var parent = Scene.CreateEntity();

            // Act
            var child = parent.CreateChildEntity();

            // Assert
            Assert.That(Scene.RootEntities, Does.Not.Contain(child));
        }

        [Test]
        public void CreateChildEntity_ShouldCall_OnEntityCreated_And_OnEntityParentChanged()
        {
            // Arrange
            var observer1 = Substitute.For<ISceneObserver>();
            var observer2 = Substitute.For<ISceneObserver>();

            Scene.AddObserver(observer1);
            Scene.AddObserver(observer2);

            var root = Scene.CreateEntity();

            observer1.ClearReceivedCalls();
            observer2.ClearReceivedCalls();

            // Act
            var child = root.CreateChildEntity();

            // Assert
            Received.InOrder(() =>
            {
                observer1.Received().OnEntityCreated(child);
                observer2.Received().OnEntityCreated(child);

                observer1.Received().OnEntityParentChanged(child, null, root);
                observer2.Received().OnEntityParentChanged(child, null, root);
            });
        }

        #endregion

        #region GetChildrenRecursively

        [Test]
        public void GetChildrenRecursively_ShouldReturnAllEntitiesInHierarchyExcludingRoot()
        {
            // Arrange
            var entitiesHierarchy = new EntitiesHierarchy(Scene);

            // Act
            var allChildren = entitiesHierarchy.Root.GetChildrenRecursively();

            // Assert
            Assert.That(allChildren, Is.EquivalentTo(new[]
            {
                entitiesHierarchy.Child1,
                entitiesHierarchy.Child2,
                entitiesHierarchy.Child11,
                entitiesHierarchy.Child12,
                entitiesHierarchy.Child111
            }));
        }

        #endregion

        #region GetChildrenRecursivelyIncludingRoot

        [Test]
        public void GetChildrenRecursivelyIncludingRoot_ShouldReturnAllEntitiesInHierarchyIncludingRoot()
        {
            // Arrange
            var entitiesHierarchy = new EntitiesHierarchy(Scene);

            // Act
            var allChildren = entitiesHierarchy.Root.GetChildrenRecursivelyIncludingRoot();

            // Assert
            Assert.That(allChildren, Is.EquivalentTo(new[]
            {
                entitiesHierarchy.Root,
                entitiesHierarchy.Child1,
                entitiesHierarchy.Child2,
                entitiesHierarchy.Child11,
                entitiesHierarchy.Child12,
                entitiesHierarchy.Child111
            }));
        }

        #endregion

        #region CreateComponent

        [Test]
        public void CreateComponent_Generic_ShouldThrowException_WhenUsedOnEntityRemovedFromTheScene()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            Scene.RemoveEntity(entity);

            // Act
            // Assert
            Assert.That(() => entity.CreateComponent<ComponentA>(), Throws.InvalidOperationException);
        }

        [Test]
        public void CreateComponent_ComponentId_ShouldThrowException_WhenUsedOnEntityRemovedFromTheScene()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            Scene.RemoveEntity(entity);

            // Act
            // Assert
            Assert.That(() => entity.CreateComponent(ComponentId.Of<ComponentA>()), Throws.InvalidOperationException);
        }

        [Test]
        public void CreateComponent_Generic_ShouldAddComponentToEntity()
        {
            // Arrange
            var entity = Scene.CreateEntity();

            // Act
            var componentA = entity.CreateComponent<ComponentA>();

            // Assert
            Assert.That(entity.Components.Count, Is.EqualTo(1));
            Assert.That(entity.Components.Single(), Is.EqualTo(componentA));
            Assert.That(entity.HasComponent<ComponentA>(), Is.True);
        }

        [Test]
        public void CreateComponent_ComponentId_ShouldAddComponentToEntity()
        {
            // Arrange
            var entity = Scene.CreateEntity();

            // Act
            var componentA = entity.CreateComponent(ComponentId.Of<ComponentA>());

            // Assert
            Assert.That(entity.Components.Count, Is.EqualTo(1));
            Assert.That(entity.Components.Single(), Is.EqualTo(componentA));
            Assert.That(entity.HasComponent<ComponentA>(), Is.True);
        }

        [Test]
        public void CreateComponent_Generic_ShouldCreateComponentWithEntitySet()
        {
            // Arrange
            var entity = Scene.CreateEntity();

            // Act
            var componentA = entity.CreateComponent<ComponentA>();

            // Assert
            Assert.That(componentA.Entity, Is.EqualTo(entity));
            Assert.That(componentA.Scene, Is.EqualTo(Scene));
        }

        [Test]
        public void CreateComponent_ComponentId_ShouldCreateComponentWithEntitySet()
        {
            // Arrange
            var entity = Scene.CreateEntity();

            // Act
            var componentA = entity.CreateComponent(ComponentId.Of<ComponentA>());

            // Assert
            Assert.That(componentA.Entity, Is.EqualTo(entity));
            Assert.That(componentA.Scene, Is.EqualTo(Scene));
        }

        [Test]
        public void CreateComponent_Generic_ShouldCall_OnComponentCreated()
        {
            // Arrange
            var observer1 = Substitute.For<ISceneObserver>();
            var observer2 = Substitute.For<ISceneObserver>();

            Scene.AddObserver(observer1);
            Scene.AddObserver(observer2);

            var entity = Scene.CreateEntity();

            observer1.ClearReceivedCalls();
            observer2.ClearReceivedCalls();

            // Act
            var componentA = entity.CreateComponent<ComponentA>();

            // Assert
            observer1.Received(1).OnComponentCreated(componentA);
            observer2.Received(1).OnComponentCreated(componentA);
        }

        [Test]
        public void CreateComponent_ComponentId_ShouldCall_OnComponentCreated()
        {
            // Arrange
            var observer1 = Substitute.For<ISceneObserver>();
            var observer2 = Substitute.For<ISceneObserver>();

            Scene.AddObserver(observer1);
            Scene.AddObserver(observer2);

            var entity = Scene.CreateEntity();

            observer1.ClearReceivedCalls();
            observer2.ClearReceivedCalls();

            // Act
            var componentA = entity.CreateComponent(ComponentId.Of<ComponentA>());

            // Assert
            observer1.Received(1).OnComponentCreated(componentA);
            observer2.Received(1).OnComponentCreated(componentA);
        }

        #endregion

        #region RemoveComponent

        [Test]
        public void RemoveComponent_ShouldThrowException_WhenUsedOnEntityRemovedFromTheScene()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var componentA = entity.CreateComponent<ComponentA>();
            Scene.RemoveEntity(entity);

            // Act
            // Assert
            Assert.That(() => entity.RemoveComponent(componentA), Throws.InvalidOperationException);
        }

        [Test]
        public void RemoveComponent_ShouldRemoveComponentFromEntity()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var componentA = entity.CreateComponent<ComponentA>();

            // Act
            entity.RemoveComponent(componentA);

            // Assert
            Assert.That(entity.Components, Is.Empty);
            Assert.That(entity.HasComponent<ComponentA>(), Is.False);
        }

        [Test]
        public void RemoveComponent_ShouldCall_OnComponentRemoved()
        {
            // Arrange
            var observer1 = Substitute.For<ISceneObserver>();
            var observer2 = Substitute.For<ISceneObserver>();

            Scene.AddObserver(observer1);
            Scene.AddObserver(observer2);

            var entity = Scene.CreateEntity();

            var componentA = entity.CreateComponent<ComponentA>();

            observer1.ClearReceivedCalls();
            observer2.ClearReceivedCalls();

            // Act
            entity.RemoveComponent(componentA);

            // Assert
            observer1.Received(1).OnComponentRemoved(componentA);
            observer2.Received(1).OnComponentRemoved(componentA);
        }

        #endregion

        #region GetComponent

        [Test]
        public void GetComponent_ShouldReturnComponentByTypeFromEntity()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var componentA = entity.CreateComponent<ComponentA>();

            // Act
            var component = entity.GetComponent<ComponentA>();

            // Assert
            Assert.That(component, Is.EqualTo(componentA));
        }

        [Test]
        public void GetComponent_ShouldReturnOnly_ComponentA_WhenThereAreManyComponentTypes()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var componentA = entity.CreateComponent<ComponentA>();
            entity.CreateComponent<ComponentB>();

            // Act
            var component = entity.GetComponent<ComponentA>();

            // Assert
            Assert.That(component, Is.EqualTo(componentA));
        }

        [Test]
        public void GetComponent_ShouldThrowException_WhenThereAreNoComponents()
        {
            // Arrange
            var entity = Scene.CreateEntity();

            // Act
            // Assert
            Assert.That(() => entity.GetComponent<ComponentA>(), Throws.InvalidOperationException);
        }

        [Test]
        public void GetComponent_ShouldThrowException_WhenThereIsNoComponentOfRequestedType()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            entity.CreateComponent<ComponentB>();

            // Act
            // Assert
            Assert.That(() => entity.GetComponent<ComponentA>(), Throws.InvalidOperationException);
        }

        [Test]
        public void GetComponent_ShouldThrowException_WhenThereAreMultipleComponentsOfTheSameType()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            entity.CreateComponent<ComponentA>();
            entity.CreateComponent<ComponentA>();

            // Act
            // Assert
            Assert.That(() => entity.GetComponent<ComponentA>(), Throws.InvalidOperationException);
        }

        [Test]
        public void GetComponent_ShouldThrowException_WhenComponentOfRequestedTypeWasAddedAndRemoved()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var component = entity.CreateComponent<ComponentA>();
            entity.RemoveComponent(component);

            // Act
            // Assert
            Assert.That(() => entity.GetComponent<ComponentA>(), Throws.InvalidOperationException);
        }

        #endregion

        #region GetComponents

        [Test]
        public void GetComponents_ShouldReturnEmptyEnumerable_WhenThereAreNoComponents()
        {
            // Arrange
            var entity = Scene.CreateEntity();

            // Act
            var actual = entity.GetComponents<ComponentA>();

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GetComponents_ShouldReturnEmptyEnumerable_WhenThereAreNoComponentsOfRequestedType()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            entity.CreateComponent<ComponentB>();
            entity.CreateComponent<ComponentB>();

            // Act
            var actual = entity.GetComponents<ComponentA>();

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GetComponents_ShouldReturnEmptyEnumerable_WhenComponentOfRequestedTypeWasAddedAndRemoved()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var component = entity.CreateComponent<ComponentA>();
            entity.RemoveComponent(component);

            // Act
            var actual = entity.GetComponents<ComponentA>();

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GetComponents_ShouldReturnEnumerableWithOnly_ComponentA_WhenThereAreManyComponentTypes()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var componentA = entity.CreateComponent<ComponentA>();
            entity.CreateComponent<ComponentB>();

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
            var entity = Scene.CreateEntity();
            var component1 = entity.CreateComponent<ComponentA>();
            var component2 = entity.CreateComponent<ComponentA>();

            // Act
            var actual = entity.GetComponents<ComponentA>();

            // Assert
            var components = actual.ToList();
            Assert.That(components.Count, Is.EqualTo(2));
            CollectionAssert.AreEquivalent(new[] { component1, component2 }, components);
        }

        #endregion

        #region HasComponent

        [Test]
        public void HasComponent_ShouldReturnTrue_WhenAskedFor_ComponentA_and_ThereIs_ComponentA_InEntity()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            entity.CreateComponent<ComponentA>();

            // Act
            var actual = entity.HasComponent<ComponentA>();

            // Assert
            Assert.That(actual, Is.True);
        }

        [Test]
        public void HasComponent_ShouldReturnFalse_WhenAskedFor_ComponentA_and_ThereIsNo_Components_InEntity()
        {
            // Arrange
            var entity = Scene.CreateEntity();

            // Act
            var actual = entity.HasComponent<ComponentA>();

            // Assert
            Assert.That(actual, Is.False);
        }

        [Test]
        public void HasComponent_ShouldReturnFalse_WhenAskedFor_ComponentA_and_ThereIs_ComponentB_InEntity()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            entity.CreateComponent<ComponentB>();

            // Act
            var actual = entity.HasComponent<ComponentA>();

            // Assert
            Assert.That(actual, Is.False);
        }

        [Test]
        public void HasComponent_ShouldReturnFalse_WhenAskedFor_ComponentA_and_ComponentA_WasAddedAndRemovedFromEntity()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var component = entity.CreateComponent<ComponentA>();
            entity.RemoveComponent(component);

            // Act
            var actual = entity.HasComponent<ComponentA>();

            // Assert
            Assert.That(actual, Is.False);
        }

        #endregion

        #region Remove

        [Test]
        public void RemoveAfterFixedTimeStep_ShouldThrowException_WhenUsedOnEntityRemovedFromTheScene()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            Scene.RemoveEntity(entity);

            // Act
            // Assert
            Assert.That(() => entity.RemoveAfterFixedTimeStep(), Throws.InvalidOperationException);
        }

        [Test]
        public void RemoveAfterFullFrame_ShouldThrowException_WhenUsedOnEntityRemovedFromTheScene()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            Scene.RemoveEntity(entity);

            // Act
            // Assert
            Assert.That(() => entity.RemoveAfterFullFrame(), Throws.InvalidOperationException);
        }

        #endregion

        #region Helpers

        private sealed class ComponentA : Component
        {
            public ComponentA(Entity entity) : base(entity)
            {
            }
        }

        private sealed class ComponentAFactory : ComponentFactory<ComponentA>
        {
            protected override ComponentA CreateComponent(Entity entity) => new ComponentA(entity);
        }

        private sealed class ComponentB : Component
        {
            public ComponentB(Entity entity) : base(entity)
            {
            }
        }

        private sealed class ComponentBFactory : ComponentFactory<ComponentB>
        {
            protected override ComponentB CreateComponent(Entity entity) => new ComponentB(entity);
        }

        private sealed class EntitiesHierarchy
        {
            public Entity Root { get; }
            public Entity Child1 { get; }
            public Entity Child2 { get; }
            public Entity Child11 { get; }
            public Entity Child12 { get; }
            public Entity Child111 { get; }

            public EntitiesHierarchy(Scene scene)
            {
                Root = scene.CreateEntity();
                Root.Name = nameof(Root);

                Child1 = Root.CreateChildEntity();
                Child1.Name = nameof(Child1);

                Child2 = Root.CreateChildEntity();
                Child2.Name = nameof(Child2);

                Child11 = Child1.CreateChildEntity();
                Child11.Name = nameof(Child11);

                Child12 = Child1.CreateChildEntity();
                Child12.Name = nameof(Child12);

                Child111 = Child11.CreateChildEntity();
                Child111.Name = nameof(Child111);
            }
        }

        #endregion
    }
}