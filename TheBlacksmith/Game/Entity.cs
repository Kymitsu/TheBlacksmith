using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith.Game
{
    public abstract class Entity
    {
        public event Action OnDeath = delegate { };

        public int Hp { get; set; }
        public string Name { get; set; }
        public int Lvl { get; set; }
        public int BaseAtk { get; set; }
        public bool IsAlive { get; set; }

        private int _currentHp;
        public int CurrentHp
        {
            get { return _currentHp; }
            set
            {
                _currentHp = value;
                if (_currentHp <= 0)
                {
                    IsAlive = false;
                    OnDeath(); //Raise death event
                }
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

        public List<Func<Entity, Entity, StringBuilder, int>> Attacks { get; set; } = new List<Func<Entity, Entity, StringBuilder, int>>();





        public static int BasicAttack(Entity entity, Entity target, StringBuilder log)
        {
            int damage = entity.BaseAtk + StaticRandom.Next(1, 6);
            log.AppendLine($"  {entity.Name} used BasicAttack. {target.Name} took {damage} HP damage.");
            log.AppendLine($"  {target.Name} has {target.CurrentHp - damage}/{target.Hp} HP left.");

            target.CurrentHp -= damage;
            return damage;
        }

        public static int StrongAttack(Entity entity, Entity target, StringBuilder log)
        {
            int damage = entity.BaseAtk * 2 + StaticRandom.Next(3, 10);
            log.AppendLine($"  {entity.Name} used StrongAttack. {target.Name} took {damage} HP damage.");
            log.AppendLine($"  {target.Name} has {target.CurrentHp - damage}/{target.Hp} HP left.");

            target.CurrentHp -= damage;
            return damage;
        }
    }
}
