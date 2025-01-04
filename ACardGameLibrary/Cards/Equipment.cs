namespace ACardGameLibrary
{
    public class Equipment : SupportCard
    {
        public Equipment()
        {
            TargetsOnPlay = true;
            IsPermanent = true;
            MinTargets = 1;
            ValidTargetTypes = new List<CardType> 
            {
                CardType.Creature 
            };
        }

        public int AddedPower { get; set; }

        public int AddedDefense { get; set; }

        public override void AddEffects(List<CardEffect> effects)
        {
            base.AddEffects(effects);

            Effects.Add(new CardEffect
            {
                EffectPhase = CardEffectPhase.OnAccepted,
                Effect = (game, owner) =>
                {
                    var target = (CreatureCard)game.TargetedCards.First();
                    target.AttachedEquipments.Add((Equipment)game.TargetingCard);
                }
            });
        }
    }
}
