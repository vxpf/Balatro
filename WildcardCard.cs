using System;
using System.Collections.Generic;
using System.Text;

namespace KlasUitwerking
{
    // Een kaart zonder suit: fungeert als wildcard.
    // Regels:
    // - Heeft geen vaste suit (kan iedere suit aannemen waar dat nodig is, bv. voor Flush).
    // - Geeft geen extra bonuspunten op zichzelf.
    // - Basis punten zijn 0 (maar kan worden aangepast als nodig).
    class WildcardCard : Card
    {
        public WildcardCard()
            : base(CardValue.Two, Suit.None)
        {
        }

        public override int GetBasePoints()
        {
            return 0;
        }

        public override int GetBonusPoints()
        {
            return 0;
        }

        public override string MakeAsString()
        {
            return "Wildcard";
        }

        public override bool MatchesSuit(Suit s)
        {
            return true;
        }
    }
}
