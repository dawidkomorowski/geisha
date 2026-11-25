using System;
using System.Linq;
using Geisha.Engine.Core.Math;
using Geisha.Engine.Core.SceneModel;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Backend;
using Geisha.TestUtils;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NSubstitute.Extensions;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input;

[TestFixture]
public class InputSystemTests
{
    private IInputBackend _inputBackend = null!;
    private InputSystem _inputSystem = null!;
    private InputScene _inputScene = null!;

    [SetUp]
    public void SetUp()
    {
        _inputBackend = Substitute.For<IInputBackend>();
        _inputBackend.GetKeyboardInput().ThrowsForAnyArgs(
            new InvalidOperationException("GetKeyboardInput() not configured. Use: _inputBackend.Configure().GetKeyboardInput().Returns(keyboardInput);")
        );
        _inputBackend.GetMouseInput().ThrowsForAnyArgs(
            new InvalidOperationException("GetMouseInput() not configured. Use: _inputBackend.Configure().GetMouseInput().Returns(mouseInput);")
        );
        _inputBackend.GetControllerState(Arg.Any<ControllerIndex>()).ThrowsForAnyArgs(
            new InvalidOperationException(
                "GetControllerState() not configured. Use: _inputBackend.Configure().GetControllerState(controllerIndex).Returns(controllerState);")
        );

        _inputSystem = new InputSystem(_inputBackend);
        _inputScene = new InputScene(_inputSystem);
    }

    private void MockEmptyKeyboardInputOnly()
    {
        _inputBackend.Configure().GetKeyboardInput().Returns(new KeyboardInput());
    }

    private void MockEmptyMouseInputOnly()
    {
        _inputBackend.Configure().GetMouseInput().Returns(new MouseInput());
    }

    private void MockEmptyControllerStateOnly()
    {
        _inputBackend.Configure().GetControllerState(ControllerIndex.One)
            .Returns(new ControllerState { Index = ControllerIndex.One });
        _inputBackend.Configure().GetControllerState(ControllerIndex.Two)
            .Returns(new ControllerState { Index = ControllerIndex.Two });
        _inputBackend.Configure().GetControllerState(ControllerIndex.Three)
            .Returns(new ControllerState { Index = ControllerIndex.Three });
        _inputBackend.Configure().GetControllerState(ControllerIndex.Four)
            .Returns(new ControllerState { Index = ControllerIndex.Four });
    }

    private void MockEmptyInput()
    {
        MockEmptyKeyboardInputOnly();
        MockEmptyMouseInputOnly();
        MockEmptyControllerStateOnly();
    }

    private void MockKeyboardInput(KeyboardInput keyboardInput)
    {
        MockEmptyInput();
        _inputBackend.Configure().GetKeyboardInput().Returns(keyboardInput);
    }

    private void MockMouseInput(MouseInput mouseInput)
    {
        MockEmptyInput();
        _inputBackend.Configure().GetMouseInput().Returns(mouseInput);
    }

    private void MockControllerState(ControllerIndex controllerIndex, ControllerState controllerState)
    {
        MockEmptyInput();
        _inputBackend.Configure().GetControllerState(controllerIndex).Returns(controllerState);
    }

    private void MockInput(HardwareInput hardwareInput)
    {
        _inputBackend.Configure().GetKeyboardInput().Returns(hardwareInput.KeyboardInput);
        _inputBackend.Configure().GetMouseInput().Returns(hardwareInput.MouseInput);
        _inputBackend.Configure().GetControllerState(ControllerIndex.One).Returns(hardwareInput.Controller1);
        _inputBackend.Configure().GetControllerState(ControllerIndex.Two).Returns(hardwareInput.Controller2);
        _inputBackend.Configure().GetControllerState(ControllerIndex.Three).Returns(hardwareInput.Controller3);
        _inputBackend.Configure().GetControllerState(ControllerIndex.Four).Returns(hardwareInput.Controller4);
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
        _inputScene.AddInputWithKeyboardActionMapping(out var inputComponent, out var moveRight, out _, out _);

        var keyboardInput1 = new KeyboardInputBuilder
        {
            Up = false,
            Down = false,
            Right = first,
            Left = false,
            Space = false
        }.Build();

        var keyboardInput2 = new KeyboardInputBuilder
        {
            Up = false,
            Down = false,
            Right = second,
            Left = false,
            Space = false
        }.Build();

        var callCounter = 0;
        inputComponent.BindAction(moveRight.ActionName, () => { callCounter++; });

        // Act
        // Initialize action states based on hardware input
        MockKeyboardInput(keyboardInput1);
        _inputSystem.ProcessInput();

        MockKeyboardInput(keyboardInput2);
        _inputSystem.ProcessInput();

        // Assert
        Assert.That(callCounter, Is.EqualTo(expectedCount));
    }


