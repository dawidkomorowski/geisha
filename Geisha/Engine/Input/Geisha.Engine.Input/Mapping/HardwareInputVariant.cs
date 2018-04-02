using System;
using Geisha.Framework.Input;

namespace Geisha.Engine.Input.Mapping
{
    public struct HardwareInputVariant : IEquatable<HardwareInputVariant>
    {
        public HardwareInputVariant(Key key)
        {
            Key = key;
            CurrentVariant = Variant.Keyboard;
        }

        public enum Variant
        {
            Keyboard
        }

        public Variant CurrentVariant { get; }

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