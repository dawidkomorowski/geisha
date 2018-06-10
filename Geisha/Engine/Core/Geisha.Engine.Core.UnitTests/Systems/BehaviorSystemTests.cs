using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Systems
{
    [TestFixture]
    public class BehaviorSystemTests
    {
        private const double DeltaTime = 0.1;
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
            var scene = new SceneWithEntitiesWithBehaviorComponents();

            // Act
            _behaviorSystem.FixedUpdate(scene);

            // Assert
            Assert.That(scene.Behavior1OfEntity1.Entity, Is.EqualTo(scene.EntityWithBehavior1));
            Assert.That(scene.Behavior2OfEntity1.Entity, Is.EqualTo(scene.EntityWithBehavior1));
            Assert.That(scene.Behavior1OfEntity2.Entity, Is.EqualTo(scene.EntityWithBehavior2));
        }

        [Test]
        public void FixedUpdate_ShouldCallOnStartOnce_WhenUpdateExecutedTwice()
        {
            // Arrange
            var scene = new SceneWithEntitiesWithBehaviorComponents();

            // Act
            _behaviorSystem.FixedUpdate(scene);
            _behaviorSystem.FixedUpdate(scene);

            // Assert
            scene.Behavior1OfEntity1.Received(1).OnStart();
        }

        [Test]
        public void FixedUpdate_ShouldCallOnStartBeforeOnFixedUpdate()
        {
            // Arrange
            var scene = new SceneWithEntitiesWithBehaviorComponents();

            // Act
            _behaviorSystem.FixedUpdate(scene);

            // Assert
            Received.InOrder(() =>
            {
                scene.Behavior1OfEntity1.Received(1).OnStart();
                scene.Behavior1OfEntity1.Received(1).OnFixedUpdate();
            });
        }

        [Test]
        public void FixedUpdate_ShouldCallOnFixedUpdateOnAllBehaviorComponents()
        {
            // Arrange
            var scene = new SceneWithEntitiesWithBehaviorComponents();

            // Act
            _behaviorSystem.FixedUpdate(scene);

            // Assert
            scene.Behavior1OfEntity1.Received(1).OnFixedUpdate();
            scene.Behavior2OfEntity1.Received(1).OnFixedUpdate();
            scene.Behavior1OfEntity2.Received(1).OnFixedUpdate();
        }

        // This test keeps implementation free of invalidating enumerator / enumerable exception while looping over entities.
        [Test]
        public void FixedUpdate_ShouldRemoveEntityWithRemoveFromSceneBehavior()
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();
            entity.AddComponent(new RemoveFromSceneBehavior());

            scene.AddEntity(entity);

            // Act
            _behaviorSystem.FixedUpdate(scene);

            // Assert
            Assert.That(scene.AllEntities, Does.Not.Contains(entity));
        }

        // This test keeps implementation free of invalidating enumerator / enumerable exception while looping over components.
        [TestCase(true, false)]
        [TestCase(false, true)]
        public void FixedUpdate_ShouldAddComponentToEntityInAddComponentBehavior(bool addComponentOnStart, bool addComponentOnFixedUpdate)
        {
            // Arrange
            var scene = new Scene();
            var entity = new Entity();
            entity.AddComponent(new AddComponentBehavior
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
            entity.AddComponent(new AddComponentBehavior
            {
                AddComponentOnStart = addComponentOnStart,
                AddComponentOnUpdate = addComponentOnUpdate
            });

            scene.AddEntity(entity);

            // Act
            _behaviorSystem.Update(scene, DeltaTime);

            // Assert
            Assert.That(entity.Components.Count, Is.EqualTo(2));
        }

        [Test]
        public void Update_ShouldCallOnUpdateOnAllBehaviorComponents()
        {
            // Arrange
            var scene = new SceneWithEntitiesWithBehaviorComponents();

            // Act
            _behaviorSystem.Update(scene, DeltaTime);

            // Assert
            scene.Behavior1OfEntity1.Received(1).OnUpdate(DeltaTime);
            scene.Behavior2OfEntity1.Received(1).OnUpdate(DeltaTime);
            scene.Behavior1OfEntity2.Received(1).OnUpdate(DeltaTime);
        }

        private class SceneWithEntitiesWithBehaviorComponents : Scene
        {
            public Entity EntityWithBehavior1 { get; }
            public Entity EntityWithBehavior2 { get; }

            public Behavior Behavior1OfEntity1 { get; }
            public Behavior Behavior2OfEntity1 { get; }
            public Behavior Behavior1OfEntity2 { get; }


            public SceneWithEntitiesWithBehaviorComponents()
            {
                Behavior1OfEntity1 = Substitute.For<Behavior>();
                Behavior2OfEntity1 = Substitute.For<Behavior>();
                Behavior1OfEntity2 = Substitute.For<Behavior>();

                EntityWithBehavior1 = new Entity();
                EntityWithBehavior1.AddComponent(new Transform());
                EntityWithBehavior1.AddComponent(Behavior1OfEntity1);
                EntityWithBehavior1.AddComponent(Behavior2OfEntity1);

                EntityWithBehavior2 = new Entity();
                EntityWithBehavior2.AddComponent(new Transform());
                EntityWithBehavior2.AddComponent(Behavior1OfEntity2);

                AddEntity(EntityWithBehavior1);
                AddEntity(EntityWithBehavior2);
            }
        }

        private class RemoveFromSceneBehavior : Behavior
        {
            public override void OnFixedUpdate()
            {
                Entity.Scene.RemoveEntity(Entity);
            }
        }

        private class AddComponentBehavior : Behavior
        {
            public bool AddComponentOnStart { get; set; }
            public bool AddComponentOnUpdate { get; set; }
            public bool AddComponentOnFixedUpdate { get; set; }

            public override void OnStart()
            {
                base.OnStart();
                if (AddComponentOnStart) Entity.AddComponent(CreateNewComponent());
            }

            public override void OnUpdate(double deltaTime)
            {
                base.OnUpdate(deltaTime);
                if (AddComponentOnUpdate) Entity.AddComponent(CreateNewComponent());
            }

            public override void OnFixedUpdate()
            {
                base.OnFixedUpdate();
                if (AddComponentOnFixedUpdate) Entity.AddComponent(CreateNewComponent());
            }

            private static IComponent CreateNewComponent() => Substitute.For<IComponent>();
        }
    }
}