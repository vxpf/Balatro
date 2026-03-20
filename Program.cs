using KlasUitwerking;

namespace KlasUitwerking
{
    class Program
    {
        static void Main(string[] args)
        {
            Deck testDeck = new Deck();
            PlayerHand hand = new PlayerHand(5);
            hand.AddCard(testDeck.TakeCard());
            hand.AddCard(testDeck.TakeCard());

            Model model = new Model(testDeck, hand);

            ViewModel viewModel = new ViewModel(model);
            viewModel.UpdateFromModel();

            viewModel.Run();
        }
    }
}