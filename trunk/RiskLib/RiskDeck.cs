using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RiskLib
{
    public enum RiskCardType 
    {
        Artillery,
        Cavalry,
        Infantry,
        Wild
    }

    public class RiskCard 
    {
        public string Name { get; private set; }
        public RiskCardType Type { get; private set; }

        public RiskCard(string name, RiskCardType c)
        {
            Name = name;
            Type = c;
        }
    }

    public class RiskDeck 
    {
        Random rand;
        public List<RiskCard> Cards { get; private set; }
        public List<RiskCard> DealtCards { get; private set; }

        public RiskDeck(RiskBoard board, Random r)
        {
            rand = r;
            Cards = new List<RiskCard>();
            DealtCards = new List<RiskCard>();

            /// Populate the deck

            int i = 0;
            RiskCardType[] types = new RiskCardType[] { RiskCardType.Artillery,
                RiskCardType.Cavalry, RiskCardType.Infantry };

            foreach (BoardTerritory bt in board.Territories)
            {
                Cards.Add(new RiskCard(bt.Name, types[i % 3]));
                i++;
            }

            /// Add two wild cards to deck
            
            Cards.Add( new RiskCard( "Wild", RiskCardType.Wild ));
            Cards.Add( new RiskCard( "Wild", RiskCardType.Wild ));
        }

        public RiskCard Deal() 
        {
            RiskCard d = Cards[rand.Next(Cards.Count)];
            Cards.Remove(d);
            DealtCards.Add(d);

            return d;
        }
    }


    public class RiskHand
    {
        public List<RiskCard> Cards { get; private set; }

        public RiskHand() 
        {
            Cards = new List<RiskCard>();
        }

        public void AddCard(RiskCard c) 
        {
            Cards.Add(c);
        }


        public int ArtilleryCards 
        {
            get
            {
                return Cards.Where(c => c.Type == RiskCardType.Artillery).Count();
            }
        }

        public int CavalryCards 
        {
            get
            {
                return Cards.Where(c => c.Type == RiskCardType.Cavalry).Count();
            }
        }

        public int InfantryCards 
        {
            get
            {
                return Cards.Where(c => c.Type == RiskCardType.Infantry).Count();
            }
        }

        public int WildCards 
        {
            get
            {
                return Cards.Where(c => c.Type == RiskCardType.Wild).Count();
            }
        }

        public bool SetAvailable 
        {
            get
            {
                /// need at least 2 to make a set, five cards always make a set
                if (Cards.Count < 3) return false;
                if (Cards.Count > 4) return true;

                /// three cards of the same type make a set
                if (ArtilleryCards > 2 || CavalryCards > 2 || InfantryCards > 2) return true;

                /// one of each makes a set
                if (ArtilleryCards > 0 && CavalryCards > 0 && InfantryCards > 0) return true;

                /// 2 plus a wild makes a set
                if (Cards.Count > 2 && WildCards > 0) return true;

                return false;
            }
        }

        public void TurnInSet()
        {
            if (!SetAvailable) throw new InvalidOperationException();

            /// This gets a litte tricky. Are all sets created equal?
            /// or are there situations where cards could be turned in differently
            /// for better results. I'm going with this simple formula for now
            

        }
    }
}
