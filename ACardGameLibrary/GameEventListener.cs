namespace ACardGameLibrary
{
    public class GameEventListener
    {
        public string Name { get; set; }

        public Player Owner { get; set; }

        public bool OwnersTurnOnly {  get; set; }

        public bool NeedsUserInput { get; set; }

        public bool IsTriggered { get; set; }

        public GameEvent Trigger { get; set; }

        public Action<GameStateManager, Player> Effect { get; set; }
    }
}
