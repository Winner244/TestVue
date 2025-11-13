using System.Text.Json;

namespace TestVue.Server.Helper
{
    public static class JsonHelper
    {
        public static Dictionary<string, object> ConvertJsonElementToDictionary(JsonElement element)
        {
            var dataDictionary = new Dictionary<string, object>();
            foreach (var property in element.EnumerateObject())
            {
                dataDictionary[property.Name] = JsonHelper.ConvertJsonElement(property.Value);
            }
            return dataDictionary;
        }

        private static object ConvertJsonElement(JsonElement element)
        {
            return element.ValueKind switch
            {
                JsonValueKind.String => element.GetString() ?? "",
                JsonValueKind.Number => ConvertNumber(element), 
                JsonValueKind.True => true,
                JsonValueKind.False => false,
                JsonValueKind.Array => element.EnumerateArray().Select(ConvertJsonElement).ToList(),
                JsonValueKind.Object => element.EnumerateObject()
                    .ToDictionary(p => p.Name, p => ConvertJsonElement(p.Value)),
                _ => element.GetRawText()
            };
        }

        private static object ConvertNumber(JsonElement element)
        {
            if (element.ValueKind == JsonValueKind.Number)
            {
                if (element.TryGetInt32(out var intValue))
                {
                    return intValue;
                }
                else if (element.TryGetInt64(out var longValue))
                {
                    return longValue;
                }
                else if (element.TryGetDouble(out var doubleValue))
                {
                    return doubleValue;
                }
            }
            throw new InvalidOperationException("The provided JsonElement is not a number.");
        }
    }
}
