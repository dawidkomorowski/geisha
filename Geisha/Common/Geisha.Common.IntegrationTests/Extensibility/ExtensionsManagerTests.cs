using System;
using System.Linq;
using Autofac;
using Geisha.Common.Extensibility;
using NUnit.Framework;

namespace Geisha.Common.IntegrationTests.Extensibility
{
    [TestFixture]
    public class ExtensionsManagerTests
    {
        [Test]
        public void LoadExtensions_ShouldLoadAndReturnExtensions()
        {
            // Arrange
            var extensionsManager = new ExtensionsManager();

            // Act
            var extensions = extensionsManager.LoadExtensions().ToList();

            // Assert
            Assert.That(extensions, Has.Count.GreaterThanOrEqualTo(2));
            Assert.That(extensions.Select(e => e.Name), Contains.Item(nameof(TestExtension1)));
            Assert.That(extensions.Select(e => e.Name), Contains.Item(nameof(TestExtension2)));
        }

        [Test]
        public void LoadExtensions_ShouldThrowException_WhenCalledMultipleTimes()
        {
            // Arrange
            var extensionsManager = new ExtensionsManager();
            extensionsManager.LoadExtensions();

            // Act
            // Assert
            Assert.That(() => { extensionsManager.LoadExtensions(); }, Throws.InvalidOperationException);
        }

        [Test]
        public void LoadExtensions_ShouldThrowException_WhenExtensionsManagerWasDisposed()
        {
            // Arrange
            var extensionsManager = new ExtensionsManager();
            extensionsManager.Dispose();

            // Act
            // Assert
            Assert.That(() => { extensionsManager.LoadExtensions(); }, Throws.TypeOf<ObjectDisposedException>());
        }

        [Test]
        public void Dispose_ShouldNotThrowException_WhenExtensionsManagerDisposed()
        {
            // Arrange
            var extensionsManager = new ExtensionsManager();
            extensionsManager.Dispose();

            // Act
            // Assert
            Assert.That(() => { extensionsManager.Dispose(); }, Throws.Nothing);
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class TestExtension1 : IExtension
        {
            public string Name => nameof(TestExtension1);
            public string Description => $"Description of {Name}";
            public string Category => "Test";
            public string Author => "Geisha";
            public Version Version => new Version(1, 0);

            public void Register(ContainerBuilder containerBuilder)
            {
            }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        private sealed class TestExtension2 : IExtension
        {
            public string Name => nameof(TestExtension2);
            public string Description => $"Description of {Name}";
            public string Category => "Test";
            public string Author => "Geisha";
            public Version Version => new Version(1, 0);

            public void Register(ContainerBuilder containerBuilder)
            {
            }
        }
    }
}