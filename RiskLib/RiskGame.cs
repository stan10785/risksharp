using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RiskLib;
using System.Drawing;

namespace RiskLib
{
    public class RiskGame 
    {
        public int[] InitalTroops = new int[] { 0, 0, 25, 20, 15, 15, 15 }; // { 0, 0, 40, 35, 30, 25, 20 };
        public Color[] PlayerColors = new Color[] { Color.LightGreen, Color.LightBlue, Color.LightPink };

        public RiskBoard Board { get; private set; }
        public List<RiskPlayer> Players { get; private set; }
        public List<BoardTerritory> UnassignedTerritories { get; private set; }
        public List<PlayerTerritory> PlayerTerritories { get; private set; }
        private int CurrentPlayerIndex;

        public RiskGameState State;

        public RiskPlayer CurrentPlayer 
        {
            get
            {
                if (CurrentPlayerIndex == -1) return null;
                return Players[CurrentPlayerIndex];
            }
        }

        public bool TurnInProgress 
        {
            get { return (CurrentPlayer != null); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="xmlPath"></param>
        public RiskGame( string xmlPath )
        {
            Board = new RiskBoard(xmlPath);
            Players = new List<RiskPlayer>();
            PlayerTerritories = new List<PlayerTerritory>();

            BoardTerritory[] tempArray = new BoardTerritory[Board.Territories.Count];
            Board.Territories.CopyTo(tempArray);
            UnassignedTerritories = tempArray.ToList();

            CurrentPlayerIndex = -1;

            State = new NotStarted(this);
        }


        public void SetCurrentPlayerIndex(int i)
        {
            CurrentPlayerIndex = i;
        }

        public void TurnOver()
        {
            CurrentPlayerIndex++;
            if (CurrentPlayerIndex >= Players.Count)
                CurrentPlayerIndex = 0;

            //State.TurnOver();
        }

        #region <Calls to State Object>

        public string TurnState()
        {
            return State.TurnState();
        }
        public void AddPlayer(string name)
        {
            State.AddPlayer(name);
        }
        public void AssignTerritoriesRandomly(Random r)
        {
            State.AssignTerritoriesRandomly(r);
        }
        public void GetNewTroops()
        {
            State.GetNewTroops();
        }
        public void TerritorySelected(string territoryName)
        {
            State.TerritorySelected(territoryName);
        }
        public void EndAttack()
        {
            State.EndAttack();
        }

        #endregion

        /// <summary>
        ///  Just for debugging
        /// </summary>
        /// <returns></returns>
        public string PlayersAsList()
        {
            string str = "";

            foreach (RiskPlayer p in Players)
            {
                str += p.Name + " "
                    + p.Territories.Count.ToString() + " "
                    + p.TotalTroops.ToString() + " "
                    + p.NewTroops.ToString()
                    + (p == CurrentPlayer ? "###" : "")
                    + "<br>";
            }
            return str;
        }
    }


    #region Game States

    public abstract class RiskGameState 
    {
        public RiskGame Game { get; set; }


        public virtual string TurnState()
        {
            return "-";
        }
        public virtual void TurnOver() { /* do nothing */ }

        // Base class throws errors.

        public virtual void AssignTerritoriesRandomly(Random rand)
        {
            throw new InvalidOperationException();
        }
        public virtual void AddPlayer(string name)
        {
            throw new InvalidOperationException();
        }
        public virtual void GetNewTroops()
        {
            throw new InvalidOperationException();
        }
        public virtual void TerritorySelected(string territoryName)
        {
            throw new InvalidOperationException();
        }
        public virtual void EndAttack()
        {
            throw new InvalidOperationException();
        }
    }

    public class NotStarted : RiskGameState 
    {
        public NotStarted(RiskGame game) { Game = game; }

        public override void AddPlayer(string name)
        {
            Color c = Game.PlayerColors[Game.Players.Count];

            Game.Players.Add(new RiskPlayer(name, Game, c));

            if (Game.Players.Count > 1)
                Game.State = new HasEnoughPlayers(this);
        }
    }

    public class HasEnoughPlayers : RiskGameState 
    {
        public HasEnoughPlayers(RiskGameState state) { Game = state.Game; }

        public override void AssignTerritoriesRandomly(Random r) 
        {
            while (Game.UnassignedTerritories.Count > 0)
            {
                foreach (RiskPlayer p in Game.Players)
                {
                    BoardTerritory t = Game.UnassignedTerritories[r.Next(Game.UnassignedTerritories.Count)];

                    Game.PlayerTerritories.Add(p.AssignTerritory(t));
                    Game.UnassignedTerritories.Remove(t);
                }
            }

            /// Select a player to go first, then change state
            Game.SetCurrentPlayerIndex(r.Next(Game.Players.Count));

            Game.State = new FirstReinforcementRound(this);
        }

        public override void AddPlayer(string name)
        {
            Color c = Game.PlayerColors[Game.Players.Count];

            Game.Players.Add(new RiskPlayer(name, Game, c));

            if (Game.Players.Count > 1)
                Game.State = new HasEnoughPlayers(this);
        }
    }

    public class FirstReinforcementRound : RiskGameState 
    {
        public FirstReinforcementRound(RiskGameState state) { Game = state.Game; }

        public override void TerritorySelected(string territoryName)
        {
            /// in this case, selecting the territory adds one reinforcement

            Game.CurrentPlayer.Reinforce(territoryName);
            Game.TurnOver();

            /// Check for state change:
            /// if all players have placed all of their troops, move into normal mode
            
            foreach (RiskPlayer p in Game.Players)
            {
                if (p.TotalTroops < Game.InitalTroops[Game.Players.Count])
                    return;
            }

            Game.State = new NormalTurnsGameState(this);
        }
    }

    public class NormalTurnsGameState : RiskGameState 
    {
        public NormalTurn Turn { get; private set; }

        public NormalTurnsGameState(RiskGameState state)
        {
            Game = state.Game;
            Turn = new NormalTurn(Game);        /// Start a normal turn
        }


        public override string TurnState()
        {
            return Turn.State.ToString();
        }

        public override void TerritorySelected(string territoryName)
        {
            Turn.State.TerritorySelected(territoryName);
        }

        public override void EndAttack()
        {
            Turn.State.EndAttack();
        }
        public override void TurnOver()
        {
            /// Start a new turn

            Game.TurnOver();
            Turn = new NormalTurn(Game);
        }
    }

    public class Completed : RiskGameState { }

    #endregion
}