    [Test]
    public void ProcessInput_ShouldCallAxisBindingsEachTimeRegardlessHardwareInput()
    {
        // Arrange
        _inputScene.AddInputWithKeyboardAxisMapping(out var inputComponent, out var moveUp, out _);

        var callCounter = 0;
        inputComponent.BindAxis(moveUp.AxisName, _ => { callCounter++; });

        var allFalseKeyboardInput = new KeyboardInputBuilder
        {
            Up = false,
            Down = false,
            Right = false,
            Left = false,
            Space = false
        }.Build();

        var allTrueKeyboardInput = new KeyboardInputBuilder
        {
            Up = true,
            Down = true,
            Right = true,
            Left = true,
            Space = true
        }.Build();

        // Act
        for (var i = 0; i < 10; i++)
        {
            MockKeyboardInput(allFalseKeyboardInput);
            _inputSystem.ProcessInput();

            MockKeyboardInput(allTrueKeyboardInput);
            _inputSystem.ProcessInput();

            MockKeyboardInput(allTrueKeyboardInput);
            _inputSystem.ProcessInput();

            MockKeyboardInput(allFalseKeyboardInput);
            _inputSystem.ProcessInput();
        }

        // Assert
        Assert.That(callCounter, Is.EqualTo(40));
    }

    [Test]
    public void ProcessInput_ShouldReadInputDevicesIntoHardwareInputSnapshot()
    {
        // Arrange
        var inputComponent = _inputScene.AddInput();

        MockInput(new HardwareInput
        {
            KeyboardInput = new KeyboardInputBuilder { Up = true }.Build(),
            MouseInput = new MouseInputBuilder { Position = new Vector2(12, 34) }.Build(),
            Controller1 = new ControllerState { Index = ControllerIndex.One, LeftThumb = new Vector2(1, 2) },
            Controller2 = new ControllerState { Index = ControllerIndex.Two, LeftThumb = new Vector2(3, 4) },
            Controller3 = new ControllerState { Index = ControllerIndex.Three, LeftThumb = new Vector2(5, 6) },
            Controller4 = new ControllerState { Index = ControllerIndex.Four, LeftThumb = new Vector2(7, 8) }
        });

        // Act
        _inputSystem.ProcessInput();

        // Assert
        var hardwareInput = inputComponent.HardwareInput;
        Assert.That(hardwareInput.KeyboardInput.Up, Is.True);
        Assert.That(hardwareInput.MouseInput.Position, Is.EqualTo(new Vector2(12, 34)));
        Assert.That(hardwareInput.Controller1.Index, Is.EqualTo(ControllerIndex.One));
        Assert.That(hardwareInput.Controller1.LeftThumb, Is.EqualTo(new Vector2(1, 2)));
        Assert.That(hardwareInput.Controller2.Index, Is.EqualTo(ControllerIndex.Two));
        Assert.That(hardwareInput.Controller2.LeftThumb, Is.EqualTo(new Vector2(3, 4)));
        Assert.That(hardwareInput.Controller3.Index, Is.EqualTo(ControllerIndex.Three));
        Assert.That(hardwareInput.Controller3.LeftThumb, Is.EqualTo(new Vector2(5, 6)));
        Assert.That(hardwareInput.Controller4.Index, Is.EqualTo(ControllerIndex.Four));
        Assert.That(hardwareInput.Controller4.LeftThumb, Is.EqualTo(new Vector2(7, 8)));
    }

