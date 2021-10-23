using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith.Game
{
    public abstract class Entity
    {
        public int Hp { get; set; }
        public string Name { get; set; }
        public int Lvl { get; set; }
        public int BaseAtk { get; set; }
        public bool IsAlive { get; set; }
        public int CurrentHp
        {
            get { return CurrentHp; }
            set
            {
                CurrentHp = value;
                if (CurrentHp <= 0)
                    IsAlive = false;
            }
        }

        protected Entity(string name, int lvl, int hp, int atk)
        {
            Name = name;
            Lvl = lvl;
            Hp = hp;
            CurrentHp = hp;
            BaseAtk = atk;
            IsAlive = true;
        }

        public List<Func<Entity, Entity, int>> Attacks { get; set; } = new List<Func<Entity, Entity, int>>();

        public static int BasicAttack(Entity entity, Entity target)
        {
            int damage = entity.BaseAtk + StaticRandom.Next(1, 6);
            target.CurrentHp -= damage;
            return damage;
        }

        public static int StrongAttack(Entity entity, Entity target)
        {
            int damage = entity.BaseAtk * 2 + StaticRandom.Next(3, 10);
            target.CurrentHp -= damage;
            return damage;
        }
    }
}
