using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Geisha.Common.Serialization
{
    // TODO Add docs
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

        public static string SerializeJson(object value)
        {
            return JsonConvert.SerializeObject(value, JsonSerializerSettings);
        }

        public static T DeserializeJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
        }
    }
}