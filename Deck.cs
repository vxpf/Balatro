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

            // maak een random generator: 5% kans op Wildcard, 10% kans op BonusCard
            var rng = new Random();
            // expliciete lijst van de vier geldige suits — voorkomt Suit.None in deck
            var suits = new[] { Suit.Clubs, Suit.Diamonds, Suit.Hearts, Suit.Spades };
            foreach (Suit suit in suits)
            {
                foreach (CardValue value in Enum.GetValues(typeof(CardValue)))
                {
                    Card card;
                    // bepaal type met één random getal (exclusieve kansen)
                    double rnd = rng.NextDouble();
                    if (rnd < 0.05)
                    {
                        // 5% kans op wildcard (vervangt deze kaartpositie)
                        card = new WildcardCard();
                    }
                    else if (rnd < 0.15)
                    {
                        // volgende 10% kans op bonus
                        card = new BonusCard(value, suit, 10);
                    }
                    else if (rnd < 0.20)
                    {
                        // volgende 5% kans op glazen kaart
                        card = new GlassCard(value, suit, multiplier: 2.0, breakChance: 0.2);
                    }
                    else
                    {
                        card = new RegularCard(value, suit);
                    }

                    this.CardsRemaining.Add(card);
                    // gebruik MakeAsString zodat speciale kaarten correct getoond worden
                    Console.WriteLine(card.MakeAsString());
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

        // Count wildcards remaining in the deck
        public int CountWildcardsRemaining()
        {
            return this.CardsRemaining.Count(c => c is WildcardCard);
        }

        public virtual void Shuffle()
        {
            this.CardsRemaining = this.CardsRemaining.Shuffle().ToList();
        }
    }
}