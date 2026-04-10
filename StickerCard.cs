using System;
using System.Collections.Generic;
using System.Text;

namespace KlasUitwerking
{
    class StickerCard : CardDecorator
    {
        private readonly int stickerBonus;

        public StickerCard(Card card, int stickerBonus = 5) : base(card)
        {
            this.stickerBonus = stickerBonus;
        }

        public override int GetScore()
        {
            return decoratedCard.GetScore() + stickerBonus;
        }

        public override string MakeAsString()
        {
            return $"⭐{decoratedCard.MakeAsString()}";
        }
    }
}
