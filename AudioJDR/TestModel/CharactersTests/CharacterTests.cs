using Model.Characters;
using Model.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Model.Resources.Localization;

namespace TestModel.CharactersTests
{
    public class CharacterTests
    {
        [Fact]
        public void SetName_Test()
        {
            FakeCharacter character = new FakeCharacter("", 0, 0);
            string enemyName = "Enemy";
            character.Name = enemyName;
            Assert.Equal(enemyName, character.Name);
        }

        [Fact]
        public void SetHealth_Test()
        {
            FakeCharacter character = new FakeCharacter("", 0, 0);
            int health = 100;
            character.Health = health;
            Assert.Equal(health, character.Health);
        }

        [Fact]
        public void SetBaseDamage_Test()
        {
            FakeCharacter character = new FakeCharacter("", 0, 0);
            int damage = 99;
            character.BaseDamage = damage;
            Assert.Equal(damage, character.BaseDamage);
        }

        [Fact]
        public void SetEquippedWeapon_Test()
        {
            FakeCharacter character = new FakeCharacter("", 0, 0);
            WeaponItem weapon = new WeaponItem("weapon", 10);
            character.EquippedWeapon = weapon;
            Assert.Equal(weapon, character.EquippedWeapon);
        }

        [Fact]
        public void Constructor_Test()
        {
            string name = "NameCharacter";
            int health = 100;
            int baseDamage = 50;
            FakeCharacter character = new FakeCharacter(name, health, baseDamage);

            Assert.Equal(name, character.Name);
            Assert.Equal(health, character.Health);
            Assert.Equal(baseDamage, character.BaseDamage);
            Assert.NotNull(character.GetInventory());
        }

        [Fact]
        public void TakeDamage_Test()
        {
            FakeCharacter character = new FakeCharacter("", 0, 0);
            int health = 100;
            character.Health = health;
            int damage = 10;
            character.TakeDamage(damage);
            Assert.Equal(health - damage, character.Health);
        }

        [Fact]
        public void TakeDamage_TestCharacterDead()
        {
            FakeCharacter character = new FakeCharacter("", 0, 0);
            int health = 100;
            character.Health = health;
            int damage = 110;
            character.TakeDamage(damage);
            Assert.Equal(0, character.Health);
        }

        [Fact]
        public void Attack_TestNoWeapon()
        {
            int baseDamage = 50;
            FakeCharacter attackingCharacter = new FakeCharacter("Attacking Character", 0, baseDamage);
            int health = 100;
            FakeCharacter characterToAttack = new FakeCharacter("Character To Attack", health, baseDamage);

            attackingCharacter.Attack(characterToAttack);
            Assert.Equal(health - 50, characterToAttack.Health);
        }

        [Fact]
        public void Attack_TestWithWeapon()
        {
            int baseDamage = 50;
            FakeCharacter attackingCharacter = new FakeCharacter("Attacking Character", 0, baseDamage);
            int health = 100;
            FakeCharacter characterToAttack = new FakeCharacter("Character To Attack", health, baseDamage);
            int weaponDamage = 99;
            WeaponItem weapon = new WeaponItem("weapon", weaponDamage);
            attackingCharacter.EquippedWeapon = weapon;

            attackingCharacter.Attack(characterToAttack);
            Assert.Equal(health - weaponDamage, characterToAttack.Health);
        }

        [Fact]
        public void GetInventory_Test()
        {
            FakeCharacter character = new FakeCharacter("", 0, 0);
            ConsumableItem item = new ConsumableItem("", 100);
            character.AddToInventory(item);

            List<Item> items = character.GetInventory();
            items.Add(item);

            Assert.Contains(item, items);
            Assert.Single(character.GetInventory());
        }

        [Fact]
        public void AddToInventory_Test()
        {
            FakeCharacter character = new FakeCharacter("", 0, 0);
            ConsumableItem item = new ConsumableItem("", 100);

            character.AddToInventory(item);
            Assert.Single(character.GetInventory());
            Assert.Contains(item, character.GetInventory());
        }

        [Fact]
        public void DropItem_Test()
        {
            FakeCharacter character = new FakeCharacter("", 0, 0);
            ConsumableItem item = new ConsumableItem("", 100);

            character.AddToInventory(item);
            character.DropItem(item);

            Assert.Empty(character.GetInventory());
        }

        [Fact]
        public void UseItem_Test()
        {
            int healthCharacter = 100;
            FakeCharacter character = new FakeCharacter("", healthCharacter, 0);

            int healthPotion = 20;
            ConsumableItem item = new ConsumableItem("Potion", healthPotion);

            character.AddToInventory(item);
            character.UseItem(item);

            Assert.Equal(healthCharacter + healthPotion, character.Health);
            Assert.DoesNotContain(item, character.GetInventory());
        }

        [Fact]
        public void UseItem_TestItemNotInInventory()
        {
            int health = 100;
            FakeCharacter character = new FakeCharacter("", health, 0);
            ConsumableItem item = new ConsumableItem("Potion", 20);

            character.UseItem(item);

            Assert.Equal(health, character.Health);
        }
    }
}
