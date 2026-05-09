using System.Collections.Generic;
using Geisha.Engine.Input;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input;

[TestFixture]
public class InputElementTests
{
    #region Keyboard

    [Test]
    public void Create_Key_ShouldSetCurrentVariantToKeyboard()
    {
        // Arrange

        // Act
        var hardwareInputVariant = InputElement.Create(Key.Space);

        // Assert
        Assert.That(hardwareInputVariant.CurrentVariant, Is.EqualTo(InputElement.Variant.Keyboard));
    }

    [Test]
    public void AsKeyboard_ShouldReturnKeyboardKey_WhenVariantIsKeyboard()
    {
        // Arrange
        const Key key = Key.Space;
        var hardwareInputVariant = InputElement.Create(key);

        // Act
        // Assert
        Assert.That(hardwareInputVariant.AsKeyboard(), Is.EqualTo(key));
    }

    [Test]
    public void AsKeyboard_ShouldThrowException_WhenVariantIsNotKeyboard()
    {
        // Arrange
        var hardwareInputVariant = InputElement.Create(InputElement.MouseVariant.LeftButton);

        // Act
        // Assert
        Assert.That(hardwareInputVariant.AsKeyboard, Throws.InvalidOperationException);
    }

    #endregion

    #region Mouse

    [Test]
    public void Create_MouseVariant_ShouldSetCurrentVariantToMouse()
    {
        // Arrange

        // Act
        var hardwareInputVariant = InputElement.Create(InputElement.MouseVariant.LeftButton);

        // Assert
        Assert.That(hardwareInputVariant.CurrentVariant, Is.EqualTo(InputElement.Variant.Mouse));
    }

    [Test]
    public void AsMouse_ShouldReturnMouseVariant_WhenVariantIsMouse()
    {
        // Arrange
        const InputElement.MouseVariant mouseVariant = InputElement.MouseVariant.LeftButton;
        var hardwareInputVariant = InputElement.Create(mouseVariant);

        // Act
        // Assert
        Assert.That(hardwareInputVariant.AsMouse(), Is.EqualTo(mouseVariant));
    }

    [Test]
    public void AsMouse_ShouldThrowException_WhenVariantIsNotMouse()
    {
        // Arrange
        var hardwareInputVariant = InputElement.Create(Key.Space);

        // Act
        // Assert
        Assert.That(hardwareInputVariant.AsMouse, Throws.InvalidOperationException);
    }

    #endregion

    #region Formatting members

    private static IEnumerable<TestCaseData> ToStringTestCases => new[]
    {
        new TestCaseData(InputElement.Create(Key.Space),
                "InputElement { CurrentVariant = Keyboard, KeyboardVariant = Space }")
            .SetName("KeyboardVariant"),
        new TestCaseData(InputElement.Create(InputElement.MouseVariant.LeftButton),
                "InputElement { CurrentVariant = Mouse, MouseVariant = LeftButton }")
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