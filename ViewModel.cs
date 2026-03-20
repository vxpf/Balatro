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

            if (key.Key == ConsoleKey.Enter)
            {
                this.SelectCard(0);
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