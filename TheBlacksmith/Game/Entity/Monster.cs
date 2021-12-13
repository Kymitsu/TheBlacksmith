using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith.Game
{
    public class Monster : Entity
    {
        public Monster(string name, int lvl): base(name, lvl, 20+lvl*5, lvl)
        {
            //Name = name;
            //Hp = 20 + lvl*5;
            //Lvl = lvl;
            //CurrentHp = Hp;
            //BaseAtk = lvl;

            //Attacks.Add(SimpleAttack);
            //Attacks.Add(StrongAttack);

            //Attacks.Add(() => BasicAttack(this));
            //Attacks.Add(() => StrongAttack(this));

            Attacks.Add(BasicAttack);
            Attacks.Add(StrongAttack);
        }

        //public int SimpleAttack()
        //{
        //    return BaseAtk + StaticRandom.Next(1, 6);
        //}

        //public int StrongAttack()
        //{
        //    return 2*BaseAtk + StaticRandom.Next(3, 10);
        //}

        
    }
}
