using System;
using System.Collections.Generic;
using System.Text;

namespace KlasUitwerking
{
    // Concrete regular card because Card is abstract now
    class RegularCard : Card
    {
        public RegularCard(CardValue value, Suit suit)
            : base(value, suit)
        {
        }
    }
}
