using System;
using System.Collections.Generic;
using System.Linq;

namespace KlasUitwerking
{
    // Centrale class voor evaluatie van kaartcombinaties
    static class CardEvaluator
    {
        // Evalueer hand en geef totale score terug
        public static int EvaluateHand(IEnumerable<Card> hand)
        {
            if (hand == null) return 0;
            var cards = hand.ToList();
            if (cards.Count == 0) return 0;

            // Basispunten van alle kaarten
            int score = cards.Sum(c => c.GetScore());
            
            // Context-bonussen (bijv. ExtraCard)
            score += cards.Sum(c => c.GetAdditionalBonus(cards));
            
            // Pair bonussen
            score += PairChecker.GetPairBonus(cards);
            
            // Flush bonus
            score += FlushChecker.GetFlushBonus(cards);
            
            // Glass multipliers
            double multiplier = 1.0;
            foreach (var g in cards.OfType<GlassCard>())
            {
                multiplier *= g.Multiplier;
            }
            
            return (int)Math.Round(score * multiplier);
        }

        // Vind beste 5-kaart combinatie uit hand
        public static int EvaluateBestCombination(IEnumerable<Card> hand)
        {
            var cards = hand.ToList();
            if (cards.Count < 5) return EvaluateHand(cards);

            int best = int.MinValue;
            foreach (var combo in GetCombinations(cards, 5))
            {
                int score = EvaluateHand(combo);
                if (score > best) best = score;
            }
            
            return best == int.MinValue ? 0 : best;
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
