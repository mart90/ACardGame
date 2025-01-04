namespace ACardGameLibrary
{
    public class CreatureCard : Card
    {
        public CreatureCard()
        {
            AttachedEquipments = new List<Equipment>();
            BlockedBy = new List<CreatureCard>();
        }

        public int Power { get; set; }
        public int Defense { get; set; }
        public int TemporaryAddedPower { get; set; }
        public int TemporaryAddedDefense { get; set; }
        public bool HasTrample { get; set; }

        public bool IsUnblockable { get; set; }
        public bool IsCopy { get; set; }
        public bool HasTaunt { get; set; }
        public bool DealtDamage { get; set; }
        public bool IsUnplayable { get; set; }

        public List<Equipment> AttachedEquipments { get; set; }

        public List<CreatureCard> BlockedBy { get; set; }

        public override void AddEffects(List<CardEffect> effects)
        {
            base.AddEffects(effects);

            Effects.Add(new CardEffect
            {
                EffectPhase = CardEffectPhase.OnAccepted,
                Effect = (game, owner) =>
                {
                    var target = (CreatureCard)game.TargetedCards.FirstOrDefault();

                    if (target != null)
                    {
                        target.BlockedBy.Add((CreatureCard)game.TargetingCard);
                    }
                }
            });
        }

        public void Reset()
        {
            IsUnblockable = false;
            IsUnplayable = false;
            DealtDamage = false;

            BlockedBy.Clear();
            AttachedEquipments.Clear();

            TemporaryAddedPower = 0;
            TemporaryAddedDefense = 0;

        }
    }
}
