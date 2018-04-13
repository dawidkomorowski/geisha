using System;
using Geisha.Framework.Input;

namespace Geisha.Engine.Input.Mapping
{
    /// <summary>
    ///     Represents single element of <see cref="HardwareInput" /> like a particular keyboard key.
    /// </summary>
    public struct HardwareInputVariant : IEquatable<HardwareInputVariant>
    {
        /// <summary>
        ///     Creates new instance of <see cref="HardwareInputVariant" /> that is of <see cref="Variant.Keyboard" /> variant and
        ///     of given keyboard key.
        /// </summary>
        /// <param name="key">Keyboard key.</param>
        public HardwareInputVariant(Key key)
        {
            Key = key;
            CurrentVariant = Variant.Keyboard;
        }

        /// <summary>
        ///     Type of input source, namely a hardware input device.
        /// </summary>
        public enum Variant
        {
            /// <summary>
            ///     Keyboard input device.
            /// </summary>
            Keyboard
        }

        /// <summary>
        ///     Current variant of input source type.
        /// </summary>
        public Variant CurrentVariant { get; }

        /// <summary>
        ///     Keyboard key of this variant. Meaningful only for <see cref="Variant.Keyboard" /> variant.
        /// </summary>
        public Key Key { get; }

        /// <summary>
        ///     Returns textual representation of <see cref="HardwareInputVariant" />.
        /// </summary>
        /// <returns>String containing information about <see cref="CurrentVariant" /> and <see cref="Key" />.</returns>
        public override string ToString()
        {
            return $"{nameof(CurrentVariant)}: {CurrentVariant}, {nameof(Key)}: {Key}";
        }

        /// <inheritdoc />
        public bool Equals(HardwareInputVariant other)
        {
            return CurrentVariant == other.CurrentVariant && Key == other.Key;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is HardwareInputVariant variant && Equals(variant);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) CurrentVariant * 397) ^ (int) Key;
            }
        }

        /// <summary>
        ///     Tests equality of two <see cref="HardwareInputVariant" /> instances.
        /// </summary>
        /// <param name="left">First instance of <see cref="HardwareInputVariant" />.</param>
        /// <param name="right">Second instance of <see cref="HardwareInputVariant" />.</param>
        /// <returns>True, if both instances are equal; false otherwise.</returns>
        public static bool operator ==(HardwareInputVariant left, HardwareInputVariant right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Tests inequality of two <see cref="HardwareInputVariant" /> instances.
        /// </summary>
        /// <param name="left">First instance of <see cref="HardwareInputVariant" />.</param>
        /// <param name="right">Second instance of <see cref="HardwareInputVariant" />.</param>
        /// <returns>True, if both instances are not equal; false otherwise.</returns>
        public static bool operator !=(HardwareInputVariant left, HardwareInputVariant right)
        {
            return !left.Equals(right);
        }
    }
}