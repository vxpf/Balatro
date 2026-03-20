using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KlasUitwerking
{
    class PlayerHand
    {
        List<Card> Hand;
        //int MaxCards;
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
            if (this.Hand.Count == 5)
            {
                return;
            }
            if (this.Hand.Count > index)
            {
                if (!this.SelectedIndexes.Contains(index))
                {
                    this.SelectedIndexes.Add(index);
                }
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
    }
}