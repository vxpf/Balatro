using System;
using System.Collections.Generic;
using System.Linq;

namespace KlasUitwerking
{
    // Detecteer pairs en geef bonuspunten
    static class PairChecker
    {
        // Geef bonuspunten voor pairs/trips/quads (standaard: pair +5, three of a kind +10, four of a kind +15)
        public static int GetPairBonus(IEnumerable<Card> hand, int pairBonus = 5)
        {
            if (hand == null) return 0;
            var list = hand.ToList();
            if (list.Count == 0) return 0;

            // Tel per rank (CardValue)
            var groups = list.GroupBy(c => c.Value);
            int total = 0;
            foreach (var g in groups)
            {
                int n = g.Count();
                if (n >= 2)
                {
                    total += (n - 1) * pairBonus;
                }
            }
            return total;
        }
    }
}
