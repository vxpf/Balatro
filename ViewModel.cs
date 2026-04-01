using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KlasUitwerking
{
    class ViewModel
    {
        private Model Model;
        private int DeckCardsTotal, DeckCardsRemaining = 0;
        private IEnumerable<Card> CardsInHand = new List<Card>();
        private IEnumerable<int> SelectedCards = new List<int>();
        public int Cursor { get; private set; } = 0;

        // view-facing properties
        public int DeckTotal => this.DeckCardsTotal;
        public int DeckRemaining => this.DeckCardsRemaining;
        public int Score => this.Model.PlayerHand.CalculateScore();
        public IEnumerable<Card> CardsInHandPublic => this.CardsInHand;
        public IEnumerable<int> SelectedCardsPublic => this.SelectedCards;
        private string Status = string.Empty;
        public string StatusPublic => this.Status;
        private bool Running = false;
        public ViewModel(Model model)
        {
            this.Model = model;
        }

        public void UpdateFromModel()
        {
            this.DeckCardsTotal = this.Model.Deck.TotalCardCount;
            this.DeckCardsRemaining = this.Model.Deck.CardsRemainingCount;
            this.CardsInHand = this.Model.PlayerHand.CardsInHand;
            this.SelectedCards = this.Model.PlayerHand.SelectedCards;
            // Ensure cursor remains within range
            int count = this.CardsInHand.Count();
            if (count == 0)
            {
                this.Cursor = 0;
            }
            else if (this.Cursor >= count)
            {
                this.Cursor = count - 1;
            }

            // Safety: als hand minder kaarten heeft dan MaxCards, vul automatisch aan vanuit deck
            int desired = this.Model.PlayerHand.MaxCards;
            while (this.Model.PlayerHand.CardsInHand.Count() < desired)
            {
                var drawn = this.Model.Deck.TakeCard();
                if (drawn == null) break;
                this.Model.PlayerHand.AddCard(drawn);
            }

            // refresh local view of hand/count after fill
            this.CardsInHand = this.Model.PlayerHand.CardsInHand;
            this.SelectedCards = this.Model.PlayerHand.SelectedCards;
        }
        // View-agnostic control methods for a ConsoleView or other view
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


        //actions
        public void SelectCard(int index)
        {
            this.Model.PlayerHand.SelectCard(index);
            this.UpdateFromModel();
        }

        // Wissel geselecteerde kaarten: verwijder geselecteerde en vul hand aan vanuit deck
        public void ReplaceSelected()
        {
            // verwijder geselecteerde kaarten uit hand
            this.Model.PlayerHand.RemoveSelected();

            this.Status = "Geselecteerde kaarten gewisseld.";

            // trek kaarten totdat hand weer vol is of deck leeg
            while (this.Model.PlayerHand.CardsInHand.Count() < this.Model.PlayerHand.MaxCards)
            {
                var drawn = this.Model.Deck.TakeCard();
                if (drawn == null) break;
                this.Model.PlayerHand.AddCard(drawn);
            }

            this.UpdateFromModel();
        }

        // request to start a new hand: reset deck, shuffle and deal
        public void DealNewHand()
        {
            this.Model.Deck.Reset();
            this.Model.Deck.Shuffle();

            // clear current hand and deal
            this.Model.PlayerHand = new PlayerHand(this.Model.PlayerHand.MaxCards);
            for (int i = 0; i < this.Model.PlayerHand.MaxCards; i++)
            {
                var c = this.Model.Deck.TakeCard();
                if (c == null) break;
                this.Model.PlayerHand.AddCard(c);
            }

            this.UpdateFromModel();
        }
    }
}