using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiskLib 
{
    public class NormalTurn 
    {
        public NormalTurnState State;
        
        public RiskGame Game { get; private set; }

        public NormalTurn(RiskGame g)
        {
            Game = g;
            //Player = Game.CurrentPlayer;
            Player.NewTroopsReset();
            State = new ReinforceState(this);
        }

        public RiskPlayer Player { get { return Game.CurrentPlayer; } }

        public List<string> GetSelectable()
        {
            return State.GetSelectable();
        }

        public void TerritorySelected(string territoryName)
        {
            State.TerritorySelected(territoryName);
        }
    }

    public abstract class NormalTurnState 
    {
        public NormalTurn Turn { get; protected set; }


        

        public virtual List<string> GetSelectable()
        {
            return new List<string>();
        }

        protected bool CanAttack()
        {
            /// Check if any attacks are possible

            return (Turn.Game.CurrentPlayer.Territories.Where(
                t => t.CanAttack).Count() > 0);
        }

        protected bool CanFortify()
        {
            /// Check if any fortifications are possile

            return (Turn.Game.CurrentPlayer.Territories.Where(
                t => t.CanFortify).Count() > 0);
        }

        /// Base class throws error

        public virtual void TerritorySelected(string territoryName)
        {
            throw new InvalidOperationException();
        }
        public virtual void EndAttack()
        {
            throw new InvalidOperationException();
        }
        /// Controller properties
        public virtual bool CanEndAttack() { return false; }

        protected void TurnOver()
        {
            // assign card here

            Turn.Game.TurnOver();
            //Turn.Player = Turn.Game.CurrentPlayer;
            Turn.State = new ReinforceState(this);
        }
    }

    public class ReinforceState 
        : NormalTurnState 
    {

        public ReinforceState(NormalTurnState t) : this(t.Turn) { }
        public ReinforceState(NormalTurn t) 
        {
            Turn = t;
            Turn.Player.GetNewTroops();
        }
        
        

        public override List<string> GetSelectable() 
        {
            // Only a player's own territories are valid

            return Turn.Game.CurrentPlayer.Territories.Select(t => t.boardTerritory.Name).ToList();
        }

        public override void TerritorySelected(string territoryName) 
        {
            PlayerTerritory pt = Turn.Game.PlayerTerritories.Where(n => n.boardTerritory.Name == territoryName).Single();

            /// if the player owns this territory, add a reinforcement

            if (Turn.Player == pt.Player)
            {
                pt.Reinforce();
            }

            /// Check for state change:
            /// if no new troops left, go to attack state

            if (pt.Player.NewTroops == 0)
            {
                Turn.State = new AttackState(this);
            }
        }
    }

    public class AttackState 
        : NormalTurnState 
    {
        /// <summary>
        /// The logic here gets a little wishy washy. This may need its own state,
        /// especially when we add the cards and retreat into the mix
        /// </summary>

        public PlayerTerritory AttackingTerritory { get; private set; }
        public PlayerTerritory DefendingTerritory { get; private set; }

        public override bool CanEndAttack() { return true; }

        public AttackState(NormalTurnState nts) 
        { 
            Turn = nts.Turn;
        }

        public override List<string> GetSelectable()
        {
            /// perhaps this should have its own state?

            if (AttackingTerritory == null)
            {
                /// return all territories that can attack

                return Turn.Game.CurrentPlayer.Territories
                    .Where(t => t.CanAttack)
                    .Select(t => t.boardTerritory.Name)
                    .ToList();
            }
            else
            {
                // return the current attacking territory's potential targets

                return AttackingTerritory.AttackableTerritories.Select(t => t.boardTerritory.Name).ToList();
            }
        }

        public override void TerritorySelected(string territoryName) 
        {
            PlayerTerritory pt = Turn.Game.PlayerTerritories.Where(n => n.boardTerritory.Name == territoryName).Single();

            if (Turn.Player == pt.Player && pt.CanAttack)
            {
                /// This is the attacking territory
                AttackingTerritory = pt;
            }
            else if (Turn.Player != pt.Player)
            {
                /// Must choose attacker first

                if (AttackingTerritory == null)
                    throw new InvalidOperationException();

                /// Must be attackable

                if (!AttackingTerritory.AttackableTerritories.Contains(pt))
                    throw new InvalidOperationException();

                DefendingTerritory = pt;

                ExecuteAttack();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private void ExecuteAttack() 
        {
            /// for now, attacking territory always wins, moves 1 troop
            
            bool AttackSuccess = true;
            int TroopsToMove = 1;

            if (AttackSuccess)
            {
                RiskPlayer ConqueredPlayer = DefendingTerritory.Player;

                /// Defending territory now controlled by attacking player

                DefendingTerritory.Conquered(AttackingTerritory.Player, TroopsToMove);
                AttackingTerritory.ReduceForce(TroopsToMove);

                /// Check for Player Defeated

                if (ConqueredPlayer.Territories.Count == 0)
                {
                    Turn.Game.PlayerDefeated(ConqueredPlayer);
                }
            }
            else
            {
                /// nothing for now
            }


            /// Find correct state change

            if (!CanAttack() && !CanFortify())
            {
                base.TurnOver();
            }
            else if (!CanAttack())
            {
                Turn.State = new FortifyState(this);
            }
            else
            {
                Turn.State = new AttackState(this);
            }
        }

        public override void EndAttack()
        {
            /// Go to Fortify mode
            if (!CanFortify())
            {
                base.TurnOver();
            }
            else
            {
                Turn.State = new FortifyState(this);
            }
        }
    }

    public class FortifyState 
        : NormalTurnState
    {
        public PlayerTerritory SourceTerritory { get; private set; }
        public PlayerTerritory TargetTerritory { get; private set; }

        public FortifyState(NormalTurnState nts) { Turn = nts.Turn; }

        public override List<string> GetSelectable()
        {
            /// perhaps this should have its own state?

            if (SourceTerritory == null)
            {
                /// return all territories that can fortify

                return Turn.Game.CurrentPlayer.Territories
                    .Where(t => t.CanFortify)
                    .Select(t => t.boardTerritory.Name)
                    .ToList();
            }
            else
            {
                // return the current source territory's potential targets

                return SourceTerritory.FortifiableTerritories.Select(t => t.boardTerritory.Name).ToList();
            }
        }

        public override void TerritorySelected(string territoryName)
        {
            PlayerTerritory pt = Turn.Game.PlayerTerritories.Where(n => n.boardTerritory.Name == territoryName).Single();

            if (Turn.Player == pt.Player && pt.CanFortify)
            {
                /// This is the source territory
                SourceTerritory = pt;
            }

            else if (SourceTerritory != null && SourceTerritory.FortifiableTerritories.Contains(pt))
            {
                TargetTerritory = pt;

                ExecuteFortify();
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private void ExecuteFortify()
        {
            /// for now, just move one troop

            TargetTerritory.Reinforce();
            SourceTerritory.ReduceForce(1);

            Turn.Game.State.TurnOver();
        }
    }

    //public class TurnOver
    //    : NormalTurnState
    //{
    //    public TurnOver(NormalTurnState t)
    //    {
    //        Turn = t.Turn;
            
    //    }
    //}
}
