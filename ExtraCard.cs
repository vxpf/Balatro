using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KlasUitwerking
{
    // ExtraCard: geeft extra punten per kaart met dezelfde rank in de hand.
    // Bijvoorbeeld: +2 punten per extra kaart met dezelfde rank (exclusief zichzelf).
    class ExtraCard : Card
    {
        private readonly int pointsPerMatch;

        public ExtraCard(CardValue value, Suit suit, int pointsPerMatch = 2)
            : base(value, suit)
        {
            this.pointsPerMatch = pointsPerMatch;
        }

        public override string MakeAsString()
        {
            return $"Extra {RankString()}{SuitSymbol()} +{this.pointsPerMatch}/match";
        }

        // Context-aware bonus: telt het aantal andere kaarten in the hand with
        // dezelfde CardValue en geeft pointsPerMatch per match.
        public override int GetAdditionalBonus(IEnumerable<Card> context)
        {
            if (context == null) return 0;
            int matches = context.Count(c => c != this && c.Value == this.Value);
            return matches * this.pointsPerMatch;
        }
    }
}
