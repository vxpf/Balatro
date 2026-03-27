using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KlasUitwerking
{
    class PlayerHand
    {
        List<Card> Hand;
        List<int> SelectedIndexes;

        public IEnumerable<Card> CardsInHand => this.Hand;
        public IEnumerable<int> SelectedCards => this.SelectedIndexes;

        public PlayerHand(int maxCards)
        {
            this.MaxCards = maxCards;
            this.Hand = new List<Card>();
            this.SelectedIndexes = new List<int>();
        }

        public int MaxCards { get; private set; }

        public void AddCard(Card newCard)
        {
            this.Hand.Add(newCard);
        }

        public void SelectCard(int index)
        {
            // validate index
            if (index < 0 || index >= this.Hand.Count)
            {
                return;
            }

            // add if not already selected
            if (!this.SelectedIndexes.Contains(index))
            {
                this.SelectedIndexes.Add(index);
            }
        }

        public void DeselectCard(int index)
        {
            SelectedIndexes.Remove(index);
        }

        public List<Card> GetSelected()
        {
            return this.Hand.
                Where((card, index) =>
                {
                    if (this.SelectedIndexes.Contains(index))
                    {
                        return true;
                    }
                    return false;
                })
            .ToList();
        }

        public void RemoveSelected()
        {
            
            this.Hand = this.Hand.
                Where((card, index) =>
                {
                    if (this.SelectedIndexes.Contains(index))
                    {
                        return false;
                    }
                    return true;
                })
            .ToList();

            this.SelectedIndexes.Clear();
        }

        // Calculate total hand score: sum of card values plus any bonus points
        public int CalculateScore()
        {
            int total = 0;
            foreach (var card in this.Hand)
            {
                total += (int)card.Value;
                total += card.GetBonusPoints();
            }
            return total;
        }
    }
}