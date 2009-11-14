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
        public int[] InitalTroops = new int[] { 0, 0, 40, 35, 30, 25, 20 };
        public Color[] PlayerColors = new Color[] { Color.LightGreen, Color.LightBlue, Color.LightPink };

        public RiskBoard Board { get; private set; }
        public List<RiskPlayer> Players { get; private set; }
        public List<BoardTerritory> UnassignedTerritories { get; private set; }
        public List<PlayerTerritory> PlayerTerritories { get; private set; }

        public RiskGameState GameState { get; set; }

        public RiskGame( string xmlPath )
        {
            Board = new RiskBoard(xmlPath);
            Players = new List<RiskPlayer>();
            PlayerTerritories = new List<PlayerTerritory>();

            BoardTerritory[] tempArray = new BoardTerritory[Board.Territories.Count];
            Board.Territories.CopyTo(tempArray);
            UnassignedTerritories = tempArray.ToList();

            GameState = new NotStarted(this);
        }

        public RiskPlayer CurrentPlayer 
        {
            get
            {
                if (CurrentPlayerIndex == -1) return null;
                return Players[CurrentPlayerIndex];
            }
        }
        private int CurrentPlayerIndex = -1;
        public void SetCurrentPlayerIndex(int i)
        {
            CurrentPlayerIndex = i;
        }

        public void TurnOver()
        {
            CurrentPlayerIndex++;
            if (CurrentPlayerIndex >= Players.Count)
                CurrentPlayerIndex = 0;
        }

        public bool TurnInProgress
        {
            get { return (CurrentPlayer != null); }
        }

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
    

        // Calls to State Object

        public void AddPlayer(string name)
        {
            GameState.AddPlayer(name);
        }

        public void AssignTerritoriesRandomly(Random r)
        {
            GameState.AssignTerritoriesRandomly(r);
        }

        public void GetNewTroops()
        {
            GameState.GetNewTroops();
        }
        public void Fortify(string territoryName)
        {
            GameState.Reinforce(territoryName);
        }
    }

    #region Game States

    public abstract class RiskGameState 
    {
        public RiskGame Game { get; set; }
        

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
        public virtual void Reinforce(string territoryName)
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

            Game.Players.Add(new RiskPlayer(name, Game.Board, c));

            if (Game.Players.Count > 1)
                Game.GameState = new HasEnoughPlayers(this);
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

            Game.GameState = new FirstReinforcementRound(this);
        }

        public override void AddPlayer(string name)
        {
            Color c = Game.PlayerColors[Game.Players.Count];

            Game.Players.Add(new RiskPlayer(name, Game.Board, c));

            if (Game.Players.Count > 1)
                Game.GameState = new HasEnoughPlayers(this);
        }
    }

    public class FirstReinforcementRound : RiskGameState 
    {
        public FirstReinforcementRound(RiskGameState state) { Game = state.Game; }

        /* public override void GetNewTroops() 
        {
            if (G.CurrentPlayer.TotalTroops < G.InitalTroops[G.Players.Count])
                G.CurrentPlayer.GiveTroops(1);
        }*/

        public override void Reinforce(string territoryName) 
        {
            /// During first fortify, turn is over after placing one troop

            Game.CurrentPlayer.Reinforce(territoryName);
            Game.TurnOver();

            /// if all players have placed all of their troops, move into normal mode

            foreach (RiskPlayer p in Game.Players)
            {
                if (p.TotalTroops < Game.InitalTroops[Game.Players.Count])
                    return;
            }

            Game.GameState = new NormalTurnsGameState(this);
        }
    }

    public class NormalTurnsGameState : RiskGameState 
    {
        public NormalTurn Turn { get; private set; }

        public NormalTurnsGameState(RiskGameState state) 
        { 
            Game = state.Game;
            Turn = new NormalTurn(Game);        /// Start the game!
        }

        public override void GetNewTroops() 
        {
            /// in this state, the player gets troops based on the following rules:
            /// - 1 troop per three territories
            /// - bonus troops for continents held
            /// - 3 troop minimum per turn
            /// (territory cards ignored for now)

            int TerrTroops = Game.CurrentPlayer.Territories.Count / 3;
            int ContinentTroops = Game.CurrentPlayer.ContinentTroops;
            int TotalTroops = TerrTroops + ContinentTroops;

            if (TotalTroops < 3) 
                Game.CurrentPlayer.GiveTroops(TotalTroops);
            else 
                Game.CurrentPlayer.GiveTroops(3);
        }
    }

    public class Completed : RiskGameState { }

    #endregion
}
