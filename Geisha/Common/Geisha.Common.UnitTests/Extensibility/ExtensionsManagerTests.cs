using System;
using System.Linq;
using Autofac;
using Geisha.Common.Extensibility;
using NUnit.Framework;

namespace Geisha.Common.UnitTests.Extensibility
{
    // TODO Add tests (disposal, something else?)
    [TestFixture]
    public class ExtensionsManagerTests
    {
        [Test]
        public void LoadExtensions_ShouldLoadAndReturnTwoExtensions()
        {
            // Arrange
            var extensionsManager = new ExtensionsManager();

            // Act
            var extensions = extensionsManager.LoadExtensions();

            // Assert
            Assert.That(extensions, Has.Count.EqualTo(2));
            Assert.That(extensions.Select(e => e.Name), Contains.Item(nameof(TestExtension1)));
            Assert.That(extensions.Select(e => e.Name), Contains.Item(nameof(TestExtension2)));
        }

        private class TestExtension1 : IExtension
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

        private class TestExtension2 : IExtension
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