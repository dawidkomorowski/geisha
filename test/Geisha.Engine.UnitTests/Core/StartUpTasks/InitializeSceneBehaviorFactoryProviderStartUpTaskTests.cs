using System.Linq;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Core.StartUpTasks;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.StartUpTasks
{
    [TestFixture]
    public class InitializeSceneBehaviorFactoryProviderStartUpTaskTests
    {
        [Test]
        public void Run_ShouldInitializeSceneBehaviorFactoryProvider()
        {
            // Arrange
            var factoryProvider = Substitute.For<ISceneBehaviorFactoryProviderInternal>();
            var factories = Enumerable.Empty<ISceneBehaviorFactory>();

            // ReSharper disable once PossibleMultipleEnumeration
            var startUpTask = new InitializeSceneBehaviorFactoryProviderStartUpTask(factoryProvider, factories);

            // Act
            startUpTask.Run();

            // Assert
            // ReSharper disable once PossibleMultipleEnumeration
            factoryProvider.Received(1).Initialize(factories);
        }
    }
}