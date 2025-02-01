namespace ACardGameLibrary
{
    public class Bot
    {
        private readonly GameStateManager _game;
        private readonly List<Card> _hand;
        private readonly List<Card> _shop;

        private readonly List<string> _cardsToBuy;

        public Bot(GameStateManager game)
        {
            _game = game;
            _hand = game.Enemy.Hand;
            _shop = game.Enemy.Shop;

            _cardsToBuy = new List<string>
            {
                "Alexander",
                "Julius Caesar",
                "Swordsman",
                "Squre",
                "Scout",
                "Rage",
                "Blacksmith",
                "Thief"
            };
        }

        private bool ShopHasPriorityCards => _shop.Any(e => _cardsToBuy.Contains(e.Name));

        private bool HaveCreature => _hand.Any(e => e.GetMainType() == CardType.Creature);

        public void DoTurn()
        {
            if (!ShopHasPriorityCards)
            {
                _game.ActionRefreshShop();
            }

            int currencyValueInHand = CurrencyValueInHand();

            if (currencyValueInHand >= 2)
            {
                var priorityCards = _shop
                    .Where(e => _cardsToBuy.Contains(e.Name))
                    .ToList();

                if (priorityCards.Count() > 1 && currencyValueInHand >= 4)
                {
                    PlaySilvers(4);

                    foreach (string cardName in _cardsToBuy)
                    {
                        var card = priorityCards.Where(e => e.Name == cardName).FirstOrDefault();

                        if (card == null)
                        {
                            continue;
                        }

                        _game.BuyCard(card);

                        if (_game.ActivePlayer.MoneyToSpend < 2)
                        {
                            break;
                        }
                    }
                }
            }

            if (HaveCreature)
            {

            }
        }

        public void DoTurnCombat()
        {

        }

        public void DoTurnPostCombat()
        {
            
        }

        // TODO Black market
        private int CurrencyValueInHand()
        {
            int value = 0;

            foreach (CurrencyCard currency in _hand.Where(e => e is CurrencyCard))
            {
                if (currency.Name == "Black market")
                {
                    value += _hand
                        .Where(e => e is CurrencyCard)
                        .Cast<CurrencyCard>()
                        .Max(e => e.CurrencyValue);
                }
                else
                {
                    value += currency.CurrencyValue;
                }
            }

            return value;
        }

        private void PlaySilvers(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                PlayCardByName("Silver");
            }
        }

        private void PlayCardByName(string name)
        {
            _game.PlayCard(_hand.First(e => e.Name == name));
        }
    }
}
