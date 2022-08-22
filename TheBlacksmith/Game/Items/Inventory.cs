using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith.Game
{
    public class Inventory
    {
        public Player Player { get; set; }

        public Dictionary<ItemType, List<ItemStack>> Items { get; set; }

        
        public void AddItem(Item item)
        {
            throw new NotImplementedException();
        }

        public void AddItems(List<Item> items)
        {
            throw new NotImplementedException();
        }

        public void RemoveItem(Item item)
        {
            throw new NotImplementedException();
        }
    }
}
