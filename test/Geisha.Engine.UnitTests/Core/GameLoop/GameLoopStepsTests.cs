using System.Collections.Generic;
using System.Linq;
using Geisha.Engine.Core;
using Geisha.Engine.Core.GameLoop;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Core.GameLoop
{
    [TestFixture]
    public class GameLoopStepsTests
    {
        private IAnimationGameLoopStep _animationStep = null!;
        private IAudioGameLoopStep _audioStep = null!;
        private IBehaviorGameLoopStep _behaviorStep = null!;
        private IInputGameLoopStep _inputStep = null!;
        private IPhysicsGameLoopStep _physicsStep = null!;
        private IRenderingGameLoopStep _renderingStep = null!;

        [SetUp]
        public void SetUp()
        {
            _animationStep = Substitute.For<IAnimationGameLoopStep>();
            _audioStep = Substitute.For<IAudioGameLoopStep>();
            _behaviorStep = Substitute.For<IBehaviorGameLoopStep>();
            _inputStep = Substitute.For<IInputGameLoopStep>();
            _physicsStep = Substitute.For<IPhysicsGameLoopStep>();
            _renderingStep = Substitute.For<IRenderingGameLoopStep>();
        }

        [Test]
        public void Constructor_ShouldThrowException_GivenConfigurationWithDuplicatedCustomStepsNames()
        {
            // Arrange
            const string customStep1Name = "CustomStep1";
            const string customStep2Name = "CustomStep2";

            var customStep1 = Substitute.For<ICustomGameLoopStep>();
            customStep1.Name.Returns(customStep1Name);
            var customStep2 = Substitute.For<ICustomGameLoopStep>();
            customStep2.Name.Returns(customStep2Name);

            // Act
            // Assert
            Assert.That(() =>
            {
                CreateGameLoopSteps(new[] { customStep1, customStep2 },
                    new[] { customStep1Name, customStep2Name, customStep1Name });
            }, Throws.ArgumentException);
        }

        [Test]
        public void Constructor_ShouldThrowException_GivenCustomStepsWithDuplicatedNames()
        {
            // Arrange
            const string customStep1Name = "CustomStep1";
            const string customStep2Name = "CustomStep2";

            var customStep1 = Substitute.For<ICustomGameLoopStep>();
            customStep1.Name.Returns(customStep1Name);
            var customStep2 = Substitute.For<ICustomGameLoopStep>();
            customStep2.Name.Returns(customStep1Name);

            // Act
            // Assert
            Assert.That(() =>
            {
                CreateGameLoopSteps(new[] { customStep1, customStep2 },
                    new[] { customStep1Name, customStep2Name });
            }, Throws.ArgumentException);
        }

        [Test]
        public void Constructor_ShouldThrowException_GivenConfigurationThatSpecifiesCustomStepsThatAreNotProvided()
        {
            // Arrange
            const string customStep1Name = "CustomStep1";
            const string customStep2Name = "CustomStep2";
            const string customStep3Name = "CustomStep3";

            var customStep1 = Substitute.For<ICustomGameLoopStep>();
            customStep1.Name.Returns(customStep1Name);
            var customStep2 = Substitute.For<ICustomGameLoopStep>();
            customStep2.Name.Returns(customStep2Name);

            // Act
            // Assert
            Assert.That(() =>
            {
                CreateGameLoopSteps(new[] { customStep1, customStep2 },
                    new[] { customStep1Name, customStep2Name, customStep3Name });
            }, Throws.ArgumentException);
        }

        [Test]
        public void StepsNames_ShouldReturnAlphabeticalListOfNamesOfStandardSteps_GivenNoCustomSteps()
        {
            // Arrange
            // Act
            var gameLoopSteps = CreateGameLoopSteps();

            // Assert
            Assert.That(gameLoopSteps.StepsNames, Is.EqualTo(new[]
            {
                gameLoopSteps.AnimationStepName,
                gameLoopSteps.AudioStepName,
                gameLoopSteps.BehaviorStepName,
                gameLoopSteps.InputStepName,
                gameLoopSteps.PhysicsStepName,
                gameLoopSteps.RenderingStepName
            }));
        }

        [Test]
        public void StepsNames_ShouldReturnAlphabeticalListOfNamesOfStandardStepsAndCustomSteps()
        {
            // Arrange
            const string customStep1Name = "CustomStep1";
            const string customStep2Name = "CustomStep2";
            const string customStep3Name = "CustomStep3";

            var customStep1 = Substitute.For<ICustomGameLoopStep>();
            customStep1.Name.Returns(customStep1Name);
            var customStep2 = Substitute.For<ICustomGameLoopStep>();
            customStep2.Name.Returns(customStep2Name);
            var customStep3 = Substitute.For<ICustomGameLoopStep>();
            customStep3.Name.Returns(customStep3Name);

            // Act
            var gameLoopSteps = CreateGameLoopSteps(new[] { customStep3, customStep2, customStep1 },
                new[] { customStep2Name, customStep3Name, customStep1Name });

            // Assert
            Assert.That(gameLoopSteps.StepsNames, Is.EqualTo(new[]
            {
                gameLoopSteps.AnimationStepName,
                gameLoopSteps.AudioStepName,
                gameLoopSteps.BehaviorStepName,
                customStep1Name,
                customStep2Name,
                customStep3Name,
                gameLoopSteps.InputStepName,
                gameLoopSteps.PhysicsStepName,
                gameLoopSteps.RenderingStepName
            }));
        }

        [Test]
        public void StepsNames_ShouldNotReturnNamesOfCustomStepsThatAreNotSpecifiedByConfiguration()
        {
            // Arrange
            const string customStep1Name = "CustomStep1";
            const string customStep2Name = "CustomStep2";
            const string customStep3Name = "CustomStep3";

            var customStep1 = Substitute.For<ICustomGameLoopStep>();
            customStep1.Name.Returns(customStep1Name);
            var customStep2 = Substitute.For<ICustomGameLoopStep>();
            customStep2.Name.Returns(customStep2Name);
            var customStep3 = Substitute.For<ICustomGameLoopStep>();
            customStep3.Name.Returns(customStep3Name);

            // Act
            var gameLoopSteps = CreateGameLoopSteps(new[] { customStep1, customStep2, customStep3 },
                new[] { customStep1Name, customStep3Name });

            // Assert
            Assert.That(gameLoopSteps.StepsNames, Is.EqualTo(new[]
            {
                gameLoopSteps.AnimationStepName,
                gameLoopSteps.AudioStepName,
                gameLoopSteps.BehaviorStepName,
                customStep1Name,
                customStep3Name,
                gameLoopSteps.InputStepName,
                gameLoopSteps.PhysicsStepName,
                gameLoopSteps.RenderingStepName
            }));
        }

        [Test]
        public void CustomSteps_ShouldReturnCustomStepsInOrderSpecifiedByConfiguration()
        {
            // Arrange
            const string customStep1Name = "CustomStep1";
            const string customStep2Name = "CustomStep2";
            const string customStep3Name = "CustomStep3";

            var customStep1 = Substitute.For<ICustomGameLoopStep>();
            customStep1.Name.Returns(customStep1Name);
            var customStep2 = Substitute.For<ICustomGameLoopStep>();
            customStep2.Name.Returns(customStep2Name);
            var customStep3 = Substitute.For<ICustomGameLoopStep>();
            customStep3.Name.Returns(customStep3Name);

            // Act
            var gameLoopSteps = CreateGameLoopSteps(new[] { customStep1, customStep2, customStep3 },
                new[] { customStep2Name, customStep3Name, customStep1Name });

            // Assert
            Assert.That(gameLoopSteps.CustomSteps, Is.EqualTo(new[]
            {
                customStep2,
                customStep3,
                customStep1
            }));
        }

        [Test]
        public void CustomSteps_ShouldNotReturnCustomStepsThatAreNotSpecifiedByConfiguration()
        {
            // Arrange
            const string customStep1Name = "CustomStep1";
            const string customStep2Name = "CustomStep2";
            const string customStep3Name = "CustomStep3";

            var customStep1 = Substitute.For<ICustomGameLoopStep>();
            customStep1.Name.Returns(customStep1Name);
            var customStep2 = Substitute.For<ICustomGameLoopStep>();
            customStep2.Name.Returns(customStep2Name);
            var customStep3 = Substitute.For<ICustomGameLoopStep>();
            customStep3.Name.Returns(customStep3Name);

            // Act
            var gameLoopSteps = CreateGameLoopSteps(new[] { customStep1, customStep2, customStep3 },
                new[] { customStep1Name, customStep3Name });

            // Assert
            Assert.That(gameLoopSteps.CustomSteps, Is.EqualTo(new[]
            {
                customStep1,
                customStep3
            }));
        }

        private GameLoopSteps CreateGameLoopSteps(IEnumerable<ICustomGameLoopStep>? customSteps = default, IEnumerable<string>? customGameLoopSteps = default)
        {
            customSteps ??= Enumerable.Empty<ICustomGameLoopStep>();
            customGameLoopSteps ??= Enumerable.Empty<string>();

            var coreConfiguration = new CoreConfiguration { CustomGameLoopSteps = customGameLoopSteps.ToList() };

            return new GameLoopSteps(
                _animationStep,
                _audioStep,
                _behaviorStep,
                _inputStep,
                _physicsStep,
                _renderingStep,
                customSteps,
                coreConfiguration);
        }
    }
}