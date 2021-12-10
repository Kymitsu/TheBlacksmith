using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith.Game
{
    public class Player : Entity
    {
        public event Action OnLevelUp = delegate { };

        public string Mention { get; set; }

        public int Money { get; set; }
        public int TotalExp { get; private set; }

        public Player(string mention, string name, int lvl, int hp, int currentHp, int atk, int totalExp) : base(name, lvl, hp, atk)
        {
            Mention = mention;
            CurrentHp = currentHp;
            Money = 0;
            TotalExp = totalExp;

            Attacks.Add(BasicAttack);
            Attacks.Add(StrongAttack);
        }


        public bool AddExp(int xp)
        {
            TotalExp += xp;
            if(TotalExp % (50*Lvl) == 0)
            {
                LvlUp();
                OnLevelUp(); //Raise lvl up event
                return true;
            }

            return false;
        }

        private void LvlUp()
        {
            Lvl++;
            Hp += 20;
            CurrentHp = Hp;
            BaseAtk += 2;
        }

        public static Player CreateNewPlayer(string mention, string name)
        {
            Player p = new Player(mention, name, 1, 100, 100, 5, 0);

            return p;
        }
    }
}
