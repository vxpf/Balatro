using System;
using System.Collections.Generic;
using System.Linq;

namespace KlasUitwerking
{
    // GameEngine bevat alle spellogica en manipuleert het Model (deck/hand/score)
    class GameEngine
    {
        private readonly Model model;
        private readonly Random rng;

        public GameEngine(Model model, Random? rng = null)
        {
            this.model = model ?? throw new ArgumentNullException(nameof(model));
            this.rng = rng ?? new Random();
        }

        // Vervang geselecteerde kaarten: behandel GlassCard breuk, verwijder uit deck, vul hand aan
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

            // Verwijder geselecteerde kaarten uit hand
            this.model.PlayerHand.RemoveSelected();

            status += "Geselecteerde kaarten gewisseld.";

            // Vul hand aan vanuit deck
            while (this.model.PlayerHand.CardsInHand.Count() < this.model.PlayerHand.MaxCards)
            {
                var drawn = this.model.Deck.TakeCard();
                if (drawn == null) break;
                this.model.PlayerHand.AddCard(drawn);
            }

            return status;
        }

        // Voeg handscore toe aan totale score en deel nieuwe hand
        public string BankCurrentHand()
        {
            int s = this.model.PlayerHand.CalculateScore();
            this.model.TotalScore += s;
            string status = $"Hand gescoord: {s} punten toegevoegd. Totale score: {this.model.TotalScore}.";

            // Maak hand leeg en deel nieuwe kaarten
            this.model.PlayerHand = new PlayerHand(this.model.PlayerHand.MaxCards);
            while (this.model.PlayerHand.CardsInHand.Count() < this.model.PlayerHand.MaxCards)
            {
                var c = this.model.Deck.TakeCard();
                if (c == null) break;
                this.model.PlayerHand.AddCard(c);
            }

            return status;
        }

        // Reset deck, schud en deel nieuwe hand
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
