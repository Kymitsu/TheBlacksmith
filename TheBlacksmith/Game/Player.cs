using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith.Game
{
    public class Player : Entity
    {
        public string Mention { get; set; }

        public int Money { get; set; }
        public int TotalExp { get; private set; }

        public Player(string mention, string name, int lvl, int hp, int currentHp, int atk, int totalExp) : base(name, lvl, hp, atk)
        {
            Mention = mention;
            CurrentHp = currentHp;
            Money = 0;
            TotalExp = totalExp;
        }


        public bool AddExp(int xp)
        {
            TotalExp += xp;
            if(TotalExp % (50*Lvl) >= 1)
            {
                //lvl up
                LvlUp();
                return true;
            }

            return false;
        }

        private void LvlUp()
        {
            Lvl++;

        }

        public static Player CreateNewPlayer(string mention, string name)
        {
            Player p = new Player(mention, name, 1, 100, 100, 5, 0);

            return p;
        }
    }
}
