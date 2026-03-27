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

            PlayerHand hand = new PlayerHand(5);
            for (int i = 0; i < hand.MaxCards; i++)
            {
                var card = testDeck.TakeCard();
                if (card == null) break;
                hand.AddCard(card);
            }

            Model model = new Model(testDeck, hand);

            ViewModel viewModel = new ViewModel(model);
            viewModel.UpdateFromModel();

            viewModel.Run();
        }
    }
}