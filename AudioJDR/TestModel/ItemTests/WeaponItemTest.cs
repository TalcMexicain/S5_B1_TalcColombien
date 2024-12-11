using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Items;

namespace TestModel.ItemTests
{
    public class WeaponItemTest
    {
        [Fact]
        public void SetDamage_Test()
        {
            WeaponItem weapon = new WeaponItem("", 0);
            int damage = 100;
            weapon.Damage = damage;
            Assert.Equal(damage, weapon.Damage);
        }

        [Fact]
        public void WeaponItem_Constructor_Test()
        {
            string weaponName = "Sword of Power";
            int weaponDamage = 100;

            WeaponItem weapon = new WeaponItem(weaponName, weaponDamage);

            Assert.Equal(weaponName, weapon.Name);
            Assert.Equal(weaponDamage, weapon.Damage);
        }
    }
}
