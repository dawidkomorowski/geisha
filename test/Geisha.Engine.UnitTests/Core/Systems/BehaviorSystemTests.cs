using System;
using System.Diagnostics;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using Geisha.TestUtils;
using NSubstitute;
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
        public void ProcessBehaviorFixedUpdate_ShouldSetEntityOnAllBehaviorComponents()
        {
            // Arrange
            var behaviorSceneBuilder = new BehaviorSceneBuilder();
            var entity1 = behaviorSceneBuilder.AddBehavior(out var behavior1OfEntity1);
            var entity2 = behaviorSceneBuilder.AddBehavior(out var behavior1OfEntity2, out var behavior2OfEntity2);
            var scene = behaviorSceneBuilder.Build();

            // Act
            _behaviorSystem.ProcessBehaviorFixedUpdate(scene);

            // Assert
            Assert.That(behavior1OfEntity1.Entity, Is.EqualTo(entity1));
            Assert.That(behavior1OfEntity2.Entity, Is.EqualTo(entity2));
            Assert.That(behavior2OfEntity2.Entity, Is.EqualTo(entity2));
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
            behaviorComponent.Received(1).OnStart();
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
            Received.InOrder(() =>
            {
                behaviorComponent.Received(1).OnStart();
                behaviorComponent.Received(1).OnFixedUpdate();
            });
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
            behavior1OfEntity1.Received(1).OnFixedUpdate();
            behavior1OfEntity2.Received(1).OnFixedUpdate();
            behavior2OfEntity2.Received(1).OnFixedUpdate();
        }

        // This test keeps implementation free of invalidating enumerator / enumerable exception while looping over entities.
        [Test]
        public void ProcessBehaviorFixedUpdate_ShouldRemoveEntityWithRemoveFromSceneBehavior()
        {
            // Arrange
            var behaviorSceneBuilder = new BehaviorSceneBuilder();
            var scene = behaviorSceneBuilder.Build();
            var entity = new Entity();
            entity.AddComponent(new RemoveFromSceneBehaviorComponent());

            scene.AddEntity(entity);

            // Act
            _behaviorSystem.ProcessBehaviorFixedUpdate(scene);

            // Assert
            Assert.That(scene.AllEntities, Does.Not.Contain(entity));
        }

        // This test keeps implementation free of invalidating enumerator / enumerable exception while looping over components.
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void ProcessBehaviorFixedUpdate_ShouldAddComponentToEntityInAddComponentBehavior(bool addComponentOnStart, bool addComponentOnFixedUpdate)
        {
            // Arrange
            var behaviorSceneBuilder = new BehaviorSceneBuilder();
            var scene = behaviorSceneBuilder.Build();
            var entity = new Entity();
            entity.AddComponent(new AddComponentBehaviorComponent
            {
                AddComponentOnStart = addComponentOnStart,
                AddComponentOnFixedUpdate = addComponentOnFixedUpdate
            });

            scene.AddEntity(entity);

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
            var entity = new Entity();
            entity.AddComponent(new AddComponentBehaviorComponent
            {
                AddComponentOnStart = addComponentOnStart,
                AddComponentOnUpdate = addComponentOnUpdate
            });

            scene.AddEntity(entity);

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
            behavior1OfEntity1.Received(1).OnUpdate(_gameTime);
            behavior1OfEntity2.Received(1).OnUpdate(_gameTime);
            behavior2OfEntity2.Received(1).OnUpdate(_gameTime);
        }

        private class RemoveFromSceneBehaviorComponent : BehaviorComponent
        {
            public override void OnFixedUpdate()
            {
                Debug.Assert(Entity != null, nameof(Entity) + " != null");
                Debug.Assert(Entity.Scene != null, "Entity.Scene != null");
                Entity.Scene.RemoveEntity(Entity);
            }
        }

        private class AddComponentBehaviorComponent : BehaviorComponent
        {
            public bool AddComponentOnStart { get; set; }
            public bool AddComponentOnUpdate { get; set; }
            public bool AddComponentOnFixedUpdate { get; set; }

            public override void OnStart()
            {
                base.OnStart();
                if (AddComponentOnStart)
                {
                    Debug.Assert(Entity != null, nameof(Entity) + " != null");
                    Entity.AddComponent(CreateNewComponent());
                }
            }

            public override void OnUpdate(GameTime gameTime)
            {
                base.OnUpdate(gameTime);
                if (AddComponentOnUpdate)
                {
                    Debug.Assert(Entity != null, nameof(Entity) + " != null");
                    Entity.AddComponent(CreateNewComponent());
                }
            }

            public override void OnFixedUpdate()
            {
                base.OnFixedUpdate();
                if (AddComponentOnFixedUpdate)
                {
                    Debug.Assert(Entity != null, nameof(Entity) + " != null");
                    Entity.AddComponent(CreateNewComponent());
                }
            }

            private static Component CreateNewComponent() => Substitute.For<Component>();
        }

        private class BehaviorSceneBuilder
        {
            private readonly Scene _scene = TestSceneFactory.Create();

            public Entity AddBehavior(out BehaviorComponent behaviorComponent)
            {
                behaviorComponent = Substitute.For<BehaviorComponent>();

                var entity = new Entity();
                entity.AddComponent(behaviorComponent);

                _scene.AddEntity(entity);

                return entity;
            }

            public Entity AddBehavior(out BehaviorComponent behaviorComponent1, out BehaviorComponent behaviorComponent2)
            {
                behaviorComponent1 = Substitute.For<BehaviorComponent>();
                behaviorComponent2 = Substitute.For<BehaviorComponent>();

                var entity = new Entity();
                entity.AddComponent(behaviorComponent1);
                entity.AddComponent(behaviorComponent2);

                _scene.AddEntity(entity);

                return entity;
            }

            public Scene Build()
            {
                return _scene;
            }
        }
    }
}