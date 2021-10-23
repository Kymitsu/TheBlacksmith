using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith.Game
{
    //public enum EncounterType
    //{
    //    Fight = 1,
    //    Event = 2
    //}

    public enum EncounterStatus
    {
        Ongoing = 0,
        Victory = 1,
        Defeat = 2,
        Retreat = 3
    }

    public class Encounter
    {
        public Monster Monster { get; set; }
        public Player Player { get; set; }
        public int Turn { get; set; }
        public bool PlayerTurn { get; set; }

        public List<string> PossibleActions { get; set; }

        public Encounter(Player p, Monster m)
        {
            Player = p;
            Monster = m;
            Turn = 0;
            PossibleActions = new List<string>();
            SetPlayerActions();
        }

        public string NextTurn(out EncounterStatus encounterStatus, string action = "")
        {
            StringBuilder sb = new StringBuilder();
            encounterStatus = EncounterStatus.Ongoing;

            if (Turn == 0) //Start of fight
            {
                sb.AppendLine($"You encounter a {Monster.Name} - LVL{Monster.Lvl}. Prepare to fight!!");

                if (StaticRandom.FlipACoin())//Player attacks first
                {
                    PlayerTurn = true;
                }
            }
            else
            {
                if (PlayerTurn)
                {
                    if (string.IsNullOrEmpty(action))
                        throw new NotImplementedException();

                    if (PossibleActions.Contains(action))
                    {
                        if (action.ToLower() == "retreat")
                        {
                            sb.AppendLine($"- You retreat from the fight. You don't gain anything but you are still alive...");
                            encounterStatus = EncounterStatus.Retreat;
                        }
                        else
                        {
                            var func = Player.Attacks[StaticRandom.Next(0, Monster.Attacks.Count - 1)];
                            int damage = func.Invoke(Player, Monster);
                            SetAttackText(sb, Player, Monster, func.Method.Name, damage);

                            if(!Monster.IsAlive)
                            {
                                int exp = Monster.Lvl * 5;
                                int gold = Monster.Lvl + StaticRandom.Next(0, 5);
                                bool isLvlUp = Player.AddExp(exp);
                                Player.Money += gold;
                                sb.AppendLine($"+ {Monster.Name} has been slain. You gain {exp} XP and {gold} gold.");
                                if (isLvlUp)
                                    sb.AppendLine($"{Player.Name} leveled up!");
                                encounterStatus = EncounterStatus.Victory;
                            }
                        }
                    }

                    PlayerTurn = false;
                }
                else//Monster action
                {
                    var func = Monster.Attacks[StaticRandom.Next(0, Monster.Attacks.Count - 1)];
                    int damage = func.Invoke(Monster, Player);
                    SetAttackText(sb, Monster, Player, func.Method.Name, damage);

                    PlayerTurn = true;

                    if(!Player.IsAlive)
                    {
                        sb.AppendLine($"- You have been slained by {Monster.Name}. Better luck next time...");
                        int loss = Convert.ToInt32(Player.Money * 0.1);
                        Player.Money -= loss;
                        sb.AppendLine($"- You lost {loss} gold.");
                        encounterStatus = EncounterStatus.Defeat;
                    }
                }
            }

            Turn++;
            return sb.ToString();
        }

        private bool IsEndOfEncounter()
        {

            return false;
        }

        private void SetAttackText(StringBuilder sb, Entity entity, Entity target, string action, int damage)
        {
            sb.AppendLine($"  {entity.Name} used {action}. {target.Name} took {damage} HP damage.");
            sb.AppendLine($"  {target.Name} has {target.CurrentHp}/{target.Hp} HP left.");
        }

        private void SetPlayerActions()
        {
            foreach (var item in Player.Attacks)
            {
                PossibleActions.Add(item.Method.Name);
            }

            PossibleActions.Add("Retreat");
        }
    }
}
