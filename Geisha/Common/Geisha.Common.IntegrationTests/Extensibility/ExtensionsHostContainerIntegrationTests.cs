using System;
using Autofac;
using Geisha.Common.Extensibility;
using NUnit.Framework;

namespace Geisha.Common.IntegrationTests.Extensibility
{
    [TestFixture]
    public class ExtensionsHostContainerIntegrationTests
    {
        [Test]
        public void CompositionRoot_ShouldBeAvailableAfterConstructor_WhenExtensionExistsThatRegistersCompositionRootType()
        {
            // Arrange
            // Act
            var extensionsHostContainer = new ExtensionsHostContainer<Root>();

            // Assert
            Assert.That(extensionsHostContainer.CompositionRoot, Is.Not.Null);
            Assert.That(extensionsHostContainer.CompositionRoot, Is.InstanceOf<Root>());
        }

        [Test]
        public void CompositionRoot_ShouldBeAvailableAfterConstructor_WhenItDependsOnComponentFromAnotherExtension()
        {
            // Arrange
            var extensionsHostContainer = new ExtensionsHostContainer<DependentRoot>();

            // Act
            // Assert
            Assert.That(extensionsHostContainer.CompositionRoot, Is.Not.Null);
            Assert.That(extensionsHostContainer.CompositionRoot, Is.InstanceOf<DependentRoot>());
        }

        [Test]
        public void CompositionRoot_ShouldBeAvailableAfterConstructor_WhenItIsRegisteredByHostServices()
        {
            // Arrange
            var someHostService = new SomeHostService();
            var hostServices = new HostServices(someHostService);
            var extensionsHostContainer = new ExtensionsHostContainer<SomeHostService>(hostServices);

            // Act
            // Assert
            Assert.That(extensionsHostContainer.CompositionRoot, Is.EqualTo(someHostService));
            Assert.That(extensionsHostContainer.CompositionRoot, Is.InstanceOf<SomeHostService>());
        }

        [Test]
        public void Dispose_ShouldDisposeComposedObjects()
        {
            // Arrange
            var extensionsHostContainer = new ExtensionsHostContainer<Root>();
            var composedObject = extensionsHostContainer.CompositionRoot;

            // Assume
            Assume.That(composedObject.Disposed, Is.False);

            // Act
            extensionsHostContainer.Dispose();

            // Assert
            Assert.That(composedObject.Disposed, Is.True);
        }

        [Test]
        public void Dispose_ShouldNotThrowException_WhenExtensionsHostContainerDisposed()
        {
            // Arrange
            var extensionsHostContainer = new ExtensionsHostContainer<Root>();
            extensionsHostContainer.Dispose();

            // Act
            // Assert
            Assert.That(() => { extensionsHostContainer.Dispose(); }, Throws.Nothing);
        }

        #region Root

        private sealed class Root : IDisposable
        {
            public bool Disposed { get; private set; }

            public void Dispose()
            {
                Disposed = true;
            }
        }

        private sealed class RootTestExtension : IExtension
        {
            public string Name => nameof(RootTestExtension);
            public string Description => $"Description of {Name}";
            public string Category => "Test";
            public string Author => "Geisha";
            public Version Version => new Version(1, 0);

            public void Register(ContainerBuilder containerBuilder)
            {
                containerBuilder.RegisterType<Root>().AsSelf().SingleInstance();
            }
        }

        #endregion

        #region DependentRoot

        private sealed class DependentRoot
        {
            private readonly RootDependency _rootDependency;

            public DependentRoot(RootDependency rootDependency)
            {
                _rootDependency = rootDependency;
            }
        }

        private sealed class DependentRootTestExtension : IExtension
        {
            public string Name => nameof(DependentRootTestExtension);
            public string Description => $"Description of {Name}";
            public string Category => "Test";
            public string Author => "Geisha";
            public Version Version => new Version(1, 0);

            public void Register(ContainerBuilder containerBuilder)
            {
                containerBuilder.RegisterType<DependentRoot>().AsSelf().SingleInstance();
            }
        }

        private sealed class RootDependency
        {
        }

        private sealed class RootDependencyTestExtension : IExtension
        {
            public string Name => nameof(RootDependencyTestExtension);
            public string Description => $"Description of {Name}";
            public string Category => "Test";
            public string Author => "Geisha";
            public Version Version => new Version(1, 0);

            public void Register(ContainerBuilder containerBuilder)
            {
                containerBuilder.RegisterType<RootDependency>().AsSelf().SingleInstance();
            }
        }

        #endregion

        #region HostService

        private sealed class SomeHostService
        {
        }

        private sealed class HostServices : IHostServices
        {
            private readonly SomeHostService _someHostService;

            public HostServices(SomeHostService someHostService)
            {
                _someHostService = someHostService;
            }

            public void Register(ContainerBuilder containerBuilder)
            {
                containerBuilder.RegisterInstance(_someHostService);
            }
        }

        #endregion
    }
}