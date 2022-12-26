using System;
using System.Collections.Generic;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.TestUtils;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Systems
{
    [TestFixture]
    public class BehaviorSystemTests
    {
        private readonly GameTime _gameTime = new(TimeSpan.FromSeconds(0.1));
        private BehaviorSystem _behaviorSystem = null!;
        private BehaviorScene _behaviorScene = null!;

        [SetUp]
        public void SetUp()
        {
            _behaviorSystem = new BehaviorSystem();
            _behaviorScene = new BehaviorScene(_behaviorSystem);
        }

        [Test]
        public void ProcessBehaviorFixedUpdate_ShouldCallOnStartOnce_WhenUpdateExecutedTwice()
        {
            // Arrange
            var behaviorComponent = _behaviorScene.AddBehavior();

            // Act
            _behaviorSystem.ProcessBehaviorFixedUpdate();
            _behaviorSystem.ProcessBehaviorFixedUpdate();

            // Assert
            Assert.That(behaviorComponent.MethodCalls, Has.Exactly(1).EqualTo(nameof(BehaviorComponent.OnStart)));
        }

        [Test]
        public void ProcessBehaviorFixedUpdate_ShouldCallOnStartBeforeOnFixedUpdate()
        {
            // Arrange
            var behaviorComponent = _behaviorScene.AddBehavior();

            // Act
            _behaviorSystem.ProcessBehaviorFixedUpdate();

            // Assert
            Assert.That(behaviorComponent.MethodCalls,
                Is.EqualTo(new[] { nameof(BehaviorComponent.OnStart), nameof(BehaviorComponent.OnFixedUpdate) }));
        }

        [Test]
        public void ProcessBehaviorFixedUpdate_ShouldCallOnFixedUpdateOnAllBehaviorComponents()
        {
            // Arrange
            var behavior1OfEntity1 = _behaviorScene.AddBehavior();
            _behaviorScene.AddBehavior(out var behavior1OfEntity2, out var behavior2OfEntity2);

            // Act
            _behaviorSystem.ProcessBehaviorFixedUpdate();

            // Assert
            Assert.That(behavior1OfEntity1.MethodCalls, Has.Exactly(1).EqualTo(nameof(BehaviorComponent.OnFixedUpdate)));
            Assert.That(behavior1OfEntity2.MethodCalls, Has.Exactly(1).EqualTo(nameof(BehaviorComponent.OnFixedUpdate)));
            Assert.That(behavior2OfEntity2.MethodCalls, Has.Exactly(1).EqualTo(nameof(BehaviorComponent.OnFixedUpdate)));
        }

        [Test]
        public void ProcessBehaviorFixedUpdate_ShouldNotCallOnFixedUpdateOnRemovedBehaviorComponents()
        {
            // Arrange
            var behavior1OfEntity1 = _behaviorScene.AddBehavior();
            _behaviorScene.AddBehavior(out var behavior1OfEntity2, out var behavior2OfEntity2);

            behavior1OfEntity1.Entity.RemoveComponent(behavior1OfEntity1);
            behavior2OfEntity2.Entity.RemoveComponent(behavior2OfEntity2);

            // Act
            _behaviorSystem.ProcessBehaviorFixedUpdate();

            // Assert
            Assert.That(behavior1OfEntity1.MethodCalls, Has.Exactly(0).EqualTo(nameof(BehaviorComponent.OnFixedUpdate)));
            Assert.That(behavior1OfEntity2.MethodCalls, Has.Exactly(1).EqualTo(nameof(BehaviorComponent.OnFixedUpdate)));
            Assert.That(behavior2OfEntity2.MethodCalls, Has.Exactly(0).EqualTo(nameof(BehaviorComponent.OnFixedUpdate)));
        }

        [Test]
        public void ProcessBehaviorFixedUpdate_ShouldCallOnRemoveForRemovedBehaviorComponents()
        {
            // Arrange
            var behavior1OfEntity1 = _behaviorScene.AddBehavior();
            _behaviorScene.AddBehavior(out var behavior1OfEntity2, out var behavior2OfEntity2);

            behavior1OfEntity1.Entity.RemoveComponent(behavior1OfEntity1);
            behavior2OfEntity2.Entity.RemoveComponent(behavior2OfEntity2);

            // Act
            _behaviorSystem.ProcessBehaviorFixedUpdate();

            // Assert
            Assert.That(behavior1OfEntity1.MethodCalls, Has.Exactly(1).EqualTo(nameof(BehaviorComponent.OnRemove)));
            Assert.That(behavior1OfEntity2.MethodCalls, Has.Exactly(0).EqualTo(nameof(BehaviorComponent.OnRemove)));
            Assert.That(behavior2OfEntity2.MethodCalls, Has.Exactly(1).EqualTo(nameof(BehaviorComponent.OnRemove)));
        }

        // This test keeps implementation free of invalidating enumerator / enumerable exception while looping over components.
        [TestCase(CreateOrRemoveComponentBehaviorComponent.ComponentAction.Create, true, false, 2)]
        [TestCase(CreateOrRemoveComponentBehaviorComponent.ComponentAction.Create, false, true, 2)]
        [TestCase(CreateOrRemoveComponentBehaviorComponent.ComponentAction.Remove, true, false, 0)]
        [TestCase(CreateOrRemoveComponentBehaviorComponent.ComponentAction.Remove, false, true, 0)]
        public void ProcessBehaviorFixedUpdate_ShouldCreateOrRemoveComponent_WhenHandlingCreateOrRemoveComponentBehavior(
            object action, bool executeOnStart, bool executeOnFixedUpdate, int expectedCount)
        {
            // Arrange
            var entity = _behaviorScene.Scene.CreateEntity();
            var behaviorComponent = entity.CreateComponent<CreateOrRemoveComponentBehaviorComponent>();
            behaviorComponent.Action = (CreateOrRemoveComponentBehaviorComponent.ComponentAction)action;
            behaviorComponent.ExecuteOnStart = executeOnStart;
            behaviorComponent.ExecuteOnFixedUpdate = executeOnFixedUpdate;

            // Act
            _behaviorSystem.ProcessBehaviorFixedUpdate();

            // Assert
            Assert.That(entity.Components, Has.Count.EqualTo(expectedCount));
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public void ProcessBehaviorFixedUpdate_ShouldCreateEntityInCreateEntityBehavior(bool createEntityOnStart, bool createEntityOnFixedUpdate)
        {
            // Arrange
            var entity = _behaviorScene.Scene.CreateEntity();
            var behaviorComponent = entity.CreateComponent<CreateEntityBehaviorComponent>();
            behaviorComponent.CreateEntityOnStart = createEntityOnStart;
            behaviorComponent.CreateEntityOnFixedUpdate = createEntityOnFixedUpdate;

            // Act
            _behaviorSystem.ProcessBehaviorFixedUpdate();

            // Assert
            Assert.That(_behaviorScene.Scene.AllEntities, Has.Count.EqualTo(2));
        }

        // This test keeps implementation free of invalidating enumerator / enumerable exception while looping over components.
        [TestCase(CreateOrRemoveComponentBehaviorComponent.ComponentAction.Create, true, false, 2)]
        [TestCase(CreateOrRemoveComponentBehaviorComponent.ComponentAction.Create, false, true, 2)]
        [TestCase(CreateOrRemoveComponentBehaviorComponent.ComponentAction.Remove, true, false, 0)]
        [TestCase(CreateOrRemoveComponentBehaviorComponent.ComponentAction.Remove, false, true, 0)]
        public void ProcessBehaviorUpdate_ShouldCreateOrRemoveComponent_WhenHandlingCreateOrRemoveComponentBehavior(
            object action, bool executeOnStart, bool executeOnUpdate, int expectedCount)
        {
            // Arrange
            var entity = _behaviorScene.Scene.CreateEntity();
            var behaviorComponent = entity.CreateComponent<CreateOrRemoveComponentBehaviorComponent>();
            behaviorComponent.Action = (CreateOrRemoveComponentBehaviorComponent.ComponentAction)action;
            behaviorComponent.ExecuteOnStart = executeOnStart;
            behaviorComponent.ExecuteOnUpdate = executeOnUpdate;

            // Act
            _behaviorSystem.ProcessBehaviorUpdate(_gameTime);

            // Assert
            Assert.That(entity.Components, Has.Count.EqualTo(expectedCount));
        }

        [TestCase(true, false)]
        [TestCase(false, true)]
        public void ProcessBehaviorUpdate_ShouldCreateEntityInCreateEntityBehavior(bool createEntityOnStart, bool createEntityOnUpdate)
        {
            // Arrange
            var entity = _behaviorScene.Scene.CreateEntity();
            var behaviorComponent = entity.CreateComponent<CreateEntityBehaviorComponent>();
            behaviorComponent.CreateEntityOnStart = createEntityOnStart;
            behaviorComponent.CreateEntityOnUpdate = createEntityOnUpdate;

            // Act
            _behaviorSystem.ProcessBehaviorUpdate(_gameTime);

            // Assert
            Assert.That(_behaviorScene.Scene.AllEntities, Has.Count.EqualTo(2));
        }

        [Test]
        public void ProcessBehaviorUpdate_ShouldCallOnUpdateOnAllBehaviorComponents()
        {
            // Arrange
            var behavior1OfEntity1 = _behaviorScene.AddBehavior();
            _behaviorScene.AddBehavior(out var behavior1OfEntity2, out var behavior2OfEntity2);

            // Act
            _behaviorSystem.ProcessBehaviorUpdate(_gameTime);

            // Assert
            Assert.That(behavior1OfEntity1.MethodCalls, Has.Exactly(1).EqualTo(nameof(BehaviorComponent.OnUpdate)));
            Assert.That(behavior1OfEntity1.OnUpdateCalls, Has.Exactly(1).EqualTo(_gameTime));
            Assert.That(behavior1OfEntity2.MethodCalls, Has.Exactly(1).EqualTo(nameof(BehaviorComponent.OnUpdate)));
            Assert.That(behavior1OfEntity2.OnUpdateCalls, Has.Exactly(1).EqualTo(_gameTime));
            Assert.That(behavior2OfEntity2.MethodCalls, Has.Exactly(1).EqualTo(nameof(BehaviorComponent.OnUpdate)));
            Assert.That(behavior2OfEntity2.OnUpdateCalls, Has.Exactly(1).EqualTo(_gameTime));
        }

        #region Helpers

        private sealed class CreateOrRemoveComponentBehaviorComponent : BehaviorComponent
        {
            public enum ComponentAction
            {
                Create,
                Remove
            }

            public CreateOrRemoveComponentBehaviorComponent(Entity entity) : base(entity)
            {
            }

            public ComponentAction Action { get; set; } = ComponentAction.Create;

            public bool ExecuteOnStart { get; set; }
            public bool ExecuteOnUpdate { get; set; }
            public bool ExecuteOnFixedUpdate { get; set; }

            public override void OnStart()
            {
                base.OnStart();
                if (ExecuteOnStart)
                {
                    Execute();
                }
            }

            public override void OnUpdate(GameTime gameTime)
            {
                base.OnUpdate(gameTime);
                if (ExecuteOnUpdate)
                {
                    Execute();
                }
            }

            public override void OnFixedUpdate()
            {
                base.OnFixedUpdate();
                if (ExecuteOnFixedUpdate)
                {
                    Execute();
                }
            }

            private void Execute()
            {
                switch (Action)
                {
                    case ComponentAction.Create:
                        Entity.CreateComponent<TestBehaviorComponent>();
                        break;
                    case ComponentAction.Remove:
                        Entity.RemoveComponent(this);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        private sealed class CreateOrRemoveComponentBehaviorComponentFactory : ComponentFactory<CreateOrRemoveComponentBehaviorComponent>
        {
            protected override CreateOrRemoveComponentBehaviorComponent CreateComponent(Entity entity) => new(entity);
        }

        private sealed class CreateEntityBehaviorComponent : BehaviorComponent
        {
            public CreateEntityBehaviorComponent(Entity entity) : base(entity)
            {
            }

            public bool CreateEntityOnStart { get; set; }
            public bool CreateEntityOnUpdate { get; set; }
            public bool CreateEntityOnFixedUpdate { get; set; }

            public override void OnStart()
            {
                base.OnStart();
                if (CreateEntityOnStart)
                {
                    Scene.CreateEntity();
                }
            }

            public override void OnUpdate(GameTime gameTime)
            {
                base.OnUpdate(gameTime);
                if (CreateEntityOnUpdate)
                {
                    Scene.CreateEntity();
                }
            }

            public override void OnFixedUpdate()
            {
                base.OnFixedUpdate();
                if (CreateEntityOnFixedUpdate)
                {
                    Scene.CreateEntity();
                }
            }
        }

        private sealed class CreateEntityBehaviorComponentFactory : ComponentFactory<CreateEntityBehaviorComponent>
        {
            protected override CreateEntityBehaviorComponent CreateComponent(Entity entity) => new(entity);
        }

        private sealed class TestBehaviorComponent : BehaviorComponent
        {
            private readonly List<string> _methodCalls = new();
            private readonly List<GameTime> _onUpdateCalls = new();

            public TestBehaviorComponent(Entity entity) : base(entity)
            {
            }

            public IReadOnlyList<string> MethodCalls => _methodCalls.AsReadOnly();
            public IReadOnlyList<GameTime> OnUpdateCalls => _onUpdateCalls.AsReadOnly();

            public override void OnStart()
            {
                _methodCalls.Add(nameof(OnStart));
            }

            public override void OnUpdate(GameTime gameTime)
            {
                _methodCalls.Add(nameof(OnUpdate));
                _onUpdateCalls.Add(gameTime);
            }

            public override void OnFixedUpdate()
            {
                _methodCalls.Add(nameof(OnFixedUpdate));
            }

            public override void OnRemove()
            {
                _methodCalls.Add(nameof(OnRemove));
            }
        }

        private sealed class TestBehaviorComponentFactory : ComponentFactory<TestBehaviorComponent>
        {
            protected override TestBehaviorComponent CreateComponent(Entity entity) => new(entity);
        }

        private class BehaviorScene
        {
            public BehaviorScene(ISceneObserver observer)
            {
                Scene.AddObserver(observer);
            }

            public Scene Scene { get; } = TestSceneFactory.Create(new IComponentFactory[]
            {
                new TestBehaviorComponentFactory(),
                new CreateOrRemoveComponentBehaviorComponentFactory(),
                new CreateEntityBehaviorComponentFactory()
            });

            public TestBehaviorComponent AddBehavior()
            {
                var entity = Scene.CreateEntity();
                return entity.CreateComponent<TestBehaviorComponent>();
            }

            public void AddBehavior(out TestBehaviorComponent behaviorComponent1, out TestBehaviorComponent behaviorComponent2)
            {
                var entity = Scene.CreateEntity();
                behaviorComponent1 = entity.CreateComponent<TestBehaviorComponent>();
                behaviorComponent2 = entity.CreateComponent<TestBehaviorComponent>();
            }
        }

        #endregion
    }
}