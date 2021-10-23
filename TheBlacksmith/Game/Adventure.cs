using System;
using System.Collections.Generic;
using System.Text;

namespace TheBlacksmith.Game
{
    public class Adventure
    {
        public ulong ChannelId { get; set; }
        public int Lvl { get; set; }
        public Player Player { get; set; }

        private int Difficulty { get; set; }
        public int CompletedEncounter { get; set; }
        private Encounter CurrentEncounter { get; set; }
        public ulong EncouterMsgID { get; set; }
        public ulong ActionsMsgID { get; set; }

        public List<string> PossibleActions { get; set; }

        public Adventure(ulong channelId, int lvl, Player player)
        {
            ChannelId = channelId;
            Lvl = lvl;
            Player = player;
            Difficulty = 0;
            CompletedEncounter = 0;
        }

        public string NextStep(string action = "")
        {
            StringBuilder sb = new StringBuilder();
            EncounterStatus status;

            if (CurrentEncounter == null)
            {
                switch (action)
                {
                    case "":
                        GenerateEncounter(this.Lvl);
                        break;
                    case "Tavern":
                        sb.AppendLine("+ You return safely with your loot. You can rest now...");

                        return sb.ToString();//TODO: A CHANGER. PAS OUF LE RETURN EN PLEIN MILIEU DE LA FONCTION

                    case "Continue":
                        GenerateEncounter(this.Lvl);
                        break;
                    case "Deeper":
                        Difficulty++;
                        GenerateEncounter(this.Lvl + Difficulty);
                        break;
                    default:
                        break;
                }
            }

            bool isEndOfEncounter = false;
            if(string.IsNullOrEmpty(action)) //1er tour avec playerTurn || action empty
            {
                if (CurrentEncounter.PlayerTurn)
                    return sb.ToString();
                else
                {
                    sb.AppendLine(CurrentEncounter.NextTurn(out status)); //Monster turn
                    return sb.ToString();
                }
            }
            else //normalement playerTurn
            {
                if (!CurrentEncounter.PlayerTurn)
                    throw new NotImplementedException("Shouldn't happen");

                sb.AppendLine(CurrentEncounter.NextTurn(out status, action)); //Player turn

                switch (status)
                {
                    case EncounterStatus.Ongoing:
                        sb.AppendLine(CurrentEncounter.NextTurn(out status)); //Monster turn
                        if (status == EncounterStatus.Defeat)
                        {
                            sb.AppendLine("Your adventure end here, but this is not the end of your story.");
                            isEndOfEncounter = true;
                        }
                        break;
                    case EncounterStatus.Victory:

                        isEndOfEncounter = true;
                        break;
                    case EncounterStatus.Retreat:
                        isEndOfEncounter = true;
                        break;
                    default:
                        throw new NotImplementedException("Shouldn't happen");
                }
            }

            if (isEndOfEncounter)
            {
                CompletedEncounter++;
                CurrentEncounter = null;
                PossibleActions.Clear();
                PossibleActions.Add("Tavern");
                PossibleActions.Add("Continue");
                PossibleActions.Add("Deeper");
                
            }
            else
            {
                PossibleActions = CurrentEncounter.PossibleActions;
            }

            return sb.ToString();
        }

        public void GenerateEncounter(int lvl)
        {
            Monster m = new Monster("Goblin", lvl);
            CurrentEncounter = new Encounter(Player, m);
            EncounterStatus status;
            CurrentEncounter.NextTurn(out status); //turn 0
        }
    }
}
