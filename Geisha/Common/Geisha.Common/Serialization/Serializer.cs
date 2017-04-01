using System.Globalization;
using Newtonsoft.Json;

namespace Geisha.Common.Serialization
{
    public static class Serializer
    {
        public static string SerializeJson(object value)
        {
            return JsonConvert.SerializeObject(value, JsonSerializerSettings);
        }

        public static T DeserializeJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, JsonSerializerSettings);
        }

        private static JsonSerializerSettings JsonSerializerSettings => new JsonSerializerSettings()
        {
            Culture = CultureInfo.InvariantCulture,
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.All
        };
    }
}