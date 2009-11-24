using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RiskLib
{
    public class RiskAiPlayer : RiskPlayer  // uses strategy pattern
    {
        
        private RiskAiStrategy Strategy;

        // Constructor

        public RiskAiPlayer(string name, RiskGame g, Color c) : base(name, g, c)
        {
            Strategy = new RandomStrategy(this);
        }


        // Strategy

        public string GetInitialReinforceTerritory()
        {
            return Strategy.GetInitialReinforceTerritory();
        }
        public string GetReinforceTerritory()
        {
            return Strategy.GetReinforceTerritory();
        }
        public RiskDecision GetAttackDecision()
        {
            return Strategy.GetAttackDecision();
        }
        public RiskDecision GetFortifyDecision()
        {
            return Strategy.GetFortifyDecision();
        }
    }

    public class AiMoveEventArgs : EventArgs 
    {
        public string Summary { get; private set; }

        public AiMoveEventArgs(string s)
        {
            Summary = s;
        }
    }

}
