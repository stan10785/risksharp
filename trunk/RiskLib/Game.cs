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
        public Color[] PlayerColors = new Color[] { Color.LightGreen, Color.LightBlue, Color.LightPink, Color.LightYellow };
        public int[] BonusTroops = new int[] { 4, 8, 12, 16, 20, 25, 30, 35, 40, 45, 50, 55, 60, 65, 70, 75, 80, 85, 90, 95, 100 };


        public RiskBoard Board { get; private set; }
        public RiskDeck Deck { get; private set; }

        public List<RiskPlayer> Players { get; private set; }
        public List<RiskPlayer> DefeatedPlayers { get; private set; }
        
        public List<BoardTerritory> UnassignedTerritories { get; private set; }
        public List<PlayerTerritory> PlayerTerritories { get; private set; }
        private int CurrentPlayerIndex;

        public RiskGameState State;


        
        // Constructor
        
        public RiskGame( string xmlPath )
        {
            Board = new RiskBoard(xmlPath);
            Deck = new RiskDeck(Board, new Random(DateTime.Now.Second));
            Players = new List<RiskPlayer>();
            DefeatedPlayers = new List<RiskPlayer>();
            PlayerTerritories = new List<PlayerTerritory>();

            BoardTerritory[] tempArray = new BoardTerritory[Board.Territories.Count];
            Board.Territories.CopyTo(tempArray);
            UnassignedTerritories = tempArray.ToList();

            CurrentPlayerIndex = -1;

            State = new NotStarted(this);
        }


        // Game Logic

        public RiskPlayer CurrentPlayer 
        {
            get
            {
                if (CurrentPlayerIndex == -1) return null;
                return Players[CurrentPlayerIndex];
            }
        }

        public bool IsTurnInProgress 
        {
            get { return (CurrentPlayer != null); }
        }

        public int CurrentBonusTroopLevel { get { return BonusTroops[0]; } }


        public void SetCurrentPlayerIndex(int i) 
        {
            CurrentPlayerIndex = i;
        }    // should be protected somehow

        public void TurnOver() 
        {
            CurrentPlayerIndex++;
            if (CurrentPlayerIndex >= Players.Count)
                CurrentPlayerIndex = 0;
        }

        public void PlayerDefeated(RiskPlayer p) 
        {
            Players.Remove(p);
            DefeatedPlayers.Add(p);


            /// Check for game over:

            if (Players.Count == 1)
            {
                State = new GameOver(this.State);
            }
        }

        public void AwardBonusTroops() 
        {
            CurrentPlayer.GiveTroops(CurrentBonusTroopLevel);
            BonusTroops = BonusTroops.Skip(1).ToArray();
        }
        


        // Controller Properties

        #region <Calls to State Object>

        public bool CanAddPlayer { get { return State.CanAddPlayer(); } }
        public bool CanRandomlyAssignTerritories { get { return State.CanRandomlyAssignTerritories(); } }
        public bool CanEndAttack { get  { return State.CanEndAttack(); }}
        public bool CanTurnInCards { get  { return State.CanTurnInCards(); }}
        public string TurnState 
        {
            get
            {
                return State.TurnState();
            }
        }

        public void AddPlayer(string name)
        {
            State.AddPlayer(name);
        }
        public List<string> GetSelectable()
        {
            return State.GetSelectable();
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
        public void TurnInCards()
        {
            State.TurnInCards();
        }

        #endregion


        // Debug

        public string GetPlayersAsList() 
        {
            /// Just for debugging

            string str = "";

            foreach (RiskPlayer p in Players)
            {
                str += p.Name + " Te:"
                    + p.Territories.Count.ToString() + " Tr:"
                    + p.TotalTroops.ToString() + " Nt:"
                    + p.NewTroops.ToString()
                    + " [" + p.Hand.ToString() + "] "
                    + (p.SetAvailable ? "* " : " ")
                    + (p == CurrentPlayer ? "###" : "")
                    + "&nbsp;&nbsp;&nbsp;&nbsp;";
            }
            return str;
        }   
    }


    #region Game States

    public abstract class RiskGameState 
    {
        public RiskGame Game { get; set; }
     

        /// Controller properties

        public virtual List<string> GetSelectable()
        {
            return new List<string>();
        }

        public virtual string TurnState()
        {
            return "-";
        }

        public virtual bool CanAddPlayer() { return false; }
        public virtual bool CanRandomlyAssignTerritories() { return false; }
        public virtual bool CanEndAttack() { return false; }
        public virtual bool CanTurnInCards() { return false; }

        /// Base class throws errors.
        
        public virtual void AssignTerritoriesRandomly(Random rand)  { throw new InvalidOperationException(); }
        public virtual void AddPlayer(string name)                  { throw new InvalidOperationException(); }
        public virtual void GetNewTroops()                          { throw new InvalidOperationException(); }
        public virtual void TerritorySelected(string territoryName) { throw new InvalidOperationException(); }
        public virtual void EndAttack()                             { throw new InvalidOperationException(); }
        public virtual void TurnOver()                              { throw new InvalidOperationException(); }
        public virtual void TurnInCards()                           { throw new InvalidOperationException(); }
    }

    public class NotStarted : RiskGameState 
    {
        public NotStarted(RiskGame game) { Game = game; }
        public NotStarted(RiskGameState state) { Game = state.Game; }

        public override bool CanAddPlayer() { return true; }
        
        public override void AddPlayer(string name)
        {
            Color c = Game.PlayerColors[Game.Players.Count];

            Game.Players.Add(new RiskPlayer(name, Game, c));

            if (Game.Players.Count > 2)
                Game.State = new HasEnoughPlayers(this);
        }
    }

    public class HasEnoughPlayers : RiskGameState 
    {
        public HasEnoughPlayers(RiskGameState state) { Game = state.Game; }

        public override bool CanRandomlyAssignTerritories() { return true; }

        public override void AssignTerritoriesRandomly(Random r) 
        {
            while (Game.UnassignedTerritories.Count > 0)
            {
                foreach (RiskPlayer p in Game.Players)
                {
                    BoardTerritory t = Game.UnassignedTerritories[r.Next(Game.UnassignedTerritories.Count)];

                    PlayerTerritory pt = new PlayerTerritory(t, p, 1);

                    Game.PlayerTerritories.Add(pt);
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

        public override List<string> GetSelectable()
        {
            // Only a player's territories are valid

            return Game.CurrentPlayer.Territories.Select(t => t.boardTerritory.Name).ToList();
        }

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

        public NormalTurnsGameState(NormalTurnsGameState ts) : this(ts.Game) { }

        public NormalTurnsGameState(RiskGameState t): this(t.Game) { }
        
        public NormalTurnsGameState(RiskGame g)
        {
            Game = g;
            Turn = new NormalTurn(Game);        /// Start a normal turn
        }

        /// Controller properties

        public override bool CanEndAttack() { return Turn.State.CanEndAttack(); }
        public override bool CanTurnInCards() { return Turn.State.CanTurnInCards(); }

        public override List<string> GetSelectable()
        {
            return Turn.GetSelectable();
        }
        public override string TurnState()
        {
            return Turn.State.ToString();
        }

        public override void TerritorySelected(string territoryName)
        {
            Turn.State.TerritorySelected(territoryName);
        }

        public override void TurnInCards()
        {
            if (Game.CurrentPlayer.Hand.TurnInSet())
                Game.AwardBonusTroops();
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

    public class GameOver : RiskGameState 
    {
        public GameOver(RiskGameState state) { Game = state.Game; }
    }

    #endregion
}
