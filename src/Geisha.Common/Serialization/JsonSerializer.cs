using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Geisha.Common.Serialization
{
    /// <inheritdoc />
    /// <summary>
    ///     Provides common JSON serialization functionality.
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
                TypeNameHandling = TypeNameHandling.Auto,
                ObjectCreationHandling = ObjectCreationHandling.Replace
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
            return JsonConvert.DeserializeObject<T>(json, _jsonSerializerSettings)!;
        }

        /// <inheritdoc />
        /// <summary>
        ///     Deserializes an object from part of JSON string specified by JSON path.
        /// </summary>
        [return: MaybeNull]
        public T DeserializePart<T>(string json, string partPath)
        {
            var jObject = JObject.Parse(json);
            var jToken = jObject.SelectToken(partPath);
            return jToken != null ? jToken.ToObject<T>(Newtonsoft.Json.JsonSerializer.Create(_jsonSerializerSettings)) : default;
        }
    }
}