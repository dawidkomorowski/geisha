using Geisha.Engine.Core.SceneModel;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.SceneModel
{
    [TestFixture]
    public class SceneBehaviorFactoryProviderTests
    {
        [Test]
        public void Initialize_ThrowsArgumentException_GivenFactoriesWithDuplicatedBehaviorNames()
        {
            // Arrange
            var factory1 = Substitute.For<ISceneBehaviorFactory>();
            factory1.BehaviorName.Returns("Behavior 1");
            var factory2 = Substitute.For<ISceneBehaviorFactory>();
            factory2.BehaviorName.Returns("Behavior 1");
            var factory3 = Substitute.For<ISceneBehaviorFactory>();
            factory3.BehaviorName.Returns("Behavior 2");

            var factoryProvider = new SceneBehaviorFactoryProvider();

            // Act
            // Assert
            Assert.That(() => factoryProvider.Initialize(new[] { factory1, factory2, factory3 }),
                Throws.ArgumentException.With.Message.Contains("Behavior 1"));
        }

        [Test]
        public void Initialize_ThrowsException_WhenAlreadyInitialized()
        {
            // Arrange
            var factory1 = Substitute.For<ISceneBehaviorFactory>();
            factory1.BehaviorName.Returns("Behavior 1");
            var factory2 = Substitute.For<ISceneBehaviorFactory>();
            factory2.BehaviorName.Returns("Behavior 2");
            var factory3 = Substitute.For<ISceneBehaviorFactory>();
            factory3.BehaviorName.Returns("Behavior 3");

            var factoryProvider = new SceneBehaviorFactoryProvider();
            factoryProvider.Initialize(new[] { factory1, factory2, factory3 });

            // Act
            // Assert
            Assert.That(() => factoryProvider.Initialize(new[] { factory1, factory2, factory3 }), Throws.InvalidOperationException);
        }

        [Test]
        public void Get_ThrowsException_WhenNotInitialized()
        {
            // Arrange
            var factoryProvider = new SceneBehaviorFactoryProvider();

            // Act
            // Assert
            Assert.That(() => factoryProvider.Get("Behavior"), Throws.InvalidOperationException);
        }

        [Test]
        public void Get_ThrowsException_GivenBehaviorNameForWhichThereIsNoFactoryAvailable()
        {
            // Arrange
            var factory = Substitute.For<ISceneBehaviorFactory>();
            factory.BehaviorName.Returns("Behavior 1");

            var factoryProvider = new SceneBehaviorFactoryProvider();
            factoryProvider.Initialize(new[] { factory });

            // Act
            // Assert
            Assert.That(() => factoryProvider.Get("Not available behavior"),
                Throws.TypeOf<SceneBehaviorFactoryNotFoundException>()
                    .With.Message.Contains("Behavior 1")
                    .And.Message.Contains("Not available behavior"));
        }

        [Test]
        public void Get_ShouldReturnFactory_GivenBehaviorName()
        {
            // Arrange
            var factory1 = Substitute.For<ISceneBehaviorFactory>();
            factory1.BehaviorName.Returns("Behavior 1");
            var factory2 = Substitute.For<ISceneBehaviorFactory>();
            factory2.BehaviorName.Returns("Behavior 2");
            var factory3 = Substitute.For<ISceneBehaviorFactory>();
            factory3.BehaviorName.Returns("Behavior 3");

            var factoryProvider = new SceneBehaviorFactoryProvider();
            factoryProvider.Initialize(new[] { factory1, factory2, factory3 });

            // Act
            var actual = factoryProvider.Get("Behavior 2");

            // Assert
            Assert.That(actual, Is.EqualTo(factory2));
        }
    }
}