using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace RiskLib
{
    public class PlayerTerritory 
    {
        public BoardTerritory boardTerritory { get; private set; }
        public int Troops { get; private set; }
        public RiskPlayer Player { get; private set; }

        public PlayerTerritory(BoardTerritory t, RiskPlayer p, int troops)
        {
            boardTerritory = t;
            Player = p;
            Troops = troops;
        }

        public void Reinforce() 
        {
            Troops++;
            Player.NewTroopPlaced();
        }

        public List<PlayerTerritory> AttackableTerritories 
        {
            get
            {
                List<PlayerTerritory> lst = new List<PlayerTerritory>();

                /// In order to attack, a territory must have 2 or more
                /// troops and border a territory owned by a neighbor

                if (Troops < 2) return lst;

                foreach (BoardTerritory bt in boardTerritory.AdjacentTerritories)
                {
                    PlayerTerritory pt = Player.Game.PlayerTerritories.Where(n => n.boardTerritory.Name == bt.Name).Single();

                    if (pt.Player != Player)
                        lst.Add(pt);
                }

                return lst;
            }
        }

        public List<PlayerTerritory> FortifiableTerritories
        {
            get
            {
                List<PlayerTerritory> lst = new List<PlayerTerritory>();

                /// In order to be a fortify source, a territory must have 2 or more
                /// troops and border a territory owned by a the same player

                if (Troops < 2) return lst;

                List<PlayerTerritory> excludeList = new List<PlayerTerritory> { this };

                lst = GetFortifiable(this, excludeList);

                /// a territory cannot fortify itself!

                if (lst.Contains(this)) lst.Remove(this);

                return lst;
            }
        }

        public bool CanFortify 
        {
            get
            {
                return (FortifiableTerritories.Count > 0);
            }
        }

        public bool CanAttack 
        {
            get
            {
                return (AttackableTerritories.Count > 0);
            }
        }

        public void Conquered(RiskPlayer p, int troopsToMove) 
        {
            Player = p;
            Troops = troopsToMove;
        }

        public void ReduceForce(int troopsToMove) 
        {
            Troops -= troopsToMove;

            if (Troops <= 0)
                throw new InvalidOperationException();
        }

        
        /// Recursively determines the valid targets for fortification
        
        private List<PlayerTerritory> GetFortifiable(PlayerTerritory pt, List<PlayerTerritory> excludeList)
        {
            List<PlayerTerritory> lst = new List<PlayerTerritory>();
            excludeList.Add(pt);

            foreach (BoardTerritory bt in pt.boardTerritory.AdjacentTerritories)
            {
                // Search all adjacentTerritories for ones that the same player owns.
                // Exclude ones we've already found

                PlayerTerritory pt_child = Player.Game.PlayerTerritories.Where(n => n.boardTerritory.Name == bt.Name).Single();

                if (!excludeList.Contains(pt_child) && pt_child.Player == pt.Player)
                {
                    lst.Add(pt_child);
                    excludeList.Add(pt_child);
                    lst.AddRange(GetFortifiable(pt_child, excludeList));
                }
            }

            return lst;
        }
    }

    public class RiskPlayer 
    {
        public string Name { get; private set; }
        //public List<PlayerTerritory> Territories { get; private set; }
        public int NewTroops { get; private set; }
        //public RiskBoard Board;
        public RiskGame Game { get; private set; }
        public Color color { get; private set; }

        public RiskPlayer(string name, RiskGame g, Color c)
        {
            Name = name;
            Game = g;
            color = c;
            //Territories = new List<PlayerTerritory>();
        }

        public List<PlayerTerritory> Territories
        {
            get
            {
                return Game.PlayerTerritories.Where(pt => pt.Player == this).ToList();
            }
        }

        //public PlayerTerritory AssignTerritory( BoardTerritory territory )
        //{
        //    PlayerTerritory t = new PlayerTerritory(territory, this, 1);
        //    Territories.Add(t);
        //    return t;
        //}

        public void GiveTroops(int i) { NewTroops += i; }
        public void NewTroopPlaced() { NewTroops--; }
        public void NewTroopsReset() { NewTroops = 0; }

        
        public void GetNewTroops()
        {
            /// in this state, the player gets troops based on the following rules:
            /// - 1 troop per three territories
            /// - bonus troops for continents held
            /// - 3 troop minimum per turn
            /// (territory cards ignored for now)

            int TerrTroops = Game.CurrentPlayer.Territories.Count / 3;
            int ContinentTroops = Game.CurrentPlayer.ContinentTroops;
            int TotalTroops = TerrTroops + ContinentTroops;

            if (TotalTroops > 3)
                Game.CurrentPlayer.GiveTroops(TotalTroops);
            else
                Game.CurrentPlayer.GiveTroops(3);
        }
        

        public int TotalTroops 
        {
            /// counts troops on the board

            get
            {
                int sum = 0;

                foreach (PlayerTerritory t in Territories)
                    sum += t.Troops;

                return sum;
            }
        }

        public void Reinforce(string territoryName)
        {
            Territories.Where(t => t.boardTerritory.Name == territoryName).Single().Reinforce();
        }

        public int ContinentTroops 
        {
            get
            {
                int troops = 0;

                foreach (BoardContinent c in Game.Board.Continents)
                {
                    // check each continent

                    if (Territories.Where(t => t.boardTerritory.Continent == c).Count()
                        == c.Territories.Count)
                    {
                        troops += c.PointValue;
                    }
                }

                return troops;
            }
        }
    }
}
