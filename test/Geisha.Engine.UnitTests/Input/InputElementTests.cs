using System.Collections.Generic;
using Geisha.Engine.Input;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input;

[TestFixture]
public class InputElementTests
{
    #region KeyboardKey

    [Test]
    public void Create_Key_ShouldSetInputTypeToKeyboard()
    {
        // Arrange
        // Act
        var inputElement = InputElement.Create(Key.Space);

        // Assert
        Assert.That(inputElement.Kind, Is.EqualTo(InputElement.InputKind.KeyboardKey));
    }

    [Test]
    public void AsKeyboardKey_ShouldReturnKeyboardKey_WhenInputTypeIsKeyboard()
    {
        // Arrange
        const Key key = Key.Space;
        var inputElement = InputElement.Create(key);

        // Act
        // Assert
        Assert.That(inputElement.AsKeyboardKey(), Is.EqualTo(key));
    }

    [Test]
    public void AsKeyboardKey_ShouldThrowException_WhenInputTypeIsNotKeyboard()
    {
        // Arrange
        var inputElement = InputElement.Create(MouseButton.Left);

        // Act
        // Assert
        Assert.That(inputElement.AsKeyboardKey, Throws.InvalidOperationException);
    }

    #endregion

    #region MouseButton

    [Test]
    public void Create_MouseButton_ShouldSetInputTypeToMouseButton()
    {
        // Arrange
        // Act
        var inputElement = InputElement.Create(MouseButton.Right);

        // Assert
        Assert.That(inputElement.Kind, Is.EqualTo(InputElement.InputKind.MouseButton));
    }

    [Test]
    public void AsMouseButton_ShouldReturnMouseButton_WhenInputTypeIsMouseButton()
    {
        // Arrange
        const MouseButton mouseButton = MouseButton.Right;
        var inputElement = InputElement.Create(mouseButton);

        // Act
        // Assert
        Assert.That(inputElement.AsMouseButton(), Is.EqualTo(mouseButton));
    }

    [Test]
    public void AsMouseButton_ShouldThrowException_WhenInputTypeIsNotMouseButton()
    {
        // Arrange
        var inputElement = InputElement.Create(Key.Space);

        // Act
        // Assert
        Assert.That(inputElement.AsMouseButton, Throws.InvalidOperationException);
    }

    #endregion

    #region MouseAxis

    [Test]
    public void Create_MouseAxis_ShouldSetInputTypeToMouseAxis()
    {
        // Arrange
        // Act
        var inputElement = InputElement.Create(MouseAxis.Y);

        // Assert
        Assert.That(inputElement.Kind, Is.EqualTo(InputElement.InputKind.MouseAxis));
    }

    [Test]
    public void AsMouseAxis_ShouldReturnMouseAxis_WhenInputTypeIsMouseAxis()
    {
        // Arrange
        const MouseAxis mouseAxis = MouseAxis.Y;
        var inputElement = InputElement.Create(mouseAxis);

        // Act
        // Assert
        Assert.That(inputElement.AsMouseAxis(), Is.EqualTo(mouseAxis));
    }

    [Test]
    public void AsMouseAxis_ShouldThrowException_WhenInputTypeIsNotMouseAxis()
    {
        // Arrange
        var inputElement = InputElement.Create(Key.Space);

        // Act
        // Assert
        Assert.That(inputElement.AsMouseAxis, Throws.InvalidOperationException);
    }

    #endregion

    #region Formatting members

    private static IEnumerable<TestCaseData> ToStringTestCases => new[]
    {
        new TestCaseData(InputElement.Create(Key.Space), "InputElement { Kind = KeyboardKey, Key = Space }")
            .SetName("KeyboardKey"),
        new TestCaseData(InputElement.Create(MouseButton.Left), "InputElement { Kind = MouseButton, Button = Left }")
            .SetName("MouseButton"),
        new TestCaseData(InputElement.Create(MouseAxis.X), "InputElement { Kind = MouseAxis, Axis = X }")
            .SetName("MouseAxis")
    };

    [TestCaseSource(nameof(ToStringTestCases))]
    public void ToString_Test(InputElement inputElement, string expected)
    {
        // Arrange
        // Act
        var actual = inputElement.ToString();

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    #endregion
}