using System;
using System.Collections.Generic;
using System.Text;

namespace KlasUitwerking
{
    class BonusCard : Card
    {
        private int bonus;
        public BonusCard(CardValue value, Suit suit, int bonusPoints = 10)
            : base(value, suit)
        {
            this.bonus = bonusPoints;
        }

        public override int GetBonusPoints()
        {
            return this.bonus;
        }
    }
}
