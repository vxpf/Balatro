using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KlasUitwerking
{
    class Deck
    {
        private List<Card> CardsRemaining;
        private List<Card> CardsTaken;
        //new
        public virtual int TotalCardCount
        {
            get
            {
                return this.CardsRemaining.Count + this.CardsTaken.Count;
            }
        }

        public virtual int CardsRemainingCount => this.CardsRemaining.Count;
        //oud
        public Deck()
        {
            this.CardsRemaining = new List<Card>();
            this.CardsTaken = new List<Card>();

            // maak een random generator: 10% kans op BonusCard
            var rng = new Random();
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
                {
                    Card card;
                    // 10% kans dat een kaart een BonusCard is
                    if (rng.NextDouble() < 0.10)
                    {
                        card = new BonusCard(value, suit, 10);
                    }
                    else
                    {
                        card = new RegularCard(value, suit);
                    }

                    this.CardsRemaining.Add(card);
                    Console.WriteLine(card.Suit.ToString()
                        + " " + card.Value.ToString());
                }
            }
        }

        public virtual void AddCard(Card card)
        {
            this.CardsRemaining.Add(card);
        }
        public virtual Card? TakeCard()
        {
            if (this.CardsRemaining.Count == 0)
            {
                return null;
            }

            Card taken = this.CardsRemaining.First();
            this.CardsRemaining.RemoveAt(0);
            this.CardsTaken.Add(taken);
            return taken;
        }

        public virtual void Reset()
        {
            this.CardsRemaining =
                this.CardsRemaining
                .Concat(this.CardsTaken)
                .ToList();
            this.CardsTaken = new List<Card>();
        }

        public virtual void Shuffle()
        {
            this.CardsRemaining = this.CardsRemaining.Shuffle().ToList();
        }
    }
}