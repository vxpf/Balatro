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
            return $"{RankString()}{SuitSymbol()}";
        }

        // helper: toon de suit als symbool (♣ ♦ ♥ ♠), of leeg voor None
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

        // helper: korte rank-naam (2..10, J, Q, K, A)
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

        // Some special cards may need context-sensitive bonuses (e.g., ExtraCard
        // gives points based on other cards in the hand). Provide a hook that
        // callers (PlayerHand) can supply the current hand/context when
        // computing the final score.
        public virtual int GetAdditionalBonus(IEnumerable<Card> context)
        {
            return 0;
        }
    }
}