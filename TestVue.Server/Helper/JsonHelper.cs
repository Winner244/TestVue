using System.Text.Json;

namespace TestVue.Server.Helper
{
    public static class JsonHelper
    {
        /// <summary>
        /// Converts a JsonElement to a Dictionary with proper type conversion.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when element is not an object</exception>
        public static Dictionary<string, object> ConvertJsonElementToDictionary(JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.Object)
            {
                throw new ArgumentException("JsonElement must be an object", nameof(element));
            }

            var dataDictionary = new Dictionary<string, object>();
            foreach (var property in element.EnumerateObject())
            {
                dataDictionary[property.Name] = ConvertJsonElement(property.Value);
            }
            return dataDictionary;
        }

        private static object ConvertJsonElement(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString() ?? string.Empty,
                JsonValueKind.Number => ConvertNumber(element),
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Null => null!,
                JsonValueKind.Array => element.EnumerateArray().Select(ConvertJsonElement).ToList(),
                JsonValueKind.Object => element.EnumerateObject()
                    .ToDictionary(p => p.Name, p => ConvertJsonElement(p.Value)),
                JsonValueKind.Undefined => throw new JsonException("Undefined JSON value encountered"),
                _ => element.GetRawText()
            };
        }

        private static object ConvertNumber(JsonElement element)
        {
            if (element.ValueKind != JsonValueKind.Number)
            {
                throw new InvalidOperationException("The provided JsonElement is not a number.");
            }

            // Try int first for common case
            if (element.TryGetInt32(out var intValue))
            {
                return intValue;
            }
            
            // Try long for large integers
            if (element.TryGetInt64(out var longValue))
            {
                return longValue;
            }
            
            // Try decimal for precise decimal numbers
            if (element.TryGetDecimal(out var decimalValue))
            {
                return decimalValue;
            }
            
            // Fallback to double
            if (element.TryGetDouble(out var doubleValue))
            {
                return doubleValue;
            }

            // Last resort: parse as string
            return element.GetRawText();
        }
    }
}
