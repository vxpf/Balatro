using System;
using System.Collections.Generic;
using System.Linq;

namespace KlasUitwerking
{
    class ConsoleView
    {
        private ViewModel viewModel;
        private bool running = false;

        public ConsoleView(ViewModel vm)
        {
            this.viewModel = vm ?? throw new ArgumentNullException(nameof(vm));
        }

        public void Run()
        {
            this.running = true;
            while (this.running)
            {
                RenderUI();
                HandleUserInput();
            }
        }

        private void RenderUI()
        {
            Console.Clear();
            Console.WriteLine($"Deck: {viewModel.DeckRemaining}/{viewModel.DeckTotal}   Hand: {viewModel.Score}   Total: {viewModel.TotalScore}");
            int wildInHand = viewModel.CardsInHandPublic.Count(c => c is WildcardCard);
            int wildInDeck = viewModel.WildcardsInDeck;
            Console.WriteLine($"Wildcards hand:{wildInHand}  deck:{wildInDeck}  selected:{viewModel.SelectedCardsPublic.Count()}");
            Console.WriteLine("Controls: ←/→  Enter select  R wissel  S bank  N new  ? help  Q quit");
            Console.WriteLine();

            var cards = viewModel.CardsInHandPublic.ToList();
            var selected = viewModel.SelectedCardsPublic.ToList();
            for (int i = 0; i < cards.Count; i++)
            {
                var card = cards[i];
                bool isCursor = (i == viewModel.Cursor);
                bool isSelected = selected.Contains(i);
                PrintCardLine(i, card, isSelected, isCursor);
            }
            Console.WriteLine();
            Console.WriteLine(viewModel.StatusPublic);
        }

        private void PrintCardLine(int index, Card card, bool selected, bool isCursor)
        {
            string idx = (index + 1).ToString().PadLeft(2, ' ');
            string cursor = isCursor ? ">" : " ";
            string sel = selected ? "[x]" : "[ ]";

            string cardText = card is WildcardCard ? "*" : card.MakeAsString();

            string markers = "";
            if (card is GlassCard) markers += " G";
            if (card is ExtraCard) markers += " E";
            if (card.GetBonusPoints() > 0) markers += " B";
            if (card is WildcardCard) markers += " W";

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

            Console.WriteLine(markers);
        }

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

        private void HandleUserInput()
        {
            var key = Console.ReadKey(true);
            if (key.KeyChar == '?')
            {
                ShowHelp();
                return;
            }
            if (key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.UpArrow)
            {
                viewModel.MoveCursorLeft();
            }
            else if (key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.DownArrow)
            {
                viewModel.MoveCursorRight();
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                viewModel.ToggleSelectAtCursor();
            }
            else if (key.Key == ConsoleKey.R || key.Key == ConsoleKey.Spacebar)
            {
                viewModel.ReplaceSelectedCards();
            }
            else if (key.Key == ConsoleKey.N)
            {
                viewModel.DealNewHand();
            }
            else if (key.Key == ConsoleKey.S)
            {
                viewModel.BankCurrentHand();
            }
            else if (key.Key == ConsoleKey.Q)
            {
                Console.Write("Weet je zeker dat je wilt stoppen? (y/n): ");
                while (true)
                {
                    var k = Console.ReadKey(true);
                    if (k.Key == ConsoleKey.Y) { this.running = false; break; }
                    if (k.Key == ConsoleKey.N) { break; }
                }
            }
        }

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
        }
    }
}
