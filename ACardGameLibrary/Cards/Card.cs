namespace ACardGameLibrary
{
    public class Card
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public bool IsInShopPool { get; set; }

        public int? Cost { get; set; }

        public int? AmountInShopPool { get; set; }

        public List<CardType> Types { get; set; }

        public List<CardEffect> Effects { get; set; }

        public Player? Owner { get; set; }

        public bool TargetsOnPlay { get; set; }

        public int MaxTargets { get; set; }
        public int MinTargets { get; set; }
        public bool MustTargetEnemy { get; set; }
        public bool MustTargetFriend { get; set; }
        public List<CardType> ValidTargetTypes { get; set; }
        public Func<GameStateManager, Card, bool> AdditionalTargetConditions { get; set; }
        public Func<GameStateManager, bool> AdditionalPlayConditions { get; set; }

        public bool IsBeingPlayed { get; set; }
        public bool IsBeingMovedToDiscard { get; set; }

        public bool IsTargeted { get; set; }
        public bool IsTargeting { get; set; }

        public bool TargetsHand { get; set; }
        public bool TargetsShop { get; set; }

        public int Counters { get; set; }

        public bool IsCombatCard => this is CreatureCard || this is SupportCard;

        public Card()
        {
            Types = new List<CardType>();
            ValidTargetTypes = new List<CardType>();
            Effects = new List<CardEffect>();
            MaxTargets = 1;
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
            Card card = (Card)MemberwiseClone();

            card.Effects = new List<CardEffect>();

            if (card is CreatureCard creature)
            {
                creature.BlockedBy = new List<CreatureCard>();
                creature.AttachedEquipments = new List<Equipment>();
            }

            return card;
        }

        public virtual void AddEffects(List<CardEffect> effects) 
        {
            Effects.AddRange(effects);
        }
    }
}
