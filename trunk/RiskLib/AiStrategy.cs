using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiskLib
{

    public abstract class RiskAiStrategy
    {
        public RiskAiPlayer Player { get; private set; }
        protected Random rand;

        // Constructor
        public RiskAiStrategy(RiskAiPlayer p)
        {
            Player = p;
            rand = new Random(DateTime.Now.Second);
        }


        public virtual string GetInitialReinforceTerritory()
        {
            throw new InvalidOperationException();
        }
        public virtual string GetReinforceTerritory()
        {
            throw new InvalidOperationException();
        }
        public virtual RiskDecision GetAttackDecision()
        {
            throw new InvalidOperationException();
        }
        public virtual RiskDecision GetFortifyDecision()
        {
            throw new InvalidOperationException();
        }
    }

    public class RandomStrategy : RiskAiStrategy
    {
        public RandomStrategy(RiskAiPlayer p) : base(p) { }

        public override string GetInitialReinforceTerritory()
        {
            int bestWeight = 0;
            string bestTerr = "";

            foreach (PlayerTerritory pt in Player.Territories)
            {
                int tempWeight = rand.Next(100);
                if (tempWeight > bestWeight)
                {
                    bestWeight = tempWeight;
                    bestTerr = pt.boardTerritory.Name;
                }
            }

            return bestTerr;
        }

        public override string GetReinforceTerritory()
        {
            int bestWeight = 0;
            string bestTerr = "";

            foreach (PlayerTerritory pt in Player.Territories)
            {
                int tempWeight = rand.Next(100);
                if (tempWeight > bestWeight)
                {
                    bestWeight = tempWeight;
                    bestTerr = pt.boardTerritory.Name;
                }
            }

            return bestTerr;
        }

        public override RiskDecision GetAttackDecision()
        {
            RiskDecision ad = new RiskDecision();

            int bestWeight = 0;
            PlayerTerritory attTerr = null;

            foreach (PlayerTerritory pt in Player.Territories.Where(t => t.CanAttack))
            {
                int terrWeight = rand.Next(100);
                if (terrWeight > bestWeight)
                {
                    bestWeight = terrWeight;
                    attTerr = pt;
                }
            }

            int endWeight = rand.Next(100);
            if (endWeight > bestWeight)
            {
                ad.End = true;
            }
            else
            {
                // get target

                PlayerTerritory defTerr = null;
                bestWeight = 0;

                foreach (PlayerTerritory pt in attTerr.AttackableTerritories)
                {
                    int terrWeight = rand.Next(100);
                    if (terrWeight > bestWeight)
                    {
                        bestWeight = terrWeight;
                        defTerr = pt;
                    }
                }

                ad.End = false;
                ad.Source = attTerr.boardTerritory.Name;
                ad.Target = defTerr.boardTerritory.Name;
            }

            return ad;
        }

        public override RiskDecision GetFortifyDecision()
        {
            RiskDecision ad = new RiskDecision();

            int bestWeight = 0;
            PlayerTerritory srcTerr = null;

            foreach (PlayerTerritory pt in Player.Territories.Where(t => t.CanFortify))
            {
                int terrWeight = rand.Next(100);
                if (terrWeight > bestWeight)
                {
                    bestWeight = terrWeight;
                    srcTerr = pt;
                }
            }

            PlayerTerritory tarTerr = null;
            bestWeight = 0;

            foreach (PlayerTerritory pt in srcTerr.FortifiableTerritories)
            {
                int terrWeight = rand.Next(100);
                if (terrWeight > bestWeight)
                {
                    bestWeight = terrWeight;
                    tarTerr = pt;
                }
            }

            ad.End = false;
            ad.Source = srcTerr.boardTerritory.Name;
            ad.Target = tarTerr.boardTerritory.Name;

            return ad;
        }

    }

    public struct RiskDecision
    {
        public bool End;
        public string Source;
        public string Target;
    }
}
