using System;
using System.Collections.Generic;
using System.Text;

namespace KlasUitwerking
{
    class GlassCard : Card
    {
        private readonly double multiplier;
        private readonly double breakChance;

        public GlassCard(CardValue value, Suit suit, double multiplier = 2.0, double breakChance = 0.2)
            : base(value, suit)
        {
            this.multiplier = multiplier;
            this.breakChance = breakChance;
        }

        public double Multiplier => this.multiplier;

        public bool TryBreak(Random rng)
        {
            if (rng == null) rng = new Random();
            return rng.NextDouble() < this.breakChance;
        }

        public override string MakeAsString()
        {
            return $"Glass {this.Suit} {this.Value} x{this.multiplier}";
        }
    }
}
