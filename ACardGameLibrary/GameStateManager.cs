namespace ACardGameLibrary
{
    public class GameStateManager
    {
        public GameStateManager() 
        {
            Players = new List<Player>()
            {
                new("A") { IsActive = true },
                new("B")
            };

            ShopPool = new List<Card>();
            CurrentShop = new List<Card>();

            ActivePlayer.CardsInHand.Add(CardLibrary.GetCard("Sauron"));
            ActivePlayer.CardsInHand.Add(CardLibrary.GetCard("Sauron"));
            ActivePlayer.CardsInHand.Add(CardLibrary.GetCard("Sauron"));
            ActivePlayer.CardsInHand.Add(CardLibrary.GetCard("Sauron"));
            ActivePlayer.CardsInHand.Add(CardLibrary.GetCard("Sauron"));
            ActivePlayer.CardsInHand.Add(CardLibrary.GetCard("Sauron"));
            ActivePlayer.CardsInHand.Add(CardLibrary.GetCard("Sauron"));
            ActivePlayer.CardsInHand.Add(CardLibrary.GetCard("Sauron"));
            ActivePlayer.CardsInHand.Add(CardLibrary.GetCard("Sauron"));
            ActivePlayer.CardsInHand.Add(CardLibrary.GetCard("Sauron"));

            ActivePlayer.DiscardPile.Add(CardLibrary.GetCard("Swordsman"));
            ActivePlayer.DiscardPile.Add(CardLibrary.GetCard("Spearman"));
            ActivePlayer.DiscardPile.Add(CardLibrary.GetCard("Gold"));

            Enemy.DiscardPile.Add(CardLibrary.GetCard("Gold"));
            Enemy.DiscardPile.Add(CardLibrary.GetCard("Swordsman"));
            Enemy.DiscardPile.Add(CardLibrary.GetCard("Spearman"));

            EventListeners = new List<GameEventListener>();
        }

        public List<Player> Players { get; private set; }

        public Player ActivePlayer => Players.Single(e => e.IsActive);

        public Player Enemy => Players.Single(e => !e.IsActive);

        public List<GameEventListener> EventListeners { get; private set; }

        public List<Card> CurrentShop { get; set; }
        public List<Card> ShopPool { get; set; }

        public void AddEventListener(GameEventListener listener)
        {
            EventListeners.Add(listener);
        }

        public void RemoveListener(string name)
        {
            EventListeners.RemoveAll(e => e.Name == name);
        }

        public void TriggerEvent(GameEvent gameEvent)
        {
            foreach (GameEventListener listener in EventListeners.Where(e => e.Trigger == gameEvent))
            {
                listener.Effect(this, listener.Owner);
            }
        }

        public void SwitchTurn()
        {
            foreach (var player in Players)
            {
                player.IsActive = !player.IsActive;
            }
        }
    }
}
