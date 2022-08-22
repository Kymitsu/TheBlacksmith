using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith.Game
{
    public class Item
    {
        public string Name { get; set; }
        public int ILvl { get; set; }
        public ItemType Type { get; set; }

        public bool IsStackable { get; set; }
        public int StackLimit { get; set; }
    }
}