    [Test]
    public void ProcessInput_ShouldSetHardwareInputOnAllInputComponents()
    {
        // Arrange
        var inputComponentOfEntity1 = _inputScene.AddInput();
        var inputComponentOfEntity2 = _inputScene.AddInput();
        var inputComponentOfEntity3 = _inputScene.AddInput();

        var keyboardInput = new KeyboardInputBuilder { Up = true }.Build();
        MockKeyboardInput(keyboardInput);

        // Act
        _inputSystem.ProcessInput();

        // Assert
        Assert.That(inputComponentOfEntity1.HardwareInput.KeyboardInput.Up, Is.True);
        Assert.That(inputComponentOfEntity2.HardwareInput.KeyboardInput.Up, Is.True);
        Assert.That(inputComponentOfEntity3.HardwareInput.KeyboardInput.Up, Is.True);
    }

    [Test]
    public void ProcessInput_ShouldNotSetHardwareInputOnRemovedInputComponents()
    {
        // Arrange
        var inputComponentOfEntity1 = _inputScene.AddInput();
        var inputComponentOfEntity2 = _inputScene.AddInput();
        var inputComponentOfEntity3 = _inputScene.AddInput();

        var keyboardInput = new KeyboardInputBuilder { Up = true }.Build();
        MockKeyboardInput(keyboardInput);

        inputComponentOfEntity1.Entity.RemoveComponent(inputComponentOfEntity1);
        inputComponentOfEntity3.Entity.RemoveComponent(inputComponentOfEntity3);

        // Act
        _inputSystem.ProcessInput();

        // Assert
        Assert.That(inputComponentOfEntity1.HardwareInput, Is.EqualTo(HardwareInput.Empty));
        Assert.That(inputComponentOfEntity2.HardwareInput.KeyboardInput.Up, Is.True);
        Assert.That(inputComponentOfEntity3.HardwareInput, Is.EqualTo(HardwareInput.Empty));
    }

    [Test]
    public void ProcessInput_ShouldNotSetHardwareInputOnDisabledInputComponents()
    {
        // Arrange
        var inputComponentOfEntity1 = _inputScene.AddInput();
        var inputComponentOfEntity2 = _inputScene.AddInput();
        var inputComponentOfEntity3 = _inputScene.AddInput();

        var keyboardInput = new KeyboardInputBuilder { Up = true }.Build();
        MockKeyboardInput(keyboardInput);

        inputComponentOfEntity1.Enabled = false;
        inputComponentOfEntity3.Enabled = false;

        // Act
        _inputSystem.ProcessInput();

        // Assert
        Assert.That(inputComponentOfEntity1.HardwareInput, Is.EqualTo(HardwareInput.Empty));
        Assert.That(inputComponentOfEntity2.HardwareInput.KeyboardInput.Up, Is.True);
        Assert.That(inputComponentOfEntity3.HardwareInput, Is.EqualTo(HardwareInput.Empty));
    }

    [Test]
    public void ProcessInput_ShouldNotCallActionBindings_WhenInputComponentIsDisabled()
    {
        // Arrange
        _inputScene.AddInputWithKeyboardActionMapping(out var inputComponent, out var moveRight, out _, out _);

        var keyboardInput1 = new KeyboardInputBuilder
        {
            Up = false,
            Down = false,
            Right = false,
            Left = false,
            Space = false
        }.Build();

        var keyboardInput2 = new KeyboardInputBuilder
        {
            Up = false,
            Down = false,
            Right = true,
            Left = false,
            Space = false
        }.Build();

        var callCounter = 0;
        inputComponent.BindAction(moveRight.ActionName, () => { callCounter++; });
        inputComponent.Enabled = false;

        // Act
        // Initialize action states based on hardware input
        MockKeyboardInput(keyboardInput1);
        _inputSystem.ProcessInput();

        MockKeyboardInput(keyboardInput2);
        _inputSystem.ProcessInput();

        // Assert
        Assert.That(callCounter, Is.EqualTo(0));
    }

    [Test]
    public void ProcessInput_ShouldNotCallAxisBindings_WhenInputComponentIsDisabled()
    {
        // Arrange
        _inputScene.AddInputWithKeyboardAxisMapping(out var inputComponent, out var moveUp, out _);

        var callCounter = 0;
        inputComponent.BindAxis(moveUp.AxisName, _ => { callCounter++; });
        inputComponent.Enabled = false;

        var allFalseKeyboardInput = new KeyboardInputBuilder
        {
            Up = false,
            Down = false,
            Right = false,
            Left = false,
            Space = false
        }.Build();

        var allTrueKeyboardInput = new KeyboardInputBuilder
        {
            Up = true,
            Down = true,
            Right = true,
            Left = true,
            Space = true
        }.Build();

        // Act
        MockKeyboardInput(allFalseKeyboardInput);
        _inputSystem.ProcessInput();

        MockKeyboardInput(allTrueKeyboardInput);
        _inputSystem.ProcessInput();

        // Assert
        Assert.That(callCounter, Is.EqualTo(0));
    }

