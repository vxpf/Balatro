using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KlasUitwerking
{
    class ViewModel
    {
        private Model Model;
        private Random rng = new Random();
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
        // Simple console view integrated in ViewModel (teacher prefers ViewModel-only)
        public void Run()
        {
            this.Running = true;
            while (this.Running)
            {
                RenderUI();
                HandleUserInput();
            }
        }

        public void RenderUI()
        {
            Console.Clear();
            Console.WriteLine("Deck: " + this.DeckRemaining + "/" + this.DeckTotal + "    " + "Score: " + this.Score);
            // toon status en wildcard telling
            int wildInHand = this.CardsInHand.Count(c => c is WildcardCard);
            int wildInDeck = this.Model.Deck.CountWildcardsRemaining();
            Console.WriteLine(this.StatusPublic + "    Wildcards hand:" + wildInHand + " Deck:" + wildInDeck);
            Console.WriteLine("Controls: ←/→ bewegen, Enter selecteer/deselecteer, R wissel, N nieuwe hand, Q stop");

            var cards = this.CardsInHand.ToList();
            var selected = this.SelectedCards.ToList();

            for (int i = 0; i < cards.Count; i++)
            {
                // toon index (1-based) voor duidelijkheid
                string idx = (i + 1).ToString().PadLeft(2, ' ');
                string marker = (i == this.Cursor) ? ">" : " ";
                string sel = selected.Contains(i) ? "[x]" : "[ ]";
                var card = cards[i];
                string text = card.MakeAsString();
                if (card is WildcardCard)
                {
                    text += " [W]"; // eenvoudige markering voor wildcard
                }
                else if (card is GlassCard g)
                {
                    // markeer glazen kaart en toon multiplier
                    text += $" [G x{g.Multiplier}]";
                }
                else if (card is ExtraCard)
                {
                    text += " [E]"; // ExtraCard marker
                }
                else if (card.GetBonusPoints() > 0)
                {
                    text += " [B]";
                }
                Console.WriteLine($"{idx} {marker}{sel} {text}");
            }
        }

        public void HandleUserInput()
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.UpArrow)
            {
                this.MoveCursorLeft();
            }
            else if (key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.DownArrow)
            {
                this.MoveCursorRight();
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                this.ToggleSelectAtCursor();
            }
            else if (key.Key == ConsoleKey.R || key.Key == ConsoleKey.Spacebar)
            {
                this.ReplaceSelected();
            }
            else if (key.Key == ConsoleKey.N)
            {
                this.DealNewHand();
            }
            else if (key.Key == ConsoleKey.Q)
            {
                Console.Write("Weet je zeker dat je wilt stoppen? (y/n): ");
                while (true)
                {
                    var k = Console.ReadKey(true);
                    if (k.Key == ConsoleKey.Y) { this.Running = false; break; }
                    if (k.Key == ConsoleKey.N) { break; }
                }
            }
        }

        // View-agnostic control methods
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
            // Controleer geselecteerde kaarten op GlassCard breuk voordat we ze verwijderen.
            var selectedCards = this.Model.PlayerHand.GetSelected();
            foreach (var card in selectedCards)
            {
                if (card is GlassCard g)
                {
                    bool broken = g.TryBreak(this.rng);
                    if (broken)
                    {
                        // Verwijder de gebroken kaart permanent uit het deck/taken lijsten
                        this.Model.Deck.RemoveCard(card);
                        this.Status = $"Glazen kaart gebroken: {card.MakeAsString()} weggegooid.";
                    }
                }
            }

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