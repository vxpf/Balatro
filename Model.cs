using System;
using System.Collections.Generic;
using System.Text;

namespace KlasUitwerking
{
    class Model
    {
        public Deck Deck;
        public PlayerHand PlayerHand;

        public Model(Deck deck, PlayerHand hand)
        {
            this.Deck = deck;
            this.PlayerHand = hand;
        }
    }
}