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
        private readonly GameTime _gameTime = new GameTime(TimeSpan.FromSeconds(0.1));
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

        // This test keeps implementation free of invalidating enumerator / enumerable exception while looping over components.
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void ProcessBehaviorFixedUpdate_ShouldAddComponentToEntityInAddComponentBehavior(bool addComponentOnStart, bool addComponentOnFixedUpdate)
        {
            // Arrange
            var entity = _behaviorScene.Scene.CreateEntity();
            var behaviorComponent = entity.CreateComponent<AddComponentBehaviorComponent>();
            behaviorComponent.AddComponentOnStart = addComponentOnStart;
            behaviorComponent.AddComponentOnFixedUpdate = addComponentOnFixedUpdate;

            // Act
            _behaviorSystem.ProcessBehaviorFixedUpdate();

            // Assert
            Assert.That(entity.Components.Count, Is.EqualTo(2));
        }

        // This test keeps implementation free of invalidating enumerator / enumerable exception while looping over components.
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void ProcessBehaviorUpdate_ShouldAddComponentToEntityInAddComponentBehavior(bool addComponentOnStart, bool addComponentOnUpdate)
        {
            // Arrange
            var entity = _behaviorScene.Scene.CreateEntity();
            var behaviorComponent = entity.CreateComponent<AddComponentBehaviorComponent>();
            behaviorComponent.AddComponentOnStart = addComponentOnStart;
            behaviorComponent.AddComponentOnUpdate = addComponentOnUpdate;

            // Act
            _behaviorSystem.ProcessBehaviorUpdate(_gameTime);

            // Assert
            Assert.That(entity.Components.Count, Is.EqualTo(2));
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

        private sealed class AddComponentBehaviorComponent : BehaviorComponent
        {
            public AddComponentBehaviorComponent(Entity entity) : base(entity)
            {
            }

            public bool AddComponentOnStart { get; set; }
            public bool AddComponentOnUpdate { get; set; }
            public bool AddComponentOnFixedUpdate { get; set; }

            public override void OnStart()
            {
                base.OnStart();
                if (AddComponentOnStart)
                {
                    Entity.CreateComponent<TestBehaviorComponent>();
                }
            }

            public override void OnUpdate(GameTime gameTime)
            {
                base.OnUpdate(gameTime);
                if (AddComponentOnUpdate)
                {
                    Entity.CreateComponent<TestBehaviorComponent>();
                }
            }

            public override void OnFixedUpdate()
            {
                base.OnFixedUpdate();
                if (AddComponentOnFixedUpdate)
                {
                    Entity.CreateComponent<TestBehaviorComponent>();
                }
            }
        }

        private sealed class AddComponentBehaviorComponentFactory : ComponentFactory<AddComponentBehaviorComponent>
        {
            protected override AddComponentBehaviorComponent CreateComponent(Entity entity) => new AddComponentBehaviorComponent(entity);
        }

        private sealed class TestBehaviorComponent : BehaviorComponent
        {
            private readonly List<string> _methodCalls = new List<string>();
            private readonly List<GameTime> _onUpdateCalls = new List<GameTime>();

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
        }

        private sealed class TestBehaviorComponentFactory : ComponentFactory<TestBehaviorComponent>
        {
            protected override TestBehaviorComponent CreateComponent(Entity entity) => new TestBehaviorComponent(entity);
        }

        private class BehaviorScene
        {
            public BehaviorScene(ISceneObserver observer)
            {
                Scene.AddObserver(observer);
            }

            public Scene Scene { get; } = TestSceneFactory.Create(new IComponentFactory[]
                { new TestBehaviorComponentFactory(), new AddComponentBehaviorComponentFactory() });

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
    }
}