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

        [Test]
        public void RemoveEntity_ShouldCall_OnComponentRemoved_And_OnEntityParentChanged_And_OnEntityRemoved_WhenRemovingSubtree()
        {
            // Arrange
            var scene = CreateScene();
            var observer1 = Substitute.For<ISceneObserver>();
            var observer2 = Substitute.For<ISceneObserver>();

            scene.AddObserver(observer1);
            scene.AddObserver(observer2);

            var entitiesHierarchy = new EntitiesHierarchyWithComponents(scene);

            observer1.ClearReceivedCalls();
            observer2.ClearReceivedCalls();

            // Act
            scene.RemoveEntity(entitiesHierarchy.Child1);

            // Assert
            Received.InOrder(() =>
            {
                // Child111
                observer1.Received(1).OnComponentRemoved(entitiesHierarchy.Child111Component);
                observer2.Received(1).OnComponentRemoved(entitiesHierarchy.Child111Component);

                observer1.Received(1).OnEntityParentChanged(entitiesHierarchy.Child111, entitiesHierarchy.Child11, null);
                observer2.Received(1).OnEntityParentChanged(entitiesHierarchy.Child111, entitiesHierarchy.Child11, null);

                observer1.Received(1).OnEntityRemoved(entitiesHierarchy.Child111);
                observer2.Received(1).OnEntityRemoved(entitiesHierarchy.Child111);

                // Child11
                observer1.Received(1).OnComponentRemoved(entitiesHierarchy.Child11Component);
                observer2.Received(1).OnComponentRemoved(entitiesHierarchy.Child11Component);

                observer1.Received(1).OnEntityParentChanged(entitiesHierarchy.Child11, entitiesHierarchy.Child1, null);
                observer2.Received(1).OnEntityParentChanged(entitiesHierarchy.Child11, entitiesHierarchy.Child1, null);

                observer1.Received(1).OnEntityRemoved(entitiesHierarchy.Child11);
                observer2.Received(1).OnEntityRemoved(entitiesHierarchy.Child11);

                // Child12
                observer1.Received(1).OnComponentRemoved(entitiesHierarchy.Child12Component);
                observer2.Received(1).OnComponentRemoved(entitiesHierarchy.Child12Component);

                observer1.Received(1).OnEntityParentChanged(entitiesHierarchy.Child12, entitiesHierarchy.Child1, null);
                observer2.Received(1).OnEntityParentChanged(entitiesHierarchy.Child12, entitiesHierarchy.Child1, null);

                observer1.Received(1).OnEntityRemoved(entitiesHierarchy.Child12);
                observer2.Received(1).OnEntityRemoved(entitiesHierarchy.Child12);

                // Child1
                observer1.Received(1).OnComponentRemoved(entitiesHierarchy.Child1Component);
                observer2.Received(1).OnComponentRemoved(entitiesHierarchy.Child1Component);

                observer1.Received(1).OnEntityParentChanged(entitiesHierarchy.Child1, entitiesHierarchy.Root, null);
                observer2.Received(1).OnEntityParentChanged(entitiesHierarchy.Child1, entitiesHierarchy.Root, null);

                observer1.Received(1).OnEntityRemoved(entitiesHierarchy.Child1);
                observer2.Received(1).OnEntityRemoved(entitiesHierarchy.Child1);
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

        #region AddObserver

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

        [Test]
        public void AddObserver_ShouldNotifyObserverAboutCurrentSceneStructure()
        {
            // Arrange
            var scene = CreateScene();
            var observer = Substitute.For<ISceneObserver>();

            var entitiesHierarchy = new EntitiesHierarchyWithComponents(scene);

            // Act
            scene.AddObserver(observer);

            // Assert
            Received.InOrder(() =>
            {
                // Root
                observer.Received(1).OnEntityCreated(entitiesHierarchy.Root);
                observer.Received(1).OnComponentCreated(entitiesHierarchy.RootComponent);

                // Child1
                observer.Received(1).OnEntityCreated(entitiesHierarchy.Child1);
                observer.Received(1).OnEntityParentChanged(entitiesHierarchy.Child1, null, entitiesHierarchy.Root);
                observer.Received(1).OnComponentCreated(entitiesHierarchy.Child1Component);

                // Child11
                observer.Received(1).OnEntityCreated(entitiesHierarchy.Child11);
                observer.Received(1).OnEntityParentChanged(entitiesHierarchy.Child11, null, entitiesHierarchy.Child1);
                observer.Received(1).OnComponentCreated(entitiesHierarchy.Child11Component);

                // Child111
                observer.Received(1).OnEntityCreated(entitiesHierarchy.Child111);
                observer.Received(1).OnEntityParentChanged(entitiesHierarchy.Child111, null, entitiesHierarchy.Child11);
                observer.Received(1).OnComponentCreated(entitiesHierarchy.Child111Component);

                // Child12
                observer.Received(1).OnEntityCreated(entitiesHierarchy.Child12);
                observer.Received(1).OnEntityParentChanged(entitiesHierarchy.Child12, null, entitiesHierarchy.Child1);
                observer.Received(1).OnComponentCreated(entitiesHierarchy.Child12Component);

                // Child2
                observer.Received(1).OnEntityCreated(entitiesHierarchy.Child2);
                observer.Received(1).OnEntityParentChanged(entitiesHierarchy.Child2, null, entitiesHierarchy.Root);
                observer.Received(1).OnComponentCreated(entitiesHierarchy.Child2Component);
            });
        }

        #endregion

        #region RemoveObserver

        [Test]
        public void RemoveObserver_ShouldThrowException_WhenRemovingObserverThatWasNeverAddedToTheScene()
        {
            // Arrange
            var scene = CreateScene();
            var observer = Substitute.For<ISceneObserver>();

            // Act
            // Assert
            Assert.That(() => scene.RemoveObserver(observer), Throws.ArgumentException);
        }

        [Test]
        public void RemoveObserver_ShouldMakeObserverNoLongerReceiveNotifications()
        {
            // Arrange
            var scene = CreateScene();
            var observer = Substitute.For<ISceneObserver>();

            scene.AddObserver(observer);

            // Assume
            var entity = scene.CreateEntity();
            observer.Received(1).OnEntityCreated(entity);

            observer.ClearReceivedCalls();

            // Act
            scene.RemoveObserver(observer);

            // Assert
            observer.ClearReceivedCalls();

            entity = scene.CreateEntity();
            observer.DidNotReceive().OnEntityCreated(entity);
        }

        [Test]
        public void RemoveObserver_ShouldMakeObserverToRemoveInternalSceneStructure()
        {
            // Arrange
            var scene = CreateScene();
            var observer = Substitute.For<ISceneObserver>();

            scene.AddObserver(observer);

            var entitiesHierarchy = new EntitiesHierarchyWithComponents(scene);

            observer.ClearReceivedCalls();

            // Act
            scene.RemoveObserver(observer);

            // Assert
            Received.InOrder(() =>
            {
                // Child111
                observer.Received(1).OnComponentRemoved(entitiesHierarchy.Child111Component);
                observer.Received(1).OnEntityParentChanged(entitiesHierarchy.Child111, entitiesHierarchy.Child11, null);
                observer.Received(1).OnEntityRemoved(entitiesHierarchy.Child111);

                // Child11
                observer.Received(1).OnComponentRemoved(entitiesHierarchy.Child11Component);
                observer.Received(1).OnEntityParentChanged(entitiesHierarchy.Child11, entitiesHierarchy.Child1, null);
                observer.Received(1).OnEntityRemoved(entitiesHierarchy.Child11);

                // Child12
                observer.Received(1).OnComponentRemoved(entitiesHierarchy.Child12Component);
                observer.Received(1).OnEntityParentChanged(entitiesHierarchy.Child12, entitiesHierarchy.Child1, null);
                observer.Received(1).OnEntityRemoved(entitiesHierarchy.Child12);

                // Child1
                observer.Received(1).OnComponentRemoved(entitiesHierarchy.Child1Component);
                observer.Received(1).OnEntityParentChanged(entitiesHierarchy.Child1, entitiesHierarchy.Root, null);
                observer.Received(1).OnEntityRemoved(entitiesHierarchy.Child1);

                // Child2
                observer.Received(1).OnComponentRemoved(entitiesHierarchy.Child2Component);
                observer.Received(1).OnEntityParentChanged(entitiesHierarchy.Child2, entitiesHierarchy.Root, null);
                observer.Received(1).OnEntityRemoved(entitiesHierarchy.Child2);

                // Root
                observer.Received(1).OnComponentRemoved(entitiesHierarchy.RootComponent);
                observer.Received(1).OnEntityRemoved(entitiesHierarchy.Root);
            });
        }

        #endregion

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

        private sealed class EntitiesHierarchyWithComponents
        {
            public Entity Root { get; }
            public Entity Child1 { get; }
            public Entity Child2 { get; }
            public Entity Child11 { get; }
            public Entity Child12 { get; }
            public Entity Child111 { get; }

            public TestComponent RootComponent { get; }
            public TestComponent Child1Component { get; }
            public TestComponent Child2Component { get; }
            public TestComponent Child11Component { get; }
            public TestComponent Child12Component { get; }
            public TestComponent Child111Component { get; }

            public EntitiesHierarchyWithComponents(Scene scene)
            {
                Root = scene.CreateEntity();
                Root.Name = nameof(Root);
                RootComponent = Root.CreateComponent<TestComponent>();

                Child1 = Root.CreateChildEntity();
                Child1.Name = nameof(Child1);
                Child1Component = Child1.CreateComponent<TestComponent>();

                Child2 = Root.CreateChildEntity();
                Child2.Name = nameof(Child2);
                Child2Component = Child2.CreateComponent<TestComponent>();

                Child11 = Child1.CreateChildEntity();
                Child11.Name = nameof(Child11);
                Child11Component = Child11.CreateComponent<TestComponent>();

                Child12 = Child1.CreateChildEntity();
                Child12.Name = nameof(Child12);
                Child12Component = Child12.CreateComponent<TestComponent>();

                Child111 = Child11.CreateChildEntity();
                Child111.Name = nameof(Child111);
                Child111Component = Child111.CreateComponent<TestComponent>();
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