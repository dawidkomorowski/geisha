using Geisha.Engine.Core.Components;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.Core.UnitTests.Systems
{
    [TestFixture]
    public class BehaviourSystemTests
    {
        private const double DeltaTime = 0.1;
        private BehaviourSystem _behaviourSystem;

        [SetUp]
        public void SetUp()
        {
            _behaviourSystem = new BehaviourSystem();
        }

        [Test]
        public void Update_ShouldSetEntityOnAllBehaviourComponents()
        {
            // Arrange
            var scene = new SceneWithEntitiesWithBehaviourComponents();
            _behaviourSystem.Scene = scene;

            // Act
            _behaviourSystem.Update(DeltaTime);

            // Assert
            Assert.That(scene.Behaviour1OfEntity1.Entity, Is.EqualTo(scene.EntityWithBehaviour1));
            Assert.That(scene.Behaviour2OfEntity1.Entity, Is.EqualTo(scene.EntityWithBehaviour1));
            Assert.That(scene.Behaviour1OfEntity2.Entity, Is.EqualTo(scene.EntityWithBehaviour2));
        }

        [Test]
        public void Update_ShouldCallOnUpdateOnAllBehaviourComponents()
        {
            // Arrange
            var scene = new SceneWithEntitiesWithBehaviourComponents();
            _behaviourSystem.Scene = scene;

            // Act
            _behaviourSystem.Update(DeltaTime);

            // Assert
            scene.Behaviour1OfEntity1.Received(1).OnUpdate(DeltaTime);
            scene.Behaviour2OfEntity1.Received(1).OnUpdate(DeltaTime);
            scene.Behaviour1OfEntity2.Received(1).OnUpdate(DeltaTime);
        }

        [Test]
        public void FixedUpdate_ShouldSetEntityOnAllBehaviourComponents()
        {
            // Arrange
            var scene = new SceneWithEntitiesWithBehaviourComponents();
            _behaviourSystem.Scene = scene;

            // Act
            _behaviourSystem.FixedUpdate();

            // Assert
            Assert.That(scene.Behaviour1OfEntity1.Entity, Is.EqualTo(scene.EntityWithBehaviour1));
            Assert.That(scene.Behaviour2OfEntity1.Entity, Is.EqualTo(scene.EntityWithBehaviour1));
            Assert.That(scene.Behaviour1OfEntity2.Entity, Is.EqualTo(scene.EntityWithBehaviour2));
        }

        [Test]
        public void FixedUpdate_ShouldCallOnFixedUpdateOnAllBehaviourComponents()
        {
            // Arrange
            var scene = new SceneWithEntitiesWithBehaviourComponents();
            _behaviourSystem.Scene = scene;

            // Act
            _behaviourSystem.FixedUpdate();

            // Assert
            scene.Behaviour1OfEntity1.Received(1).OnFixedUpdate();
            scene.Behaviour2OfEntity1.Received(1).OnFixedUpdate();
            scene.Behaviour1OfEntity2.Received(1).OnFixedUpdate();
        }

        private class SceneWithEntitiesWithBehaviourComponents : Scene
        {
            public Entity EntityWithoutBehaviour { get; }
            public Entity EntityWithBehaviour1 { get; }
            public Entity EntityWithBehaviour2 { get; }

            public Behaviour Behaviour1OfEntity1 { get; }
            public Behaviour Behaviour2OfEntity1 { get; }
            public Behaviour Behaviour1OfEntity2 { get; }


            public SceneWithEntitiesWithBehaviourComponents()
            {
                RootEntity = new Entity();

                EntityWithoutBehaviour = new Entity {Parent = RootEntity};
                EntityWithoutBehaviour.AddComponent(new Transform());

                Behaviour1OfEntity1 = Substitute.For<Behaviour>();
                Behaviour2OfEntity1 = Substitute.For<Behaviour>();
                Behaviour1OfEntity2 = Substitute.For<Behaviour>();

                EntityWithBehaviour1 = new Entity {Parent = RootEntity};
                EntityWithBehaviour1.AddComponent(new Transform());
                EntityWithBehaviour1.AddComponent(Behaviour1OfEntity1);
                EntityWithBehaviour1.AddComponent(Behaviour2OfEntity1);

                EntityWithBehaviour2 = new Entity {Parent = RootEntity};
                EntityWithBehaviour2.AddComponent(new Transform());
                EntityWithBehaviour2.AddComponent(Behaviour1OfEntity2);
            }
        }
    }
}