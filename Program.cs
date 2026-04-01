using KlasUitwerking;

namespace KlasUitwerking
{
    class Program
    {
        static void Main(string[] args)
        {
            Deck testDeck = new Deck();
            // shuffle deck and deal a full hand
            testDeck.Shuffle();

            // speler heeft nu 8 kaarten in de hand
            PlayerHand hand = new PlayerHand(8);
            for (int i = 0; i < hand.MaxCards; i++)
            {
                var card = testDeck.TakeCard();
                if (card == null) break;
                hand.AddCard(card);
            }

            Model model = new Model(testDeck, hand);

            ViewModel viewModel = new ViewModel(model);
            viewModel.UpdateFromModel();

            // start the ViewModel which now handles console I/O
            viewModel.Run();
        }
    }
}