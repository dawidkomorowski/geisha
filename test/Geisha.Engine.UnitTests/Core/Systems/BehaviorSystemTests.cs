using System;
using Geisha.Engine.Core;
using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.Systems
{
    [TestFixture]
    public class BehaviorSystemTests
    {
        private readonly GameTime _gameTime = new GameTime(TimeSpan.FromSeconds(0.1));
        private BehaviorSystem _behaviorSystem;

        [SetUp]
        public void SetUp()
        {
            _behaviorSystem = new BehaviorSystem();
        }

        [Test]
        public void FixedUpdate_ShouldSetEntityOnAllBehaviorComponents()
        {
            // Arrange
            var behaviorSceneBuilder = new BehaviorSceneBuilder();
            var entity1 = behaviorSceneBuilder.AddBehavior(out var behavior1OfEntity1);
            var entity2 = behaviorSceneBuilder.AddBehavior(out var behavior1OfEntity2, out var behavior2OfEntity2);
            var scene = behaviorSceneBuilder.Build();

            // Act
            _behaviorSystem.FixedUpdate(scene);

            // Assert
            Assert.That(behavior1OfEntity1.Entity, Is.EqualTo(entity1));
            Assert.That(behavior1OfEntity2.Entity, Is.EqualTo(entity2));
            Assert.That(behavior2OfEntity2.Entity, Is.EqualTo(entity2));
        }

        [Test]
        public void FixedUpdate_ShouldCallOnStartOnce_WhenUpdateExecutedTwice()
        {
            // Arrange
            var behaviorSceneBuilder = new BehaviorSceneBuilder();
            behaviorSceneBuilder.AddBehavior(out var behaviorComponent);
            var scene = behaviorSceneBuilder.Build();

            // Act
            _behaviorSystem.FixedUpdate(scene);
            _behaviorSystem.FixedUpdate(scene);

            // Assert
            behaviorComponent.Received(1).OnStart();
        }

        [Test]
        public void FixedUpdate_ShouldCallOnStartBeforeOnFixedUpdate()
        {
            // Arrange
            var behaviorSceneBuilder = new BehaviorSceneBuilder();
            behaviorSceneBuilder.AddBehavior(out var behaviorComponent);
            var scene = behaviorSceneBuilder.Build();

            // Act
            _behaviorSystem.FixedUpdate(scene);

            // Assert
            Received.InOrder(() =>
            {
                behaviorComponent.Received(1).OnStart();
                behaviorComponent.Received(1).OnFixedUpdate();
            });
        }

        [Test]
        public void FixedUpdate_ShouldCallOnFixedUpdateOnAllBehaviorComponents()
        {
            // Arrange
            var behaviorSceneBuilder = new BehaviorSceneBuilder();
            behaviorSceneBuilder.AddBehavior(out var behavior1OfEntity1);
            behaviorSceneBuilder.AddBehavior(out var behavior1OfEntity2, out var behavior2OfEntity2);
            var scene = behaviorSceneBuilder.Build();

            // Act
            _behaviorSystem.FixedUpdate(scene);

            // Assert
            behavior1OfEntity1.Received(1).OnFixedUpdate();
            behavior1OfEntity2.Received(1).OnFixedUpdate();
            behavior2OfEntity2.Received(1).OnFixedUpdate();
        }

        // This test keeps implementation free of invalidating enumerator / enumerable exception while looping over entities.
        [Test]
        public void FixedUpdate_ShouldRemoveEntityWithRemoveFromSceneBehavior()
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();
            entity.AddComponent(new RemoveFromSceneBehaviorComponent());

            scene.AddEntity(entity);

            // Act
            _behaviorSystem.FixedUpdate(scene);

            // Assert
            Assert.That(scene.AllEntities, Does.Not.Contain(entity));
        }

        // This test keeps implementation free of invalidating enumerator / enumerable exception while looping over components.
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void FixedUpdate_ShouldAddComponentToEntityInAddComponentBehavior(bool addComponentOnStart, bool addComponentOnFixedUpdate)
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();
            entity.AddComponent(new AddComponentBehaviorComponent
            {
                AddComponentOnStart = addComponentOnStart,
                AddComponentOnFixedUpdate = addComponentOnFixedUpdate
            });

            scene.AddEntity(entity);

            // Act
            _behaviorSystem.FixedUpdate(scene);

            // Assert
            Assert.That(entity.Components.Count, Is.EqualTo(2));
        }

        // This test keeps implementation free of invalidating enumerator / enumerable exception while looping over components.
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void Update_ShouldAddComponentToEntityInAddComponentBehavior(bool addComponentOnStart, bool addComponentOnUpdate)
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();
            entity.AddComponent(new AddComponentBehaviorComponent
            {
                AddComponentOnStart = addComponentOnStart,
                AddComponentOnUpdate = addComponentOnUpdate
            });

            scene.AddEntity(entity);

            // Act
            _behaviorSystem.Update(scene, _gameTime);

            // Assert
            Assert.That(entity.Components.Count, Is.EqualTo(2));
        }

        [Test]
        public void Update_ShouldCallOnUpdateOnAllBehaviorComponents()
        {
            // Arrange
            var behaviorSceneBuilder = new BehaviorSceneBuilder();
            behaviorSceneBuilder.AddBehavior(out var behavior1OfEntity1);
            behaviorSceneBuilder.AddBehavior(out var behavior1OfEntity2, out var behavior2OfEntity2);
            var scene = behaviorSceneBuilder.Build();

            // Act
            _behaviorSystem.Update(scene, _gameTime);

            // Assert
            behavior1OfEntity1.Received(1).OnUpdate(_gameTime);
            behavior1OfEntity2.Received(1).OnUpdate(_gameTime);
            behavior2OfEntity2.Received(1).OnUpdate(_gameTime);
        }

        private class RemoveFromSceneBehaviorComponent : BehaviorComponent
        {
            public override void OnFixedUpdate()
            {
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
                if (AddComponentOnStart) Entity.AddComponent(CreateNewComponent());
            }

            public override void OnUpdate(GameTime gameTime)
            {
                base.OnUpdate(gameTime);
                if (AddComponentOnUpdate) Entity.AddComponent(CreateNewComponent());
            }

            public override void OnFixedUpdate()
            {
                base.OnFixedUpdate();
                if (AddComponentOnFixedUpdate) Entity.AddComponent(CreateNewComponent());
            }

            private static IComponent CreateNewComponent() => Substitute.For<IComponent>();
        }

        private class BehaviorSceneBuilder
        {
            private readonly Scene _scene = new Scene();

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