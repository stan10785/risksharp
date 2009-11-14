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

        public void Fortify()
        {
            Troops++;
        }
    }

    public class RiskPlayer 
    {
        public string Name { get; private set; }
        public List<PlayerTerritory> Territories { get; private set; }
        public int NewTroops { get; private set; }
        public RiskBoard Board;
        public Color color { get; private set; }

        public RiskPlayer(string name, RiskBoard board, Color c)
        {
            Name = name;
            Board = board;
            color = c;
            Territories = new List<PlayerTerritory>();
        }

        public PlayerTerritory AssignTerritory( BoardTerritory territory )
        {
            PlayerTerritory t = new PlayerTerritory(territory, this, 1);
            Territories.Add(t);
            return t;
        }

        public void GiveTroops(int i)
        {
            NewTroops += i;
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
            Territories.Where(t => t.boardTerritory.Name == territoryName).Single().Fortify();
        }

        public int ContinentTroops
        {
            get
            {
                int troops = 0;

                foreach (BoardContinent c in Board.Continents)
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
