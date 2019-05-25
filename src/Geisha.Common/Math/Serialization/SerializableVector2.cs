namespace Geisha.Common.Math.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="Vector2" />.
    /// </summary>
    public sealed class SerializableVector2
    {
        public double X { get; set; }
        public double Y { get; set; }

        public static Vector2 ToVector2(SerializableVector2 serializable) => new Vector2(serializable.X, serializable.Y);
        public static SerializableVector2 FromVector2(Vector2 vector) => new SerializableVector2 {X = vector.X, Y = vector.Y};
    }
}