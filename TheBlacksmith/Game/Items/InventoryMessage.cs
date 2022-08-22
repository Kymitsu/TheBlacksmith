using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith.Game.Items
{
    public class InventoryMessage : PagedMessage<string>
    {
        public Player Player { get; set; }

        public InventoryMessage(Player p)
        {
            Player = p;
            BuildPages();
        }

        public override void BuildPages()
        {
            throw new NotImplementedException();
        }

        private List<string> EquipedItemsBlock()
        {
            List<string> equipedItemsMsg = new List<string>();



            return equipedItemsMsg;
        }
    }
}
