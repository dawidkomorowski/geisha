using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.StartUpTasks;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.StartUpTasks
{
    [TestFixture]
    public class InitializeComponentFactoryProviderStartUpTaskTests
    {
        [Test]
        public void Run_ShouldInitializeComponentFactoryProvider()
        {
            // Arrange
            var componentFactoryProvider = Substitute.For<IComponentFactoryProvider>();
            var factories = Enumerable.Empty<IComponentFactory>();

            // ReSharper disable once PossibleMultipleEnumeration
            var startUpTask = new InitializeComponentFactoryProviderStartUpTask(componentFactoryProvider, factories);

            // Act
            startUpTask.Run();

            // Assert
            // ReSharper disable once PossibleMultipleEnumeration
            componentFactoryProvider.Received(1).Initialize(factories);
        }
    }
}