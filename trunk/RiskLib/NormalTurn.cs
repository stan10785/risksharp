using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiskLib
{
    public class NormalTurn
    {
        public TurnState State { get; private set; }
        public RiskPlayer Player { get; private set; }
        public RiskGame Game { get; private set; }

        public NormalTurn(RiskGame g)
        {
            Game = g;
            Player = Game.CurrentPlayer;
            State = new ReinforceState(this);
        }
    }

    public abstract class TurnState
    {
        protected NormalTurn Turn { get; set; }
    }

    public class ReinforceState : TurnState
    {
        public ReinforceState(NormalTurn t) { Turn = t; } 
    }

    public class AttackState : TurnState
    {

    }

    public class FortifyState : TurnState
    {

    }
}
