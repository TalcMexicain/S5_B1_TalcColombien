using Model.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Model.Storage
{
    /// <summary>
    /// Permits serialization of the classes derived from Item
    /// </summary>
    public class ItemConverter : JsonConverter<Item>
    {
        public override Item Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (var doc = JsonDocument.ParseValue(ref reader))
            {
                var root = doc.RootElement;

                if (root.TryGetProperty("Type", out var typeProperty))
                {
                    var type = typeProperty.GetString();
                    return type switch
                    {
                        nameof(WeaponItem) => JsonSerializer.Deserialize<WeaponItem>(root.GetRawText(), options),
                        nameof(ConsumableItem) => JsonSerializer.Deserialize<ConsumableItem>(root.GetRawText(), options),
                        nameof(KeyItem) => JsonSerializer.Deserialize<KeyItem>(root.GetRawText(), options),
                        _ => throw new JsonException($"Unknown type: {type}")
                    };
                }

                throw new JsonException("Missing 'Type' property for polymorphic deserialization.");
            }
        }

        public override void Write(Utf8JsonWriter writer, Item value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, value.GetType(), options);
        }
    }
}
