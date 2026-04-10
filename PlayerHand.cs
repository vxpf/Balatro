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
            // Valideer index
            if (index < 0 || index >= this.Hand.Count)
            {
                return;
            }

            // Voeg toe als nog niet geselecteerd
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

        // Bereken totale handscore: som van kaartwaarden plus bonuspunten
        public int CalculateScore()
        {
            // Bij 5+ kaarten: evalueer alle 5-kaart combinaties en geef beste score
            var cards = this.Hand.ToList();
            if (cards.Count >= 5)
            {
                int best = int.MinValue;
                foreach (var combo in GetCombinations(cards, 5))
                {
                    int s = combo.Sum(c => c.GetScore());
                    // Voeg context-bonussen toe (bijv. ExtraCard)
                    s += combo.Sum(c => c.GetAdditionalBonus(combo));
                    // Voeg pair bonussen toe
                    s += PairChecker.GetPairBonus(combo);
                    s += FlushChecker.GetFlushBonus(combo);
                    // Pas glass multipliers toe indien aanwezig
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

            // Anders: som alle kaartscores en mogelijke flush bonus
            int baseTotal = cards.Sum(c => c.GetScore());
            baseTotal += cards.Sum(c => c.GetAdditionalBonus(cards));
            baseTotal += FlushChecker.GetFlushBonus(cards);
            // Voeg pair bonussen toe voor hele hand
            baseTotal += PairChecker.GetPairBonus(cards);
            double multiplierTotal = 1.0;
            foreach (var g in cards.OfType<GlassCard>()) multiplierTotal *= g.Multiplier;
            return (int)Math.Round(baseTotal * multiplierTotal);
        }

        // Genereer alle k-combinaties van een lijst
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