using System.Collections.Generic;
using Geisha.Engine.Input;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input;

[TestFixture]
public class InputSourceTests
{
    #region KeyboardKey

    [Test]
    public void Create_Key_ShouldSetInputTypeToKeyboardKey()
    {
        // Arrange
        // Act
        var inputSource = InputSource.Create(Key.Space);

        // Assert
        Assert.That(inputSource.Kind, Is.EqualTo(InputSource.InputKind.KeyboardKey));
    }

    [Test]
    public void AsKeyboardKey_ShouldReturnKeyboardKey_WhenInputTypeIsKeyboard()
    {
        // Arrange
        const Key key = Key.Space;
        var inputSource = InputSource.Create(key);

        // Act
        // Assert
        Assert.That(inputSource.AsKeyboardKey(), Is.EqualTo(key));
    }

    [Test]
    public void AsKeyboardKey_ShouldThrowException_WhenInputTypeIsNotKeyboard()
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
    public void Create_MouseButton_ShouldSetInputTypeToMouseButton()
    {
        // Arrange
        // Act
        var inputSource = InputSource.Create(MouseButton.Right);

        // Assert
        Assert.That(inputSource.Kind, Is.EqualTo(InputSource.InputKind.MouseButton));
    }

    [Test]
    public void AsMouseButton_ShouldReturnMouseButton_WhenInputTypeIsMouseButton()
    {
        // Arrange
        const MouseButton mouseButton = MouseButton.Right;
        var inputSource = InputSource.Create(mouseButton);

        // Act
        // Assert
        Assert.That(inputSource.AsMouseButton(), Is.EqualTo(mouseButton));
    }

    [Test]
    public void AsMouseButton_ShouldThrowException_WhenInputTypeIsNotMouseButton()
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
    public void Create_MouseAxis_ShouldSetInputTypeToMouseAxis()
    {
        // Arrange
        // Act
        var inputSource = InputSource.Create(MouseAxis.Y);

        // Assert
        Assert.That(inputSource.Kind, Is.EqualTo(InputSource.InputKind.MouseAxis));
    }

    [Test]
    public void AsMouseAxis_ShouldReturnMouseAxis_WhenInputTypeIsMouseAxis()
    {
        // Arrange
        const MouseAxis mouseAxis = MouseAxis.Y;
        var inputSource = InputSource.Create(mouseAxis);

        // Act
        // Assert
        Assert.That(inputSource.AsMouseAxis(), Is.EqualTo(mouseAxis));
    }

    [Test]
    public void AsMouseAxis_ShouldThrowException_WhenInputTypeIsNotMouseAxis()
    {
        // Arrange
        var inputSource = InputSource.Create(Key.Space);

        // Act
        // Assert
        Assert.That(inputSource.AsMouseAxis, Throws.InvalidOperationException);
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
            .SetName("MouseAxis")
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
