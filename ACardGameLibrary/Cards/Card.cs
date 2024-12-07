namespace ACardGameLibrary
{
    public class Card
    {
        public string Name { get; set; }

        public string Text { get; set; }

        public bool IsBuyable { get; set; }

        public int? Cost { get; set; }

        public int? AmountInShopPool { get; set; }

        public List<CardType> Types { get; set; }

        public List<CardEffect> Effects { get; set; }

        public Player Owner { get; set; }

        public Card()
        {
            Types = new List<CardType>();
            Effects = new List<CardEffect>();
        }

        public CardType GetMainType()
        {
            var subTypes = new List<CardType>()
            {
                CardType.Flying,
                CardType.Infantry,
                CardType.Cavalry,
                CardType.Ranged,
                CardType.Legendary,
                CardType.Siege
            };

            return Types.Single(e => !subTypes.Contains(e));
        }

        public CardType[] GetSubTypes()
        {
            var mainTypes = new List<CardType>() 
            { 
                CardType.Action,
                CardType.Creature,
                CardType.Currency,
                CardType.Leader,
                CardType.Support
            };

            return Types.Where(e => !mainTypes.Contains(e)).ToArray();
        }

        public Card Clone()
        {
            return (Card)MemberwiseClone();
        }
    }
}
