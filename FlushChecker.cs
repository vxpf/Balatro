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

            var firstSuit = list.First().Suit;
            bool allSame = list.All(c => c.Suit == firstSuit);
            if (allSame)
            {
                // eenvoudige vaste bonus voor flush
                return 50;
            }
            return 0;
        }
    }
}
