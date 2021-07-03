using System;

namespace Geisha.Engine.Core.Assets
{
    public readonly struct AssetType : IEquatable<AssetType>
    {
        private readonly string? _value;

        public AssetType(string value)
        {
            _value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Value => _value ?? string.Empty;

        public override string ToString() => Value;

        public bool Equals(AssetType other) => Value == other.Value;

        public override bool Equals(object? obj) => obj is AssetType other && Equals(other);

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(AssetType left, AssetType right) => left.Equals(right);

        public static bool operator !=(AssetType left, AssetType right) => !left.Equals(right);
    }
}