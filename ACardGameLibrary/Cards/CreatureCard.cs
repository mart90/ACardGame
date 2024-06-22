namespace ACardGameLibrary
{
    public class CreatureCard : Card
    {
        public CreatureCard()
        {
            AttachedSupportCards = new List<Card>();
        }

        public int Power { get; set; }

        public int Defense { get; set; }

        public List<Card> AttachedSupportCards { get; set; }
    }
}
