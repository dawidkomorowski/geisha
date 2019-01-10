namespace Geisha.Common.Math.Serialization
{
    /// <summary>
    ///     Represents serializable <see cref="Vector3" />.
    /// </summary>
    public sealed class SerializableVector3
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public static Vector3 ToVector3(SerializableVector3 serializable) => new Vector3(serializable.X, serializable.Y, serializable.Z);
        public static SerializableVector3 FromVector3(Vector3 vector) => new SerializableVector3 {X = vector.X, Y = vector.Y, Z = vector.Z};
    }
}