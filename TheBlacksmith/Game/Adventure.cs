using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TheBlacksmith.Game
{
    public class Adventure
    {
        public event EventHandler<EndOfEventArgs> OnEndOfAdventure = delegate { };

        public ulong ChannelId { get; set; }
        public int Lvl { get; set; }
        public Player Player { get; set; }

        private int Difficulty { get; set; }
        public int CompletedEncounter { get; set; }
        private Encounter CurrentEncounter { get; set; }
        public ulong EncouterMsgID { get; set; }
        public StringBuilder EncounterMsg { get; set; }
        public ulong AdvMsgID { get; set; }
        public StringBuilder AdvMsg { get; set; }
        public bool ReSendMsg { get; set; }
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
            ReSendMsg = false;

            GenerateEncounter(this.Lvl);
            CurrentEncounter.PossibleActions.CopyTo(PossibleActions);

            EncounterMsg = CurrentEncounter.FightLog;
        }

        //Imporant de unsubscribe des events si subscriber (ici Adventure) vit plus lontemps que le event producer (Encounter)
        //https://stackoverflow.com/questions/4172809/should-i-unsubscribe-from-events
        private void OnEndOfEncounter(object sender, EndOfEventArgs e)
        {
            CompletedEncounter++;
            PossibleActions.Clear();
            PossibleActions.Add("Tavern");
            PossibleActions.Add("Continue");
            PossibleActions.Add("Deeper");

            if (CurrentEncounter == null)
                throw new Exception("Something wrong happened");

            EncounterMsg = CurrentEncounter.FightLog;

            //Current Encounter Status
            switch (e.Status)
            {
                case State.Ongoing:
                    throw new Exception("shouldn't happen");
                case State.Victory:
                    AdvMsg.AppendLine("The vile creature is dead.");
                    break;
                case State.Retreat:
                    AdvMsg.AppendLine("You managed to distance the monster.");
                    break;
                case State.Defeat:
                    PossibleActions.Clear();
                    AdvMsg.AppendLine("Your adventure ends here, but this is not the end of your story.");
                    OnEndOfAdventure(this, new EndOfEventArgs(State.Defeat));
                    break;
                default:
                    throw new Exception("shouldn't happen");
            }

            CurrentEncounter.OnEndOfEncounter -= OnEndOfEncounter;
            CurrentEncounter = null;

        }

        public void NextStep(string action = "")
        {
            if (CurrentEncounter == null)
            {
                switch (action)
                {
                    //case "":
                    //    GenerateEncounter(this.Lvl);
                    //    break;
                    case "Tavern":
                        AdvMsg.AppendLine("You return with your loot. You are safe now...");
                        //Event end of adv ??
                        OnEndOfAdventure(this, new EndOfEventArgs(State.Retreat));

                        return;

                    case "Continue":
                        ReSendMsg = true;
                        GenerateEncounter(this.Lvl + Difficulty);
                        break;
                    case "Deeper":
                        ReSendMsg = true;
                        Difficulty++;
                        GenerateEncounter(this.Lvl + Difficulty);
                        break;
                    default:
                        throw new Exception("wrong action");
                }
                CurrentEncounter.PossibleActions.CopyTo(PossibleActions);
            }
            else
            {
                CurrentEncounter.PlayerTurn(action);
                if(CurrentEncounter != null && CurrentEncounter.Status == State.Ongoing)
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

            AdvMsg.Clear();
            PossibleActions.Clear();
        }

        public string BuildAdventureMessage()
        {
            string temp;
            temp = $"```md{ Environment.NewLine}";

            if (AdvMsg.Length != 0)
                temp += $"{ AdvMsg}{ Environment.NewLine}";

            if (PossibleActions.Any())
            {
                temp += $"Vos actions:{Environment.NewLine}";
                temp += $"-----------------------------------{Environment.NewLine}";
                temp += $"{FormatPossibleActions("/adv", PossibleActions)}```";
            }
                

            return temp;
        }

        private string FormatPossibleActions(string prefix, List<string> actions)
        {
            StringBuilder sb = new StringBuilder();
            int longestAction = prefix.Length + actions.Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur).Length + 1;

            for (int i = 0; i < actions.Count; i++)
            {
                if ((i + 1) % 3 == 1)
                    sb.Append("|");

                string temp = $"{prefix} {actions[i]}";

                sb.Append(string.Format($"{{0,-{longestAction + 1}}}|", temp));

                if ((i + 1) % 3 == 0)
                    sb.AppendLine("\u200b");
            }

            return sb.ToString();
        }
    }
}
