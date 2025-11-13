using System.Text.Json;
using System.Text.Json.Serialization;

namespace TestVue.Server.Configuration
{
    public static class JsonConfiguration
    {
        public static JsonSerializerOptions DefaultOptions { get; } = new JsonSerializerOptions
        {
            // Preserve original (declared) property casing
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = null,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
            Converters = { new JsonStringEnumConverter() }
        };
    }
}
