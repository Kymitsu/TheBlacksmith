using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith.Game
{
    public class PlayerActionEvenArgs : EventArgs
    {
        public Player Player { get; set; }
        public string action { get; set; }

        public PlayerActionEvenArgs(Player player, string action)
        {
            Player = player;
            this.action = action;
        }
    }

    public class Adventure
    {
        public ulong ChannelId { get; set; }
        public int Lvl { get; set; }
        public Player Player { get; set; }

        private int Difficulty { get; set; }
        public int CompletedEncounter { get; set; }
        private Encounter CurrentEncounter { get; set; }
        public ulong EncouterMsgID { get; set; }
        public StringBuilder EncounterMsg { get; set; }
        public ulong ActionsMsgID { get; set; }
        public StringBuilder AdvMsg { get; set; }
        public bool IsWaitingForPlayerAction { get; set; }

        public List<string> PossibleActions { get; set; }

        public Adventure(ulong channelId, int lvl, Player player)
        {
            ChannelId = channelId;
            Lvl = lvl;
            Player = player;
            Difficulty = 0;
            CompletedEncounter = 0;

            PossibleActions = new List<string>();

            AdvMsg = new StringBuilder();

            GenerateEncounter(this.Lvl);
            CurrentEncounter.PossibleActions.CopyTo(PossibleActions);

            EncounterMsg = CurrentEncounter.FightLog;
        }

        //TODO: unsubscribe from event? pas sûr que ça soit nécessaire
        //https://stackoverflow.com/questions/4172809/should-i-unsubscribe-from-events
        private void OnEndOfEncounter(object sender, EndOfEncounterEventArgs e)
        {
            CompletedEncounter++;
            PossibleActions.Clear();
            PossibleActions.Add("Tavern");
            PossibleActions.Add("Continue");
            PossibleActions.Add("Deeper");

            EncounterMsg = CurrentEncounter.FightLog;

            switch (e.Status)
            {
                case EncounterStatus.Ongoing:
                    throw new Exception("shouldn't happen");
                case EncounterStatus.Victory:
                    break;
                case EncounterStatus.Defeat:
                    AdvMsg.AppendLine("Your adventure end here, but this is not the end of your story.");
                    break;
                case EncounterStatus.Retreat:
                    break;
                default:
                    break;
            }

            CurrentEncounter = null;

        }

        public void NextStep(string action = "")
        {
            if (CurrentEncounter == null)
            {
                switch (action)
                {
                    case "":
                        GenerateEncounter(this.Lvl);
                        break;
                    case "Tavern":
                        AdvMsg.AppendLine("+ You return with your loot. You are safe now...");
                        //Event end of adv ??
                        return;

                    case "Continue":
                        GenerateEncounter(this.Lvl + Difficulty);
                        break;
                    case "Deeper":
                        Difficulty++;
                        GenerateEncounter(this.Lvl + Difficulty);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                CurrentEncounter.PlayerTurnTODO(action);
                if(CurrentEncounter != null && CurrentEncounter.Status == EncounterStatus.Ongoing)
                    CurrentEncounter.PossibleActions.CopyTo(PossibleActions);

            }

            if (CurrentEncounter != null)
            {
                EncounterMsg = CurrentEncounter.FightLog; 
            }
        }

        public void GenerateEncounter(int lvl)
        {
            Monster m = new Monster("Goblin", lvl);
            CurrentEncounter = new Encounter(Player, m);

            CurrentEncounter.OnEndOfEncounter += OnEndOfEncounter;
        }
    }
}
