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
        private int Cursor = 0;
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
        }

        public void RenderUI()
        {
            Console.Clear();

            Console.WriteLine("Deck: "
                + this.DeckCardsRemaining.ToString()
                + "/"
                + this.DeckCardsTotal.ToString());

            for (int i = 0; i < this.CardsInHand.Count(); i++)
            {
                Card card = this.CardsInHand.ElementAt(i);
                // show cursor marker
                if (i == this.Cursor)
                {
                    Console.Write(">");
                }
                else
                {
                    Console.Write(" ");
                }

                if (this.SelectedCards.Contains(i))
                {
                    Console.Write("[x]");
                }
                else
                {
                    Console.Write("[ ]");
                }

                Console.WriteLine(card.MakeAsString());
            }
        }

        public void HandleUserInput()
        {
            ConsoleKeyInfo key = Console.ReadKey();

            // navigation with arrow keys
            if (key.Key == ConsoleKey.LeftArrow || key.Key == ConsoleKey.UpArrow)
            {
                if (this.CardsInHand.Any())
                {
                    this.Cursor = (this.Cursor - 1 + this.CardsInHand.Count()) % this.CardsInHand.Count();
                }
            }
            else if (key.Key == ConsoleKey.RightArrow || key.Key == ConsoleKey.DownArrow)
            {
                if (this.CardsInHand.Any())
                {
                    this.Cursor = (this.Cursor + 1) % this.CardsInHand.Count();
                }
            }
            else if (key.Key == ConsoleKey.Enter)
            {
                // toggle selection at cursor
                if (this.CardsInHand.Any())
                {
                    if (this.SelectedCards.Contains(this.Cursor))
                    {
                        this.Model.PlayerHand.DeselectCard(this.Cursor);
                    }
                    else
                    {
                        this.Model.PlayerHand.SelectCard(this.Cursor);
                    }
                    this.UpdateFromModel();
                }
            }
        }

        public void Run()
        {
            while (true)
            {
                this.RenderUI();
                this.HandleUserInput();
            }
        }


        //actions
        public void SelectCard(int index)
        {
            this.Model.PlayerHand.SelectCard(index);
            this.UpdateFromModel();
        }
    }
}