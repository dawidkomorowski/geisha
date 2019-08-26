using System;
using System.Linq;
using Autofac;
using Geisha.Common.Extensibility;
using Geisha.Common.TestUtils;
using NUnit.Framework;

namespace Geisha.Common.IntegrationTests.Extensibility
{
    [TestFixture]
    public class ExtensionsManagerIntegrationTests
    {
        [Test]
        public void LoadExtensions_ShouldLoadAndReturnExtensions()
        {
            // Arrange
            var extensionsManager = new ExtensionsManager();

            // Act
            var extensions = extensionsManager.LoadExtensions(Utils.TestDirectory).ToList();

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
            extensionsManager.LoadExtensions(Utils.TestDirectory);

            // Act
            // Assert
            Assert.That(() => { extensionsManager.LoadExtensions(Utils.TestDirectory); }, Throws.InvalidOperationException);
        }

        // ReSharper disable once ClassNeverInstantiated.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public sealed class TestExtension1 : IExtension
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

        // ReSharper disable once ClassNeverInstantiated.Global
        // ReSharper disable once MemberCanBePrivate.Global
        public sealed class TestExtension2 : IExtension
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