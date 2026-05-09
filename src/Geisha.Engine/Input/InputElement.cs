using System;
using System.Text;

namespace Geisha.Engine.Input;

// TODO: Review implementation of InputMappingAssetContent.
// TODO: Consider extending/refatoring integration test: GetAsset_ShouldLoadAndReturn_InputMapping
// TODO: Review implementation of InputMappingAssetLoader.
// TODO: Find legacy name usages (HardwareInputVariant, variant) and update to use new name.

/// <summary>
///     Represents a hardware input control such as a keyboard key, mouse button, or mouse axis.
///     Use <see cref="Kind" /> to determine the actual kind of input and the corresponding <c>As</c> method to retrieve
///     the underlying value.
/// </summary>
public readonly record struct InputElement
{
    private readonly Key _keyboardKey;
    private readonly MouseButton _mouseButton;
    private readonly MouseAxis _mouseAxis;

    /// <summary>
    ///     Creates a new instance of <see cref="InputElement" /> representing the specified keyboard key.
    /// </summary>
    /// <param name="key">Keyboard key to be represented by the <see cref="InputElement" /> instance.</param>
    /// <returns><see cref="InputElement" /> representing the specified keyboard key.</returns>
    public static InputElement Create(Key key) => new(key);

    /// <summary>
    ///     Creates a new instance of <see cref="InputElement" /> representing the specified mouse button.
    /// </summary>
    /// <param name="mouseButton">Mouse button to be represented by the <see cref="InputElement" /> instance.</param>
    /// <returns><see cref="InputElement" /> representing the specified mouse button.</returns>
    public static InputElement Create(MouseButton mouseButton) => new(mouseButton);

    /// <summary>
    ///     Creates a new instance of <see cref="InputElement" /> representing the specified mouse axis.
    /// </summary>
    /// <param name="mouseAxis">Mouse axis to be represented by the <see cref="InputElement" /> instance.</param>
    /// <returns><see cref="InputElement" /> representing the specified mouse axis.</returns>
    public static InputElement Create(MouseAxis mouseAxis) => new(mouseAxis);

    private InputElement(Key keyboardKey)
    {
        _keyboardKey = keyboardKey;
        _mouseButton = default;
        _mouseAxis = default;
        Kind = InputKind.KeyboardKey;
    }

    private InputElement(MouseButton mouseButton)
    {
        _keyboardKey = default;
        _mouseButton = mouseButton;
        _mouseAxis = default;
        Kind = InputKind.MouseButton;
    }

    private InputElement(MouseAxis mouseAxis)
    {
        _keyboardKey = default;
        _mouseButton = default;
        _mouseAxis = mouseAxis;
        Kind = InputKind.MouseAxis;
    }

    /// <summary>
    ///     Specifies the kind of hardware input represented by an <see cref="InputElement" />.
    /// </summary>
    public enum InputKind
    {
        /// <summary>
        ///     A keyboard key.
        /// </summary>
        KeyboardKey,

        /// <summary>
        ///     A mouse button.
        /// </summary>
        MouseButton,

        /// <summary>
        ///     A mouse axis.
        /// </summary>
        MouseAxis
    }

    /// <summary>
    ///     The kind of hardware input represented by this <see cref="InputElement" />.
    /// </summary>
    public InputKind Kind { get; }

    /// <summary>
    ///     Returns the <see cref="Key" /> represented by this <see cref="InputElement" />.
    /// </summary>
    /// <returns><see cref="Key" /> represented by this instance.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="Kind" /> is not <see cref="InputKind.KeyboardKey" />.
    /// </exception>
    public Key AsKeyboardKey() => Kind is InputKind.KeyboardKey ? _keyboardKey : throw CreateInvalidInputKindException(Kind);

    /// <summary>
    ///     Returns the <see cref="MouseButton" /> represented by this <see cref="InputElement" />.
    /// </summary>
    /// <returns><see cref="MouseButton" /> represented by this instance.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="Kind" /> is not <see cref="InputKind.MouseButton" />.
    /// </exception>
    public MouseButton AsMouseButton() => Kind is InputKind.MouseButton ? _mouseButton : throw CreateInvalidInputKindException(Kind);

    /// <summary>
    ///     Returns the <see cref="MouseAxis" /> represented by this <see cref="InputElement" />.
    /// </summary>
    /// <returns><see cref="MouseAxis" /> represented by this instance.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="Kind" /> is not <see cref="InputKind.MouseAxis" />.
    /// </exception>
    public MouseAxis AsMouseAxis() => Kind is InputKind.MouseAxis ? _mouseAxis : throw CreateInvalidInputKindException(Kind);

    /// <summary>
    ///     Custom <see cref="PrintMembers"/> for synthesized <see cref="ToString" />.
    /// </summary>
    private bool PrintMembers(StringBuilder builder)
    {
        switch (Kind)
        {
            case InputKind.KeyboardKey:
                builder.Append($"{nameof(Kind)} = {Kind}, Key = {_keyboardKey}");
                break;
            case InputKind.MouseButton:
                builder.Append($"{nameof(Kind)} = {Kind}, Button = {_mouseButton}");
                break;
            case InputKind.MouseAxis:
                builder.Append($"{nameof(Kind)} = {Kind}, Axis = {_mouseAxis}");
                break;
            default:
                throw new InvalidOperationException(); // TODO Convert to unreachable exception?
        }

        return true;
    }

    private static InvalidOperationException CreateInvalidInputKindException(InputKind inputKind)
    {
        return new InvalidOperationException($"Operation is not valid for input kind: {inputKind}.");
    }
}