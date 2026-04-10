using System;
using System.Collections.Generic;
using System.Linq;

namespace KlasUitwerking
{
    // Helper class om te detecteren of een hand een Flush bevat
    // (alle kaarten dezelfde suit). Geeft bonuspunten voor flush.
    static class FlushChecker
    {
        // Geef bonuspunten voor flush; 0 als geen flush.
        public static int GetFlushBonus(IEnumerable<Card> hand)
        {
            if (hand == null) return 0;
            var list = hand.ToList();
            if (list.Count == 0) return 0;

            // Wildcard past bij elke suit; gebruik MatchesSuit voor centrale wildcard-regel
            var nonWild = list.Where(c => !(c is WildcardCard)).ToList();
            if (nonWild.Count == 0)
            {
                return 50;
            }

            var target = nonWild.First().Suit;
            bool allMatch = list.All(c => c.MatchesSuit(target));
            if (allMatch)
            {
                return 50;
            }
            return 0;
        }
    }
}
