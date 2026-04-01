using System;
using System.Collections.Generic;
using System.Text;

namespace KlasUitwerking
{
    class Model
    {
        public Deck Deck;
        public PlayerHand PlayerHand;
        // totaal score van de speler; start op 0
        public int TotalScore { get; set; } = 0;

        public Model(Deck deck, PlayerHand hand)
        {
            this.Deck = deck;
            this.PlayerHand = hand;
        }
    }
}