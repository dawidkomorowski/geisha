namespace Geisha.Common.Serialization
{
    /// <summary>
    ///     Provides common JSON serialization functionality.
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

        /// <summary>
        ///     Deserializes an object from part of JSON string specified by JSON path.
        /// </summary>
        /// <typeparam name="T">Type of object to be deserialized.</typeparam>
        /// <param name="json">String with JSON content.</param>
        /// <param name="partPath">JSON path to part of JSON that will be deserialized to an object.</param>
        /// <returns>Instance of deserialized object initialized with data from part of JSON string if part under the path exists; otherwise default value of <typeparamref name="T"/>.</returns>
        T DeserializePart<T>(string json, string partPath);
    }
}