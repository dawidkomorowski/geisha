using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
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
            Scene = TestSceneFactory.Create();
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

        #region AddComponent

        [Test]
        public void AddComponent_ShouldAddComponentToEntity()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var componentA = new ComponentA();

            // Act
            entity.AddComponent(componentA);

            // Assert
            Assert.That(entity.Components.Count, Is.EqualTo(1));
            Assert.That(entity.Components.Single(), Is.EqualTo(componentA));
        }

        #endregion

        #region RemoveComponent

        [Test]
        public void RemoveComponent_ShouldRemoveComponentFromEntity()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            var componentA = new ComponentA();
            entity.AddComponent(componentA);

            // Act
            entity.RemoveComponent(componentA);

            // Assert
            Assert.That(entity.Components, Is.Empty);
        }

        #endregion

        #region GetComponent

        [Test]
        public void GetComponent_ShouldReturnComponentByTypeFromEntity()
        {
            // Arrange
            var entity = Scene.CreateEntity();
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
            var entity = Scene.CreateEntity();
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
            entity.AddComponent(new ComponentB());

            // Act
            // Assert
            Assert.That(() => entity.GetComponent<ComponentA>(), Throws.InvalidOperationException);
        }

        [Test]
        public void GetComponent_ShouldThrowException_WhenThereAreMultipleComponentsOfTheSameType()
        {
            // Arrange
            var entity = Scene.CreateEntity();
            entity.AddComponent(new ComponentA());
            entity.AddComponent(new ComponentA());

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
            var entity = Scene.CreateEntity();

            // Act
            var actual = entity.GetComponents<ComponentA>();

            // Assert
            Assert.That(actual, Is.Empty);
        }

        [Test]
        public void GetComponents_ShouldReturnEnumerableWithOnly_ComponentA_WhenThereAreManyComponentsTypes()
        {
            // Arrange
            var entity = Scene.CreateEntity();
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
            var entity = Scene.CreateEntity();
            var component1 = new ComponentA();
            var component2 = new ComponentA();
            entity.AddComponent(component1);
            entity.AddComponent(component2);

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
            var entity = Scene.CreateEntity();

            // Act
            var actual = entity.HasComponent<ComponentA>();

            // Assert
            Assert.That(actual, Is.False);
        }

        #endregion

        #region Destroy

        [Test]
        public void DestroyAfterFixedTimeStep_ShouldSet_DestructionTime_To_AfterFixedTimeStep()
        {
            // Arrange
            var entity = Scene.CreateEntity();

            // Assume
            Assume.That(entity.DestructionTime, Is.EqualTo(DestructionTime.Never));

            // Act
            entity.DestroyAfterFixedTimeStep();

            // Assert
            Assert.That(entity.DestructionTime, Is.EqualTo(DestructionTime.AfterFixedTimeStep));
        }

        [Test]
        public void DestroyAfterFixedTimeStep_ShouldMake_IsScheduledForDestruction_ToBeTrue()
        {
            // Arrange
            var entity = Scene.CreateEntity();

            // Assume
            Assume.That(entity.IsScheduledForDestruction, Is.False);

            // Act
            entity.DestroyAfterFixedTimeStep();

            // Assert
            Assert.That(entity.IsScheduledForDestruction, Is.True);
        }

        [Test]
        public void DestroyAfterFullFrame_ShouldSet_DestructionTime_To_AfterFullFrame()
        {
            // Arrange
            var entity = Scene.CreateEntity();

            // Assume
            Assume.That(entity.DestructionTime, Is.EqualTo(DestructionTime.Never));

            // Act
            entity.DestroyAfterFullFrame();

            // Assert
            Assert.That(entity.DestructionTime, Is.EqualTo(DestructionTime.AfterFullFrame));
        }

        [Test]
        public void DestroyAfterFullFrame_ShouldMake_IsScheduledForDestruction_ToBeTrue()
        {
            // Arrange
            var entity = Scene.CreateEntity();

            // Assume
            Assume.That(entity.IsScheduledForDestruction, Is.False);

            // Act
            entity.DestroyAfterFullFrame();

            // Assert
            Assert.That(entity.IsScheduledForDestruction, Is.True);
        }

        #endregion

        #region Helpers

        private class ComponentA : Component
        {
        }

        private class ComponentB : Component
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