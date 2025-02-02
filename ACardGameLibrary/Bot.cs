﻿namespace ACardGameLibrary
{
    /// <summary>
    /// 2-cost aggro bot. Only buys 2-cost aggro cards, doesn't defend
    /// </summary>
    public class Bot
    {
        private readonly GameStateManager _game;
        private readonly List<Card> _hand;
        private readonly List<Card> _shop;

        private readonly List<string> _cardsToBuy;
        private readonly List<string> _combatPlayPriority;

        private bool _boughtLeader;

        public Bot(GameStateManager game)
        {
            _game = game;
            _hand = game.Enemy.Hand;
            _shop = game.Enemy.Shop;

            _cardsToBuy = new List<string>
            {
                "Alexander",
                "Swordsman",
                "Squire",
                "Scout",
                "Rage",
                "Blacksmith",
                "Thief",
            };

            _combatPlayPriority = new List<string>
            {
                "Scout",
                "Swordsman",
                "Squire",
                "Thief",
                "Blacksmith",
                "Rage"
            };
        }

        private Player Me => _game.Players.Single(e => e.Name == "Bot");

        private bool ShopHasPriorityCards => _shop.Any(e => _cardsToBuy.Contains(e.Name));

        private bool HaveCreature => _hand.Any(e => e.GetMainType() == CardType.Creature);

        private int CombatCardsInHand => _hand.Count(e => e is CreatureCard || e is SupportCard);

        public void DoTurn()
        {
            var leader = _hand.SingleOrDefault(e => e.GetMainType() == CardType.Leader);

            if (leader != null)
            {
                PlayCardByName(leader.Name);
            }

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
                }
                else
                {
                    PlaySilvers(2);
                }

                foreach (string cardName in _cardsToBuy)
                {
                    var card = priorityCards.Where(e => e.Name == cardName).FirstOrDefault();

                    if (card == null)
                    {
                        continue;
                    }

                    if (card.GetMainType() == CardType.Leader)
                    {
                        if (_boughtLeader)
                        {
                            continue;
                        }

                        _boughtLeader = true;
                    }

                    _game.BuyCard(card);

                    if (_game.ActivePlayer.MoneyToSpend < 2)
                    {
                        break;
                    }
                }
            }

            int totalCardsToDraw = Me.DiscardPile.Count + Me.Deck.Count + Me.CardsPlayedThisTurn.Count;
            int combatCardsToDraw = totalCardsToDraw - (4 - CurrencyValueInHand());

            if (HaveCreature && combatCardsToDraw < CombatCardsInHand / 1.5)
            {
                PlayCombatCard();
            }
            else
            {
                _game.SwitchTurn();
            }
        }

        public void DoTurnCombat()
        {
            if (Me.IsAttacking)
            {
                PlayCombatCard();
            }
            else
            {
                _game.CombatPass();
            }
        }

        public void DoTurnPostCombat()
        {
            _game.SwitchTurn();
        }

        public Card? GetTarget(string cardName)
        {
            if (cardName == "Squire" || cardName == "Rage")
            {
                var creature = Me.ActiveCombatCards.FirstOrDefault(e => 
                    e is CreatureCard creature 
                    && !creature.BlockedBy.Any()
                    && !e.IsBeingPlayed);

                if (creature != null)
                {
                    return creature;
                }

                return Me.ActiveCombatCards.First(e => e is CreatureCard);
            }

            return null;
        }

        private void PlayCombatCard()
        {
            foreach (string cardName in _combatPlayPriority)
            {
                var card = _hand.FirstOrDefault(e => 
                    e.Name == cardName 
                    && (e is SupportCard || (e is CreatureCard creature && !creature.IsUnplayable)));

                if (card != null)
                {
                    PlayCardByName(cardName);
                    return;
                }
            }

            _game.CombatPass();
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
            if (name == "Rage")
            {
                _game.RequireAccept = true;
                _hand.First(e => e.Name == name).IsTargeting = true;
                GetTarget("Rage").IsTargeted = true;
            }

            _game.PlayCard(_hand.First(e => e.Name == name));
        }
    }
}
