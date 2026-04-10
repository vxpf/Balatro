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
        public IEnumerable<Card> CardsInHandPublic => this.CardsInHand;
        public IEnumerable<int> SelectedCardsPublic => this.SelectedCards;
        private string Status = string.Empty;
        public string StatusPublic => this.Status;
        private bool Running = false;
        public ViewModel(Model model)
        {
            this.Model = model;
            this.Engine = new GameEngine(model);
        }

        public void UpdateFromModel()
        {
            this.DeckCardsTotal = this.Model.Deck.TotalCardCount;
            this.DeckCardsRemaining = this.Model.Deck.CardsRemainingCount;
            this.CardsInHand = this.Model.PlayerHand.CardsInHand;
            this.SelectedCards = this.Model.PlayerHand.SelectedCards;
            // Houd cursor binnen bereik
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

            // Ververs lokale view na aanvullen
            this.CardsInHand = this.Model.PlayerHand.CardsInHand;
            this.SelectedCards = this.Model.PlayerHand.SelectedCards;
        }
        // Console view geïntegreerd in ViewModel
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
            // Compacte UI
            Console.Clear();
            Console.WriteLine($"Deck: {this.DeckRemaining}/{this.DeckTotal}   Hand: {this.Score}   Total: {this.Model.TotalScore}");
            int wildInHand = this.CardsInHand.Count(c => c is WildcardCard);
            int wildInDeck = this.Model.Deck.CountWildcardsRemaining();
            Console.WriteLine($"Wildcards hand:{wildInHand}  deck:{wildInDeck}  selected:{this.SelectedCards.Count()}");
            Console.WriteLine("Controls: ←/→  Enter select  R wissel  S bank  N new  ? help  Q quit");
            Console.WriteLine();

            var cards = this.CardsInHand.ToList();
            var selected = this.SelectedCards.ToList();
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                bool isCursor = (i == this.Cursor);
                bool isSelected = selected.Contains(i);
                PrintCardLine(i, card, isSelected, isCursor);
            }
            Console.WriteLine();
            Console.WriteLine(this.StatusPublic);
        }

        // Print kaartlijn met index, selectie, cursor en type markers
        private void PrintCardLine(int index, Card card, bool selected, bool isCursor)
        {
            // Index (1-gebaseerd)
            string idx = (index + 1).ToString().PadLeft(2, ' ');
            string cursor = isCursor ? ">" : " ";
            string sel = selected ? "[x]" : "[ ]";

            // Kaarttekst (wildcard als '*')
            string cardText = card is WildcardCard ? "*" : card.MakeAsString();

            // Bepaal compacte markers
            string markers = "";
            if (card is GlassCard) markers += " G"; // Glas kaart
            if (card is ExtraCard) markers += " E"; // Extra kaart
            if (card.GetBonusPoints() > 0) markers += " B"; // Bonuspunten
            if (card is WildcardCard) markers += " W"; // Wildcard

            // Print kaart met gekleurde suit
            Console.Write($"{idx} {cursor}{sel} ");
            if (card is WildcardCard)
            {
                Console.Write(cardText);
            }
            else
            {
                var prev = Console.ForegroundColor;
                Console.ForegroundColor = SuitColor(card.Suit);
                Console.Write(cardText);
                Console.ForegroundColor = prev;
            }

            // Compacte markers
            Console.WriteLine(markers);
        }

        // Map suit naar console kleur
        private ConsoleColor SuitColor(Suit s)
        {
            switch (s)
            {
                case Suit.Hearts:
                case Suit.Diamonds:
                    return ConsoleColor.Red;
                case Suit.Spades:
                case Suit.Clubs:
                default:
                    return ConsoleColor.White;
            }
        }

        public void HandleUserInput()
        {
            var key = Console.ReadKey(true);
            // Toon help bij '?' toets
            if (key.KeyChar == '?')
            {
                ShowHelp();
                return;
            }
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
                // Delegeer naar engine en toon status
                this.Status = this.Engine.ReplaceSelectedCards();
                this.UpdateFromModel();
            }
            else if (key.Key == ConsoleKey.N)
            {
                this.Engine.DealNewHand();
                this.Status = "Nieuwe hand gedeeld.";
                this.UpdateFromModel();
            }
            else if (key.Key == ConsoleKey.S)
            {
                this.Status = this.Engine.BankCurrentHand();
                this.UpdateFromModel();
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

        // Besturingsmethodes
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

        // Toon help scherm met spelregels en kaartgedrag
        private void ShowHelp()
        {
            Console.Clear();
            Console.WriteLine("Speluitleg - Balatro (korte handleiding):\n");
            Console.WriteLine("Doel: bouw een zo hoog mogelijke handscore met de kaarten in je hand.");
            Console.WriteLine("Bediening:");
            Console.WriteLine(" - Gebruik ←/→ of ↑/↓ om te navigeren, Enter om kaarten te selecteren/deselecteren.");
            Console.WriteLine(" - R of spatie: wissel geselecteerde kaarten (trek nieuwe van deck).");
            Console.WriteLine(" - N: nieuwe hand (reset deck en deel opnieuw).");
            Console.WriteLine(" - S: bankeer huidige handscore naar je totale score.");
            Console.WriteLine(" - ?: toon deze help (nu).\n");

            Console.WriteLine("Kaartsoorten en wat ze doen:");
            Console.WriteLine(" - RegularCard: standaard kaart met basispunten gelijk aan de kaartwaarde (2..A).");
            Console.WriteLine(" - BonusCard: geeft extra vaste bonuspunten (zie [B] in UI). GetBonusPoints() retourneert die bonus.");
            Console.WriteLine("   Voorbeeld: BonusCard met +10 geeft altijd +10 bovenop zijn basispunten.");
            Console.WriteLine(" - ExtraCard: geeft extra punten gebaseerd op andere kaarten in je hand (zie [E] in UI).\n   Implementatie: +X punten per andere kaart met dezelfde rank.");
            Console.WriteLine(" - GlassCard: vermenigvuldigt de (deel)score met een multiplier (zie [G xN] in UI).\n   Let op: bij sommige acties kan een GlassCard breken en dan wordt hij permanent uit het deck verwijderd.");
            Console.WriteLine("   In deze implementatie: glazen kaarten kunnen breken wanneer je geselecteerde kaarten wisselt; als ze breken verdwijnen ze.");
            Console.WriteLine(" - Wildcard: heeft geen suit en telt als elke suit voor combinaties zoals Flush (zie [W] in UI). Geef zelf geen basispunten.");

            Console.WriteLine("Scoreberekening (kort):");
            Console.WriteLine(" - Handscore = som van basispunten (kaartwaarden) + bonuspunten (BonusCard) + context-bonussen (ExtraCard) + flush-bonus.");
            Console.WriteLine(" - Als er één of meerdere GlassCards in de combinatie/hand zitten, wordt de score vermenigvuldigd met het product van hun multipliers.");
            Console.WriteLine(" - Voor handen met >=5 kaarten wordt de beste 5-kaart combinatie gekozen (incl. bonussen en multipliers).");

            Console.WriteLine("Voorbeeldinteracties:");
            Console.WriteLine(" - Selecteer 2 kaarten en druk R om ze te wisselen; als een geselecteerde GlassCard breekt, verdwijnt hij permanent.");
            Console.WriteLine(" - Druk S om je huidige handscore op te tellen bij je totaalscore en meteen een nieuwe hand te krijgen.");

            Console.WriteLine("Druk een toets om terug te keren naar het spel...");
            Console.ReadKey(true);
            this.UpdateFromModel();
        }


        public void SelectCard(int index)
        {
            this.Model.PlayerHand.SelectCard(index);
            this.UpdateFromModel();
        }

    }
}