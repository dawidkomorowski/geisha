namespace Geisha.Engine.Core.Math.Serialization
{
    // TODO Rename to RawVector2 and move to Raw namespace?
    // TODO Or Should it be handled differently?
    /// <summary>
    ///     Represents serializable <see cref="Vector2" />.
    /// </summary>
    public struct SerializableVector2
    {
        public double X { get; set; }
        public double Y { get; set; }

        public static Vector2 ToVector2(SerializableVector2 serializable) => new(serializable.X, serializable.Y);
        public static SerializableVector2 FromVector2(Vector2 vector) => new() { X = vector.X, Y = vector.Y };
    }
}