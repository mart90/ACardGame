namespace ACardGameLibrary
{
    public class CreatureCard : Card
    {
        public CreatureCard()
        {
            AttachedSupportCards = new List<Card>();
            BlockedBy = new List<CreatureCard>();
        }

        public int Power { get; set; }

        public int Defense { get; set; }

        public List<Card> AttachedSupportCards { get; set; }

        public List<CreatureCard> BlockedBy { get; set; }
    }
}
