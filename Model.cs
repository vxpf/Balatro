using System;
using System.Collections.Generic;
using System.Text;

namespace KlasUitwerking
{
    class Model
    {
        public Deck Deck;
        public PlayerHand PlayerHand;
        // Totale score van speler
        public int TotalScore { get; set; } = 0;

        public Model(Deck deck, PlayerHand hand)
        {
            this.Deck = deck;
            this.PlayerHand = hand;
        }
    }
}