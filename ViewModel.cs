using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KlasUitwerking
{
    class ViewModel
    {
        private Model Model;
        private GameEngine Engine;

        private int DeckCardsTotal, DeckCardsRemaining = 0;
        private IEnumerable<Card> CardsInHand = new List<Card>();
        private IEnumerable<int> SelectedCards = new List<int>();
        public int Cursor { get; private set; } = 0;

        // View properties
        public int DeckTotal => this.DeckCardsTotal;
        public int DeckRemaining => this.DeckCardsRemaining;
        public int Score => this.Model.PlayerHand.CalculateScore();
        public int TotalScore => this.Model.TotalScore;
        public IEnumerable<Card> CardsInHandPublic => this.CardsInHand;
        public IEnumerable<int> SelectedCardsPublic => this.SelectedCards;
        public int WildcardsInDeck => this.Model.Deck.CountWildcardsRemaining();
        private string Status = string.Empty;
        public string StatusPublic => this.Status;

        public ViewModel(Model model)
        {
            this.Model = model ?? throw new ArgumentNullException(nameof(model));
            this.Engine = new GameEngine(model);
        }

        public void UpdateFromModel()
        {
            this.DeckCardsTotal = this.Model.Deck.TotalCardCount;
            this.DeckCardsRemaining = this.Model.Deck.CardsRemainingCount;
            this.CardsInHand = this.Model.PlayerHand.CardsInHand;
            this.SelectedCards = this.Model.PlayerHand.SelectedCards;

            // Safety: als hand minder kaarten heeft dan MaxCards, vul automatisch aan vanuit deck
            int desired = this.Model.PlayerHand.MaxCards;
            while (this.Model.PlayerHand.CardsInHand.Count() < desired)
            {
                var drawn = this.Model.Deck.TakeCard();
                if (drawn == null) break;
                this.Model.PlayerHand.AddCard(drawn);
            }

            this.CardsInHand = this.Model.PlayerHand.CardsInHand;
            this.SelectedCards = this.Model.PlayerHand.SelectedCards;
        }

        public void MoveCursorLeft()
        {
            if (this.CardsInHand.Any())
            {
                this.Cursor = (this.Cursor - 1 + this.CardsInHand.Count()) % this.CardsInHand.Count();
            }
        }

        public void MoveCursorRight()
        {
            if (this.CardsInHand.Any())
            {
                this.Cursor = (this.Cursor + 1) % this.CardsInHand.Count();
            }
        }

        public void ToggleSelectAtCursor()
        {
            if (this.CardsInHand.Any())
            {
                if (this.SelectedCards.Contains(this.Cursor))
                {
                    this.Model.PlayerHand.DeselectCard(this.Cursor);
                    this.Status = $"Gedeselecteerd kaart {this.Cursor + 1}.";
                }
                else
                {
                    this.Model.PlayerHand.SelectCard(this.Cursor);
                    this.Status = $"Geselecteerd kaart {this.Cursor + 1}.";
                }
                this.UpdateFromModel();
            }
        }

        public void ReplaceSelectedCards()
        {
            this.Status = this.Engine.ReplaceSelectedCards();
            this.UpdateFromModel();
        }

        public void DealNewHand()
        {
            this.Engine.DealNewHand();
            this.Status = "Nieuwe hand gedeeld.";
            this.UpdateFromModel();
        }

        public void BankCurrentHand()
        {
            this.Status = this.Engine.BankCurrentHand();
            this.UpdateFromModel();
        }
    }
}