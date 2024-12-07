namespace ACardGameLibrary
{
    public static class CardLibrary
    {
        public static Card GetCard(string cardName)
        {
            return Cards.Single(e => e.Name == cardName);
        }

        public static List<Card> GetStartingShop()
        {
            var shop = new List<Card>();

            foreach (var card in Cards)
            {
                if (card.IsBuyable)
                {
                    for (int i = 0; i < card.AmountInShopPool; i++)
                    {
                        shop.Add(card.Clone());
                    }
                }
            }

            return shop;
        }

        public static List<Card> Cards = new List<Card>
        {
            #region Currency
            new CurrencyCard
            {
                Name = "Silver",
                CurrencyValue = 1,
                Types = new List<CardType>
                {
                    CardType.Currency
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            owner.MoneyToSpend++;
                        }
                    }
                }
            },

            new CurrencyCard
            {
                Name = "Gold",
                CurrencyValue = 2,
                Types = new List<CardType>
                {
                    CardType.Currency
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            owner.MoneyToSpend += 2;
                        }
                    }
                }
            },
            #endregion

            #region Leaders
            new Card
            {
                Name = "M. Aurelius",
                Text = "At the start of each round, before drawing, look at the top two cards of your deck. You may discard one or both of them.",
                IsBuyable = true,
                Cost = 2,
                AmountInShopPool = 1,
                Types = new List<CardType>
                {
                    CardType.Leader
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.AddEventListener(new GameEventListener
                            {
                                Name = "M. Aurelius",
                                Owner = owner,
                                Trigger = GameEvent.StartingRound,
                                Effect = (game, owner) =>
                                {
                                    // TODO
                                }
                            });
                        }
                    }
                }
            },
            #endregion

            #region Creatures
            new CreatureCard
            {
                Name = "Sauron",
                Text = "If the Ring of power is attached to this at the end of combat, you win the game.",
                Power = 5,
                Defense = 5,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Infantry
                }
            },

            new CreatureCard
            {
                Name = "Spearman",
                Text = "When this fights cavalry, it gets +1 defense.",
                Power = 1,
                Defense = 3,
                IsBuyable = true,
                Cost = 2,
                AmountInShopPool = 3,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Infantry
                }
            },

            new CreatureCard
            {
                Name = "Swordsman",
                Power = 2,
                Defense = 2,
                IsBuyable = true,
                Cost = 2,
                AmountInShopPool = 3,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Infantry
                }
            },
            #endregion

            #region Supports
            new Card
            {
                Name = "Shield",
                Text = "Attach to a creature. It has +3 defense.",
                IsBuyable = true,
                Cost = 2,
                AmountInShopPool = 3,
                Types = new List<CardType>
                {
                    CardType.Support
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            // TODO
                        }
                    }
                }
            },

            new Card
            {
                Name = "Rout",
                Text = "Return target creature to its owner's hand. Attached support cards are discarded. Draw a card.",
                IsBuyable = true,
                Cost = 2,
                AmountInShopPool = 3,
                Types = new List<CardType>
                {
                    CardType.Support
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            // TODO
                        }
                    }
                }
            },
            #endregion
        };
    }
}
