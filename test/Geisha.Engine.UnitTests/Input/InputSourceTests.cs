using System.Collections.Generic;
using Geisha.Engine.Input;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input;

[TestFixture]
public class InputSourceTests
{
    #region KeyboardKey

    [Test]
    public void Create_Key_ShouldSetInputKindToKeyboardKey()
    {
        // Arrange
        // Act
        var inputSource = InputSource.Create(Key.Space);

        // Assert
        Assert.That(inputSource.Kind, Is.EqualTo(InputSource.InputKind.KeyboardKey));
    }

    [Test]
    public void AsKeyboardKey_ShouldReturnKeyboardKey_WhenInputKindIsKeyboardKey()
    {
        // Arrange
        const Key key = Key.Space;
        var inputSource = InputSource.Create(key);

        // Act
        // Assert
        Assert.That(inputSource.AsKeyboardKey(), Is.EqualTo(key));
    }

    [Test]
    public void AsKeyboardKey_ShouldThrowException_WhenInputKindIsNotKeyboardKey()
    {
        // Arrange
        var inputSource = InputSource.Create(MouseButton.Left);

        // Act
        // Assert
        Assert.That(inputSource.AsKeyboardKey, Throws.InvalidOperationException);
    }

    #endregion

    #region MouseButton

    [Test]
    public void Create_MouseButton_ShouldSetInputKindToMouseButton()
    {
        // Arrange
        // Act
        var inputSource = InputSource.Create(MouseButton.Right);

        // Assert
        Assert.That(inputSource.Kind, Is.EqualTo(InputSource.InputKind.MouseButton));
    }

    [Test]
    public void AsMouseButton_ShouldReturnMouseButton_WhenInputKindIsMouseButton()
    {
        // Arrange
        const MouseButton mouseButton = MouseButton.Right;
        var inputSource = InputSource.Create(mouseButton);

        // Act
        // Assert
        Assert.That(inputSource.AsMouseButton(), Is.EqualTo(mouseButton));
    }

    [Test]
    public void AsMouseButton_ShouldThrowException_WhenInputKindIsNotMouseButton()
    {
        // Arrange
        var inputSource = InputSource.Create(Key.Space);

        // Act
        // Assert
        Assert.That(inputSource.AsMouseButton, Throws.InvalidOperationException);
    }

    #endregion

    #region MouseAxis

    [Test]
    public void Create_MouseAxis_ShouldSetInputKindToMouseAxis()
    {
        // Arrange
        // Act
        var inputSource = InputSource.Create(MouseAxis.Y);

        // Assert
        Assert.That(inputSource.Kind, Is.EqualTo(InputSource.InputKind.MouseAxis));
    }

    [Test]
    public void AsMouseAxis_ShouldReturnMouseAxis_WhenInputKindIsMouseAxis()
    {
        // Arrange
        const MouseAxis mouseAxis = MouseAxis.Y;
        var inputSource = InputSource.Create(mouseAxis);

        // Act
        // Assert
        Assert.That(inputSource.AsMouseAxis(), Is.EqualTo(mouseAxis));
    }

    [Test]
    public void AsMouseAxis_ShouldThrowException_WhenInputKindIsNotMouseAxis()
    {
        // Arrange
        var inputSource = InputSource.Create(Key.Space);

        // Act
        // Assert
        Assert.That(inputSource.AsMouseAxis, Throws.InvalidOperationException);
    }

    #endregion

    #region ControllerButton

    [Test]
    public void Create_ControllerButton_ShouldSetInputKindToControllerButton()
    {
        // Arrange
        // Act
        var inputSource = InputSource.Create(ControllerIndex.Four, ControllerButton.Start);

        // Assert
        Assert.That(inputSource.Kind, Is.EqualTo(InputSource.InputKind.ControllerButton));
    }

    [Test]
    public void AsControllerButton_ShouldReturnControllerButton_WhenInputKindIsControllerButton()
    {
        // Arrange
        const ControllerIndex index = ControllerIndex.Four;
        const ControllerButton button = ControllerButton.Back;
        var inputSource = InputSource.Create(index, button);

        // Act
        var controllerButton = inputSource.AsControllerButton();

        // Assert
        Assert.That(controllerButton.Index, Is.EqualTo(index));
        Assert.That(controllerButton.Button, Is.EqualTo(button));
    }

    [Test]
    public void AsControllerButton_ShouldThrowException_WhenInputKindIsNotControllerButton()
    {
        // Arrange
        var inputSource = InputSource.Create(Key.Space);

        // Act
        // Assert
        Assert.That(inputSource.AsControllerButton, Throws.InvalidOperationException);
    }

    #endregion

    #region ControllerAxis

    [Test]
    public void Create_ControllerAxis_ShouldSetInputKindToControllerAxis()
    {
        // Arrange
        // Act
        var inputSource = InputSource.Create(ControllerIndex.Four, ControllerAxis.RightTrigger);

        // Assert
        Assert.That(inputSource.Kind, Is.EqualTo(InputSource.InputKind.ControllerAxis));
    }

    [Test]
    public void AsControllerAxis_ShouldReturnControllerAxis_WhenInputKindIsControllerAxis()
    {
        // Arrange
        const ControllerIndex index = ControllerIndex.Four;
        const ControllerAxis axis = ControllerAxis.RightTrigger;
        var inputSource = InputSource.Create(index, axis);

        // Act
        var controllerAxis = inputSource.AsControllerAxis();

        // Assert
        Assert.That(controllerAxis.Index, Is.EqualTo(index));
        Assert.That(controllerAxis.Axis, Is.EqualTo(axis));
    }

    [Test]
    public void AsControllerAxis_ShouldThrowException_WhenInputKindIsNotControllerAxis()
    {
        // Arrange
        var inputSource = InputSource.Create(Key.Space);

        // Act
        // Assert
        Assert.That(inputSource.AsControllerAxis, Throws.InvalidOperationException);
    }

    #endregion

    #region Formatting members

    private static IEnumerable<TestCaseData> ToStringTestCases => new[]
    {
        new TestCaseData(InputSource.Create(Key.Space), "InputSource { Kind = KeyboardKey, Key = Space }")
            .SetName("KeyboardKey"),
        new TestCaseData(InputSource.Create(MouseButton.Left), "InputSource { Kind = MouseButton, Button = Left }")
            .SetName("MouseButton"),
        new TestCaseData(InputSource.Create(MouseAxis.X), "InputSource { Kind = MouseAxis, Axis = X }")
            .SetName("MouseAxis"),
        new TestCaseData(InputSource.Create(ControllerIndex.Four, ControllerButton.Back),
                "InputSource { Kind = ControllerButton, Index = Four, Button = Back }")
            .SetName("ControllerButton"),
        new TestCaseData(InputSource.Create(ControllerIndex.Four, ControllerAxis.RightTrigger),
                "InputSource { Kind = ControllerAxis, Index = Four, Axis = RightTrigger }")
            .SetName("ControllerAxis")
    };

    [TestCaseSource(nameof(ToStringTestCases))]
    public void ToString_Test(InputSource inputSource, string expected)
    {
        // Arrange
        // Act
        var actual = inputSource.ToString();

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    #endregion
}