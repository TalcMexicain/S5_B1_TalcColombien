using System;
using System.Text.Json;
using Model.Items;
using Model.Storage;
using Xunit;

namespace TestModel.Storage
{
    public class ItemConverterTests
    {
        private readonly JsonSerializerOptions _options;

        public ItemConverterTests()
        {
            _options = new JsonSerializerOptions
            {
                Converters = { new ItemConverter() },
                WriteIndented = false
            };
        }

        [Fact]
        public void Serialize_WeaponItem_Test()
        {
            WeaponItem weapon = new WeaponItem
            {
                IdItem = 1,
                Name = "Sword",
                Damage = 50
            };

            string json = JsonSerializer.Serialize<Item>(weapon, _options);

            Assert.Contains("\"Type\":\"WeaponItem\"", json);
            Assert.Contains("\"Damage\":50", json);
        }

        [Fact]
        public void Deserialize_WeaponItem_Test()
        {
            string json = "{\"Type\":\"WeaponItem\",\"IdItem\":1,\"Name\":\"Sword\",\"Damage\":50}";

            Item item = JsonSerializer.Deserialize<Item>(json, _options);

            Assert.IsType<WeaponItem>(item);
            WeaponItem weapon = (WeaponItem)item;
            Assert.NotNull(weapon);
            Assert.Equal(1, weapon.IdItem);
            Assert.Equal("Sword", weapon.Name);
            Assert.Equal(50, weapon.Damage);
        }

        [Fact]
        public void Deserialize_ConsumableItem_Test()
        {
            string json = "{\"Type\":\"ConsumableItem\",\"IdItem\":2,\"Name\":\"Health Potion\",\"Effect\":\"Heal\"}";

            Item item = JsonSerializer.Deserialize<Item>(json, _options);

            Assert.IsType<ConsumableItem>(item);
            ConsumableItem consumable = (ConsumableItem)item;
            Assert.NotNull(consumable);
            Assert.Equal(2, consumable.IdItem);
            Assert.Equal("Health Potion", consumable.Name);
        }

        [Fact]
        public void Deserialize_UnknownType_Test()
        {
            string json = "{\"Type\":\"UnknownItem\",\"IdItem\":1,\"Name\":\"Mystery Item\"}";

            Exception exception = Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Item>(json, _options));
            Assert.Equal("Unknown type: UnknownItem", exception.Message);
        }

        [Fact]
        public void Deserialize_MissingType_Test()
        {
            string json = "{\"IdItem\":1,\"Name\":\"Missing Type\"}";

            Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<Item>(json, _options));
        }
    }
}
