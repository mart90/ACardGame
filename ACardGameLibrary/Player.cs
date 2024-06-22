namespace ACardGameLibrary
{
    public class Player
    {
        public Player(string name) 
        {
            Name = name;
            Life = 20;
            MoneyToSpend = 0;

            Deck = new List<Card>();
            DiscardPile = new List<Card>();
            CardsInPlay = new List<Card>();
            CardsToPickFrom = new List<Card>();
            CardsInHand = new List<Card>
            {
                CardLibrary.GetCard("Silver"),
                CardLibrary.GetCard("Silver")
            };
        }

        public string Name { get; set; }

        public int Life { get; set; }

        public bool IsActive { get; set; }

        public int MoneyToSpend { get; set; }

        public bool IsPickingCards { get; set; }
        public int AmountOfCardsToPick { get; set; }
        public List<Card> CardsToPickFrom { get; private set; }

        public List<Card> Deck { get; private set; }

        public List<Card> DiscardPile { get; private set; }

        public List<Card> CardsInPlay { get; private set; }

        public List<Card> CardsInHand { get; private set; }
    }
}
