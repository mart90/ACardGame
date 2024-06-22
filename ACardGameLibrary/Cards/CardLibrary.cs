namespace ACardGameLibrary
{
    public class CardLibrary
    {
        public static Card GetCard(string cardName)
        {
            return Cards.Single(e => e.Name == cardName);
        }

        public static List<Card> Cards = new List<Card>
        {
            #region Currency
            new Card
            {
                Name = "Silver",
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

            new Card
            {
                Name = "Gold",
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
                Name = "Marcus Aurelius",
                Text = "At the start of each round, before drawing, look at the top two cards of your deck. You may discard one or both of them",
                IsBuyable = true,
                Cost = 1,
                AmountInShop = 1,
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
                                Name = "Marcus Aurelius",
                                Owner = owner,
                                Trigger = GameEvent.StartingRound,
                                Effect = (game, owner) =>
                                {
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
                Defense = 2,
                IsBuyable = true,
                Cost = 1,
                AmountInShop = 3,
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
                Cost = 1,
                AmountInShop = 3,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Infantry
                }
            },
            #endregion
        };
    }
}
