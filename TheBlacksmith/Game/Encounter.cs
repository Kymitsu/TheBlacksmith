using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith.Game
{

    public class Encounter
    {
        public event EventHandler<EndOfEventArgs> OnEndOfEncounter = delegate { };

        public Monster Monster { get; set; }
        public Player Player { get; set; }
        public int Turn { get; set; }
        //public bool PlayerTurn { get; set; }
        public StringBuilder FightLog { get; set; }
        public State Status { get; set; }


        public List<string> PossibleActions { get; set; }

        public Encounter(Player p, Monster m)
        {
            Player = p;
            Monster = m;
            Turn = 0;
            Status = State.Ongoing;
            PossibleActions = new List<string>();
            FightLog = new StringBuilder();
            SetPlayerActions();

            Player.OnLevelUp += OnPlayerLevelUp;
            Player.OnDeath += OnPlayerDeath;
            Monster.OnDeath += OnMonsterKill;

            EncounterInit();
        }

        //TODO
        private void EncounterInit()
        {
            if (StaticRandom.FlipACoin())//Monster attacks first
            {
                FightLog.AppendLine($"A {Monster.Name} - LVL{Monster.Lvl} jumps on you. It attacks first!");

                MonsterTurn();
            }
            else
            {
                FightLog.AppendLine($"You encounter a {Monster.Name} - LVL{Monster.Lvl}. Prepare to fight!!");
                FightLog.AppendLine($"They haven't seen you yet!");
            }
        }

        public void PlayerTurn(string action = "")
        {
            if (string.IsNullOrWhiteSpace(action))
                throw new Exception("shouldn't happen?");

            if (action.ToLower() == "retreat")
            {
                FightLog.AppendLine($"- You retreat from the fight. You don't gain anything but you are still alive...");
                Status = State.Retreat;
                OnEndOfEncounter(this, new EndOfEventArgs(State.Retreat));
            }
            else
            {
                var func = Player.Attacks.Find(x => x.Method.Name == action);
                int damage = func.Invoke(Player, Monster, FightLog);


                if (Monster.IsAlive)
                {
                    MonsterTurn();
                }
            }

            SetPlayerActions();
        }

        public void MonsterTurn()
        {
            
            var func = Monster.Attacks[StaticRandom.Next(0, Monster.Attacks.Count - 1)];//TODO: Monster pattern ?
            int damage = func.Invoke(Monster, Player, FightLog);


        }

        //Response to Player & Monster Event
        private void OnPlayerLevelUp()
        {
            FightLog.AppendLine($"+ {Player.Name} leveled up! {Player.Name} is now Lvl {Player.Lvl}");

        }

        private void OnPlayerDeath()
        {
            FightLog.AppendLine($"- You have been slained by {Monster.Name}. Better luck next time...");
            int loss = Convert.ToInt32(Player.Money * 0.1);
            Player.Money -= loss;
            FightLog.AppendLine($"- You lost {loss} gold.");

            Status = State.Defeat;
            OnEndOfEncounter(this, new EndOfEventArgs(State.Defeat));
        }

        private void OnMonsterKill()
        {
            //TODO: Monster loot table
            int exp = Monster.Lvl * 5;
            int gold = Monster.Lvl + StaticRandom.Next(0, 5);
            FightLog.AppendLine($"+ {Monster.Name} has been slain. You gain {exp} XP and {gold} gold.");


            Player.AddExp(exp);
            Player.Money += gold;

            Status = State.Victory;
            OnEndOfEncounter(this, new EndOfEventArgs(State.Victory));
        }


        private void SetPlayerActions()
        {
            PossibleActions.Clear();
            foreach (var item in Player.Attacks)
            {
                PossibleActions.Add(item.Method.Name);
            }

            PossibleActions.Add("Retreat");
        }
    }
}
