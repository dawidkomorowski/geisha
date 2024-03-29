using System;

namespace Geisha.Engine.Input.Mapping
{
    /// <summary>
    ///     Represents single element of <see cref="HardwareInput" /> like a particular keyboard key, mouse button, mouse axis.
    /// </summary>
    public readonly struct HardwareInputVariant : IEquatable<HardwareInputVariant>
    {
        private readonly Key _keyboardVariant;
        private readonly MouseVariant _mouseVariant;

        /// <summary>
        ///     Creates new instance of <see cref="HardwareInputVariant" /> that represents keyboard input variant like a
        ///     particular keyboard key.
        /// </summary>
        /// <param name="keyboardVariant">
        ///     Variant of keyboard input to be represented by <see cref="HardwareInputVariant" />
        ///     instance.
        /// </param>
        /// <returns><see cref="HardwareInputVariant" /> representing specified keyboard variant.</returns>
        public static HardwareInputVariant CreateKeyboardVariant(Key keyboardVariant) => new HardwareInputVariant(keyboardVariant);

        /// <summary>
        ///     Creates new instance of <see cref="HardwareInputVariant" /> that represents mouse input variant like a particular
        ///     mouse button or mouse axis.
        /// </summary>
        /// <param name="mouseVariant">Variant of mouse input to be represented by <see cref="HardwareInputVariant" /> instance.</param>
        /// <returns><see cref="HardwareInputVariant" /> representing specified mouse variant.</returns>
        public static HardwareInputVariant CreateMouseVariant(MouseVariant mouseVariant) => new(mouseVariant);

        private HardwareInputVariant(Key keyboardVariant)
        {
            _keyboardVariant = keyboardVariant;
            _mouseVariant = default;
            CurrentVariant = Variant.Keyboard;
        }

        private HardwareInputVariant(MouseVariant mouseVariant)
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
        ///     Converts this instance of <see cref="HardwareInputVariant" /> to keyboard variant if possible.
        /// </summary>
        /// <returns><see cref="Key" /> of keyboard if this instance is keyboard variant; otherwise throws exception.</returns>
        /// <exception cref="InvalidOperationForCurrentVariantException">
        ///     Thrown when <see cref="CurrentVariant" /> is not
        ///     <see cref="Variant.Keyboard" />.
        /// </exception>
        public Key AsKeyboard() => CurrentVariant == Variant.Keyboard ? _keyboardVariant : throw new InvalidOperationForCurrentVariantException(CurrentVariant);

        /// <summary>
        ///     Converts this instance of <see cref="HardwareInputVariant" /> to mouse variant if possible.
        /// </summary>
        /// <returns><see cref="MouseVariant" /> if this instance is mouse variant; otherwise throws exception.</returns>
        /// <exception cref="InvalidOperationForCurrentVariantException">
        ///     Thrown when <see cref="CurrentVariant" /> is not
        ///     <see cref="Variant.Mouse" />.
        /// </exception>
        public MouseVariant AsMouse() => CurrentVariant == Variant.Mouse ? _mouseVariant : throw new InvalidOperationForCurrentVariantException(CurrentVariant);

        /// <summary>
        ///     Converts the value of the current <see cref="HardwareInputVariant" /> object to its equivalent string
        ///     representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="HardwareInputVariant" /> object.</returns>
        public override string ToString()
        {
            return CurrentVariant switch
            {
                Variant.Keyboard => $"{nameof(CurrentVariant)}: {CurrentVariant}, KeyboardVariant: {_keyboardVariant}",
                Variant.Mouse => $"{nameof(CurrentVariant)}: {CurrentVariant}, MouseVariant: {_mouseVariant}",
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <inheritdoc />
        public bool Equals(HardwareInputVariant other) =>
            _keyboardVariant == other._keyboardVariant && _mouseVariant == other._mouseVariant && CurrentVariant == other.CurrentVariant;

        /// <inheritdoc />
        public override bool Equals(object? obj) => obj is HardwareInputVariant other && Equals(other);

        /// <inheritdoc />
        public override int GetHashCode() => HashCode.Combine((int)_keyboardVariant, (int)_mouseVariant, (int)CurrentVariant);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="HardwareInputVariant" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="HardwareInputVariant" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(HardwareInputVariant left, HardwareInputVariant right) => left.Equals(right);

        /// <summary>
        ///     Determines whether two specified instances of <see cref="HardwareInputVariant" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="HardwareInputVariant" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(HardwareInputVariant left, HardwareInputVariant right) => !left.Equals(right);
    }

    /// <summary>
    ///     The exception that is thrown when converting <see cref="HardwareInputVariant" /> to specific input device variant
    ///     that this instance of <see cref="HardwareInputVariant" /> does not represent.
    /// </summary>
    public sealed class InvalidOperationForCurrentVariantException : Exception
    {
        public InvalidOperationForCurrentVariantException(HardwareInputVariant.Variant variant) : base(
            $"Operation is not valid for current variant: {variant}.")
        {
            Variant = variant;
        }

        /// <summary>
        ///     Current variant of <see cref="HardwareInputVariant" />.
        /// </summary>
        public HardwareInputVariant.Variant Variant { get; }
    }
}