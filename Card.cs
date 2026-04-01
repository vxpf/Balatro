using System;
using System.Collections.Generic;
using System.Text;

namespace KlasUitwerking
{
    abstract class Card
    {
        public CardValue Value;
        public Suit Suit;
        public Card(CardValue startValue, Suit startSuit)
        {
            this.Value = startValue;
            this.Suit = startSuit;
        }   

        public void PrintMe()
        {
            Console.WriteLine(this.Value);
        }

        // representatie als string; kan overschreven worden door afgeleide kaarten
        public virtual string MakeAsString()
        {
            return this.Suit.ToString() + " " + this.Value.ToString();
        }

        // Geef aan of deze kaart bij een bepaalde suit past (wildcard past altijd)
        public virtual bool MatchesSuit(Suit s)
        {
            return this.Suit == s;
        }

        
        public virtual int GetBonusPoints()
        {
            return 0;
        }

        // Basis punten voor de kaart (kan overschreven worden voor speciale kaarten)
        public virtual int GetBasePoints()
        {
            return (int)this.Value;
        }

        // Totale score van de kaart: basispunten + eventuele bonuspunten
        public virtual int GetScore()
        {
            return this.GetBasePoints() + this.GetBonusPoints();
        }
    }
}