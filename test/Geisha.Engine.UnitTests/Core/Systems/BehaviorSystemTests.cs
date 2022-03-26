using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        [SetUp]
        public void SetUp()
        {
            _behaviorSystem = new BehaviorSystem();
        }

        [Test]
        public void ProcessBehaviorFixedUpdate_ShouldCallOnStartOnce_WhenUpdateExecutedTwice()
        {
            // Arrange
            var behaviorSceneBuilder = new BehaviorSceneBuilder();
            behaviorSceneBuilder.AddBehavior(out var behaviorComponent);
            var scene = behaviorSceneBuilder.Build();

            // Act
            _behaviorSystem.ProcessBehaviorFixedUpdate(scene);
            _behaviorSystem.ProcessBehaviorFixedUpdate(scene);

            // Assert
            Assert.That(behaviorComponent.MethodCalls, Has.Exactly(1).EqualTo(nameof(BehaviorComponent.OnStart)));
        }

        [Test]
        public void ProcessBehaviorFixedUpdate_ShouldCallOnStartBeforeOnFixedUpdate()
        {
            // Arrange
            var behaviorSceneBuilder = new BehaviorSceneBuilder();
            behaviorSceneBuilder.AddBehavior(out var behaviorComponent);
            var scene = behaviorSceneBuilder.Build();

            // Act
            _behaviorSystem.ProcessBehaviorFixedUpdate(scene);

            // Assert
            Assert.That(behaviorComponent.MethodCalls,
                Is.EqualTo(new[] { nameof(BehaviorComponent.OnStart), nameof(BehaviorComponent.OnFixedUpdate) }));
        }

        [Test]
        public void ProcessBehaviorFixedUpdate_ShouldCallOnFixedUpdateOnAllBehaviorComponents()
        {
            // Arrange
            var behaviorSceneBuilder = new BehaviorSceneBuilder();
            behaviorSceneBuilder.AddBehavior(out var behavior1OfEntity1);
            behaviorSceneBuilder.AddBehavior(out var behavior1OfEntity2, out var behavior2OfEntity2);
            var scene = behaviorSceneBuilder.Build();

            // Act
            _behaviorSystem.ProcessBehaviorFixedUpdate(scene);

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
            var behaviorSceneBuilder = new BehaviorSceneBuilder();
            var scene = behaviorSceneBuilder.Build();
            var entity = scene.CreateEntity();
            var behaviorComponent = entity.CreateComponent<AddComponentBehaviorComponent>();
            behaviorComponent.AddComponentOnStart = addComponentOnStart;
            behaviorComponent.AddComponentOnFixedUpdate = addComponentOnFixedUpdate;

            // Act
            _behaviorSystem.ProcessBehaviorFixedUpdate(scene);

            // Assert
            Assert.That(entity.Components.Count, Is.EqualTo(2));
        }

        // This test keeps implementation free of invalidating enumerator / enumerable exception while looping over components.
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void ProcessBehaviorUpdate_ShouldAddComponentToEntityInAddComponentBehavior(bool addComponentOnStart, bool addComponentOnUpdate)
        {
            // Arrange
            var behaviorSceneBuilder = new BehaviorSceneBuilder();
            var scene = behaviorSceneBuilder.Build();
            var entity = scene.CreateEntity();
            var behaviorComponent = entity.CreateComponent<AddComponentBehaviorComponent>();
            behaviorComponent.AddComponentOnStart = addComponentOnStart;
            behaviorComponent.AddComponentOnUpdate = addComponentOnUpdate;

            // Act
            _behaviorSystem.ProcessBehaviorUpdate(scene, _gameTime);

            // Assert
            Assert.That(entity.Components.Count, Is.EqualTo(2));
        }

        [Test]
        public void ProcessBehaviorUpdate_ShouldCallOnUpdateOnAllBehaviorComponents()
        {
            // Arrange
            var behaviorSceneBuilder = new BehaviorSceneBuilder();
            behaviorSceneBuilder.AddBehavior(out var behavior1OfEntity1);
            behaviorSceneBuilder.AddBehavior(out var behavior1OfEntity2, out var behavior2OfEntity2);
            var scene = behaviorSceneBuilder.Build();

            // Act
            _behaviorSystem.ProcessBehaviorUpdate(scene, _gameTime);

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

        private class BehaviorSceneBuilder
        {
            private readonly Scene _scene = TestSceneFactory.Create(new IComponentFactory[]
                { new TestBehaviorComponentFactory(), new AddComponentBehaviorComponentFactory() });

            public Entity AddBehavior(out TestBehaviorComponent behaviorComponent)
            {
                var entity = _scene.CreateEntity();
                behaviorComponent = entity.CreateComponent<TestBehaviorComponent>();

                return entity;
            }

            public Entity AddBehavior(out TestBehaviorComponent behaviorComponent1, out TestBehaviorComponent behaviorComponent2)
            {
                var entity = _scene.CreateEntity();
                behaviorComponent1 = entity.CreateComponent<TestBehaviorComponent>();
                behaviorComponent2 = entity.CreateComponent<TestBehaviorComponent>();

                return entity;
            }

            public Scene Build()
            {
                return _scene;
            }
        }
    }
}