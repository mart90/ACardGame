namespace ACardGameLibrary
{
    public class PlayCardParams
    {
        public Card Card { get; set; }
        public bool IsActivePlayer { get; set; }
        public CreatureCard? IsBlockingCreature { get; set; }
        public CreatureCard? IsAttachingToCreature { get; set; }
        public Card? Target { get; set; }
    }
}