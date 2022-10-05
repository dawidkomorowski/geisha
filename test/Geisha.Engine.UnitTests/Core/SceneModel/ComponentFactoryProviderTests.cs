using Geisha.Engine.Core.SceneModel;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel
{
    [TestFixture]
    public class ComponentFactoryProviderTests
    {
        [Test]
        public void Initialize_ThrowsArgumentException_GivenFactoriesWithDuplicatedComponentType()
        {
            // Arrange
            var factory1 = CreateFactory<TestComponent1>("Component 1");
            var factory2 = CreateFactory<TestComponent1>("Component 2");
            var factory3 = CreateFactory<TestComponent2>("Component 3");

            var factoryProvider = new ComponentFactoryProvider();

            // Act
            // Assert
            Assert.That(() => factoryProvider.Initialize(new[] { factory1, factory2, factory3 }),
                Throws.ArgumentException.With.Message.Contains(typeof(TestComponent1).FullName));
        }

        [Test]
        public void Initialize_ThrowsArgumentException_GivenFactoriesWithDuplicatedComponentId()
        {
            // Arrange
            var factory1 = CreateFactory<TestComponent1>("Component 1");
            var factory2 = CreateFactory<TestComponent2>("Component 1");
            var factory3 = CreateFactory<TestComponent3>("Component 2");

            var factoryProvider = new ComponentFactoryProvider();

            // Act
            // Assert
            Assert.That(() => factoryProvider.Initialize(new[] { factory1, factory2, factory3 }),
                Throws.ArgumentException.With.Message.Contains("Component 1"));
        }

        [Test]
        public void Initialize_ThrowsException_WhenAlreadyInitialized()
        {
            // Arrange
            var factory = CreateFactory<TestComponent1>("Component 1");

            var factoryProvider = new ComponentFactoryProvider();
            factoryProvider.Initialize(new[] { factory });

            // Act
            // Assert
            Assert.That(() => factoryProvider.Initialize(new[] { factory }), Throws.InvalidOperationException);
        }

        [Test]
        public void Get_ThrowsException_WhenNotInitialized_GivenComponentTypeGenericParameter()
        {
            // Arrange
            var factoryProvider = new ComponentFactoryProvider();

            // Act
            // Assert
            Assert.That(() => factoryProvider.Get<TestComponent1>(), Throws.InvalidOperationException);
        }

        [Test]
        public void Get_ThrowsException_GivenComponentTypeGenericParameterForWhichThereIsNoFactoryAvailable()
        {
            // Arrange
            var factory = CreateFactory<TestComponent1>("Component 1");

            var factoryProvider = new ComponentFactoryProvider();
            factoryProvider.Initialize(new[] { factory });

            // Act
            // Assert
            Assert.That(() => factoryProvider.Get<TestComponent2>(),
                Throws.TypeOf<ComponentFactoryNotFoundException>()
                    .With.Message.Contains(typeof(TestComponent2).FullName)
                    .And.Message.Contains(typeof(TestComponent1).FullName));
        }

        [Test]
        public void Get_ShouldReturnFactory_GivenComponentTypeGenericParameter()
        {
            // Arrange
            var factory1 = CreateFactory<TestComponent1>("Component 1");
            var factory2 = CreateFactory<TestComponent2>("Component 2");
            var factory3 = CreateFactory<TestComponent3>("Component 3");

            var factoryProvider = new ComponentFactoryProvider();
            factoryProvider.Initialize(new[] { factory1, factory2, factory3 });

            // Act
            var actual = factoryProvider.Get<TestComponent2>();

            // Assert
            Assert.That(actual, Is.EqualTo(factory2));
        }

        [Test]
        public void Get_ThrowsException_WhenNotInitialized_GivenComponentType()
        {
            // Arrange
            var factoryProvider = new ComponentFactoryProvider();

            // Act
            // Assert
            Assert.That(() => factoryProvider.Get(typeof(TestComponent1)), Throws.InvalidOperationException);
        }

        [Test]
        public void Get_ThrowsException_GivenComponentTypeForWhichThereIsNoFactoryAvailable()
        {
            // Arrange
            var factory = CreateFactory<TestComponent1>("Component 1");

            var factoryProvider = new ComponentFactoryProvider();
            factoryProvider.Initialize(new[] { factory });

            // Act
            // Assert
            Assert.That(() => factoryProvider.Get(typeof(TestComponent2)),
                Throws.TypeOf<ComponentFactoryNotFoundException>()
                    .With.Message.Contains(typeof(TestComponent2).FullName)
                    .And.Message.Contains(typeof(TestComponent1).FullName));
        }

        [Test]
        public void Get_ShouldReturnFactory_GivenComponentType()
        {
            // Arrange
            var factory1 = CreateFactory<TestComponent1>("Component 1");
            var factory2 = CreateFactory<TestComponent2>("Component 2");
            var factory3 = CreateFactory<TestComponent3>("Component 3");

            var factoryProvider = new ComponentFactoryProvider();
            factoryProvider.Initialize(new[] { factory1, factory2, factory3 });

            // Act
            var actual = factoryProvider.Get(typeof(TestComponent2));

            // Assert
            Assert.That(actual, Is.EqualTo(factory2));
        }

        [Test]
        public void Get_ThrowsException_WhenNotInitialized_GivenComponentId()
        {
            // Arrange
            var factoryProvider = new ComponentFactoryProvider();

            // Act
            // Assert
            Assert.That(() => factoryProvider.Get(new ComponentId("Component 1")), Throws.InvalidOperationException);
        }

        [Test]
        public void Get_ThrowsException_GivenComponentIdForWhichThereIsNoFactoryAvailable()
        {
            // Arrange
            var factory = CreateFactory<TestComponent1>("Component 1");

            var factoryProvider = new ComponentFactoryProvider();
            factoryProvider.Initialize(new[] { factory });

            // Act
            // Assert
            Assert.That(() => factoryProvider.Get(new ComponentId("Component 2")),
                Throws.TypeOf<ComponentFactoryNotFoundException>()
                    .With.Message.Contains("Component 2")
                    .And.Message.Contains("Component 1"));
        }

        [Test]
        public void Get_ShouldReturnFactory_GivenComponentId()
        {
            // Arrange
            var factory1 = CreateFactory<TestComponent1>("Component 1");
            var factory2 = CreateFactory<TestComponent2>("Component 2");
            var factory3 = CreateFactory<TestComponent3>("Component 3");

            var factoryProvider = new ComponentFactoryProvider();
            factoryProvider.Initialize(new[] { factory1, factory2, factory3 });

            // Act
            var actual = factoryProvider.Get(new ComponentId("Component 2"));

            // Assert
            Assert.That(actual, Is.EqualTo(factory2));
        }

        #region Helpers

        private static IComponentFactory CreateFactory<TComponent>(string componentId) where TComponent : Component
        {
            var factory = Substitute.For<IComponentFactory>();
            factory.ComponentType.Returns(typeof(TComponent));
            factory.ComponentId.Returns(new ComponentId(componentId));
            return factory;
        }

        [ComponentId("TestComponent1")]
        private sealed class TestComponent1 : Component
        {
            public TestComponent1(Entity entity) : base(entity)
            {
            }
        }

        [ComponentId("TestComponent2")]
        private sealed class TestComponent2 : Component
        {
            public TestComponent2(Entity entity) : base(entity)
            {
            }
        }

        [ComponentId("TestComponent3")]
        private sealed class TestComponent3 : Component
        {
            public TestComponent3(Entity entity) : base(entity)
            {
            }
        }

        #endregion
    }
}