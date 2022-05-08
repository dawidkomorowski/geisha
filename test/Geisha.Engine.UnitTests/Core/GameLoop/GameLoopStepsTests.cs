using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core;
using Geisha.Engine.Core.GameLoop;
using Geisha.Engine.Core.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.GameLoop
{
    [TestFixture]
    public class GameLoopStepsTests
    {
        private IAnimationSystem _animationSystem = null!;
        private IAudioSystem _audioSystem = null!;
        private IBehaviorSystem _behaviorSystem = null!;
        private IInputSystem _inputSystem = null!;
        private IPhysicsSystem _physicsSystem = null!;
        private IRenderingSystem _renderingSystem = null!;

        [SetUp]
        public void SetUp()
        {
            _animationSystem = Substitute.For<IAnimationSystem>();
            _audioSystem = Substitute.For<IAudioSystem>();
            _behaviorSystem = Substitute.For<IBehaviorSystem>();
            _inputSystem = Substitute.For<IInputSystem>();
            _physicsSystem = Substitute.For<IPhysicsSystem>();
            _renderingSystem = Substitute.For<IRenderingSystem>();
        }

        [Test]
        public void Constructor_ShouldThrowException_GivenConfigurationWithDuplicatedCustomSystemsNames()
        {
            // Arrange
            const string customSystem1Name = "CustomSystem1";
            const string customSystem2Name = "CustomSystem2";

            var customSystem1 = Substitute.For<ICustomSystem>();
            customSystem1.Name.Returns(customSystem1Name);
            var customSystem2 = Substitute.For<ICustomSystem>();
            customSystem2.Name.Returns(customSystem2Name);

            // Act
            // Assert
            Assert.That(() =>
            {
                CreateGameLoopSteps(new[] { customSystem1, customSystem2 },
                    new[] { customSystem1Name, customSystem2Name, customSystem1Name });
            }, Throws.ArgumentException);
        }

        [Test]
        public void Constructor_ShouldThrowException_GivenCustomSystemsWithDuplicatedNames()
        {
            // Arrange
            const string customSystem1Name = "CustomSystem1";
            const string customSystem2Name = "CustomSystem2";

            var customSystem1 = Substitute.For<ICustomSystem>();
            customSystem1.Name.Returns(customSystem1Name);
            var customSystem2 = Substitute.For<ICustomSystem>();
            customSystem2.Name.Returns(customSystem1Name);

            // Act
            // Assert
            Assert.That(() =>
            {
                CreateGameLoopSteps(new[] { customSystem1, customSystem2 },
                    new[] { customSystem1Name, customSystem2Name });
            }, Throws.ArgumentException);
        }

        [Test]
        public void Constructor_ShouldThrowException_GivenConfigurationThatSpecifiesCustomSystemsThatAreNotProvided()
        {
            // Arrange
            const string customSystem1Name = "CustomSystem1";
            const string customSystem2Name = "CustomSystem2";
            const string customSystem3Name = "CustomSystem3";

            var customSystem1 = Substitute.For<ICustomSystem>();
            customSystem1.Name.Returns(customSystem1Name);
            var customSystem2 = Substitute.For<ICustomSystem>();
            customSystem2.Name.Returns(customSystem2Name);

            // Act
            // Assert
            Assert.That(() =>
            {
                CreateGameLoopSteps(new[] { customSystem1, customSystem2 },
                    new[] { customSystem1Name, customSystem2Name, customSystem3Name });
            }, Throws.ArgumentException);
        }

        [Test]
        public void SystemsNames_ShouldReturnAlphabeticalListOfNamesOfStandardSystems_GivenNoCustomSystems()
        {
            // Arrange
            // Act
            var gameLoopSteps = CreateGameLoopSteps();

            // Assert
            Assert.That(gameLoopSteps.SystemsNames, Is.EqualTo(new[]
            {
                gameLoopSteps.AnimationSystemName,
                gameLoopSteps.AudioSystemName,
                gameLoopSteps.BehaviorSystemName,
                gameLoopSteps.InputSystemName,
                gameLoopSteps.PhysicsSystemName,
                gameLoopSteps.RenderingSystemName
            }));
        }

        [Test]
        public void SystemsNames_ShouldReturnAlphabeticalListOfNamesOfStandardSystemsAndCustomSystems()
        {
            // Arrange
            const string customSystem1Name = "CustomSystem1";
            const string customSystem2Name = "CustomSystem2";
            const string customSystem3Name = "CustomSystem3";

            var customSystem1 = Substitute.For<ICustomSystem>();
            customSystem1.Name.Returns(customSystem1Name);
            var customSystem2 = Substitute.For<ICustomSystem>();
            customSystem2.Name.Returns(customSystem2Name);
            var customSystem3 = Substitute.For<ICustomSystem>();
            customSystem3.Name.Returns(customSystem3Name);

            // Act
            var gameLoopSteps = CreateGameLoopSteps(new[] { customSystem3, customSystem2, customSystem1 },
                new[] { customSystem2Name, customSystem3Name, customSystem1Name });

            // Assert
            Assert.That(gameLoopSteps.SystemsNames, Is.EqualTo(new[]
            {
                gameLoopSteps.AnimationSystemName,
                gameLoopSteps.AudioSystemName,
                gameLoopSteps.BehaviorSystemName,
                customSystem1Name,
                customSystem2Name,
                customSystem3Name,
                gameLoopSteps.InputSystemName,
                gameLoopSteps.PhysicsSystemName,
                gameLoopSteps.RenderingSystemName
            }));
        }

        [Test]
        public void SystemsNames_ShouldNotReturnNamesOfCustomSystemsThatAreNotSpecifiedByConfiguration()
        {
            // Arrange
            const string customSystem1Name = "CustomSystem1";
            const string customSystem2Name = "CustomSystem2";
            const string customSystem3Name = "CustomSystem3";

            var customSystem1 = Substitute.For<ICustomSystem>();
            customSystem1.Name.Returns(customSystem1Name);
            var customSystem2 = Substitute.For<ICustomSystem>();
            customSystem2.Name.Returns(customSystem2Name);
            var customSystem3 = Substitute.For<ICustomSystem>();
            customSystem3.Name.Returns(customSystem3Name);

            // Act
            var gameLoopSteps = CreateGameLoopSteps(new[] { customSystem1, customSystem2, customSystem3 },
                new[] { customSystem1Name, customSystem3Name });

            // Assert
            Assert.That(gameLoopSteps.SystemsNames, Is.EqualTo(new[]
            {
                gameLoopSteps.AnimationSystemName,
                gameLoopSteps.AudioSystemName,
                gameLoopSteps.BehaviorSystemName,
                customSystem1Name,
                customSystem3Name,
                gameLoopSteps.InputSystemName,
                gameLoopSteps.PhysicsSystemName,
                gameLoopSteps.RenderingSystemName
            }));
        }

        [Test]
        public void CustomSystems_ShouldReturnCustomSystemsInOrderSpecifiedByConfiguration()
        {
            // Arrange
            const string customSystem1Name = "CustomSystem1";
            const string customSystem2Name = "CustomSystem2";
            const string customSystem3Name = "CustomSystem3";

            var customSystem1 = Substitute.For<ICustomSystem>();
            customSystem1.Name.Returns(customSystem1Name);
            var customSystem2 = Substitute.For<ICustomSystem>();
            customSystem2.Name.Returns(customSystem2Name);
            var customSystem3 = Substitute.For<ICustomSystem>();
            customSystem3.Name.Returns(customSystem3Name);

            // Act
            var gameLoopSteps = CreateGameLoopSteps(new[] { customSystem1, customSystem2, customSystem3 },
                new[] { customSystem2Name, customSystem3Name, customSystem1Name });

            // Assert
            Assert.That(gameLoopSteps.CustomSystems, Is.EqualTo(new[]
            {
                customSystem2,
                customSystem3,
                customSystem1
            }));
        }

        [Test]
        public void CustomSystems_ShouldNotReturnCustomSystemsThatAreNotSpecifiedByConfiguration()
        {
            // Arrange
            const string customSystem1Name = "CustomSystem1";
            const string customSystem2Name = "CustomSystem2";
            const string customSystem3Name = "CustomSystem3";

            var customSystem1 = Substitute.For<ICustomSystem>();
            customSystem1.Name.Returns(customSystem1Name);
            var customSystem2 = Substitute.For<ICustomSystem>();
            customSystem2.Name.Returns(customSystem2Name);
            var customSystem3 = Substitute.For<ICustomSystem>();
            customSystem3.Name.Returns(customSystem3Name);

            // Act
            var gameLoopSteps = CreateGameLoopSteps(new[] { customSystem1, customSystem2, customSystem3 },
                new[] { customSystem1Name, customSystem3Name });

            // Assert
            Assert.That(gameLoopSteps.CustomSystems, Is.EqualTo(new[]
            {
                customSystem1,
                customSystem3
            }));
        }

        private GameLoopSteps CreateGameLoopSteps(IEnumerable<ICustomSystem>? customSystems = default,
            IEnumerable<string>? customSystemsExecutionOrder = default)
        {
            customSystems ??= Enumerable.Empty<ICustomSystem>();
            customSystemsExecutionOrder ??= Enumerable.Empty<string>();

            var coreConfiguration = CoreConfiguration.CreateBuilder()
                .WithCustomSystemsExecutionOrder(customSystemsExecutionOrder.ToList()).Build();

            return new GameLoopSteps(
                _animationSystem,
                _audioSystem,
                _behaviorSystem,
                _inputSystem,
                _physicsSystem,
                _renderingSystem,
                customSystems,
                coreConfiguration);
        }
    }
}