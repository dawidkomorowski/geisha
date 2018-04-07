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

        public override string ToString()
        {
            return $"{nameof(CurrentVariant)}: {CurrentVariant}, {nameof(Key)}: {Key}";
        }

        public bool Equals(HardwareInputVariant other)
        {
            return CurrentVariant == other.CurrentVariant && Key == other.Key;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is HardwareInputVariant variant && Equals(variant);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) CurrentVariant * 397) ^ (int) Key;
            }
        }

        public static bool operator ==(HardwareInputVariant left, HardwareInputVariant right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(HardwareInputVariant left, HardwareInputVariant right)
        {
            return !left.Equals(right);
        }
    }
}