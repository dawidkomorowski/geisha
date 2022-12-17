using System;
using Autofac;
using Geisha.Engine.Core;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.Systems;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests
{
    [TestFixture]
    public class ComponentsRegistryTests
    {
        [Test]
        public void RegisterComponentFactory_ShouldRegisterComponentFactoryAsSingleInstance()
        {
            // Arrange
            var containerBuilder = new ContainerBuilder();
            var componentsRegistry = new ComponentsRegistry(containerBuilder);

            // Act
            componentsRegistry.RegisterComponentFactory<TestComponentFactory>();

            // Assert
            var container = containerBuilder.Build();
            var actual1 = container.Resolve<IComponentFactory>();
            var actual2 = container.Resolve<IComponentFactory>();
            Assert.That(actual1, Is.EqualTo(actual2));
        }

        [Test]
        public void RegisterSceneBehaviorFactory_ShouldRegisterSceneBehaviorFactoryAsSingleInstance()
        {
            // Arrange
            var containerBuilder = new ContainerBuilder();
            var componentsRegistry = new ComponentsRegistry(containerBuilder);

            // Act
            componentsRegistry.RegisterSceneBehaviorFactory<TestSceneBehaviorFactory>();

            // Assert
            var container = containerBuilder.Build();
            var actual1 = container.Resolve<ISceneBehaviorFactory>();
            var actual2 = container.Resolve<ISceneBehaviorFactory>();
            Assert.That(actual1, Is.EqualTo(actual2));
        }

        [Test]
        public void RegisterSystem_ShouldRegisterSystem_AsCustomGameLoopStep_AndAsSceneObserver_AndAsSingleInstance()
        {
            // Arrange
            var containerBuilder = new ContainerBuilder();
            var componentsRegistry = new ComponentsRegistry(containerBuilder);

            // Act
            componentsRegistry.RegisterSystem<TestCustomSystem>();

            // Assert
            var container = containerBuilder.Build();
            var actual1 = container.Resolve<ICustomGameLoopStep>();
            var actual2 = container.Resolve<ICustomGameLoopStep>();
            var actual3 = container.Resolve<ISceneObserver>();
            var actual4 = container.Resolve<ISceneObserver>();
            Assert.That(actual1, Is.EqualTo(actual2));
            Assert.That(actual1, Is.EqualTo(actual3));
            Assert.That(actual1, Is.EqualTo(actual4));
        }

        [Test]
        public void RegisterSingleInstance_ShouldRegisterImplementation_AsSingleInstance()
        {
            // Arrange
            var containerBuilder = new ContainerBuilder();
            var componentsRegistry = new ComponentsRegistry(containerBuilder);

            // Act
            componentsRegistry.RegisterSingleInstance<TestService>();

            // Assert
            var container = containerBuilder.Build();
            var actual1 = container.Resolve<TestService>();
            var actual2 = container.Resolve<TestService>();
            Assert.That(actual1, Is.EqualTo(actual2));
        }

        [Test]
        public void RegisterSingleInstance_ShouldRegisterImplementation_AsInterface_AndAsSingleInstance()
        {
            // Arrange
            var containerBuilder = new ContainerBuilder();
            var componentsRegistry = new ComponentsRegistry(containerBuilder);

            // Act
            componentsRegistry.RegisterSingleInstance<TestService, ITestService>();

            // Assert
            var container = containerBuilder.Build();
            var actual1 = container.Resolve<ITestService>();
            var actual2 = container.Resolve<ITestService>();
            Assert.That(actual1, Is.EqualTo(actual2));
        }

        private sealed class TestComponentFactory : ComponentFactory<Component>
        {
            protected override Component CreateComponent(Entity entity) => throw new NotSupportedException();
        }

        private sealed class TestSceneBehaviorFactory : ISceneBehaviorFactory
        {
            public string BehaviorName => throw new NotSupportedException();
            public SceneBehavior Create(Scene scene) => throw new NotSupportedException();
        }

        private sealed class TestCustomSystem : ICustomSystem
        {
            public string Name => throw new NotSupportedException();

            public void ProcessFixedUpdate()
            {
                throw new NotSupportedException();
            }

            public void ProcessUpdate(GameTime gameTime)
            {
                throw new NotSupportedException();
            }

            public void OnEntityCreated(Entity entity)
            {
                throw new NotSupportedException();
            }

            public void OnEntityRemoved(Entity entity)
            {
                throw new NotSupportedException();
            }

            public void OnEntityParentChanged(Entity entity, Entity? oldParent, Entity? newParent)
            {
                throw new NotSupportedException();
            }

            public void OnComponentCreated(Component component)
            {
                throw new NotSupportedException();
            }

            public void OnComponentRemoved(Component component)
            {
                throw new NotSupportedException();
            }
        }

        private interface ITestService
        {
        }

        private sealed class TestService : ITestService
        {
        }
    }
}