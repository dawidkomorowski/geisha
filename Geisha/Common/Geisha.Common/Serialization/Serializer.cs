using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Geisha.Common.Serialization
{
    // TODO Shouldn't it be registered in container?
    /// <summary>
    ///     Provides common serialization functionality.
    /// </summary>
    public static class Serializer
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings;

        static Serializer()
        {
            JsonSerializerSettings = new JsonSerializerSettings
            {
                Culture = CultureInfo.InvariantCulture,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            };
            JsonSerializerSettings.Converters.Add(new StringEnumConverter());
        }

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
        public static string SerializeJson(object value)
        {
            return JsonConvert.SerializeObject(value, JsonSerializerSettings);
        }

        /// <summary>
        ///     Deserializes an object from JSON string.
        /// </summary>
        /// <typeparam name="T">Type of object to be deserialized.</typeparam>
        /// <param name="json">String with serialized object JSON content.</param>
        /// <returns>Instance of deserialized object initialized with data from JSON string.</returns>
        public static T DeserializeJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
        }
    }
}