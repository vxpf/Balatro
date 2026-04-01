using System;
using System.Collections.Generic;
using System.Linq;

namespace KlasUitwerking
{
    // Helper to detect pairs and award bonus points
    static class PairChecker
    {
        // Returns bonus points for pairs/trips/quads in the given hand
        // Default: +5 points for a pair (2 of a kind), +10 for three of a kind, +15 for four of a kind
        // Implemented as (count - 1) * pairBonus, where pairBonus defaults to 5
        public static int GetPairBonus(IEnumerable<Card> hand, int pairBonus = 5)
        {
            if (hand == null) return 0;
            var list = hand.ToList();
            if (list.Count == 0) return 0;

            // Count by rank (CardValue)
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
