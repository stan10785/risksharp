using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiskLib
{
    public class NormalTurn
    {
        public TurnState State;
        public RiskPlayer Player { get; private set; }
        public RiskGame Game { get; private set; }

        public NormalTurn(RiskGame g)
        {
            Game = g;
            Player = Game.CurrentPlayer;
            Player.NewTroopsReset();
            State = new ReinforceState(this);
        }

        public void TerritorySelected(string territoryName)
        {
            State.TerritorySelected(territoryName);
        }
    }

    public abstract class TurnState 
    {
        protected NormalTurn Turn { get; set; }


        /// Base class throws error

        public virtual void TerritorySelected(string territoryName)
        {
            throw new InvalidOperationException();
        }
        public virtual void EndAttack()
        {
            throw new InvalidOperationException();
        }
    }

    public class ReinforceState 
        : TurnState 
    {
        public ReinforceState(NormalTurn t) 
        { 
            Turn = t;
            Turn.Player.GetNewTroops();
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
                Turn.State = new AttackState(Turn);
            }
        }
    }

    public class AttackState 
        : TurnState 
    {
        public PlayerTerritory AttackingTerritory { get; private set; }
        public PlayerTerritory DefendingTerritory { get; private set; }

        public AttackState(NormalTurn t) { Turn = t; }

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
            /// for now, attacking territory always wins, takes 1 troop
            bool AttackSuccess = true;
            int TroopsToMove = 1;

            if (AttackSuccess)
            {
                /// Defending territory now controlled by attacking player

                DefendingTerritory.Conquered(AttackingTerritory.Player, TroopsToMove);
                AttackingTerritory.ReduceForce(TroopsToMove);
            }
            else
            {
                /// nothing for now
            }


            /// Start a new Attack

            Turn.State = new AttackState(Turn);
        }

        public override void EndAttack()
        {
            /// Go to Fortify mode

            Turn.State = new FortifyState(Turn);
        }
    }

    public class FortifyState 
        : TurnState
    {
        public PlayerTerritory SourceTerritory { get; private set; }
        public PlayerTerritory TargetTerritory { get; private set; }

        public FortifyState(NormalTurn t) { Turn = t; }

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
}
