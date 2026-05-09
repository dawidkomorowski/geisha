using System.Collections.Generic;
using Geisha.Engine.Input;
using Geisha.Engine.Input.Mapping;
using NUnit.Framework;

namespace Geisha.Engine.UnitTests.Input.Mapping;

[TestFixture]
public class HardwareInputVariantTests
{
    #region Keyboard

    [Test]
    public void CreateKeyboardVariant_ShouldSetCurrentVariantToKeyboard()
    {
        // Arrange

        // Act
        var hardwareInputVariant = HardwareInputVariant.Create(Key.Space);

        // Assert
        Assert.That(hardwareInputVariant.CurrentVariant, Is.EqualTo(HardwareInputVariant.Variant.Keyboard));
    }

    [Test]
    public void AsKeyboard_ShouldReturnKeyboardKey_WhenVariantIsKeyboard()
    {
        // Arrange
        const Key key = Key.Space;
        var hardwareInputVariant = HardwareInputVariant.Create(key);

        // Act
        // Assert
        Assert.That(hardwareInputVariant.AsKeyboard(), Is.EqualTo(key));
    }

    [Test]
    public void AsKeyboard_ShouldThrowException_WhenVariantIsNotKeyboard()
    {
        // Arrange
        var hardwareInputVariant = HardwareInputVariant.Create(HardwareInputVariant.MouseVariant.LeftButton);

        // Act
        // Assert
        Assert.That(hardwareInputVariant.AsKeyboard, Throws.InvalidOperationException);
    }

    #endregion

    #region Mouse

    [Test]
    public void CreateMouseVariant_ShouldSetCurrentVariantToMouse()
    {
        // Arrange

        // Act
        var hardwareInputVariant = HardwareInputVariant.Create(HardwareInputVariant.MouseVariant.LeftButton);

        // Assert
        Assert.That(hardwareInputVariant.CurrentVariant, Is.EqualTo(HardwareInputVariant.Variant.Mouse));
    }

    [Test]
    public void AsMouse_ShouldReturnMouseVariant_WhenVariantIsMouse()
    {
        // Arrange
        const HardwareInputVariant.MouseVariant mouseVariant = HardwareInputVariant.MouseVariant.LeftButton;
        var hardwareInputVariant = HardwareInputVariant.Create(mouseVariant);

        // Act
        // Assert
        Assert.That(hardwareInputVariant.AsMouse(), Is.EqualTo(mouseVariant));
    }

    [Test]
    public void AsMouse_ShouldThrowException_WhenVariantIsNotMouse()
    {
        // Arrange
        var hardwareInputVariant = HardwareInputVariant.Create(Key.Space);

        // Act
        // Assert
        Assert.That(hardwareInputVariant.AsMouse, Throws.InvalidOperationException);
    }

    #endregion

    #region Formatting members

    private static IEnumerable<TestCaseData> ToStringTestCases => new[]
    {
        new TestCaseData(HardwareInputVariant.Create(Key.Space),
                "HardwareInputVariant { CurrentVariant = Keyboard, KeyboardVariant = Space }")
            .SetName("KeyboardVariant"),
        new TestCaseData(HardwareInputVariant.Create(HardwareInputVariant.MouseVariant.LeftButton),
                "HardwareInputVariant { CurrentVariant = Mouse, MouseVariant = LeftButton }")
            .SetName("MouseVariant")
    };

    [TestCaseSource(nameof(ToStringTestCases))]
    public void ToString_Test(HardwareInputVariant variant, string expected)
    {
        // Arrange
        // Act
        var actual = variant.ToString();

        // Assert
        Assert.That(actual, Is.EqualTo(expected));
    }

    #endregion
}