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

            RoundNumber = 1;

            EventListeners = new List<GameEventListener>();
            AttackingCreatures = new List<CreatureCard>();
            GlobalSupportsInPlay = new List<Card>();

            ShopPool = CardLibrary.GetStartingShop().Shuffle().ToList();

            CurrentShop = new List<Card>();
            RefreshShop();
        }

        public List<Player> Players { get; private set; }

        public Player ActivePlayer => Players.Single(e => e.IsActive);
        public Player Enemy => Players.Single(e => !e.IsActive);

        public Player Attacker => Players.Single(e => e.IsAttacking);
        public Player Defender => Players.Single(e => !e.IsAttacking);

        public List<GameEventListener> EventListeners { get; private set; }

        public List<Card> CurrentShop { get; set; }
        public List<Card> ShopPool { get; set; }

        public int RoundNumber;

        public bool IsInCombat { get; set; }
        public List<CreatureCard> AttackingCreatures { get; set; }
        public List<Card> GlobalSupportsInPlay { get; set; }

        public bool CanPlayCard(bool isActivePlayer, Card card)
        {
            var type = card.GetMainType();
            var player = isActivePlayer ? ActivePlayer : Enemy;

            if (!IsInCombat && type == CardType.Support)
            {
                return false;
            }

            if (!IsInCombat && type == CardType.Creature && player.HasAttackedThisRound)
            {
                return false;
            }

            if (IsInCombat && (type == CardType.Action || type == CardType.Currency))
            {
                return false;
            }

            return true;
        }

        public bool CanBuyCard(Card card)
        {
            if (ActivePlayer.MoneyToSpend < card.Cost)
            {
                return false;
            }

            return true;
        }

        public void PlayCard(PlayCardParams param)
        {
            Player player = param.IsActivePlayer ? ActivePlayer : Enemy;
            Card card = param.Card;
            CardType type = card.GetMainType();

            card.Effects.Where(e => e.EffectPhase == CardEffectPhase.OnPlay).SingleOrDefault()?.Effect(this, player);

            player.Hand.Remove(card);

            if (type == CardType.Creature && !IsInCombat)
            {
                IsInCombat = true;
                player.IsAttacking = true;
                player.HasAttackedThisRound = true;
            }

            if (IsInCombat) 
            {
                PlayCardCombat(param);
            }
            else
            {
                player.DiscardPile.Add(card);
            }

            if (type == CardType.Leader)
            {
                PlayLeader(player, card);
            }

            switch (type)
            {
                case CardType.Currency: TriggerEvent(GameEvent.PlayingCurrency); break;
                case CardType.Action: TriggerEvent(GameEvent.PlayingAction); break;
                case CardType.Creature: TriggerEvent(GameEvent.PlayingCreature); break;
                case CardType.Support: TriggerEvent(GameEvent.PlayingSupport); break;
                case CardType.Leader: TriggerEvent(GameEvent.PlayingLeader); break;
            }

            if (type != CardType.Currency)
            {
                SwitchTurn();
            }
        }

        private void PlayCardCombat(PlayCardParams param)
        {
            Player player = param.IsActivePlayer ? ActivePlayer : Enemy;
            Card card = param.Card;
            CardType type = card.GetMainType();

            if (type == CardType.Creature)
            {
                if (param.IsBlockingCreature == null)
                {
                    AttackingCreatures.Add((CreatureCard)card);
                }
                else
                {
                    param.IsBlockingCreature.BlockedBy.Add((CreatureCard)card);
                }
            }
            else // Support
            {
                if (param.IsAttachingToCreature != null)
                {
                    param.IsAttachingToCreature.AttachedSupportCards.Add(card);
                }
                else
                {
                    GlobalSupportsInPlay.Add(card);
                }
            }
        }

        private void PlayLeader(Player player, Card card)
        {
            if (player.Leader != null)
            {
                RemoveListener(player.Leader.Name);
            }

            player.Leader = card;
        }

        public void BuyCard(bool isActivePlayer, Card card)
        {
            Player player = isActivePlayer ? ActivePlayer : Enemy;

            TriggerEvent(GameEvent.Buying);

            player.DiscardPile.Add(card);
            card.Owner = player;

            player.MoneyToSpend -= card.Cost.Value;

            CurrentShop.Remove(card);

            SwitchTurn();
        }

        public void Pass(Player player)
        {
            player.IsPassed = true;

            if (Players.Single(e => e != player).IsPassed)
            {
                NewRound();
            }
            else
            {
                SwitchTurn();
            }
        }

        public void AddEventListener(GameEventListener listener)
        {
            EventListeners.Add(listener);
        }

        public void RemoveListener(string name)
        {
            EventListeners.RemoveAll(e => e.Name == name);
        }

        private void TriggerEvent(GameEvent gameEvent)
        {
            foreach (GameEventListener listener in EventListeners.Where(e => e.Trigger == gameEvent))
            {
                listener.Effect(this, listener.Owner);
            }
        }

        private void SwitchTurn()
        {
            // TODO OnTurnEnd card effects

            if (!Enemy.IsPassed)
            {
                foreach (var player in Players)
                {
                    player.IsActive = !player.IsActive;
                }
            }

            ActivePlayer.MoneyToSpend = 0;
        }

        private void NewRound()
        {
            TriggerEvent(GameEvent.EndingRound);

            RoundNumber++;
            RefreshShop();

            SwitchTurn();

            foreach (var player in Players)
            {
                player.IsPassed = false;
                player.HasAttackedThisRound = false;
            }

            TriggerEvent(GameEvent.StartingRound);
        }

        private void RefreshShop()
        {
            CurrentShop.Clear();

            for (int cost = 2; cost <= 7; cost++)
            {
                for (int i = 0; i < GetAmountOfShopCards(cost); i++)
                {
                    var card = ShopPool.First(e => e.Cost == cost);
                    CurrentShop.Add(card);
                    ShopPool.Remove(card);
                }
            }

            TriggerEvent(GameEvent.RefreshingShop);
        }

        private int GetAmountOfShopCards(int cost)
        {
            if (cost == 2)
            {
                return RoundNumber < 6 ? 6 - RoundNumber : 0;
            }

            int effectiveRoundNumber = RoundNumber <= 11 ? RoundNumber : 11;

            int lowerBound = (cost - 2) * 2;
            int upperBound = cost * 2 + 1;

            if (effectiveRoundNumber > lowerBound && effectiveRoundNumber < upperBound)
            {
                return 2;
            }
            else if (effectiveRoundNumber == lowerBound || effectiveRoundNumber == upperBound)
            {
                return 1; 
            }
            else
            {
                return 0;
            }
        }
    }
}
