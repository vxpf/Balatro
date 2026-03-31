using System;
using System.Linq;

namespace KlasUitwerking
{
    // Simple console-based View that uses ViewModel for state and actions
    class ConsoleView
    {
        private ViewModel vm;
        private bool running = true;

        public ConsoleView(ViewModel viewModel)
        {
            this.vm = viewModel;
        }

        public void Run()
        {
            while (this.running)
            {
                Render();
                HandleInput();
            }
        }

        private void Render()
        {
            Console.Clear();
            Console.WriteLine("Deck: " + this.vm.DeckRemaining + "/" + this.vm.DeckTotal);
            Console.WriteLine("Score: " + this.vm.Score);
            Console.WriteLine("Controls: ←/→ bewegen, Enter selecteer/deselecteer, R wissel, N nieuwe hand, Q stop");

            var cards = this.vm.CardsInHandPublic.ToList();
            var selected = this.vm.SelectedCardsPublic.ToList();

            for (int i = 0; i < cards.Count; i++)
            {
                string marker = (i == this.vm.Cursor) ? ">" : " ";
                string sel = selected.Contains(i) ? "[x]" : "[ ]";
                var card = cards[i];
                string text = card.MakeAsString();
                if (card.GetBonusPoints() > 0) text += " [B]";
                Console.WriteLine($"{marker}{sel} {text}");
            }
        }

        private void HandleInput()
        {
            var key = Console.ReadKey(true);
            if (key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.UpArrow)
            {
                this.vm.MoveCursorLeft();
            }
            else if (key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.DownArrow)
            {
                this.vm.MoveCursorRight();
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                this.vm.ToggleSelectAtCursor();
            }
            else if (key.Key == ConsoleKey.R || key.Key == ConsoleKey.Spacebar)
            {
                this.vm.ReplaceSelected();
            }
            else if (key.Key == ConsoleKey.N)
            {
                this.vm.DealNewHand();
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
    }
}
