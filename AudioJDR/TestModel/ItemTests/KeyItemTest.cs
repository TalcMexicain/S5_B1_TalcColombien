using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.Items;

namespace TestModel.ItemTests
{
    public class KeyItemTest
    {
        [Fact]
        public void KeyItem_Constructor_Test()
        {
            string keyName = "Dungeon Key";

            KeyItem key = new KeyItem(keyName);

            Assert.Equal(keyName, key.Name);
        }
    }
}
