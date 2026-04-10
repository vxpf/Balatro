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

        // String representatie; kan overschreven worden door afgeleide kaarten
        public virtual string MakeAsString()
        {
            return $"{RankString()}{SuitSymbol()}";
        }

        // Toon suit als symbool (♣ ♦ ♥ ♠), of leeg voor None
        protected string SuitSymbol()
        {
            switch (this.Suit)
            {
                case Suit.Clubs: return "\u2663"; // ♣
                case Suit.Diamonds: return "\u2666"; // ♦
                case Suit.Hearts: return "\u2665"; // ♥
                case Suit.Spades: return "\u2660"; // ♠
                default: return "";
            }
        }

        // Korte rank-naam (2..10, J, Q, K, A)
        protected string RankString()
        {
            switch (this.Value)
            {
                case CardValue.Two: return "2";
                case CardValue.Three: return "3";
                case CardValue.Four: return "4";
                case CardValue.Five: return "5";
                case CardValue.Six: return "6";
                case CardValue.Seven: return "7";
                case CardValue.Eight: return "8";
                case CardValue.Nine: return "9";
                case CardValue.Ten: return "10";
                case CardValue.J: return "J";
                case CardValue.Q: return "Q";
                case CardValue.K: return "K";
                case CardValue.A: return "A";
                default: return this.Value.ToString();
            }
        }

        // Check of kaart bij een suit past (wildcard past altijd)
        public virtual bool MatchesSuit(Suit s)
        {
            return this.Suit == s;
        }

        
        public virtual int GetBonusPoints()
        {
            return 0;
        }

        // Basispunten van de kaart (kan overschreven worden)
        public virtual int GetBasePoints()
        {
            return (int)this.Value;
        }

        // Totale score: basispunten + bonuspunten
        public virtual int GetScore()
        {
            return this.GetBasePoints() + this.GetBonusPoints();
        }

        // Bonus afhankelijk van context
        public virtual int GetAdditionalBonus(IEnumerable<Card> context)
        {
            return 0;
        }
    }
}