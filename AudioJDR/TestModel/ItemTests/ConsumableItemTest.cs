using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Items;

namespace TestModel.ItemTests
{
    public class ConsumableItemTest
    {
        [Fact]
        public void SetHealthAmount_Test()
        {
            ConsumableItem item = new ConsumableItem("", 0);
            int healAmount = 10;
            item.HealAmount = healAmount;
            Assert.Equal(healAmount, item.HealAmount);
        }

        [Fact]
        public void ConsumableItem_Constructor_Test()
        {
            string itemName = "Health Potion";
            int healAmount = 20;

            ConsumableItem item = new ConsumableItem(itemName, healAmount);

            Assert.Equal(itemName, item.Name);
            Assert.Equal(healAmount, item.HealAmount);
        }
    }
}
