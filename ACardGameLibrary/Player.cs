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
            Hand = new List<Card>
            {
                CardLibrary.GetCard("Swordsman"),
                CardLibrary.GetCard("Spearman"),
                CardLibrary.GetCard("Rout"),
                CardLibrary.GetCard("Shield"),
                CardLibrary.GetCard("Silver"),
                CardLibrary.GetCard("Silver"),
                CardLibrary.GetCard("Spearman"),
                CardLibrary.GetCard("Spearman"),
                CardLibrary.GetCard("Spearman"),
                CardLibrary.GetCard("Spearman")
            };

            foreach (var card in Hand)
            {
                card.Owner = this;
            }
        }

        public string Name { get; set; }

        public int Life { get; set; }

        public bool IsActive { get; set; }

        public int MoneyToSpend { get; set; }

        public bool IsPassed { get; set; }
        public bool IsPassedCombat { get; set; }

        public bool IsAttacking { get; set; }
        public bool HasAttackedThisRound { get; set; }

        public List<Card> Deck { get; private set; }
        public List<Card> DiscardPile { get; private set; }
        public List<Card> CardsInPlay { get; private set; }
        public List<Card> Hand { get; private set; }

        public Card Leader { get; set; }
    }
}
