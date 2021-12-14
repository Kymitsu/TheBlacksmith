using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith.Game
{
    public class Player : Entity
    {
        public event Action OnLevelUp = delegate { };

        public string Mention { get; set; }
        public ulong ChannelID { get; set; }
        public StatusEmbed Status { get; set; }

        public int Money { get; set; }
        public int TotalExp { get; private set; }

        public int CurrentExp { get; private set; }
        public int ExpToLvl { get; private set; }

        public Player(string mention, string name, int lvl, int hp, int currentHp, int atk) : base(name, lvl, hp, atk)
        {
            Mention = mention;
            ChannelID = 0;
            CurrentHp = currentHp;
            Money = 0;
            TotalExp = 0;
            CurrentExp = 0;
            ExpToLvl = 50 * Lvl;

            Attacks.Add(BasicAttack);
            Attacks.Add(StrongAttack);

            Status = new StatusEmbed(this);
        }


        public void AddExp(int xp)
        {
            TotalExp += xp;
            CurrentExp += xp;
            if(CurrentExp >= ExpToLvl)
            {
                LvlUp();
                OnLevelUp(); //Raise lvl up event
            }

            return;
        }

        private void LvlUp()
        {
            Lvl++;
            Hp += 20;
            CurrentHp = Hp;
            BaseAtk += 2;

            CurrentExp -= ExpToLvl;
            ExpToLvl = 50 * Lvl;
        }

        public static Player CreateNewPlayer(string mention, string name)
        {
            Player p = new Player(mention, name, 1, 100, 100, 5);

            return p;
        }
    }
}
