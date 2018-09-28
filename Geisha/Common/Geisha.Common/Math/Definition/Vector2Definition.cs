namespace Geisha.Common.Math.Definition
{
    // TODO Is Definition word best describing data that can be easily serialized (transferred? DTO? or maybe SerializableSomething?)?
    /// <summary>
    ///     Represents serializable <see cref="Vector2" />.
    /// </summary>
    public sealed class Vector2Definition
    {
        public double X { get; set; }
        public double Y { get; set; }

        public static Vector2 ToVector2(Vector2Definition definition) => new Vector2(definition.X, definition.Y);
        public static Vector2Definition FromVector2(Vector2 vector) => new Vector2Definition {X = vector.X, Y = vector.Y};
    }
}