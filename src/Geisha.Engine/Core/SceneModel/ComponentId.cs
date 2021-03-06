using System;

namespace Geisha.Engine.Core.SceneModel
{
    public readonly struct ComponentId : IEquatable<ComponentId>
    {
        public ComponentId(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public override string ToString() => $"{nameof(Value)}: {Value}";

        public bool Equals(ComponentId other) => Value == other.Value;
        public override bool Equals(object? obj) => obj is ComponentId other && Equals(other);
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(ComponentId left, ComponentId right) => left.Equals(right);
        public static bool operator !=(ComponentId left, ComponentId right) => !left.Equals(right);
    }
}