namespace ACardGameLibrary
{
    public class CombatModifier
    {
        public string Name { get; set; }
        public Player Owner { get; set; }
        public bool OwnerOnly { get; set; }
        public bool EnemyOnly { get; set; }
        public int AddedPower { get; set; }
        public int AddedDefense { get; set; }
        public Predicate<CreatureCard> Conditions { get; set; }
        public Predicate<CreatureCard> ConditionsEnemy { get; set; }

        public bool CardMeetsConditions(CreatureCard card)
        {
            if (Conditions == null)
            {
                return true;
            }

            return Conditions(card);
        }

        public bool EnemyMeetsConditions(CreatureCard enemy)
        {
            if (ConditionsEnemy == null)
            {
                return true;
            }

            return ConditionsEnemy(enemy); 
        }
    }
}
