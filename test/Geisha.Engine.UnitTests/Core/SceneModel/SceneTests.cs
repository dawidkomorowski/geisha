using Geisha.Engine.Core.SceneModel;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel
{
    [TestFixture]
    public class SceneTests
    {
        #region CreateEntity

        [Test]
        public void CreateEntity_ShouldCreateNewRootEntityInTheScene()
        {
            // Arrange
            var scene = CreateScene();

            // Act
            var entity = scene.CreateEntity();

            // Assert
            Assert.That(scene.AllEntities, Is.EquivalentTo(new[] { entity }));
            Assert.That(scene.RootEntities, Is.EquivalentTo(new[] { entity }));
        }

        [Test]
        public void CreateEntity_ShouldCreateEntityWithSceneSet()
        {
            // Arrange
            var scene = CreateScene();

            // Act
            var entity = scene.CreateEntity();

            // Assert
            Assert.That(entity.Scene, Is.EqualTo(scene));
        }

        [Test]
        public void CreateEntity_ShouldCreateEntityWithNoParent()
        {
            // Arrange
            var scene = CreateScene();

            // Act
            var entity = scene.CreateEntity();

            // Assert
            Assert.That(entity.Parent, Is.Null);
        }

        [Test]
        public void CreateEntity_ShouldCreateEntityWithNoChildren()
        {
            // Arrange
            var scene = CreateScene();

            // Act
            var entity = scene.CreateEntity();

            // Assert
            Assert.That(entity.Children, Is.Empty);
        }

        [Test]
        public void CreateEntity_ShouldCreateEntityWithNoComponents()
        {
            // Arrange
            var scene = CreateScene();

            // Act
            var entity = scene.CreateEntity();

            // Assert
            Assert.That(entity.Components, Is.Empty);
        }

        [Test]
        public void CreateEntity_ShouldCall_OnEntityCreated()
        {
            // Arrange
            var scene = CreateScene();
            var observer1 = Substitute.For<ISceneObserver>();
            var observer2 = Substitute.For<ISceneObserver>();

            scene.AddObserver(observer1);
            scene.AddObserver(observer2);

            observer1.ClearReceivedCalls();
            observer2.ClearReceivedCalls();

            // Act
            var entity = scene.CreateEntity();

            // Assert
            observer1.Received(1).OnEntityCreated(entity);
            observer2.Received(1).OnEntityCreated(entity);
        }

        #endregion

        #region RemoveEntity

        [Test]
        public void RemoveEntity_ShouldThrowException_GivenEntityCreatedByAnotherScene()
        {
            // Arrange
            var scene1 = CreateScene();
            var scene2 = CreateScene();

            var entityInScene1 = scene1.CreateEntity();

            // Act
            // Assert
            Assert.That(() => scene2.RemoveEntity(entityInScene1), Throws.ArgumentException);
        }

        [Test]
        public void RemoveEntity_ShouldRemoveEntityFromRootEntities()
        {
            // Arrange
            var scene = CreateScene();
            var entity = scene.CreateEntity();

            // Act
            scene.RemoveEntity(entity);

            // Assert
            Assert.That(scene.RootEntities, Is.Empty);
        }

        [Test]
        public void RemoveEntity_ShouldRemoveEntityFromAllEntities()
        {
            // Arrange
            var scene = CreateScene();
            var entity = scene.CreateEntity();

            // Act
            scene.RemoveEntity(entity);

            // Assert
            Assert.That(scene.AllEntities, Is.Empty);
        }

        [Test]
        public void RemoveEntity_ShouldRemoveChildOfEntity_WhenEntityIsParent()
        {
            // Arrange
            var scene = CreateScene();

            var rootEntity = scene.CreateEntity();
            var childOfRoot = rootEntity.CreateChildEntity();

            // Act
            scene.RemoveEntity(rootEntity);

            // Assert
            Assert.That(scene.AllEntities, Is.Empty);
            Assert.That(scene.RootEntities, Is.Empty);
            Assert.That(rootEntity.Children, Has.Count.EqualTo(0));
            Assert.That(childOfRoot.Parent, Is.Null);
        }

        [Test]
        public void RemoveEntity_ShouldRemoveChildOfOtherEntity_WhenEntityIsChild()
        {
            // Arrange
            var scene = CreateScene();

            var rootEntity = scene.CreateEntity();
            var childOfRoot = rootEntity.CreateChildEntity();

            // Act
            scene.RemoveEntity(childOfRoot);

            // Assert
            Assert.That(scene.AllEntities.Count, Is.EqualTo(1));
            Assert.That(scene.AllEntities, Does.Not.Contains(childOfRoot));
            Assert.That(rootEntity.Children, Has.Count.EqualTo(0));
            Assert.That(childOfRoot.Parent, Is.Null);
        }

        [Test]
        public void RemoveEntity_ShouldRemoveAllEntitiesInSubtree_WhenEntityIsRootOfSubtree()
        {
            // Arrange
            var scene = CreateScene();

            var entitiesHierarchy = new EntitiesHierarchy(scene);

            // Assume
            Assume.That(scene.AllEntities.Count, Is.EqualTo(6));

            // Act
            scene.RemoveEntity(entitiesHierarchy.Root);

            // Assert
            Assert.That(scene.AllEntities, Is.Empty);
            Assert.That(scene.RootEntities, Is.Empty);
            Assert.That(entitiesHierarchy.Root.Children, Has.Count.EqualTo(0));
            Assert.That(entitiesHierarchy.Child1.Parent, Is.Null);
            Assert.That(entitiesHierarchy.Child2.Parent, Is.Null);
        }

        [Test]
        public void RemoveEntity_ShouldCall_OnEntityRemoved()
        {
            // Arrange
            var scene = CreateScene();
            var observer1 = Substitute.For<ISceneObserver>();
            var observer2 = Substitute.For<ISceneObserver>();

            scene.AddObserver(observer1);
            scene.AddObserver(observer2);

            var entity = scene.CreateEntity();

            observer1.ClearReceivedCalls();
            observer2.ClearReceivedCalls();

            // Act
            scene.RemoveEntity(entity);

            // Assert
            observer1.Received(1).OnEntityRemoved(entity);
            observer2.Received(1).OnEntityRemoved(entity);
        }

        [Test]
        public void RemoveEntity_ShouldCall_OnEntityParentChanged_And_OnEntityRemoved()
        {
            // Arrange
            var scene = CreateScene();
            var observer1 = Substitute.For<ISceneObserver>();
            var observer2 = Substitute.For<ISceneObserver>();

            scene.AddObserver(observer1);
            scene.AddObserver(observer2);

            var parent = scene.CreateEntity();
            var child = parent.CreateChildEntity();

            observer1.ClearReceivedCalls();
            observer2.ClearReceivedCalls();

            // Act
            scene.RemoveEntity(child);

            // Assert
            Received.InOrder(() =>
            {
                observer1.Received(1).OnEntityParentChanged(child, parent, null);
                observer2.Received(1).OnEntityParentChanged(child, parent, null);

                observer1.Received(1).OnEntityRemoved(child);
                observer2.Received(1).OnEntityRemoved(child);
            });
        }

        [Test]
        public void RemoveEntity_ShouldCall_OnEntityRemoved_FirstForChildSecondForParent_WhenParentIsRemoved()
        {
            // Arrange
            var scene = CreateScene();
            var observer1 = Substitute.For<ISceneObserver>();
            var observer2 = Substitute.For<ISceneObserver>();

            scene.AddObserver(observer1);
            scene.AddObserver(observer2);

            var parent = scene.CreateEntity();
            var child1 = parent.CreateChildEntity();
            var child2 = parent.CreateChildEntity();

            observer1.ClearReceivedCalls();
            observer2.ClearReceivedCalls();

            // Act
            scene.RemoveEntity(parent);

            // Assert
            Received.InOrder(() =>
            {
                observer1.Received(1).OnEntityRemoved(child1);
                observer2.Received(1).OnEntityRemoved(child1);

                observer1.Received(1).OnEntityRemoved(child2);
                observer2.Received(1).OnEntityRemoved(child2);

                observer1.Received(1).OnEntityRemoved(parent);
                observer2.Received(1).OnEntityRemoved(parent);
            });
        }

        [Test]
        public void RemoveEntity_ShouldCall_OnComponentRemoved_And_OnEntityRemoved()
        {
            // Arrange
            var scene = CreateScene();
            var observer1 = Substitute.For<ISceneObserver>();
            var observer2 = Substitute.For<ISceneObserver>();

            scene.AddObserver(observer1);
            scene.AddObserver(observer2);

            var entity = scene.CreateEntity();
            var component1 = entity.CreateComponent<TestComponent>();
            var component2 = entity.CreateComponent<TestComponent>();

            observer1.ClearReceivedCalls();
            observer2.ClearReceivedCalls();

            // Act
            scene.RemoveEntity(entity);

            // Assert
            Received.InOrder(() =>
            {
                observer1.Received(1).OnComponentRemoved(component1);
                observer2.Received(1).OnComponentRemoved(component1);

                observer1.Received(1).OnComponentRemoved(component2);
                observer2.Received(1).OnComponentRemoved(component2);

                observer1.Received(1).OnEntityRemoved(entity);
                observer2.Received(1).OnEntityRemoved(entity);
            });
        }

        #endregion

        [Test]
        public void AllEntities_ShouldReturnAllEntitiesInTheScene()
        {
            // Arrange
            var scene = CreateScene();

            var rootEntity1 = scene.CreateEntity();
            var rootEntity2 = scene.CreateEntity();
            var child1OfRoot1 = rootEntity1.CreateChildEntity();
            var child2OfRoot1 = rootEntity1.CreateChildEntity();
            var child1OfRoot2 = rootEntity2.CreateChildEntity();
            var child1OfChild1OfRoot1 = child1OfRoot1.CreateChildEntity();

            // Act
            var allEntities = scene.AllEntities;

            // Assert
            Assert.That(allEntities, Is.EquivalentTo(new[] { rootEntity1, rootEntity2, child1OfRoot1, child2OfRoot1, child1OfRoot2, child1OfChild1OfRoot1 }));
        }

        [Test]
        public void RootEntities_ShouldReturnOnlyRootEntitiesOfTheScene()
        {
            // Arrange
            var scene = CreateScene();

            var rootEntity1 = scene.CreateEntity();
            var rootEntity2 = scene.CreateEntity();
            var child1OfRoot1 = rootEntity1.CreateChildEntity();
            _ = rootEntity1.CreateChildEntity();
            _ = rootEntity2.CreateChildEntity();
            _ = child1OfRoot1.CreateChildEntity();

            // Act
            var rootEntities = scene.RootEntities;

            // Assert
            Assert.That(rootEntities, Is.EquivalentTo(new[] { rootEntity1, rootEntity2 }));
        }

        [Test]
        public void SceneBehavior_ShouldBeSetToEmptySceneBehavior_WhenSceneConstructed()
        {
            // Arrange
            var scene = CreateScene();

            // Act
            var actual = scene.SceneBehavior;

            // Assert
            Assert.That(actual, Is.TypeOf(SceneBehavior.CreateEmpty(scene).GetType()));
        }

        [Test]
        public void OnLoaded_ShouldExecuteOnLoadedOfSceneBehavior()
        {
            // Arrange
            var scene = CreateScene();
            var sceneBehavior = Substitute.ForPartsOf<SceneBehavior>(scene);
            scene.SceneBehavior = sceneBehavior;

            // Act
            scene.OnLoaded();

            // Assert
            sceneBehavior.Received(1).OnLoaded();
        }

        [Test]
        public void AddObserver_ShouldThrowException_WhenAddingTheSameObserverTwice()
        {
            // Arrange
            var scene = CreateScene();
            var observer = Substitute.For<ISceneObserver>();

            scene.AddObserver(observer);

            // Act
            // Assert
            Assert.That(() => scene.AddObserver(observer), Throws.ArgumentException);
        }

        #region Helpers

        private static Scene CreateScene()
        {
            return TestSceneFactory.Create(new[] { new TestComponentFactory() });
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

        private sealed class TestComponent : Component
        {
            public TestComponent(Entity entity) : base(entity)
            {
            }
        }

        private sealed class TestComponentFactory : ComponentFactory<TestComponent>
        {
            protected override TestComponent CreateComponent(Entity entity) => new TestComponent(entity);
        }

        #endregion
    }
}