namespace Geisha.Common.Serialization
{
    /// <summary>
    ///     Provides common serialization functionality.
    /// </summary>
    public interface IJsonSerializer
    {
        /// <summary>
        ///     Serializes an object to JSON string.
        /// </summary>
        /// <param name="value">Object to be serialized.</param>
        /// <returns>String with serialized object JSON content.</returns>
        /// <remarks>
        ///     JSON serialization handles objects types names automatically. No type information is serialized as long as
        ///     object type is the same as its declared type. If those differs then type information is included in serialized
        ///     data.
        /// </remarks>
        string Serialize(object value);

        /// <summary>
        ///     Deserializes an object from JSON string.
        /// </summary>
        /// <typeparam name="T">Type of object to be deserialized.</typeparam>
        /// <param name="json">String with serialized object JSON content.</param>
        /// <returns>Instance of deserialized object initialized with data from JSON string.</returns>
        T Deserialize<T>(string json);
    }
}