    #endregion

    #region Keyboard test cases

    [TestCase(false, false, false, false, false, false, false)]
    [TestCase(true, true, true, true, true, true, true)]
    [TestCase(true, false, true, false, true, false, true)]
    [TestCase(false, true, false, true, false, true, true)]
    public void ProcessInput_Keyboard_ShouldSetActionStatesAccordingToKeyboardInputAndInputMapping(bool right, bool left, bool up,
        bool space, bool expectedRight, bool expectedLeft, bool expectedJump)
    {
        // Arrange
        _inputScene.AddInputWithKeyboardActionMapping(out var inputComponent, out var moveRight, out var moveLeft, out var jump);

        var keyboardInput = new KeyboardInputBuilder
        {
            Right = right,
            Left = left,
            Up = up,
            Space = space
        }.Build();
        MockKeyboardInput(keyboardInput);

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
    public void ProcessInput_Keyboard_ShouldSetAxisStatesAccordingToKeyboardInputAndInputMapping(bool up, bool down, bool right,
        bool left, bool space, double expectedUp, double expectedRight)
    {
        // Arrange
        _inputScene.AddInputWithKeyboardAxisMapping(out var inputComponent, out var moveUp, out var moveRight);

        var keyboardInput = new KeyboardInputBuilder
        {
            Up = up,
            Down = down,
            Right = right,
            Left = left,
            Space = space
        }.Build();
        MockKeyboardInput(keyboardInput);

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
    public void ProcessInput_Keyboard_ShouldCallActionBindingsAccordingToKeyboardInputAndInputMapping(bool right, bool left,
        bool up, bool space, int expectedRightCount, int expectedLeftCount, int expectedJumpCount)
    {
        // Arrange
        _inputScene.AddInputWithKeyboardActionMapping(out var inputComponent, out var moveRight, out var moveLeft, out var jump);

        var moveRightCallCounter = 0;
        var moveLeftCallCounter = 0;
        var jumpCallCounter = 0;

        inputComponent.BindAction(moveRight.ActionName, () => { moveRightCallCounter++; });
        inputComponent.BindAction(moveLeft.ActionName, () => { moveLeftCallCounter++; });
        inputComponent.BindAction(jump.ActionName, () => { jumpCallCounter++; });

        // Initialize action states based on keyboard input
        var initialKeyboardInput = new KeyboardInputBuilder
        {
            Right = false,
            Left = false,
            Up = false,
            Space = false
        }.Build();
        MockKeyboardInput(initialKeyboardInput);
        _inputSystem.ProcessInput();

        var keyboardInput = new KeyboardInputBuilder
        {
            Right = right,
            Left = left,
            Up = up,
            Space = space
        }.Build();
        MockKeyboardInput(keyboardInput);

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
    public void ProcessInput_Keyboard_ShouldCallAxisBindingsAccordingToKeyboardInputAndInputMapping(bool up, bool down, bool right,
        bool left, bool space, double expectedUp, double expectedRight)
    {
        // Arrange
        _inputScene.AddInputWithKeyboardAxisMapping(out var inputComponent, out var moveUp, out var moveRight);

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

        var keyboardInput = new KeyboardInputBuilder
        {
            Up = up,
            Down = down,
            Right = right,
            Left = left,
            Space = space
        }.Build();
        MockKeyboardInput(keyboardInput);

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
    public void ProcessInput_Mouse_ShouldSetActionStatesAccordingToMouseInputAndInputMapping(bool left, bool middle, bool right,
        bool x1, bool x2, bool expectedFire, bool expectedZoom, bool expectedAltFire, bool expectedMelee)
    {
        // Arrange
        _inputScene.AddInputWithMouseActionMapping(out var inputComponent, out var fire, out var zoom, out var altFire, out var melee);

        var mouseInput = new MouseInputBuilder
        {
            LeftButton = left,
            MiddleButton = middle,
            RightButton = right,
            XButton1 = x1,
            XButton2 = x2
        }.Build();
        MockMouseInput(mouseInput);

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
    public void ProcessInput_Mouse_ShouldSetAxisStatesAccordingToMouseInputAndInputMapping(int xPos, int yPos, double expectedLookRight,
        double expectedLookUp)
    {
        // Arrange
        _inputScene.AddInputWithMouseAxisMapping(out var inputComponent, out var lookRight, out var lookUp);

        var mouseInput = new MouseInputBuilder { PositionDelta = new Vector2(xPos, yPos) }.Build();
        MockMouseInput(mouseInput);

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
    public void ProcessInput_Mouse_ShouldCallActionBindingsAccordingToMouseInputAndInputMapping(bool left, bool middle, bool right,
        bool x1, bool x2, int expectedFireCount, int expectedZoomCount, int expectedAltFireCount, int expectedMeleeCount)
    {
        // Arrange
        _inputScene.AddInputWithMouseActionMapping(out var inputComponent, out var fire, out var zoom, out var altFire, out var melee);

        var fireCallCounter = 0;
        var zoomCallCounter = 0;
        var altFireCallCounter = 0;
        var meleeCallCounter = 0;

        inputComponent.BindAction(fire.ActionName, () => { fireCallCounter++; });
        inputComponent.BindAction(zoom.ActionName, () => { zoomCallCounter++; });
        inputComponent.BindAction(altFire.ActionName, () => { altFireCallCounter++; });
        inputComponent.BindAction(melee.ActionName, () => { meleeCallCounter++; });

        // Initialize action states based on mouse input
        var initialMouseInput = new MouseInputBuilder
        {
            LeftButton = false,
            MiddleButton = false,
            RightButton = false,
            XButton1 = false,
            XButton2 = false
        }.Build();
        MockMouseInput(initialMouseInput);
        _inputSystem.ProcessInput();

        var mouseInput = new MouseInputBuilder
        {
            LeftButton = left,
            MiddleButton = middle,
            RightButton = right,
            XButton1 = x1,
            XButton2 = x2
        }.Build();
        MockMouseInput(mouseInput);

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
    public void ProcessInput_Mouse_ShouldCallAxisBindingsAccordingToMouseInputAndInputMapping(int xPos, int yPos, double expectedLookRight,
        double expectedLookUp)
    {
        // Arrange
        _inputScene.AddInputWithMouseAxisMapping(out var inputComponent, out var lookRight, out var lookUp);

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

        var mouseInput = new MouseInputBuilder { PositionDelta = new Vector2(xPos, yPos) }.Build();
        MockMouseInput(mouseInput);

        // Act
        _inputSystem.ProcessInput();

        // Assert
        Assert.That(lookRightCallCounter, Is.EqualTo(1));
        Assert.That(lookUpCallCounter, Is.EqualTo(1));

        Assert.That(lookRightState, Is.EqualTo(expectedLookRight));
        Assert.That(lookUpState, Is.EqualTo(expectedLookUp));
    }

    #endregion

    #region Controller test cases

    [TestCase(ControllerIndex.One)]
    [TestCase(ControllerIndex.Two)]
    [TestCase(ControllerIndex.Three)]
    [TestCase(ControllerIndex.Four)]
    public void GetControllerState_ShouldReturnInputBackendControllerState(ControllerIndex controllerIndex)
    {
        // Arrange
        var controllerState = new ControllerState
        {
            PacketNumber = 123
        };

        MockControllerState(controllerIndex, controllerState);

        // Act
        var actual = _inputSystem.GetControllerState(controllerIndex);

        // Assert
        Assert.That(actual, Is.EqualTo(controllerState));
    }

    [Test]
    public void SetControllerVibration_ShouldSetControllerVibrationOnInputBackend()
    {
        // Arrange
        var vibration = new Vibration
        {
            LeftMotorSpeed = 12.34,
            RightMotorSpeed = 56.78
        };

        // Act
        _inputSystem.SetControllerVibration(ControllerIndex.One, vibration);

        // Assert
        _inputBackend.Received(1).SetControllerVibration(ControllerIndex.One, vibration);
    }

    [TestCase(false, false, false, false, false, false, false)]
    [TestCase(true, true, true, true, true, true, true)]
    [TestCase(true, false, true, false, true, false, true)]
    [TestCase(false, true, false, true, false, true, true)]
    public void ProcessInput_Controller_ShouldSetActionStatesAccordingToControllerStateAndInputMapping(bool dPadRight, bool dPadLeft, bool dPadUp,
        bool buttonA, bool expectedRight, bool expectedLeft, bool expectedJump)
    {
        // Arrange
        _inputScene.AddInputWithControllerActionMapping(out var inputComponent, out var moveRight, out var moveLeft, out var jump);

        var buttons = ControllerButtons.None;
        buttons |= dPadRight ? ControllerButtons.DPadRight : ControllerButtons.None;
        buttons |= dPadLeft ? ControllerButtons.DPadLeft : ControllerButtons.None;
        buttons |= dPadUp ? ControllerButtons.DPadUp : ControllerButtons.None;
        buttons |= buttonA ? ControllerButtons.A : ControllerButtons.None;

        var controllerState = new ControllerState
        {
            Index = ControllerIndex.One,
            Buttons = buttons
        };

        MockControllerState(ControllerIndex.One, controllerState);

        // Act
        _inputSystem.ProcessInput();

        // Assert
        Assert.That(inputComponent.GetActionState(moveRight.ActionName), Is.EqualTo(expectedRight));
        Assert.That(inputComponent.GetActionState(moveLeft.ActionName), Is.EqualTo(expectedLeft));
        Assert.That(inputComponent.GetActionState(jump.ActionName), Is.EqualTo(expectedJump));
    }


    [TestCase(0, 0, 0, 0)]
    [TestCase(0.5, 0.7, 0.5, 0.7)]
    [TestCase(-0.5, -0.7, -0.5, -0.7)]
    public void ProcessInput_Controller_ShouldSetAxisStatesAccordingToControllerStateAndInputMapping(double xPos, double yPos, double expectedLookRight,
        double expectedLookUp)
    {
        // Arrange
        _inputScene.AddInputWithControllerAxisMapping(out var inputComponent, out var lookRight, out var lookUp);

        var controllerState = new ControllerState
        {
            Index = ControllerIndex.One,
            RightThumb = new Vector2(xPos, yPos)
        };

        MockControllerState(ControllerIndex.One, controllerState);

        // Act
        _inputSystem.ProcessInput();

        // Assert
        Assert.That(inputComponent.GetAxisState(lookRight.AxisName), Is.EqualTo(expectedLookRight));
        Assert.That(inputComponent.GetAxisState(lookUp.AxisName), Is.EqualTo(expectedLookUp));
    }

    [TestCase(false, false, false, false, 0, 0, 0)]
    [TestCase(true, true, true, true, 1, 1, 1)]
    [TestCase(true, false, true, false, 1, 0, 1)]
    [TestCase(false, true, false, true, 0, 1, 1)]
    public void ProcessInput_Controller_ShouldCallActionBindingsAccordingToControllerStateAndInputMapping(bool dPadRight, bool dPadLeft,
        bool dPadUp, bool buttonA, int expectedRightCount, int expectedLeftCount, int expectedJumpCount)
    {
        // Arrange
        _inputScene.AddInputWithControllerActionMapping(out var inputComponent, out var moveRight, out var moveLeft, out var jump);

        var moveRightCallCounter = 0;
        var moveLeftCallCounter = 0;
        var jumpCallCounter = 0;

        inputComponent.BindAction(moveRight.ActionName, () => { moveRightCallCounter++; });
        inputComponent.BindAction(moveLeft.ActionName, () => { moveLeftCallCounter++; });
        inputComponent.BindAction(jump.ActionName, () => { jumpCallCounter++; });

        // Initialize action states based on controller state
        MockControllerState(ControllerIndex.One, new ControllerState());
        _inputSystem.ProcessInput();

        var buttons = ControllerButtons.None;
        buttons |= dPadRight ? ControllerButtons.DPadRight : ControllerButtons.None;
        buttons |= dPadLeft ? ControllerButtons.DPadLeft : ControllerButtons.None;
        buttons |= dPadUp ? ControllerButtons.DPadUp : ControllerButtons.None;
        buttons |= buttonA ? ControllerButtons.A : ControllerButtons.None;

        var controllerState = new ControllerState
        {
            Index = ControllerIndex.One,
            Buttons = buttons
        };

        MockControllerState(ControllerIndex.One, controllerState);

        // Act
        _inputSystem.ProcessInput();

        // Assert
        Assert.That(moveRightCallCounter, Is.EqualTo(expectedRightCount));
        Assert.That(moveLeftCallCounter, Is.EqualTo(expectedLeftCount));
        Assert.That(jumpCallCounter, Is.EqualTo(expectedJumpCount));
    }

    [TestCase(0, 0, 0, 0)]
    [TestCase(0.5, 0.7, 0.5, 0.7)]
    [TestCase(-0.5, -0.7, -0.5, -0.7)]
    public void ProcessInput_Controller_ShouldCallAxisBindingsAccordingToControllerStateAndInputMapping(double xPos, double yPos, double expectedLookRight,
        double expectedLookUp)
    {
        // Arrange
        _inputScene.AddInputWithControllerAxisMapping(out var inputComponent, out var lookRight, out var lookUp);

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

        var controllerState = new ControllerState
        {
            Index = ControllerIndex.One,
            RightThumb = new Vector2(xPos, yPos)
        };

        MockControllerState(ControllerIndex.One, controllerState);

        // Act
        _inputSystem.ProcessInput();

        // Assert
        Assert.That(lookRightCallCounter, Is.EqualTo(1));
        Assert.That(lookUpCallCounter, Is.EqualTo(1));

        Assert.That(lookRightState, Is.EqualTo(expectedLookRight));
        Assert.That(lookUpState, Is.EqualTo(expectedLookUp));
    }

    // TODO: Add tests covering different controller index.

    #endregion

    #region Regression test cases

    [Test]
    public void ProcessInput_ShouldNotThrowException_WhenEntityIsAddedInInputBindingFunction()
    {
        // Arrange
        _inputScene.AddInputWithKeyboardActionMapping(out var inputComponent, out _, out _, out var jump);

        inputComponent.BindAction(jump.ActionName, () => { _inputScene.Scene.CreateEntity(); });

        var keyboardInput = new KeyboardInputBuilder { Space = true }.Build();
        MockKeyboardInput(keyboardInput);

        // Act
        // Assert
        Assert.That(() => _inputSystem.ProcessInput(), Throws.Nothing);
    }

    #endregion

    #region Helpers

    private class InputScene
    {
        public InputScene(ISceneObserver observer)
        {
            Scene.AddObserver(observer);
        }

        public Scene Scene { get; } = TestSceneFactory.Create();

        public InputComponent AddInput()
        {
            var entity = Scene.CreateEntity();
            return entity.CreateComponent<InputComponent>();
        }

        public void AddInputWithKeyboardActionMapping(out InputComponent inputComponent, out ActionMapping moveRight, out ActionMapping moveLeft,
            out ActionMapping jump)
        {
            var inputMapping = InputMapping
                .CreateBuilder()
                .MapAction(nameof(moveRight), Key.Right)
                .MapAction(nameof(moveLeft), Key.Left)
                .MapAction(nameof(jump), Key.Up)
                .MapAction(nameof(jump), Key.Space)
                .Build();

            moveRight = GetActionMapping(inputMapping, nameof(moveRight));
            moveLeft = GetActionMapping(inputMapping, nameof(moveLeft));
            jump = GetActionMapping(inputMapping, nameof(jump));

            var entity = Scene.CreateEntity();
            inputComponent = entity.CreateComponent<InputComponent>();
            inputComponent.InputMapping = inputMapping;
        }

        public void AddInputWithKeyboardAxisMapping(out InputComponent inputComponent, out AxisMapping moveUp, out AxisMapping moveRight)
        {
            var inputMapping = InputMapping
                .CreateBuilder()
                .MapAxis(nameof(moveUp), Key.Up, 1.0)
                .MapAxis(nameof(moveUp), Key.Down, -1.0)
                .MapAxis(nameof(moveUp), Key.Space, 5.0)
                .MapAxis(nameof(moveRight), Key.Right, 1.0)
                .MapAxis(nameof(moveRight), Key.Left, -1.0)
                .Build();

            moveUp = GetAxisMapping(inputMapping, nameof(moveUp));
            moveRight = GetAxisMapping(inputMapping, nameof(moveRight));

            var entity = Scene.CreateEntity();
            inputComponent = entity.CreateComponent<InputComponent>();
            inputComponent.InputMapping = inputMapping;
        }

        public void AddInputWithMouseActionMapping(out InputComponent inputComponent, out ActionMapping fire, out ActionMapping zoom,
            out ActionMapping altFire, out ActionMapping melee)
        {
            var inputMapping = InputMapping
                .CreateBuilder()
                .MapAction(nameof(fire), MouseButton.Left)
                .MapAction(nameof(zoom), MouseButton.Middle)
                .MapAction(nameof(altFire), MouseButton.Right)
                .MapAction(nameof(melee), MouseButton.XButton1)
                .MapAction(nameof(melee), MouseButton.XButton2)
                .Build();

            fire = GetActionMapping(inputMapping, nameof(fire));
            zoom = GetActionMapping(inputMapping, nameof(zoom));
            altFire = GetActionMapping(inputMapping, nameof(altFire));
            melee = GetActionMapping(inputMapping, nameof(melee));

            var entity = Scene.CreateEntity();
            inputComponent = entity.CreateComponent<InputComponent>();
            inputComponent.InputMapping = inputMapping;
        }

        public void AddInputWithMouseAxisMapping(out InputComponent inputComponent, out AxisMapping lookRight, out AxisMapping lookUp)
        {
            var inputMapping = InputMapping
                .CreateBuilder()
                .MapAxis(nameof(lookRight), MouseAxis.X, 1.0)
                .MapAxis(nameof(lookUp), MouseAxis.Y, 1.0)
                .Build();

            lookRight = GetAxisMapping(inputMapping, nameof(lookRight));
            lookUp = GetAxisMapping(inputMapping, nameof(lookUp));

            var entity = Scene.CreateEntity();
            inputComponent = entity.CreateComponent<InputComponent>();
            inputComponent.InputMapping = inputMapping;
        }

        public void AddInputWithControllerActionMapping(out InputComponent inputComponent, out ActionMapping moveRight, out ActionMapping moveLeft,
            out ActionMapping jump)
        {
            var inputMapping = InputMapping
                .CreateBuilder()
                .MapAction(nameof(moveRight), ControllerIndex.One, ControllerButton.DPadRight)
                .MapAction(nameof(moveLeft), ControllerIndex.One, ControllerButton.DPadLeft)
                .MapAction(nameof(jump), ControllerIndex.One, ControllerButton.DPadUp)
                .MapAction(nameof(jump), ControllerIndex.One, ControllerButton.A)
                .Build();

            moveRight = GetActionMapping(inputMapping, nameof(moveRight));
            moveLeft = GetActionMapping(inputMapping, nameof(moveLeft));
            jump = GetActionMapping(inputMapping, nameof(jump));

            var entity = Scene.CreateEntity();
            inputComponent = entity.CreateComponent<InputComponent>();
            inputComponent.InputMapping = inputMapping;
        }

        public void AddInputWithControllerAxisMapping(out InputComponent inputComponent, out AxisMapping lookRight, out AxisMapping lookUp)
        {
            var inputMapping = InputMapping
                .CreateBuilder()
                .MapAxis(nameof(lookRight), ControllerIndex.One, ControllerAxis.RightThumbX, 1.0)
                .MapAxis(nameof(lookUp), ControllerIndex.One, ControllerAxis.RightThumbY, 1.0)
                .Build();

            lookRight = GetAxisMapping(inputMapping, nameof(lookRight));
            lookUp = GetAxisMapping(inputMapping, nameof(lookUp));

            var entity = Scene.CreateEntity();
            inputComponent = entity.CreateComponent<InputComponent>();
            inputComponent.InputMapping = inputMapping;
        }

        private static ActionMapping GetActionMapping(InputMapping inputMapping, string actionName)
            => inputMapping.ActionMappings.Single(am => am.ActionName == actionName);

        private static AxisMapping GetAxisMapping(InputMapping inputMapping, string axisName)
            => inputMapping.AxisMappings.Single(am => am.AxisName == axisName);
    }

    #endregion
}