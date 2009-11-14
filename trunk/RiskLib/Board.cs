using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace RiskLib
{
    /// <summary>
    /// An implementation of the board, without any reference to 
    /// the pieces or players.
    /// </summary>
    public class RiskBoard 
    {
        XmlDocument xml;
        public List<BoardContinent> Continents { get; private set; }
        public List<BoardTerritory> Territories { get; private set; }

        public RiskBoard(string xmlPath) 
        {
            xml = new XmlDocument();
            xml.Load(xmlPath);

            Continents = new List<BoardContinent>();
            Territories = new List<BoardTerritory>();
            
            LoadContinents();
            BoardValidate();
        }

        private void LoadContinents() 
        {
            foreach( XmlNode n in xml.SelectNodes("/Risk/Continent") )
            {
                BoardContinent c = new BoardContinent( n.Attributes["name"].Value, Int32.Parse(n.Attributes["pointValue"].Value), this);

                foreach (XmlNode cn in n.ChildNodes)
                {
                    BoardTerritory t = new BoardTerritory(cn.Attributes["name"].Value, c, this);
                    Territories.Add(t);
                    c.AddTerritory(t);
                }
                
                Continents.Add(c);
            }

            // Load the Adjacent Territories

            foreach (BoardTerritory t in Territories)
            {
                foreach (XmlNode n in xml.SelectNodes("/Risk/Continent/Territory[@name='" + t.Name + "']"))
                {
                    foreach (XmlNode cn in n.ChildNodes)
                    {
                        t.AddAdjacentTerritory(Territories.Where(x => x.Name == cn.Attributes["name"].Value).Single());
                    }
                }
            }
        }

        private void BoardValidate()
        {
            /// check that no territory list

            /// check that all borders go both ways
            /// 
            foreach (BoardTerritory b in Territories)
            {
                foreach (BoardTerritory b_Child in b.AdjacentTerritories)
                {
                    /// check that no territory lists itself as adjacent!
                    /// 
                    if (b_Child == b)
                        throw new InvalidOperationException();

                    /// check that all borders go both ways
                    /// 
                    if(!b_Child.AdjacentTerritories.Contains(b))
                        throw new InvalidOperationException();
                }
            }
        }
    }

    public class BoardContinent 
    {
        public RiskBoard Board { get; private set; }

        public string Name { get; private set; }
        public int PointValue { get; private set; }

        public List<BoardTerritory> Territories { get; private set; }

        public BoardContinent(string name, int pointValue, RiskBoard board)
        {
            Board = board;
            Name = name;
            PointValue = pointValue;
            Territories = new List<BoardTerritory>();
        }

        public void AddTerritory(BoardTerritory t)
        {
            Territories.Add(t);
        }
    }

    public class BoardTerritory 
    {
        public RiskBoard Board { get; private set; }
        public BoardContinent Continent { get; private set; }

        public string Name { get; private set; }
        public List<BoardTerritory> AdjacentTerritories { get; private set; }

        public BoardTerritory(string name, BoardContinent continent, RiskBoard board)
        {
            Board = board;
            Continent = continent;
            Name = name;
            AdjacentTerritories = new List<BoardTerritory>();
        }

        public void AddAdjacentTerritory( BoardTerritory t )
        {
            AdjacentTerritories.Add(t);
        }
    }
}
