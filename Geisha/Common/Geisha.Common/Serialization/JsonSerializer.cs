using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Geisha.Common.Serialization
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides common serialization functionality.
    /// </summary>
    public class JsonSerializer : IJsonSerializer
    {
        private readonly JsonSerializerSettings _jsonSerializerSettings;

        public JsonSerializer()
        {
            _jsonSerializerSettings = new JsonSerializerSettings
            {
                Culture = CultureInfo.InvariantCulture,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.Auto
            };
            _jsonSerializerSettings.Converters.Add(new StringEnumConverter());
        }

        /// <inheritdoc />
        /// <summary>
        ///     Serializes an object to JSON string.
        /// </summary>
        public string Serialize(object value)
        {
            return JsonConvert.SerializeObject(value, _jsonSerializerSettings);
        }

        /// <inheritdoc />
        /// <summary>
        ///     Deserializes an object from JSON string.
        /// </summary>
        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings);
        }
    }
}