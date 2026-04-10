using System;
using System.Collections.Generic;
using System.Text;

namespace KlasUitwerking
{
    abstract class CardDecorator : Card
    {
        protected Card decoratedCard;

        public CardDecorator(Card card) : base(card.Value, card.Suit)
        {
            this.decoratedCard = card;
        }

        public override int GetBasePoints()
        {
            return decoratedCard.GetBasePoints();
        }

        public override int GetBonusPoints()
        {
            return decoratedCard.GetBonusPoints();
        }

        public override int GetScore()
        {
            return decoratedCard.GetScore();
        }

        public override int GetAdditionalBonus(IEnumerable<Card> context)
        {
            return decoratedCard.GetAdditionalBonus(context);
        }

        public override bool MatchesSuit(Suit s)
        {
            return decoratedCard.MatchesSuit(s);
        }
    }
}
