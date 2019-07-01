using System.Collections.Generic;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Input.Systems;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input.Systems
{
    [TestFixture]
    public class InputSystemTests
    {
        [SetUp]
        public void SetUp()
        {
            _inputProvider = Substitute.For<IInputProvider>();
            var inputBackend = Substitute.For<IInputBackend>();
            inputBackend.CreateInputProvider().Returns(_inputProvider);
            _inputSystem = new InputSystem(inputBackend);
        }

        private IInputProvider _inputProvider;
        private InputSystem _inputSystem;

        [TestCase(false, false, false, false, false, false, false)]
        [TestCase(true, true, true, true, true, true, true)]
        [TestCase(true, false, true, false, true, false, true)]
        [TestCase(false, true, false, true, false, true, true)]
        public void FixedUpdate_ShouldSetActionStatesAccordingToHardwareInputAndInputMapping(bool right, bool left, bool up,
            bool space, bool expectedRight, bool expectedLeft, bool expectedJump)
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInputWithSampleActionMappings(out var inputComponent, out var moveRight, out var moveLeft, out var jump);
            var scene = inputSceneBuilder.Build();

            var hardwareInput = GetKeyboardInput(new Dictionary<Key, bool>
            {
                [Key.Right] = right,
                [Key.Left] = left,
                [Key.Up] = up,
                [Key.Space] = space
            });
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.FixedUpdate(scene);

            // Assert
            Assert.That(inputComponent.GetActionState(moveRight.ActionName), Is.EqualTo(expectedRight));
            Assert.That(inputComponent.GetActionState(moveLeft.ActionName), Is.EqualTo(expectedLeft));
            Assert.That(inputComponent.GetActionState(jump.ActionName), Is.EqualTo(expectedJump));
        }

        [TestCase(false, false, false, false, false, 0, 0)]
        [TestCase(true, true, true, true, false, 0, 0)]
        [TestCase(true, false, true, false, false, 1, 1)]
        [TestCase(false, true, false, true, false, -1, -1)]
        [TestCase(false, false, false, false, true, 5, 0)]
        [TestCase(true, false, false, false, true, 6, 0)]
        public void FixedUpdate_ShouldSetAxisStatesAccordingToHardwareInputAndInputMapping(bool up, bool down, bool right,
            bool left, bool space, double expectedUp, double expectedRight)
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInputWithSampleAxisMappings(out var inputComponent, out var moveUp, out var moveRight);
            var scene = inputSceneBuilder.Build();

            var hardwareInput = GetKeyboardInput(new Dictionary<Key, bool>
            {
                [Key.Up] = up,
                [Key.Down] = down,
                [Key.Right] = right,
                [Key.Left] = left,
                [Key.Space] = space
            });
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.FixedUpdate(scene);

            // Assert
            Assert.That(inputComponent.GetAxisState(moveUp.AxisName), Is.EqualTo(expectedUp));
            Assert.That(inputComponent.GetAxisState(moveRight.AxisName), Is.EqualTo(expectedRight));
        }

        [TestCase(false, false, false, false, 0, 0, 0)]
        [TestCase(true, true, true, true, 1, 1, 1)]
        [TestCase(true, false, true, false, 1, 0, 1)]
        [TestCase(false, true, false, true, 0, 1, 1)]
        public void FixedUpdate_ShouldCallActionBindingsAccordingToHardwareInputAndInputMapping(bool right, bool left,
            bool up, bool space, int expectedRightCount, int expectedLeftCount, int expectedJumpCount)
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInputWithSampleActionMappings(out var inputComponent, out var moveRight, out var moveLeft, out var jump);
            var scene = inputSceneBuilder.Build();

            var moveRightCallCounter = 0;
            var moveLeftCallCounter = 0;
            var jumpCallCounter = 0;

            inputComponent.BindAction(moveRight.ActionName, () => { moveRightCallCounter++; });
            inputComponent.BindAction(moveLeft.ActionName, () => { moveLeftCallCounter++; });
            inputComponent.BindAction(jump.ActionName, () => { jumpCallCounter++; });

            var hardwareInput = GetKeyboardInput(new Dictionary<Key, bool>
            {
                [Key.Right] = right,
                [Key.Left] = left,
                [Key.Up] = up,
                [Key.Space] = space
            });
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.FixedUpdate(scene);

            // Assert
            Assert.That(moveRightCallCounter, Is.EqualTo(expectedRightCount));
            Assert.That(moveLeftCallCounter, Is.EqualTo(expectedLeftCount));
            Assert.That(jumpCallCounter, Is.EqualTo(expectedJumpCount));
        }

        [TestCase(false, false, false, false, false, 0, 0)]
        [TestCase(true, true, true, true, false, 0, 0)]
        [TestCase(true, false, true, false, false, 1, 1)]
        [TestCase(false, true, false, true, false, -1, -1)]
        [TestCase(false, false, false, false, true, 5, 0)]
        [TestCase(true, false, false, false, true, 6, 0)]
        public void FixedUpdate_ShouldCallAxisBindingsAccordingToHardwareInputAndInputMapping(bool up, bool down, bool right,
            bool left, bool space, double expectedUp, double expectedRight)
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInputWithSampleAxisMappings(out var inputComponent, out var moveUp, out var moveRight);
            var scene = inputSceneBuilder.Build();

            var moveUpCallCounter = 0;
            var moveRightCallCounter = 0;

            var moveUpState = 0.0;
            var moveRightState = 0.0;

            inputComponent.BindAxis(moveUp.AxisName, value =>
            {
                moveUpCallCounter++;
                moveUpState = value;
            });
            inputComponent.BindAxis(moveRight.AxisName, value =>
            {
                moveRightCallCounter++;
                moveRightState = value;
            });

            var hardwareInput = GetKeyboardInput(new Dictionary<Key, bool>
            {
                [Key.Up] = up,
                [Key.Down] = down,
                [Key.Right] = right,
                [Key.Left] = left,
                [Key.Space] = space
            });
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.FixedUpdate(scene);

            // Assert
            Assert.That(moveUpCallCounter, Is.EqualTo(1));
            Assert.That(moveRightCallCounter, Is.EqualTo(1));

            Assert.That(moveUpState, Is.EqualTo(expectedUp));
            Assert.That(moveRightState, Is.EqualTo(expectedRight));
        }

        [TestCase(false, false, 0)]
        [TestCase(false, true, 1)]
        [TestCase(true, true, 0)]
        [TestCase(true, false, 0)]
        public void FixedUpdate_ShouldCallActionBindingsCorrectNumberOfTimes_WhenExecutedTwice(bool first, bool second,
            int expectedCount)
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInputWithSampleActionMappings(out var inputComponent, out var moveRight, out _, out _);
            var scene = inputSceneBuilder.Build();

            var hardwareInput1 = GetKeyboardInput(new Dictionary<Key, bool>
            {
                [Key.Up] = false,
                [Key.Down] = false,
                [Key.Right] = first,
                [Key.Left] = false,
                [Key.Space] = false
            });

            var hardwareInput2 = GetKeyboardInput(new Dictionary<Key, bool>
            {
                [Key.Up] = false,
                [Key.Down] = false,
                [Key.Right] = second,
                [Key.Left] = false,
                [Key.Space] = false
            });

            // fill in action states based on hardwareInput
            _inputProvider.Capture().Returns(hardwareInput1);
            _inputSystem.FixedUpdate(scene);

            var callCounter = 0;
            inputComponent.BindAction(moveRight.ActionName, () => { callCounter++; });

            // Act
            _inputProvider.Capture().Returns(hardwareInput1);
            _inputSystem.FixedUpdate(scene);

            _inputProvider.Capture().Returns(hardwareInput2);
            _inputSystem.FixedUpdate(scene);

            // Assert
            Assert.That(callCounter, Is.EqualTo(expectedCount));
        }


        [Test]
        public void FixedUpdate_ShouldCallAxisBindingsEachTimeRegardlessHardwareInput()
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInputWithSampleAxisMappings(out var inputComponent, out var moveUp, out _);
            var scene = inputSceneBuilder.Build();

            var callCounter = 0;
            inputComponent.BindAxis(moveUp.AxisName, value => { callCounter++; });

            var allFalseHardwareInput = GetKeyboardInput(new Dictionary<Key, bool>
            {
                [Key.Up] = false,
                [Key.Down] = false,
                [Key.Right] = false,
                [Key.Left] = false,
                [Key.Space] = false
            });

            var allTrueHardwareInput = GetKeyboardInput(new Dictionary<Key, bool>
            {
                [Key.Up] = true,
                [Key.Down] = true,
                [Key.Right] = true,
                [Key.Left] = true,
                [Key.Space] = true
            });


            // Act
            for (var i = 0; i < 10; i++)
            {
                _inputProvider.Capture().Returns(allFalseHardwareInput);
                _inputSystem.FixedUpdate(scene);

                _inputProvider.Capture().Returns(allTrueHardwareInput);
                _inputSystem.FixedUpdate(scene);

                _inputProvider.Capture().Returns(allTrueHardwareInput);
                _inputSystem.FixedUpdate(scene);

                _inputProvider.Capture().Returns(allFalseHardwareInput);
                _inputSystem.FixedUpdate(scene);
            }

            // Assert
            Assert.That(callCounter, Is.EqualTo(40));
        }

        [Test]
        public void FixedUpdate_ShouldCaptureHardwareInputOnce()
        {
            // Arrange
            var scene = new Scene();

            // Act
            _inputSystem.FixedUpdate(scene);

            // Assert
            _inputProvider.Received(1).Capture();
        }

        [Test]
        public void FixedUpdate_ShouldSetHardwareInputOnAllInputComponents()
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInput(out var inputComponentOfEntity1);
            inputSceneBuilder.AddInput(out var inputComponentOfEntity2);
            inputSceneBuilder.AddInput(out var inputComponentOfEntity3);
            var scene = inputSceneBuilder.Build();

            var hardwareInput = HardwareInput.Empty;
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.FixedUpdate(scene);

            // Assert
            Assert.That(inputComponentOfEntity1.HardwareInput, Is.EqualTo(hardwareInput));
            Assert.That(inputComponentOfEntity2.HardwareInput, Is.EqualTo(hardwareInput));
            Assert.That(inputComponentOfEntity3.HardwareInput, Is.EqualTo(hardwareInput));
        }

        private HardwareInput GetKeyboardInput(IReadOnlyDictionary<Key, bool> keyStates)
        {
            return new HardwareInput(KeyboardInput.CreateFromLimitedState(keyStates), default);
        }

        private class InputSceneBuilder
        {
            private readonly Scene _scene = new Scene();

            public Entity AddInput(out InputComponent inputComponent)
            {
                inputComponent = new InputComponent();
                var entity = new Entity();
                entity.AddComponent(inputComponent);
                _scene.AddEntity(entity);

                return entity;
            }

            public Entity AddInputWithSampleActionMappings(out InputComponent inputComponent, out ActionMapping moveRight, out ActionMapping moveLeft,
                out ActionMapping jump)
            {
                moveRight = new ActionMapping {ActionName = nameof(moveRight)};
                moveRight.HardwareActions.Add(new HardwareAction
                {
                    HardwareInputVariant = new HardwareInputVariant(Key.Right)
                });

                moveLeft = new ActionMapping {ActionName = nameof(moveLeft)};
                moveLeft.HardwareActions.Add(new HardwareAction
                {
                    HardwareInputVariant = new HardwareInputVariant(Key.Left)
                });

                jump = new ActionMapping {ActionName = nameof(jump)};
                jump.HardwareActions.Add(new HardwareAction
                {
                    HardwareInputVariant = new HardwareInputVariant(Key.Up)
                });
                jump.HardwareActions.Add(new HardwareAction
                {
                    HardwareInputVariant = new HardwareInputVariant(Key.Space)
                });

                var inputMapping = new InputMapping();
                inputMapping.ActionMappings.Add(moveRight);
                inputMapping.ActionMappings.Add(moveLeft);
                inputMapping.ActionMappings.Add(jump);

                inputComponent = new InputComponent {InputMapping = inputMapping};

                var entity = new Entity();
                entity.AddComponent(inputComponent);

                _scene.AddEntity(entity);

                return entity;
            }

            public Entity AddInputWithSampleAxisMappings(out InputComponent inputComponent, out AxisMapping moveUp, out AxisMapping moveRight)
            {
                moveUp = new AxisMapping {AxisName = nameof(moveUp)};
                moveUp.HardwareAxes.Add(new HardwareAxis
                {
                    HardwareInputVariant = new HardwareInputVariant(Key.Up),
                    Scale = 1.0
                });
                moveUp.HardwareAxes.Add(new HardwareAxis
                {
                    HardwareInputVariant = new HardwareInputVariant(Key.Down),
                    Scale = -1.0
                });
                moveUp.HardwareAxes.Add(new HardwareAxis
                {
                    HardwareInputVariant = new HardwareInputVariant(Key.Space),
                    Scale = 5.0
                });

                moveRight = new AxisMapping {AxisName = nameof(moveRight)};
                moveRight.HardwareAxes.Add(new HardwareAxis
                {
                    HardwareInputVariant = new HardwareInputVariant(Key.Right),
                    Scale = 1.0
                });
                moveRight.HardwareAxes.Add(new HardwareAxis
                {
                    HardwareInputVariant = new HardwareInputVariant(Key.Left),
                    Scale = -1.0
                });

                var inputMapping = new InputMapping();
                inputMapping.AxisMappings.Add(moveUp);
                inputMapping.AxisMappings.Add(moveRight);

                inputComponent = new InputComponent {InputMapping = inputMapping};

                var entity = new Entity();
                entity.AddComponent(inputComponent);

                _scene.AddEntity(entity);

                return entity;
            }

            public Scene Build()
            {
                return _scene;
            }
        }
    }
}