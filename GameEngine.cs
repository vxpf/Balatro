using System;
using System.Collections.Generic;
using System.Linq;

namespace KlasUitwerking
{
    // GameEngine contains all game logic and manipulates the Model (deck/hand/score).
    // This keeps business rules out of the ViewModel and centralizes RNG and rule changes.
    class GameEngine
    {
        private readonly Model model;
        private readonly Random rng;

        public GameEngine(Model model, Random? rng = null)
        {
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            this.rng = rng ?? new Random();
        }

        // Replace selected cards: handle GlassCard breakage, remove broken cards from deck,
        // remove selected from hand and refill from deck. Returns a status message.
        public string ReplaceSelectedCards()
        {
            var selected = this.model.PlayerHand.GetSelected();
            string status = string.Empty;
            foreach (var card in selected)
            {
                if (card is GlassCard g)
                {
                    bool broken = g.TryBreak(this.rng);
                    if (broken)
                    {
                        this.model.Deck.RemoveCard(card);
                        status += $"Glazen kaart gebroken: {card.MakeAsString()}. ";
                    }
                }
            }

            // verwijder geselecteerde kaarten uit hand
            this.model.PlayerHand.RemoveSelected();

            status += "Geselecteerde kaarten gewisseld.";

            // vul hand aan vanuit deck
            while (this.model.PlayerHand.CardsInHand.Count() < this.model.PlayerHand.MaxCards)
            {
                var drawn = this.model.Deck.TakeCard();
                if (drawn == null) break;
                this.model.PlayerHand.AddCard(drawn);
            }

            return status;
        }

        // Bank the current hand score into total score and deal a fresh hand. Returns status.
        public string BankCurrentHand()
        {
            int s = this.model.PlayerHand.CalculateScore();
            this.model.TotalScore += s;
            string status = $"Hand gescoord: {s} punten toegevoegd. Totale score: {this.model.TotalScore}.";

            // clear current hand and deal
            this.model.PlayerHand = new PlayerHand(this.model.PlayerHand.MaxCards);
            while (this.model.PlayerHand.CardsInHand.Count() < this.model.PlayerHand.MaxCards)
            {
                var c = this.model.Deck.TakeCard();
                if (c == null) break;
                this.model.PlayerHand.AddCard(c);
            }

            return status;
        }

        // Reset deck, shuffle and deal a fresh hand
        public void DealNewHand()
        {
            this.model.Deck.Reset();
            this.model.Deck.Shuffle();
            this.model.PlayerHand = new PlayerHand(this.model.PlayerHand.MaxCards);
            for (int i = 0; i < this.model.PlayerHand.MaxCards; i++)
            {
                var c = this.model.Deck.TakeCard();
                if (c == null) break;
                this.model.PlayerHand.AddCard(c);
            }
        }
    }
}
