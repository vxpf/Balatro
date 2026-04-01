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
            // if we have at least 5 cards, evaluate all 5-card combinations and return the best score
            var cards = this.Hand.ToList();
            if (cards.Count >= 5)
            {
                int best = int.MinValue;
                foreach (var combo in GetCombinations(cards, 5))
                {
                    int s = combo.Sum(c => c.GetScore());
                    // add context-aware bonuses from cards like ExtraCard
                    s += combo.Sum(c => c.GetAdditionalBonus(combo));
                    s += FlushChecker.GetFlushBonus(combo);
                    // apply glass multipliers if any GlassCard present
                    double multiplier = 1.0;
                    foreach (var g in combo.OfType<GlassCard>())
                    {
                        multiplier *= g.Multiplier;
                    }
                    int final = (int)Math.Round(s * multiplier);
                    if (final > best) best = final;
                }
                return best == int.MinValue ? 0 : best;
            }

            // otherwise sum all card scores and possible flush bonus
            int baseTotal = cards.Sum(c => c.GetScore());
            baseTotal += cards.Sum(c => c.GetAdditionalBonus(cards));
            baseTotal += FlushChecker.GetFlushBonus(cards);
            double multiplierTotal = 1.0;
            foreach (var g in cards.OfType<GlassCard>()) multiplierTotal *= g.Multiplier;
            return (int)Math.Round(baseTotal * multiplierTotal);
        }

        // helper: generate all k-combinations of a list
        private static IEnumerable<List<Card>> GetCombinations(List<Card> list, int k)
        {
            int n = list.Count;
            if (k > n || k <= 0) yield break;
            int[] indices = Enumerable.Range(0, k).ToArray();
            while (true)
            {
                var combo = indices.Select(i => list[i]).ToList();
                yield return combo;

                int pos = k - 1;
                while (pos >= 0 && indices[pos] == pos + n - k) pos--;
                if (pos < 0) break;
                indices[pos]++;
                for (int i = pos + 1; i < k; i++) indices[i] = indices[i - 1] + 1;
            }
        }
    }
}