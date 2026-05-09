using System.Collections.Generic;
using Geisha.Engine.Input;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input;

[TestFixture]
public class InputElementTests
{
    #region Keyboard

    [Test]
    public void Create_Key_ShouldSetInputTypeToKeyboard()
    {
        // Arrange
        // Act
        var inputElement = InputElement.Create(Key.Space);

        // Assert
        Assert.That(inputElement.Type, Is.EqualTo(InputElement.InputType.Keyboard));
    }

    [Test]
    public void AsKeyboard_ShouldReturnKeyboardKey_WhenInputTypeIsKeyboard()
    {
        // Arrange
        const Key key = Key.Space;
        var inputElement = InputElement.Create(key);

        // Act
        // Assert
        Assert.That(inputElement.AsKeyboard(), Is.EqualTo(key));
    }

    [Test]
    public void AsKeyboard_ShouldThrowException_WhenInputTypeIsNotKeyboard()
    {
        // Arrange
        var inputElement = InputElement.Create(MouseButton.Left);

        // Act
        // Assert
        Assert.That(inputElement.AsKeyboard, Throws.InvalidOperationException);
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
        Assert.That(inputElement.Type, Is.EqualTo(InputElement.InputType.MouseButton));
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

    #region Mouse

    [Test]
    public void Create_MouseVariant_ShouldSetCurrentVariantToMouse()
    {
        // Arrange

        // Act
        var inputElement = InputElement.Create(InputElement.MouseVariant.LeftButton);

        // Assert
        Assert.That(inputElement.Type, Is.EqualTo(InputElement.InputType.Mouse));
    }

    [Test]
    public void AsMouse_ShouldReturnMouseVariant_WhenVariantIsMouse()
    {
        // Arrange
        const InputElement.MouseVariant mouseVariant = InputElement.MouseVariant.LeftButton;
        var inputElement = InputElement.Create(mouseVariant);

        // Act
        // Assert
        Assert.That(inputElement.AsMouse(), Is.EqualTo(mouseVariant));
    }

    [Test]
    public void AsMouse_ShouldThrowException_WhenVariantIsNotMouse()
    {
        // Arrange
        var inputElement = InputElement.Create(Key.Space);

        // Act
        // Assert
        Assert.That(inputElement.AsMouse, Throws.InvalidOperationException);
    }

    #endregion

    #region Formatting members

    private static IEnumerable<TestCaseData> ToStringTestCases => new[]
    {
        new TestCaseData(InputElement.Create(Key.Space), "InputElement { Type = Keyboard, KeyboardKey = Space }")
            .SetName("KeyboardVariant"),
        new TestCaseData(InputElement.Create(MouseButton.Left), "InputElement { Type = MouseButton, MouseButton = Left }")
            .SetName("MouseButton"),
        new TestCaseData(InputElement.Create(InputElement.MouseVariant.LeftButton), "InputElement { Type = Mouse, MouseVariant = LeftButton }")
            .SetName("MouseVariant")
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