using System;
using System.Text;

namespace Geisha.Engine.Input;

// TODO: Review documentation of MouseAxis.
// TODO: Review documentation of MouseButton.
// TODO: Review implementation of InputMappingAssetContent.
// TODO: Consider extending/refatoring integration test: GetAsset_ShouldLoadAndReturn_InputMapping
// TODO: Review implementation of InputMappingAssetLoader.
// TODO: Find legacy name usages (HardwareInputVariant, variant) and update to use new name.
// TODO: Review documentation of InputElement.

/// <summary>
///     Represents single element of <see cref="HardwareInput" /> like a particular keyboard key, mouse button, mouse axis.
/// </summary>
public readonly record struct InputElement
{
    private readonly Key _keyboardVariant;
    private readonly MouseVariant _mouseVariant;

    /// <summary>
    ///     Creates new instance of <see cref="InputElement" /> that represents keyboard input variant like a
    ///     particular keyboard key.
    /// </summary>
    /// <param name="keyboardVariant">
    ///     Variant of keyboard input to be represented by <see cref="InputElement" />
    ///     instance.
    /// </param>
    /// <returns><see cref="InputElement" /> representing specified keyboard variant.</returns>
    public static InputElement Create(Key keyboardVariant) => new(keyboardVariant);

    /// <summary>
    ///     Creates new instance of <see cref="InputElement" /> that represents mouse input variant like a particular
    ///     mouse button or mouse axis.
    /// </summary>
    /// <param name="mouseVariant">Variant of mouse input to be represented by <see cref="InputElement" /> instance.</param>
    /// <returns><see cref="InputElement" /> representing specified mouse variant.</returns>
    public static InputElement Create(MouseVariant mouseVariant) => new(mouseVariant);

    private InputElement(Key keyboardVariant)
    {
        _keyboardVariant = keyboardVariant;
        _mouseVariant = default;
        CurrentVariant = Variant.Keyboard;
    }

    private InputElement(MouseVariant mouseVariant)
    {
        _keyboardVariant = default;
        _mouseVariant = mouseVariant;
        CurrentVariant = Variant.Mouse;
    }

    /// <summary>
    ///     Type of input source, namely a hardware input device.
    /// </summary>
    public enum Variant
    {
        /// <summary>
        ///     Keyboard input device.
        /// </summary>
        Keyboard,

        /// <summary>
        ///     Mouse input device.
        /// </summary>
        Mouse
    }

    /// <summary>
    ///     Enumerates supported variants of mouse input.
    /// </summary>
    public enum MouseVariant
    {
        /// <summary>
        ///     Left mouse button.
        /// </summary>
        LeftButton,

        /// <summary>
        ///     Middle mouse button.
        /// </summary>
        MiddleButton,

        /// <summary>
        ///     Right mouse button.
        /// </summary>
        RightButton,

        /// <summary>
        ///     First extended mouse button.
        /// </summary>
        XButton1,

        /// <summary>
        ///     Second extended mouse button.
        /// </summary>
        XButton2,

        /// <summary>
        ///     Horizontal axis of mouse movement.
        /// </summary>
        AxisX,

        /// <summary>
        ///     Vertical axis of mouse movement.
        /// </summary>
        AxisY
    }

    /// <summary>
    ///     Current variant of input source type.
    /// </summary>
    public Variant CurrentVariant { get; }

    /// <summary>
    ///     Converts this instance of <see cref="InputElement" /> to keyboard variant if possible.
    /// </summary>
    /// <returns><see cref="Key" /> of keyboard if this instance is keyboard variant; otherwise throws exception.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="CurrentVariant" /> is not
    ///     <see cref="Variant.Keyboard" />.
    /// </exception>
    public Key AsKeyboard() => CurrentVariant == Variant.Keyboard ? _keyboardVariant : throw CreateInvalidVariantException(CurrentVariant);

    /// <summary>
    ///     Converts this instance of <see cref="InputElement" /> to mouse variant if possible.
    /// </summary>
    /// <returns><see cref="MouseVariant" /> if this instance is mouse variant; otherwise throws exception.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="CurrentVariant" /> is not
    ///     <see cref="Variant.Mouse" />.
    /// </exception>
    public MouseVariant AsMouse() => CurrentVariant == Variant.Mouse ? _mouseVariant : throw CreateInvalidVariantException(CurrentVariant);

    /// <summary>
    ///     Custom <see cref="PrintMembers"/> for synthesized <see cref="ToString" />.
    /// </summary>
    private bool PrintMembers(StringBuilder builder)
    {
        switch (CurrentVariant)
        {
            case Variant.Keyboard:
                builder.Append($"{nameof(CurrentVariant)} = {CurrentVariant}, KeyboardVariant = {_keyboardVariant}");
                break;
            case Variant.Mouse:
                builder.Append($"{nameof(CurrentVariant)} = {CurrentVariant}, MouseVariant = {_mouseVariant}");
                break;
            default:
                throw new InvalidOperationException(); // TODO Convert to unreachable exception?
        }

        return true;
    }

    private static InvalidOperationException CreateInvalidVariantException(Variant variant)
    {
        return new InvalidOperationException($"Operation is not valid for current variant: {variant}.");
    }
}