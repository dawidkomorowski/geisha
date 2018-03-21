namespace Geisha.Common.Math.Definition
{
    /// <summary>
    ///     Represents serializable <see cref="Vector3" />.
    /// </summary>
    public class Vector3Definition
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public static Vector3 ToVector3(Vector3Definition definition) => new Vector3(definition.X, definition.Y, definition.Z);
        public static Vector3Definition FromVector3(Vector3 vector) => new Vector3Definition {X = vector.X, Y = vector.Y, Z = vector.Z};
    }
}