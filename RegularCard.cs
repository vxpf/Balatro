using System;
using System.Collections.Generic;
using System.Text;

namespace KlasUitwerking
{
    // Standaard kaart (Card is abstract)
    class RegularCard : Card
    {
        public RegularCard(CardValue value, Suit suit)
            : base(value, suit)
        {
        }
    }
}
