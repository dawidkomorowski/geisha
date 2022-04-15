using Geisha.Common.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Backend;
using Geisha.Engine.Input.Components;
using Geisha.Engine.Input.Mapping;
using Geisha.Engine.Input.Systems;
using Geisha.TestUtils;
using NSubstitute;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input.Systems
{
    [TestFixture]
    public class InputSystemTests
    {
        private IInputProvider _inputProvider = null!;
        private InputSystem _inputSystem = null!;

        [SetUp]
        public void SetUp()
        {
            _inputProvider = Substitute.For<IInputProvider>();
            var inputBackend = Substitute.For<IInputBackend>();
            inputBackend.CreateInputProvider().Returns(_inputProvider);
            _inputSystem = new InputSystem(inputBackend);
        }

        #region Common test cases

        [TestCase(false, false, 0)]
        [TestCase(false, true, 1)]
        [TestCase(true, true, 0)]
        [TestCase(true, false, 0)]
        public void ProcessInput_ShouldCallActionBindingsCorrectNumberOfTimes_WhenExecutedTwice(bool first, bool second,
            int expectedCount)
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInputWithSampleKeyboardActionMappings(out var inputComponent, out var moveRight, out _, out _);
            var scene = inputSceneBuilder.Build();

            var hardwareInput1 = GetKeyboardInput(new KeyboardInputBuilder
            {
                Up = false,
                Down = false,
                Right = first,
                Left = false,
                Space = false
            });

            var hardwareInput2 = GetKeyboardInput(new KeyboardInputBuilder
            {
                Up = false,
                Down = false,
                Right = second,
                Left = false,
                Space = false
            });

            // fill in action states based on hardwareInput
            _inputProvider.Capture().Returns(hardwareInput1);
            _inputSystem.ProcessInput();

            var callCounter = 0;
            inputComponent.BindAction(moveRight.ActionName, () => { callCounter++; });

            // Act
            _inputProvider.Capture().Returns(hardwareInput1);
            _inputSystem.ProcessInput();

            _inputProvider.Capture().Returns(hardwareInput2);
            _inputSystem.ProcessInput();

            // Assert
            Assert.That(callCounter, Is.EqualTo(expectedCount));
        }


        [Test]
        public void ProcessInput_ShouldCallAxisBindingsEachTimeRegardlessHardwareInput()
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInputWithSampleKeyboardAxisMappings(out var inputComponent, out var moveUp, out _);
            var scene = inputSceneBuilder.Build();

            var callCounter = 0;
            inputComponent.BindAxis(moveUp.AxisName, value => { callCounter++; });

            var allFalseHardwareInput = GetKeyboardInput(new KeyboardInputBuilder
            {
                Up = false,
                Down = false,
                Right = false,
                Left = false,
                Space = false
            });

            var allTrueHardwareInput = GetKeyboardInput(new KeyboardInputBuilder
            {
                Up = true,
                Down = true,
                Right = true,
                Left = true,
                Space = true
            });

            // Act
            for (var i = 0; i < 10; i++)
            {
                _inputProvider.Capture().Returns(allFalseHardwareInput);
                _inputSystem.ProcessInput();

                _inputProvider.Capture().Returns(allTrueHardwareInput);
                _inputSystem.ProcessInput();

                _inputProvider.Capture().Returns(allTrueHardwareInput);
                _inputSystem.ProcessInput();

                _inputProvider.Capture().Returns(allFalseHardwareInput);
                _inputSystem.ProcessInput();
            }

            // Assert
            Assert.That(callCounter, Is.EqualTo(40));
        }

        [Test]
        public void ProcessInput_ShouldCaptureHardwareInputOnce()
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            var scene = inputSceneBuilder.Build();

            // Act
            _inputSystem.ProcessInput();

            // Assert
            _inputProvider.Received(1).Capture();
        }

        [Test]
        public void ProcessInput_ShouldSetHardwareInputOnAllInputComponents()
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
            _inputSystem.ProcessInput();

            // Assert
            Assert.That(inputComponentOfEntity1.HardwareInput, Is.EqualTo(hardwareInput));
            Assert.That(inputComponentOfEntity2.HardwareInput, Is.EqualTo(hardwareInput));
            Assert.That(inputComponentOfEntity3.HardwareInput, Is.EqualTo(hardwareInput));
        }

        #endregion

        #region Keyboard test cases

        [TestCase(false, false, false, false, false, false, false)]
        [TestCase(true, true, true, true, true, true, true)]
        [TestCase(true, false, true, false, true, false, true)]
        [TestCase(false, true, false, true, false, true, true)]
        public void ProcessInput_Keyboard_ShouldSetActionStatesAccordingToHardwareInputAndInputMapping(bool right, bool left, bool up,
            bool space, bool expectedRight, bool expectedLeft, bool expectedJump)
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInputWithSampleKeyboardActionMappings(out var inputComponent, out var moveRight, out var moveLeft, out var jump);
            var scene = inputSceneBuilder.Build();

            var hardwareInput = GetKeyboardInput(new KeyboardInputBuilder
            {
                Right = right,
                Left = left,
                Up = up,
                Space = space
            });
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.ProcessInput();

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
        public void ProcessInput_Keyboard_ShouldSetAxisStatesAccordingToHardwareInputAndInputMapping(bool up, bool down, bool right,
            bool left, bool space, double expectedUp, double expectedRight)
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInputWithSampleKeyboardAxisMappings(out var inputComponent, out var moveUp, out var moveRight);
            var scene = inputSceneBuilder.Build();

            var hardwareInput = GetKeyboardInput(new KeyboardInputBuilder
            {
                Up = up,
                Down = down,
                Right = right,
                Left = left,
                Space = space
            });
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.ProcessInput();

            // Assert
            Assert.That(inputComponent.GetAxisState(moveUp.AxisName), Is.EqualTo(expectedUp));
            Assert.That(inputComponent.GetAxisState(moveRight.AxisName), Is.EqualTo(expectedRight));
        }

        [TestCase(false, false, false, false, 0, 0, 0)]
        [TestCase(true, true, true, true, 1, 1, 1)]
        [TestCase(true, false, true, false, 1, 0, 1)]
        [TestCase(false, true, false, true, 0, 1, 1)]
        public void ProcessInput_Keyboard_ShouldCallActionBindingsAccordingToHardwareInputAndInputMapping(bool right, bool left,
            bool up, bool space, int expectedRightCount, int expectedLeftCount, int expectedJumpCount)
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInputWithSampleKeyboardActionMappings(out var inputComponent, out var moveRight, out var moveLeft, out var jump);
            var scene = inputSceneBuilder.Build();

            var moveRightCallCounter = 0;
            var moveLeftCallCounter = 0;
            var jumpCallCounter = 0;

            inputComponent.BindAction(moveRight.ActionName, () => { moveRightCallCounter++; });
            inputComponent.BindAction(moveLeft.ActionName, () => { moveLeftCallCounter++; });
            inputComponent.BindAction(jump.ActionName, () => { jumpCallCounter++; });

            var hardwareInput = GetKeyboardInput(new KeyboardInputBuilder
            {
                Right = right,
                Left = left,
                Up = up,
                Space = space
            });
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.ProcessInput();

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
        public void ProcessInput_Keyboard_ShouldCallAxisBindingsAccordingToHardwareInputAndInputMapping(bool up, bool down, bool right,
            bool left, bool space, double expectedUp, double expectedRight)
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInputWithSampleKeyboardAxisMappings(out var inputComponent, out var moveUp, out var moveRight);
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

            var hardwareInput = GetKeyboardInput(new KeyboardInputBuilder
            {
                Up = up,
                Down = down,
                Right = right,
                Left = left,
                Space = space
            });
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.ProcessInput();

            // Assert
            Assert.That(moveUpCallCounter, Is.EqualTo(1));
            Assert.That(moveRightCallCounter, Is.EqualTo(1));

            Assert.That(moveUpState, Is.EqualTo(expectedUp));
            Assert.That(moveRightState, Is.EqualTo(expectedRight));
        }

        #endregion

        #region Mouse test cases

        [TestCase(false, false, false, false, false,
            false, false, false, false)]
        [TestCase(true, true, true, true, true,
            true, true, true, true)]
        [TestCase(true, false, true, false, true,
            true, false, true, true)]
        [TestCase(false, true, false, true, false,
            false, true, false, true)]
        public void ProcessInput_Mouse_ShouldSetActionStatesAccordingToHardwareInputAndInputMapping(bool left, bool middle, bool right,
            bool x1, bool x2, bool expectedFire, bool expectedZoom, bool expectedAltFire, bool expectedMelee)
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInputWithSampleMouseActionMappings(out var inputComponent, out var fire, out var zoom, out var altFire, out var melee);
            var scene = inputSceneBuilder.Build();

            var hardwareInput = GetMouseInput(new MouseInputBuilder
            {
                LeftButton = left,
                MiddleButton = middle,
                RightButton = right,
                XButton1 = x1,
                XButton2 = x2
            });
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.ProcessInput();

            // Assert
            Assert.That(inputComponent.GetActionState(fire.ActionName), Is.EqualTo(expectedFire));
            Assert.That(inputComponent.GetActionState(zoom.ActionName), Is.EqualTo(expectedZoom));
            Assert.That(inputComponent.GetActionState(altFire.ActionName), Is.EqualTo(expectedAltFire));
            Assert.That(inputComponent.GetActionState(melee.ActionName), Is.EqualTo(expectedMelee));
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(5, 7, 5, -7)]
        [TestCase(-5, -7, -5, 7)]
        public void ProcessInput_Mouse_ShouldSetAxisStatesAccordingToHardwareInputAndInputMapping(int xPos, int yPos, double expectedLookRight,
            double expectedLookUp)
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInputWithSampleMouseAxisMappings(out var inputComponent, out var lookRight, out var lookUp);
            var scene = inputSceneBuilder.Build();

            var hardwareInput = GetMouseInput(new MouseInputBuilder
            {
                PositionDelta = new Vector2(xPos, yPos)
            });
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.ProcessInput();

            // Assert
            Assert.That(inputComponent.GetAxisState(lookRight.AxisName), Is.EqualTo(expectedLookRight));
            Assert.That(inputComponent.GetAxisState(lookUp.AxisName), Is.EqualTo(expectedLookUp));
        }

        [TestCase(false, false, false, false, false,
            0, 0, 0, 0)]
        [TestCase(true, true, true, true, true,
            1, 1, 1, 1)]
        [TestCase(true, false, true, false, true,
            1, 0, 1, 1)]
        [TestCase(false, true, false, true, false,
            0, 1, 0, 1)]
        public void ProcessInput_Mouse_ShouldCallActionBindingsAccordingToHardwareInputAndInputMapping(bool left, bool middle, bool right,
            bool x1, bool x2, int expectedFireCount, int expectedZoomCount, int expectedAltFireCount, int expectedMeleeCount)
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInputWithSampleMouseActionMappings(out var inputComponent, out var fire, out var zoom, out var altFire, out var melee);
            var scene = inputSceneBuilder.Build();

            var fireCallCounter = 0;
            var zoomCallCounter = 0;
            var altFireCallCounter = 0;
            var meleeCallCounter = 0;

            inputComponent.BindAction(fire.ActionName, () => { fireCallCounter++; });
            inputComponent.BindAction(zoom.ActionName, () => { zoomCallCounter++; });
            inputComponent.BindAction(altFire.ActionName, () => { altFireCallCounter++; });
            inputComponent.BindAction(melee.ActionName, () => { meleeCallCounter++; });

            var hardwareInput = GetMouseInput(new MouseInputBuilder
            {
                LeftButton = left,
                MiddleButton = middle,
                RightButton = right,
                XButton1 = x1,
                XButton2 = x2
            });
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.ProcessInput();

            // Assert
            Assert.That(fireCallCounter, Is.EqualTo(expectedFireCount));
            Assert.That(zoomCallCounter, Is.EqualTo(expectedZoomCount));
            Assert.That(altFireCallCounter, Is.EqualTo(expectedAltFireCount));
            Assert.That(meleeCallCounter, Is.EqualTo(expectedMeleeCount));
        }

        [TestCase(0, 0, 0, 0)]
        [TestCase(5, 7, 5, -7)]
        [TestCase(-5, -7, -5, 7)]
        public void ProcessInput_Mouse_ShouldCallAxisBindingsAccordingToHardwareInputAndInputMapping(int xPos, int yPos, double expectedLookRight,
            double expectedLookUp)
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInputWithSampleMouseAxisMappings(out var inputComponent, out var lookRight, out var lookUp);
            var scene = inputSceneBuilder.Build();

            var lookRightCallCounter = 0;
            var lookUpCallCounter = 0;

            var lookRightState = 0.0;
            var lookUpState = 0.0;

            inputComponent.BindAxis(lookRight.AxisName, value =>
            {
                lookRightCallCounter++;
                lookRightState = value;
            });
            inputComponent.BindAxis(lookUp.AxisName, value =>
            {
                lookUpCallCounter++;
                lookUpState = value;
            });

            var hardwareInput = GetMouseInput(new MouseInputBuilder
            {
                PositionDelta = new Vector2(xPos, yPos)
            });
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            _inputSystem.ProcessInput();

            // Assert
            Assert.That(lookRightCallCounter, Is.EqualTo(1));
            Assert.That(lookUpCallCounter, Is.EqualTo(1));

            Assert.That(lookRightState, Is.EqualTo(expectedLookRight));
            Assert.That(lookUpState, Is.EqualTo(expectedLookUp));
        }

        #endregion

        #region Regression test cases

        [Test]
        public void ProcessInput_ShouldNotThrowException_WhenEntityIsAddedInInputBindingFunction()
        {
            // Arrange
            var inputSceneBuilder = new InputSceneBuilder();
            inputSceneBuilder.AddInputWithSampleKeyboardActionMappings(out var inputComponent, out _, out _, out var jump);
            var scene = inputSceneBuilder.Build();

            inputComponent.BindAction(jump.ActionName, () => { scene.CreateEntity(); });

            var hardwareInput = GetKeyboardInput(new KeyboardInputBuilder
            {
                Space = true
            });
            _inputProvider.Capture().Returns(hardwareInput);

            // Act
            // Assert
            Assert.That(() => _inputSystem.ProcessInput(), Throws.Nothing);
        }

        #endregion

        #region Helpers

        private static HardwareInput GetKeyboardInput(KeyboardInputBuilder keyboardInputBuilder)
        {
            return new HardwareInput(keyboardInputBuilder.Build(), default);
        }

        private static HardwareInput GetMouseInput(MouseInputBuilder mouseInputBuilder)
        {
            return new HardwareInput(default, mouseInputBuilder.Build());
        }

        private class InputSceneBuilder
        {
            private readonly Scene _scene = TestSceneFactory.Create();

            public void AddInput(out InputComponent inputComponent)
            {
                var entity = _scene.CreateEntity();
                inputComponent = entity.CreateComponent<InputComponent>();
            }

            public void AddInputWithSampleKeyboardActionMappings(out InputComponent inputComponent, out ActionMapping moveRight, out ActionMapping moveLeft,
                out ActionMapping jump)
            {
                moveRight = new ActionMapping { ActionName = nameof(moveRight) };
                moveRight.HardwareActions.Add(new HardwareAction
                {
                    HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Right)
                });

                moveLeft = new ActionMapping { ActionName = nameof(moveLeft) };
                moveLeft.HardwareActions.Add(new HardwareAction
                {
                    HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Left)
                });

                jump = new ActionMapping { ActionName = nameof(jump) };
                jump.HardwareActions.Add(new HardwareAction
                {
                    HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Up)
                });
                jump.HardwareActions.Add(new HardwareAction
                {
                    HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Space)
                });

                var inputMapping = new InputMapping();
                inputMapping.ActionMappings.Add(moveRight);
                inputMapping.ActionMappings.Add(moveLeft);
                inputMapping.ActionMappings.Add(jump);

                var entity = _scene.CreateEntity();
                inputComponent = entity.CreateComponent<InputComponent>();
                inputComponent.InputMapping = inputMapping;
            }

            public void AddInputWithSampleKeyboardAxisMappings(out InputComponent inputComponent, out AxisMapping moveUp, out AxisMapping moveRight)
            {
                moveUp = new AxisMapping { AxisName = nameof(moveUp) };
                moveUp.HardwareAxes.Add(new HardwareAxis
                {
                    HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Up),
                    Scale = 1.0
                });
                moveUp.HardwareAxes.Add(new HardwareAxis
                {
                    HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Down),
                    Scale = -1.0
                });
                moveUp.HardwareAxes.Add(new HardwareAxis
                {
                    HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Space),
                    Scale = 5.0
                });

                moveRight = new AxisMapping { AxisName = nameof(moveRight) };
                moveRight.HardwareAxes.Add(new HardwareAxis
                {
                    HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Right),
                    Scale = 1.0
                });
                moveRight.HardwareAxes.Add(new HardwareAxis
                {
                    HardwareInputVariant = HardwareInputVariant.CreateKeyboardVariant(Key.Left),
                    Scale = -1.0
                });

                var inputMapping = new InputMapping();
                inputMapping.AxisMappings.Add(moveUp);
                inputMapping.AxisMappings.Add(moveRight);

                var entity = _scene.CreateEntity();
                inputComponent = entity.CreateComponent<InputComponent>();
                inputComponent.InputMapping = inputMapping;
            }

            public void AddInputWithSampleMouseActionMappings(out InputComponent inputComponent, out ActionMapping fire, out ActionMapping zoom,
                out ActionMapping altFire, out ActionMapping melee)
            {
                fire = new ActionMapping { ActionName = nameof(fire) };
                fire.HardwareActions.Add(new HardwareAction
                {
                    HardwareInputVariant = HardwareInputVariant.CreateMouseVariant(HardwareInputVariant.MouseVariant.LeftButton)
                });

                zoom = new ActionMapping { ActionName = nameof(zoom) };
                zoom.HardwareActions.Add(new HardwareAction
                {
                    HardwareInputVariant = HardwareInputVariant.CreateMouseVariant(HardwareInputVariant.MouseVariant.MiddleButton)
                });

                altFire = new ActionMapping { ActionName = nameof(altFire) };
                altFire.HardwareActions.Add(new HardwareAction
                {
                    HardwareInputVariant = HardwareInputVariant.CreateMouseVariant(HardwareInputVariant.MouseVariant.RightButton)
                });

                melee = new ActionMapping { ActionName = nameof(melee) };
                melee.HardwareActions.Add(new HardwareAction
                {
                    HardwareInputVariant = HardwareInputVariant.CreateMouseVariant(HardwareInputVariant.MouseVariant.XButton1)
                });
                melee.HardwareActions.Add(new HardwareAction
                {
                    HardwareInputVariant = HardwareInputVariant.CreateMouseVariant(HardwareInputVariant.MouseVariant.XButton2)
                });

                var inputMapping = new InputMapping();
                inputMapping.ActionMappings.Add(fire);
                inputMapping.ActionMappings.Add(zoom);
                inputMapping.ActionMappings.Add(altFire);
                inputMapping.ActionMappings.Add(melee);

                var entity = _scene.CreateEntity();
                inputComponent = entity.CreateComponent<InputComponent>();
                inputComponent.InputMapping = inputMapping;
            }

            public void AddInputWithSampleMouseAxisMappings(out InputComponent inputComponent, out AxisMapping lookRight, out AxisMapping lookUp)
            {
                lookRight = new AxisMapping { AxisName = nameof(lookRight) };
                lookRight.HardwareAxes.Add(new HardwareAxis
                {
                    HardwareInputVariant = HardwareInputVariant.CreateMouseVariant(HardwareInputVariant.MouseVariant.AxisX),
                    Scale = 1.0
                });

                lookUp = new AxisMapping { AxisName = nameof(lookUp) };
                lookUp.HardwareAxes.Add(new HardwareAxis
                {
                    HardwareInputVariant = HardwareInputVariant.CreateMouseVariant(HardwareInputVariant.MouseVariant.AxisY),
                    Scale = 1.0
                });

                var inputMapping = new InputMapping();
                inputMapping.AxisMappings.Add(lookUp);
                inputMapping.AxisMappings.Add(lookRight);

                var entity = _scene.CreateEntity();
                inputComponent = entity.CreateComponent<InputComponent>();
                inputComponent.InputMapping = inputMapping;
            }

            public Scene Build()
            {
                return _scene;
            }
        }

        #endregion
    }
}