namespace ACardGameLibrary
{
    public class Card
    {
        public string Name { get; set; }

        public string Text { get; set; }

        public bool IsBuyable { get; set; }

        public int? Cost { get; set; }

        public int? AmountInShop { get; set; }

        public List<CardType> Types { get; set; }

        public List<CardEffect> Effects { get; set; }
    }
}
