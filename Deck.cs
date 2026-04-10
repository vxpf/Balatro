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
        public virtual int TotalCardCount
        {
            get
            {
                return this.CardsRemaining.Count + this.CardsTaken.Count;
            }
        }

        public virtual int CardsRemainingCount => this.CardsRemaining.Count;
        public Deck()
        {
            this.CardsRemaining = new List<Card>();
            this.CardsTaken = new List<Card>();

            var suits = new[] { Suit.Clubs, Suit.Diamonds, Suit.Hearts, Suit.Spades };

            var values = Enum.GetValues(typeof(CardValue));
            int totalCards = suits.Length * values.Length;
            var rng = new Random();
            int bonusIndex = rng.Next(totalCards);
            int glassIndex;
            do { glassIndex = rng.Next(totalCards); } while (glassIndex == bonusIndex);
            int wildcardIndex;
            do { wildcardIndex = rng.Next(totalCards); } while (wildcardIndex == bonusIndex || wildcardIndex == glassIndex);

            int idx = 0;
            foreach (Suit suit in suits)
            {
                foreach (CardValue value in values)
                {
                    Card card;
                    if (idx == wildcardIndex)
                    {
                        card = new WildcardCard();
                    }
                    else if (idx == bonusIndex)
                    {
                        card = new BonusCard(value, suit, 10);
                    }
                    else if (idx == glassIndex)
                    {
                        card = new GlassCard(value, suit, multiplier: 2.0, breakChance: 0.2);
                    }
                    else
                    {
                        card = new RegularCard(value, suit);
                    }

                    this.CardsRemaining.Add(card);
                    // Gebruik MakeAsString voor correcte weergave speciale kaarten
                    Console.WriteLine(card.MakeAsString());
                    idx++;
                }
            }
        }

        public virtual void AddCard(Card card)
        {
            this.CardsRemaining.Add(card);
        }
        // Verwijder kaart permanent uit deck (remaining en taken)
        public void RemoveCard(Card card)
        {
            if (card == null) return;
            this.CardsRemaining.Remove(card);
            this.CardsTaken.Remove(card);
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

        // Tel wildcards in deck
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