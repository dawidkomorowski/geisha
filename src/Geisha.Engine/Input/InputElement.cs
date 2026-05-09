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
// TODO: Add support for dedicated MouseButton.
// TODO: Add support for dedicated MouseAxis.
// TODO: Rename Keyboard to KeyboardKey?

/// <summary>
///     Represents single element of <see cref="HardwareInput" /> like a particular keyboard key, mouse button, mouse axis.
/// </summary>
public readonly record struct InputElement
{
    private readonly Key _keyboardKey;
    private readonly MouseButton _mouseButton;
    private readonly MouseAxis _mouseAxis;
    private readonly MouseVariant _mouseVariant;

    /// <summary>
    ///     Creates new instance of <see cref="InputElement" /> that represents keyboard input variant like a
    ///     particular keyboard key.
    /// </summary>
    /// <param name="key">
    ///     Variant of keyboard input to be represented by <see cref="InputElement" />
    ///     instance.
    /// </param>
    /// <returns><see cref="InputElement" /> representing specified keyboard variant.</returns>
    public static InputElement Create(Key key) => new(key);

    public static InputElement Create(MouseButton mouseButton) => new(mouseButton);

    public static InputElement Create(MouseAxis mouseAxis) => new(mouseAxis);

    /// <summary>
    ///     Creates new instance of <see cref="InputElement" /> that represents mouse input variant like a particular
    ///     mouse button or mouse axis.
    /// </summary>
    /// <param name="mouseVariant">Variant of mouse input to be represented by <see cref="InputElement" /> instance.</param>
    /// <returns><see cref="InputElement" /> representing specified mouse variant.</returns>
    public static InputElement Create(MouseVariant mouseVariant) => new(mouseVariant);

    private InputElement(Key keyboardKey)
    {
        _keyboardKey = keyboardKey;
        _mouseButton = default;
        _mouseAxis = default;
        _mouseVariant = default;
        Type = InputType.Keyboard;
    }

    private InputElement(MouseButton mouseButton)
    {
        _keyboardKey = default;
        _mouseButton = mouseButton;
        _mouseAxis = default;
        _mouseVariant = default;
        Type = InputType.MouseButton;
    }

    private InputElement(MouseAxis mouseAxis)
    {
        _keyboardKey = default;
        _mouseButton = default;
        _mouseAxis = mouseAxis;
        _mouseVariant = default;
        Type = InputType.MouseAxis;
    }

    private InputElement(MouseVariant mouseVariant)
    {
        _keyboardKey = default;
        _mouseButton = default;
        _mouseAxis = default;
        _mouseVariant = mouseVariant;
        Type = InputType.Mouse;
    }

    /// <summary>
    ///     Type of input source, namely a hardware input device.
    /// </summary>
    public enum InputType
    {
        /// <summary>
        ///     Keyboard input device.
        /// </summary>
        Keyboard,

        /// <summary>
        ///     Mouse input device.
        /// </summary>
        Mouse,
        MouseButton,
        MouseAxis
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
    public InputType Type { get; }

    /// <summary>
    ///     Converts this instance of <see cref="InputElement" /> to keyboard variant if possible.
    /// </summary>
    /// <returns><see cref="Key" /> of keyboard if this instance is keyboard variant; otherwise throws exception.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="Type" /> is not
    ///     <see cref="InputType.Keyboard" />.
    /// </exception>
    public Key AsKeyboard() => Type is InputType.Keyboard ? _keyboardKey : throw CreateInvalidInputTypeException(Type);

    public MouseButton AsMouseButton() => Type is InputType.MouseButton ? _mouseButton : throw CreateInvalidInputTypeException(Type);

    public MouseAxis AsMouseAxis() => Type is InputType.MouseAxis ? _mouseAxis : throw CreateInvalidInputTypeException(Type);

    /// <summary>
    ///     Converts this instance of <see cref="InputElement" /> to mouse variant if possible.
    /// </summary>
    /// <returns><see cref="MouseVariant" /> if this instance is mouse variant; otherwise throws exception.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="Type" /> is not
    ///     <see cref="InputType.Mouse" />.
    /// </exception>
    public MouseVariant AsMouse() => Type == InputType.Mouse ? _mouseVariant : throw CreateInvalidInputTypeException(Type);

    /// <summary>
    ///     Custom <see cref="PrintMembers"/> for synthesized <see cref="ToString" />.
    /// </summary>
    private bool PrintMembers(StringBuilder builder)
    {
        switch (Type)
        {
            case InputType.Keyboard:
                builder.Append($"{nameof(Type)} = {Type}, KeyboardKey = {_keyboardKey}");
                break;
            case InputType.MouseButton:
                builder.Append($"{nameof(Type)} = {Type}, MouseButton = {_mouseButton}");
                break;
            case InputType.MouseAxis:
                builder.Append($"{nameof(Type)} = {Type}, MouseAxis = {_mouseAxis}");
                break;
            case InputType.Mouse:
                builder.Append($"{nameof(Type)} = {Type}, MouseVariant = {_mouseVariant}");
                break;
            default:
                throw new InvalidOperationException(); // TODO Convert to unreachable exception?
        }

        return true;
    }

    private static InvalidOperationException CreateInvalidInputTypeException(InputType inputType)
    {
        return new InvalidOperationException($"Operation is not valid for input type: {inputType}.");
    }
}