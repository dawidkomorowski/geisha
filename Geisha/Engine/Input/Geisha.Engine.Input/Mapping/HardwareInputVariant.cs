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
        ///     Converts the value of the current <see cref="HardwareInputVariant" /> object to its equivalent string
        ///     representation.
        /// </summary>
        /// <returns>A string representation of the value of the current <see cref="HardwareInputVariant" /> object.</returns>
        public override string ToString()
        {
            return $"{nameof(CurrentVariant)}: {CurrentVariant}, {nameof(Key)}: {Key}";
        }

        /// <summary>
        ///     Returns a value indicating whether the value of this instance is equal to the value of the specified
        ///     <see cref="HardwareInputVariant" /> instance.
        /// </summary>
        /// <param name="other">The object to compare to this instance.</param>
        /// <returns>
        ///     <c>true</c> if the <paramref name="other" /> parameter equals the value of this instance; otherwise,
        ///     <c>false</c>.
        /// </returns>
        public bool Equals(HardwareInputVariant other)
        {
            return CurrentVariant == other.CurrentVariant && Key == other.Key;
        }

        /// <summary>
        ///     Returns a value indicating whether this instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="obj" /> is an instance of <see cref="HardwareInputVariant" /> and equals the value
        ///     of this
        ///     instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is HardwareInputVariant variant && Equals(variant);
        }

        /// <summary>
        ///     Returns the hash code for this instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) CurrentVariant * 397) ^ (int) Key;
            }
        }

        /// <summary>
        ///     Determines whether two specified instances of <see cref="HardwareInputVariant" /> are equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> represent the same
        ///     <see cref="HardwareInputVariant" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator ==(HardwareInputVariant left, HardwareInputVariant right)
        {
            return left.Equals(right);
        }

        /// <summary>
        ///     Determines whether two specified instances of <see cref="HardwareInputVariant" /> are not equal.
        /// </summary>
        /// <param name="left">The first object to compare.</param>
        /// <param name="right">The second object to compare.</param>
        /// <returns>
        ///     <c>true</c> if <paramref name="left" /> and <paramref name="right" /> do not represent the same
        ///     <see cref="HardwareInputVariant" />; otherwise, <c>false</c>.
        /// </returns>
        public static bool operator !=(HardwareInputVariant left, HardwareInputVariant right)
        {
            return !left.Equals(right);
        }
    }
}