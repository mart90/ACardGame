namespace ACardGameLibrary
{
    public static class CardLibrary
    {
        public static int NextCardId = 1;

        public static Card GetCard(string cardName)
        {
            var card = Cards.Single(e => e.Name == cardName);
            var newCard = card.Clone();
            newCard.AddEffects(card.Effects);

            newCard.Id = NextCardId;
            NextCardId++;
            
            return newCard;
        }

        public static List<Card> GetStartingShop()
        {
            var shop = new List<Card>();

            foreach (var card in Cards)
            {
                if (card.IsInShopPool)
                {
                    for (int i = 0; i < card.AmountInShopPool; i++)
                    {
                        shop.Add(GetCard(card.Name));
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
                Cost = 3,
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
                Cost = 6,
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

            new CurrencyCard
            {
                Name = "Black market",
                Text = "This card's value is equal to the highest value currency you've played this turn. Exile the currency you used to buy this.",
                IsInShopPool = true,
                Cost = 3,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Currency
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnBuy,
                        Effect = (game, owner) =>
                        {
                            var reverseCurrencyPlayed = new List<CurrencyCard>(owner.CardsPlayedThisTurn.Where(e => e is CurrencyCard).Cast<CurrencyCard>());
                            reverseCurrencyPlayed.Reverse();

                            int valueExiled = 0;
                            for (int i = 0; i < 3; i++)
                            {
                                CurrencyCard card = reverseCurrencyPlayed[i];
                                owner.CardsPlayedThisTurn.Remove(card);
                                valueExiled += card.CurrencyValue;

                                if (valueExiled >= game.CardBeingBought.Cost)
                                {
                                    return;
                                }
                            }
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            if (!owner.CardsPlayedThisTurn.Any(e => e is CurrencyCard))
                            {
                                return;
                            }

                            int value = owner.CardsPlayedThisTurn
                                .Where(e => e is CurrencyCard)
                                .Cast<CurrencyCard>()
                                .Max(e => e.CurrencyValue);

                            ((CurrencyCard)game.CardBeingPlayed).CurrencyValue = value;
                            owner.MoneyToSpend += value;
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnMoveToDiscard,
                        Effect = (game, owner) =>
                        {
                            ((CurrencyCard)game.CardGoingToDiscard).CurrencyValue = 0;
                        }
                    }
                }
            },

            new CurrencyCard
            {
                Name = "Diamond",
                CurrencyValue = 3,
                Text = "When you play this, you lose 2 life.",
                IsInShopPool = true,
                Cost = 7,
                AmountInShopPool = 2,
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
                            owner.MoneyToSpend += 3;
                            owner.Life -= 2;
                        }
                    }
                }
            },
            #endregion

            #region Leaders
            new Card
            {
                Name = "Marcus Aurelius",
                Text = "You get an extra free shop refresh at the start of each turn.",
                IsInShopPool = true,
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
                                Name = "Marcus Aurelius",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.StartingTurn,
                                Effect = (game, owner) =>
                                {
                                    owner.FreeShopRefreshes++;
                                }
                            });
                        }
                    }
                }
            },

            new Card
            {
                Name = "Cleopatra",
                Text = "At the start of your turn, gain 1 life.",
                IsInShopPool = true,
                Cost = 2,
                AmountInShopPool = 1,
                MaxTargets = 2,
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
                                Name = "Cleopatra",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.StartingTurn,
                                Effect = (game, owner) =>
                                {
                                    owner.Life++;
                                }
                            });
                        }
                    }
                }
            },

            new Card
            {
                Name = "Julius Caesar",
                Text = "Each combat, just before it resolves, you may target a creature. It gets +1/+1.",
                IsInShopPool = true,
                Cost = 2,
                AmountInShopPool = 1,
                Types = new List<CardType>
                {
                    CardType.Leader
                },
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Creature
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
                                Name = "Julius Caesar",
                                Owner = owner,
                                OwnersTurnOnly = false,
                                NeedsUserInput = true,
                                Trigger = GameEvent.ResolvingCombat,
                                Effect = (game, owner) =>
                                {
                                    game.SetTargetingCard(owner.Leader);

                                    game.MessageToPlayer = new MessageToPlayerParams
                                    {
                                        Message = "You may target a creature",
                                        Severity = MessageSeverity.Information
                                    };

                                    if (!owner.IsActive)
                                    {
                                        game.SwitchActivePlayer();
                                    }
                                }
                            });
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAccepted,
                        Effect = (game, owner) =>
                        {
                            var target = (CreatureCard)game.TargetedCards.SingleOrDefault();

                            if (target != null)
                            {
                                target.TemporaryAddedPower++;
                                target.TemporaryAddedDefense++;

                                game.AddPublicLog($"{owner.Name} resolved Julius Caesar and added +1/+1 to {target.Name}");
                            }

                            game.ResolveCombat();
                        }
                    },
                }
            },

            new Card
            {
                Name = "Alexander",
                Text = "When you play a support card, draw a card.",
                IsInShopPool = true,
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
                                Name = "Alexander",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.PlayingSupport,
                                Effect = (game, owner) =>
                                {
                                    owner.DrawCards(1);
                                    game.AddPublicLog($"{owner.Name} resolved Alexander and drew a card");
                                }
                            });
                        }
                    }
                }
            },

            new Card
            {
                Name = "Adam Smith",
                Text = "When you buy a currency card, put it into your hand.",
                IsInShopPool = true,
                Cost = 4,
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
                                Name = "Adam Smith",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.Buying,
                                Effect = (game, owner) =>
                                {
                                    if (game.CardBeingBought is CurrencyCard)
                                    {
                                        owner.DiscardPile.Remove(game.CardBeingBought);
                                        owner.Hand.Add(game.CardBeingBought);
                                    }
                                }
                            });
                        }
                    }
                }
            },

            new Card
            {
                Name = "Osama Bin Laden",
                Text = "Whenever a source would deal damage to your opponent, it deals one additional damage.",
                IsInShopPool = true,
                Cost = 3,
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
                                Name = "Osama Bin Laden",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.DealingDamage,
                                Effect = (game, owner) =>
                                {
                                    if (game.DamageBeingDealt > 0)
                                    {
                                        game.DamageBeingDealt++;
                                    }
                                }
                            });
                        }
                    }
                }
            },

            new Card
            {
                Name = "Thomas Aquinas",
                Text = "When you buy a card, gain 2 life.",
                IsInShopPool = true,
                Cost = 3,
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
                                Name = "Thomas Aquinas",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.Buying,
                                Effect = (game, owner) =>
                                {
                                    owner.Life += 2;
                                }
                            });
                        }
                    }
                }
            },

            new Card
            {
                Name = "Moses",
                Text = "Draw one extra card at the end of your turn.",
                IsInShopPool = true,
                Cost = 4,
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
                                Name = "Thomas Aquinas",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.EndingTurn,
                                Effect = (game, owner) =>
                                {
                                    owner.DrawCards(1);
                                }
                            });
                        }
                    }
                }
            },

            new Card
            {
                Name = "Socrates",
                Text = "When a card effect causes you to draw cards, draw one more.",
                IsInShopPool = true,
                Cost = 4,
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
                                Name = "Socrates",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.DrawingCardsFromCardEffect,
                                Effect = (game, owner) =>
                                {
                                    owner.DrawCards(1);
                                }
                            });
                        }
                    }
                }
            },

            new Card
            {
                Name = "Charlemagne",
                Text = "When attacking, the first creature you play can't be blocked.",
                IsInShopPool = true,
                Cost = 3,
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
                                Name = "Charlemagne",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.StartingCombat,
                                Effect = (game, owner) =>
                                {
                                    ((CreatureCard)game.CardBeingPlayed).IsUnblockable = true;
                                }
                            });
                        }
                    }
                }
            },

            new Card
            {
                Name = "Gandhi",
                Text = "When you are attacked, gain 2 life and draw a card.",
                IsInShopPool = true,
                Cost = 3,
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
                                Name = "Gandhi",
                                Owner = owner,
                                OwnersTurnOnly = false,
                                Trigger = GameEvent.StartingCombat,
                                Effect = (game, owner) =>
                                {
                                    if (!owner.IsAttacking)
                                    {
                                        owner.Life += 2;
                                        owner.DrawCards(1);
                                    }
                                }
                            });
                        }
                    }
                }
            },

            new Card
            {
                Name = "Genghis Khan",
                Text = "Your cavalry have +2/+1.",
                IsInShopPool = true,
                Cost = 4,
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
                                Name = "Genghis Khan",
                                Owner = owner,
                                OwnersTurnOnly = false,
                                Trigger = GameEvent.StartingCombat,
                                Effect = (game, owner) =>
                                {
                                    game.CombatModifiers.Add(new CombatModifier
                                    {
                                        Name = "Genghis Khan",
                                        Owner = owner,
                                        OwnerOnly = true,
                                        AddedPower = 2,
                                        AddedDefense = 1,
                                        Conditions = (creature) => creature.Types.Contains(CardType.Cavalry)
                                    });
                                }
                            });
                        }
                    }
                }
            },

            new Card
            {
                Name = "Napoleon",
                Text = "Your creatures have +2 power.",
                IsInShopPool = true,
                Cost = 5,
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
                                Name = "Napoleon",
                                Owner = owner,
                                OwnersTurnOnly = false,
                                Trigger = GameEvent.StartingCombat,
                                Effect = (game, owner) =>
                                {
                                    game.CombatModifiers.Add(new CombatModifier
                                    {
                                        Name = "Napoleon",
                                        Owner = owner,
                                        OwnerOnly = true,
                                        AddedPower = 2
                                    });
                                }
                            });
                        }
                    }
                }
            },

            new Card
            {
                Name = "Albert Einstein",
                Text = "When you play an action card with cost 4 or less, you may resolve its effect twice.",
                IsInShopPool = true,
                Cost = 5,
                AmountInShopPool = 1,
                MaxTargets = 1,
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
                                Name = "Albert Einstein",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.PlayingAction,
                                Effect = (game, owner) =>
                                {
                                    if (game.CardBeingPlayed.Cost > 4)
                                    {
                                        return;
                                    }

                                    var actionCopy = GetCard(game.CardBeingPlayed.Name);
                                    actionCopy.Owner = owner;
                                    game.ActionQueued = actionCopy;

                                    game.AddEventListener(new GameEventListener
                                    {
                                        Name = "Albert Einstein - Copy",
                                        Owner = owner,
                                        OwnersTurnOnly = true,
                                        NeedsUserInput = true,
                                        Trigger = GameEvent.DoneResolving,
                                        Effect = (game, owner) =>
                                        {
                                            if (!game.EinsteinResolvedFlag)
                                            {
                                                game.SetTargetingCard(owner.Leader);

                                                game.RemoveOptionsPickerFlag = false;

                                                game.OptionsPickerOptions = new List<string>
                                                {
                                                    "Yes",
                                                    "No"
                                                };

                                                game.MessageToPlayer = new MessageToPlayerParams
                                                {
                                                    Message = $"Resolve {actionCopy.Name}'s effects twice?",
                                                    Severity = MessageSeverity.Information
                                                };

                                                game.EinsteinResolvedFlag = true;
                                            }
                                            else
                                            {
                                                game.EinsteinResolvedFlag = false;
                                                owner.CardsPlayedThisTurn.RemoveAt(owner.CardsPlayedThisTurn.Count - 1);
                                            }

                                            game.RemoveFirstListener("Albert Einstein - Copy");
                                        }
                                    });
                                }
                            });
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAccepted,
                        Effect = (game, owner) =>
                        {
                            if (game.OptionsPicked.SingleOrDefault() == "No")
                            {
                                game.EinsteinResolvedFlag = false;
                                game.ActionQueued = null;
                            }
                            else
                            {
                                game.AddPublicLog($"{owner.Name} used Albert Einstein to resolve {game.ActionQueued.Name} twice");
                            }

                            game.RemoveOptionsPickerFlag = true;
                        }
                    },
                }
            },

            new Card
            {
                Name = "Victoria",
                Text = "When you buy a card, put it into your hand.",
                IsInShopPool = true,
                Cost = 6,
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
                                Name = "Victoria",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.Buying,
                                Effect = (game, owner) =>
                                {
                                    var card = game.CardBeingBought;
                                    owner.DiscardPile.Remove(card);
                                    owner.Hand.Add(card);
                                }
                            });
                        }
                    }
                }
            },

            new Card
            {
                Name = "Joan of Arc",
                Text = "Your defending creatures have +5 power.",
                IsInShopPool = true,
                Cost = 6,
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
                                Name = "Joan of Arc",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.StartingCombat,
                                Effect = (game, owner) =>
                                {
                                    game.CombatModifiers.Add(new CombatModifier
                                    {
                                        Name = "Joan of Arc",
                                        Owner = owner,
                                        OwnerOnly = true,
                                        AddedPower = 5,
                                        Conditions = (creature) => !creature.Owner.IsAttacking
                                    });
                                }
                            });
                        }
                    }
                }
            },

            new Card
            {
                Name = "Elizabeth",
                Text = "When you play an action card, draw a card.",
                IsInShopPool = true,
                Cost = 6,
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
                                Name = "Elizabeth",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.PlayingAction,
                                Effect = (game, owner) =>
                                {
                                    game.TriggerEvent(GameEvent.DrawingCardsFromCardEffect);
                                    owner.DrawCards(1);
                                }
                            });
                        }
                    }
                }
            },

            new Card
            {
                Name = "Beethoven",
                Text = "Cards in your shop cost 2 less.",
                IsInShopPool = true,
                Cost = 6,
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
                            owner.Shop.ForEach(e => e.Cost -= 2);

                            game.AddEventListener(new GameEventListener
                            {
                                Name = "Beethoven",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.RefreshingShop,
                                Effect = (game, owner) =>
                                {
                                    owner.Shop.ForEach(e => e.Cost += 2);
                                }
                            });

                            game.AddEventListener(new GameEventListener
                            {
                                Name = "Beethoven",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.RefreshedShop,
                                Effect = (game, owner) =>
                                {
                                    owner.Shop.ForEach(e => e.Cost -= 2);
                                }
                            });
                        }
                    }
                }
            },

            new Card
            {
                Name = "Eva",
                Text = "You may spend 6 currency to worship. If you do, add a counter to this card. When it reaches 10 counters, you win the game.",
                IsInShopPool = true,
                Cost = 7,
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
                            owner.CanWorship = true;
                        }
                    }
                }
            },
            #endregion

            #region Creatures
            new CreatureCard
            {
                Name = "Sauron",
                Text = "Trample. If the Ring of power is attached to this at the end of combat, you win the game.",
                Power = 6,
                Defense = 6,
                HasTrample = true,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Infantry
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
                                Name = "Sauron",
                                Owner = owner,
                                OwnersTurnOnly = false,
                                Trigger = GameEvent.EndingCombat,
                                Effect = (game, owner) =>
                                {
                                    var sauron = (CreatureCard?)owner.ActiveCombatCards.SingleOrDefault(e => e.Name == "Sauron");

                                    if (sauron != null && sauron.AttachedEquipments.Any(e => e.Name == "Ring of power"))
                                    {
                                        game.Winner = owner;
                                    }

                                    game.RemoveFirstListener("Sauron", owner);
                                }
                            });
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnRemove,
                        Effect = (game, owner) =>
                        {
                            game.RemoveFirstListener("Sauron", owner);
                        }
                    }
                }
            },

            new CreatureCard
            {
                Name = "Thief",
                Text = "At the end of combat, if this dealt damage to your opponent, they discard a card at random.",
                Power = 1,
                Defense = 1,
                IsInShopPool = true,
                Cost = 2,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            if (game.EventListeners.Any(e => e.Name == "Thief" && e.Owner == owner))
                            {
                                return;
                            }

                            game.AddEventListener(new GameEventListener
                            {
                                Name = "Thief",
                                Owner = owner,
                                OwnersTurnOnly = false,
                                Trigger = GameEvent.EndingCombat,
                                Effect = (game, owner) =>
                                {
                                    var thieves = owner.ActiveCombatCards.Where(e => e.Name == "Thief").Cast<CreatureCard>();
                                    var enemy = game.Players.Single(e => e != owner);

                                    var enemyHandShuffled = new List<Card>(enemy.Hand);
                                    enemyHandShuffled.Shuffle();

                                    foreach (var thief in thieves)
                                    {
                                        if (thief.DealtDamage)
                                        {
                                            var card = enemyHandShuffled.First();

                                            enemy.Hand.Remove(card);
                                            game.MoveToDiscard(card);

                                            enemyHandShuffled.RemoveAt(0);

                                            game.AddPublicLog($"{enemy.Name} was forced to discard {card.Name} by Thief");
                                        }
                                    }

                                    game.RemoveFirstListener("Thief", owner);
                                }
                            });
                        }
                    }
                }
            },

            new CreatureCard
            {
                Name = "Swordsman",
                Power = 2,
                Defense = 2,
                IsInShopPool = true,
                Cost = 2,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Infantry
                }
            },

            new CreatureCard
            {
                Name = "Squire",
                Text = "When you play this, you may target another creature. It gets +1/+1.",
                Power = 1,
                Defense = 2,
                IsInShopPool = true,
                Cost = 2,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Infantry
                },
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Creature
                },
                AdditionalTargetConditions = (game, target) => target != game.TargetingCard,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.CardBeingPlayedIsTargeting();

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "You may target another creature",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        Effect = (game, owner) =>
                        {
                            var target = game.TargetedCards.SingleOrDefault();

                            if (target != null)
                            {
                                ((CreatureCard)target).TemporaryAddedPower++;
                                ((CreatureCard)target).TemporaryAddedDefense++;
                                game.AddPublicLog($"{owner.Name} gave {target.Name} +1/+1");
                            }

                            game.SwitchActivePlayer();
                        }
                    }
                }
            },

            new CreatureCard
            {
                Name = "Scout",
                Text = "When you play this, look at your opponent's hand.",
                Power = 1,
                Defense = 1,
                IsInShopPool = true,
                Cost = 2,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Cavalry
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.RevealOpponentHand = true;
                            game.CardBeingPlayedIsTargeting();

                            game.AddPublicLog($"{game.Enemy.Name} reveals their hand");
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        Effect = (game, owner) =>
                        {
                            game.RevealOpponentHand = false;
                            game.SwitchActivePlayer();
                        }
                    }
                }
            },

            new CreatureCard
            {
                Name = "Horseman",
                Power = 3,
                Defense = 2,
                IsInShopPool = true,
                Cost = 3,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Cavalry
                }
            },

            new CreatureCard
            {
                Name = "Assassin",
                Text = "Can't block. When this is successfully blocked, exile it.",
                Power = 5,
                Defense = 1,
                IsInShopPool = true,
                Cost = 3,
                AmountInShopPool = 2,
                AdditionalPlayConditions = (game) => game.ActivePlayer.IsAttacking || !game.IsInCombat,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Infantry
                }
            },

            new CreatureCard
            {
                Name = "Arrow runner",
                Text = "When you play this, you may target a ranged creature. It gets +3 power.",
                Power = 2,
                Defense = 1,
                IsInShopPool = true,
                Cost = 3,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Infantry
                },
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Ranged
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.CardBeingPlayedIsTargeting();

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "You may target a ranged creature",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        Effect = (game, owner) =>
                        {
                            var target = game.TargetedCards.SingleOrDefault();

                            if (target != null)
                            {
                                ((CreatureCard)target).TemporaryAddedPower += 3;
                                game.AddPublicLog($"{owner.Name} gave {target.Name} +3 power");
                            }

                            game.SwitchActivePlayer();
                        }
                    }
                }
            },

            new CreatureCard
            {
                Name = "Archer",
                Text = "When attacking, at the end of combat, this deals damage to your opponent equal to its power.",
                Power = 1,
                Defense = 1,
                IsInShopPool = true,
                Cost = 3,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Ranged
                }
            },

            new CreatureCard
            {
                Name = "Pikeman",
                Text = "When this fights cavalry, it gets +3 defense.",
                Power = 2,
                Defense = 3,
                IsInShopPool = true,
                Cost = 4,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Infantry
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            if (game.CombatModifiers.Where(e => e.Name == "Pikeman").Any())
                            {
                                return;
                            }

                            game.CombatModifiers.Add(new CombatModifier
                            {
                                Name = "Pikeman",
                                Owner = owner,
                                OwnerOnly = false,
                                AddedDefense = 3,
                                Conditions = (creature) => creature.Name == "Pikeman",
                                ConditionsEnemy = (creature) => creature.Types.Contains(CardType.Cavalry)
                            });
                        }
                    }
                }
            },

            new CreatureCard
            {
                Name = "Hero",
                Text = "When you play this, it gets +1/+1 for each other creature you have in play.",
                Power = 1,
                Defense = 1,
                IsInShopPool = true,
                Cost = 4,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Infantry
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            var hero = (CreatureCard)game.CardBeingPlayed;

                            int creaturesInPlay = owner.ActiveCombatCards.OfType<CreatureCard>().Count();

                            hero.TemporaryAddedPower = creaturesInPlay;
                            hero.TemporaryAddedDefense = creaturesInPlay;
                        }
                    }
                }
            },

            new CreatureCard
            {
                Name = "Captain",
                Text = "Your infantry have +2/+2.",
                Power = 1,
                Defense = 2,
                IsInShopPool = true,
                Cost = 4,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.CombatModifiers.Add(new CombatModifier
                            {
                                Name = "Captain",
                                Owner = owner,
                                OwnerOnly = true,
                                AddedPower = 2,
                                AddedDefense = 2,
                                Conditions = (creature) => creature.Types.Contains(CardType.Infantry)
                            });
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnRemove,
                        Effect = (game, owner) =>
                        {
                            var modifier = game.CombatModifiers.First(e => e.Name == "Captain" && e.Owner == owner);
                            game.CombatModifiers.Remove(modifier);
                        }
                    },
                }
            },

            new CreatureCard
            {
                Name = "Scorpion",
                Text = "When you play this, you may target a creature with 2 or less defense. Its owner discards it.",
                Power = 2,
                Defense = 1,
                IsInShopPool = true,
                Cost = 4,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature
                },
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Creature
                },
                AdditionalTargetConditions = (game, target) => game.CreatureDefense((CreatureCard)target) <= 2,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.CardBeingPlayedIsTargeting();

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "You may target a creature with 2 or less defense",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        Effect = (game, owner) =>
                        {
                            var target = (CreatureCard)game.TargetedCards.FirstOrDefault();

                            if (target == null)
                            {
                                return;
                            }

                            game.AddPublicLog($"{game.ActivePlayer.Name} used Scorpion to discard {target.Name}");

                            game.RemoveCardFromBattlefield(target);

                            game.MoveToDiscard(target);

                            game.SwitchActivePlayer();
                        }
                    },
                }
            },

            new CreatureCard
            {
                Name = "Horse archer",
                Text = "Can only be blocked by ranged creatures.",
                Power = 3,
                Defense = 1,
                IsInShopPool = true,
                Cost = 4,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Ranged,
                    CardType.Cavalry
                }
            },

            new CreatureCard
            {
                Name = "Knight",
                Power = 4,
                Defense = 3,
                IsInShopPool = true,
                Cost = 5,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Cavalry
                }
            },

            new CreatureCard
            {
                Name = "Falcon",
                Text = "When you play this, look at your opponent's hand. You may return this to your hand.",
                Power = 2,
                Defense = 2,
                IsInShopPool = true,
                Cost = 5,
                AmountInShopPool = 2,
                MaxTargets = 1,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Flying
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.RevealOpponentHand = true;
                            game.CardBeingPlayedIsTargeting();

                            game.AddPublicLog($"{game.Enemy.Name} reveals their hand");
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        ResolveOrder = 0,
                        Effect = (game, owner) =>
                        {
                            game.RevealOpponentHand = false;

                            game.CardBeingPlayedIsTargeting();

                            game.OptionsPickerOptions = new List<string>
                            {
                                "Yes",
                                "No"
                            };

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "Return Falcon to your hand?",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        ResolveOrder = 1,
                        Effect = (game, owner) =>
                        {
                            game.RemoveOptionsPickerFlag = true;

                            var optionPicked = game.OptionsPicked.SingleOrDefault();

                            if (optionPicked == "Yes")
                            {
                                var falcon = owner.ActiveCombatCards.Last();

                                game.RemoveCardFromBattlefield(falcon);
                                owner.Hand.Add(falcon);

                                game.AddPublicLog($"{owner.Name} returned Falcon to their hand");
                            }

                            game.SwitchActivePlayer();
                        }
                    }
                }
            },

            new CreatureCard
            {
                Name = "Crossbowman",
                Text = "Your opponent's creatures have -1 defense.",
                Power = 4,
                Defense = 2,
                IsInShopPool = true,
                Cost = 5,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Ranged
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.CombatModifiers.Add(new CombatModifier
                            {
                                Name = "Crossbowman",
                                Owner = owner,
                                EnemyOnly = true,
                                AddedDefense = -1
                            });

                            foreach (var creature in game.Enemy.ActiveCombatCards.Where(e => e is CreatureCard).Cast<CreatureCard>())
                            {
                                game.RemoveIfDead(creature);
                            }
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnRemove,
                        Effect = (game, owner) =>
                        {
                            var modifier = game.CombatModifiers.First(e => e.Name == "Crossbowman" && e.Owner == owner);
                            game.CombatModifiers.Remove(modifier);
                        }
                    },
                }
            },

            new CreatureCard
            {
                Name = "Catapult",
                Text = "Trample.",
                Power = 5,
                Defense = 1,
                HasTrample = true,
                IsInShopPool = true,
                Cost = 5,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Siege
                }
            },

            new CreatureCard
            {
                Name = "Paladin",
                Text = "When you play this, you may target a creature. It loses 2 defense.",
                Power = 5,
                Defense = 3,
                IsInShopPool = true,
                Cost = 6,
                AmountInShopPool = 2,
                MaxTargets = 1,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Cavalry
                },
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Creature
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.CardBeingPlayedIsTargeting();

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "You may target a creature",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        Effect = (game, owner) =>
                        {
                            var target = (CreatureCard?)game.TargetedCards.SingleOrDefault();

                            if (target != null)
                            {
                                target.TemporaryAddedDefense -= 2;

                                game.AddPublicLog($"{target.Name} loses 2 defense");

                                game.RemoveIfDead(target);
                            }

                            game.SwitchActivePlayer();
                        }
                    },
                }
            },

            new CreatureCard
            {
                Name = "Great eagle",
                Text = "When you play this, you may target a support card in play. Its owner discards it.",
                Power = 3,
                Defense = 3,
                IsInShopPool = true,
                Cost = 6,
                AmountInShopPool = 2,
                MaxTargets = 1,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Flying
                },
                ValidTargetTypes = new List<CardType>
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
                            game.CardBeingPlayedIsTargeting();

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "You may target a support card",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        Effect = (game, owner) =>
                        {
                            var target = (SupportCard?)game.TargetedCards.SingleOrDefault();

                            if (target != null)
                            {
                                game.RemoveCardFromBattlefield(target);
                                game.MoveToDiscard(target);

                                game.AddPublicLog($"{target.Name} was discarded");
                            }

                            game.SwitchActivePlayer();
                        }
                    },
                }
            },

            new CreatureCard
            {
                Name = "Champion",
                Text = "When this attacks, the defender must block it before they can block other creatures.",
                Power = 4,
                Defense = 5,
                HasTaunt = true,
                IsInShopPool = true,
                Cost = 6,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Infantry
                }
            },

            new CreatureCard
            {
                Name = "Trebuchet",
                Text = "Can't block. Trample.",
                Power = 7,
                Defense = 1,
                HasTrample = true,
                IsInShopPool = true,
                Cost = 6,
                AmountInShopPool = 2,
                AdditionalPlayConditions = (game) => game.ActivePlayer.IsAttacking || !game.IsInCombat,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Siege
                }
            },

            new CreatureCard
            {
                Name = "Storyteller",
                Text = "When you play this, you may put a legendary creature from your discard pile into your hand.",
                Power = 4,
                Defense = 4,
                IsInShopPool = true,
                Cost = 7,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Infantry
                },
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Legendary
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.CardBeingPlayedIsTargeting();

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "You may select a legendary creature from your discard pile",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        Effect = (game, owner) =>
                        {
                            var target = game.TargetedCards.SingleOrDefault();

                            if (target != null)
                            {
                                game.AddPublicLog($"{owner.Name} put {target.Name} into their hand");
                                owner.DiscardPile.Remove(target);
                                owner.Hand.Add(target);
                            }

                            game.SwitchActivePlayer();
                        }
                    }
                }
            },

            new CreatureCard
            {
                Name = "Tree of life",
                Text = "Can block any creature. When this blocks a creature, gain 3 life.",
                Power = 1,
                Defense = 8,
                IsInShopPool = true,
                Cost = 7,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Legendary
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            if (!owner.IsAttacking)
                            {
                                owner.Life += 3;
                            }
                        }
                    }
                }
            },

            new CreatureCard
            {
                Name = "Pegasus",
                Text = "This has +1/+1 for each other cavalry creature you have in play.",
                Power = 4,
                Defense = 4,
                IsInShopPool = true,
                Cost = 7,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Legendary,
                    CardType.Cavalry
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            if (game.EventListeners.Any(e => e.Name == "Pegasus" && e.Owner == owner))
                            {
                                return;
                            }

                            game.AddEventListener(new GameEventListener
                            {
                                Name = "Pegasus",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.ResolvingCombat,
                                Effect = (game, owner) =>
                                {
                                    int amountOfCavalry = owner.ActiveCombatCards.Count(e => e.Types.Contains(CardType.Cavalry));

                                    foreach (var pegasus in owner.ActiveCombatCards.Where(e => e.Name == "Pegasus").Cast<CreatureCard>())
                                    {
                                        pegasus.TemporaryAddedPower += amountOfCavalry - 1;
                                        pegasus.TemporaryAddedDefense += amountOfCavalry - 1;
                                    }
                                }
                            });
                            game.AddEventListener(new GameEventListener
                            {
                                Name = "PegasusEnd",
                                Owner = owner,
                                OwnersTurnOnly = false,
                                Trigger = GameEvent.EndingCombat,
                                Effect = (game, owner) =>
                                {
                                    game.RemoveFirstListener("Pegasus", owner);
                                    game.RemoveFirstListener("PegasusEnd", owner);
                                }
                            });
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnRemove,
                        Effect = (game, owner) =>
                        {
                            game.RemoveFirstListener("Pegasus", owner);
                            game.RemoveFirstListener("PegasusEnd", owner);
                        }
                    },
                }
            },

            new CreatureCard
            {
                Name = "Dragon",
                Text = "When you play this, draw a card.",
                Power = 6,
                Defense = 3,
                IsInShopPool = true,
                Cost = 7,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Legendary,
                    CardType.Flying
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.TriggerEvent(GameEvent.DrawingCardsFromCardEffect);
                            owner.DrawCards(1);
                        }
                    }
                }
            },
            #endregion

            #region Supports
            new Equipment
            {
                Name = "Rage",
                Text = "Attach to a creature. It has +3 power.",
                IsInShopPool = true,
                Cost = 2,
                AmountInShopPool = 2,
                AddedPower = 3
            },

            new Equipment
            {
                Name = "Lifelink",
                Text = "Attach to a friendly non-siege creature. At the end of combat, you gain life equal to its power.",
                IsInShopPool = true,
                Cost = 2,
                AmountInShopPool = 2,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.AddEventListenerIfNotExists(new GameEventListener
                            {
                                Name = "Lifelink",
                                Owner = owner,
                                OwnersTurnOnly = false,
                                Trigger = GameEvent.EndingCombat,
                                Effect = (game, owner) =>
                                {
                                    var lifelinkedCreatures = owner.ActiveCombatCards.Where(e => e is CreatureCard creature && creature.AttachedEquipments.Any(e => e.Name == "Lifelink")).Cast<CreatureCard>();

                                    foreach (var creature in lifelinkedCreatures)
                                    {
                                        int creaturePower = game.CreaturePower(creature);
                                        owner.Life += creaturePower;
                                        game.AddPublicLog($"{owner.Name} gained {creaturePower} life from Lifelink");
                                    }

                                    game.RemoveFirstListener("Lifelink", owner);
                                }
                            });
                        }
                    }
                }
            },

            new SupportCard
            {
                Name = "Rout",
                Text = "Return target creature to its owner's hand. They can't play it again this combat.",
                IsInShopPool = true,
                Cost = 2,
                AmountInShopPool = 2,
                TargetsOnPlay = true,
                MinTargets = 1,
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Creature
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAccepted,
                        Effect = (game, owner) =>
                        {
                            CreatureCard target = (CreatureCard)game.TargetedCards.Single();
                            target.IsUnplayable = true;

                            game.AddPublicLog($"{game.ActivePlayer.Name} returned {target.Name} to its owner's hand");

                            game.RemoveCardFromBattlefield(target);

                            target.Owner.Hand.Add(target);
                        }
                    }
                }
            },

            new Equipment
            {
                Name = "Inspiration",
                Text = "Attach to a creature. It has +3/+3.",
                IsInShopPool = true,
                Cost = 3,
                AmountInShopPool = 2,
                AddedPower = 3,
                AddedDefense = 3
            },

            new SupportCard
            {
                Name = "Wall",
                Text = "Attacking creatures have -2 power.",
                IsInShopPool = true,
                Cost = 3,
                AmountInShopPool = 2,
                IsPermanent = true,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.CombatModifiers.Add(new CombatModifier
                            {
                                Name = "Wall",
                                Owner = owner,
                                OwnerOnly = false,
                                AddedPower = -2,
                                Conditions = (creature) => creature.Owner.IsAttacking
                            });
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnRemove,
                        Effect = (game, owner) =>
                        {
                            var modifier = game.CombatModifiers.First(e => e.Name == "Wall" && e.Owner == owner);
                            game.CombatModifiers.Remove(modifier);
                        }
                    },
                }
            },

            new SupportCard
            {
                Name = "Cancel",
                Text = "Target a support card. Its owner discards it.",
                IsInShopPool = true,
                Cost = 4,
                AmountInShopPool = 2,
                TargetsOnPlay = true,
                MinTargets = 1,
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Support
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAccepted,
                        Effect = (game, owner) =>
                        {
                            SupportCard target = (SupportCard)game.TargetedCards.Single();

                            game.AddPublicLog($"{game.ActivePlayer.Name} used Cancel to discard {target.Name}");

                            game.RemoveCardFromBattlefield(target);
                            game.MoveToDiscard(target);
                        }
                    },
                }
            },

            new SupportCard
            {
                Name = "Copy",
                Text = "This becomes a copy of target friendly attacking creature for the rest of combat.",
                IsInShopPool = true,
                Cost = 4,
                AmountInShopPool = 2,
                TargetsOnPlay = true,
                MustTargetFriend = true,
                MinTargets = 1,
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Creature
                },
                AdditionalPlayConditions = (game) => game.ActivePlayer.IsAttacking || !game.IsInCombat,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAccepted,
                        Effect = (game, owner) =>
                        {
                            CreatureCard target = (CreatureCard)game.TargetedCards.Single();

                            var card = (CreatureCard)GetCard(target.Name);
                            card.Owner = owner;
                            card.IsCopy = true;

                            card.TemporaryAddedPower = target.TemporaryAddedPower;
                            card.TemporaryAddedDefense = target.TemporaryAddedDefense;

                            game.AddPublicLog($"{game.ActivePlayer.Name} copied {target.Name}");

                            game.EventListeners.Add(new GameEventListener
                            {
                                Name = "Copy",
                                Owner = owner,
                                OwnersTurnOnly = false,
                                Trigger = GameEvent.EndingCombat,
                                Effect = (game, owner) =>
                                {
                                    var copy = owner.ActiveCombatCards
                                        .Where(e => e is CreatureCard creature && creature.IsCopy)
                                        .First();

                                    game.RemoveCardFromBattlefield(copy);
                                    game.RemoveFirstListener("Copy");
                                }
                            });

                            game.AttackingCreatures.Add(card);
                            owner.ActiveCombatCards.Add(card);
                        }
                    }
                }
            },

            new SupportCard
            {
                Name = "Snipe",
                Text = "Target a creature. Its owner discards it.",
                IsInShopPool = true,
                Cost = 5,
                AmountInShopPool = 2,
                TargetsOnPlay = true,
                MinTargets = 1,
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Creature
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAccepted,
                        Effect = (game, owner) =>
                        {
                            CreatureCard target = (CreatureCard)game.TargetedCards.Single();

                            game.AddPublicLog($"{game.ActivePlayer.Name} used Snipe to discard {target.Name}");

                            game.RemoveCardFromBattlefield(target);
                            game.MoveToDiscard(target);
                        }
                    },
                }
            },

            new Equipment
            {
                Name = "Vigilance",
                Text = "Attach to a friendly creature. It has +2/+2. At the end of combat, put it into your hand.",
                IsInShopPool = true,
                Cost = 4,
                AmountInShopPool = 2,
                MustTargetFriend = true,
                AddedPower = 2,
                AddedDefense = 2,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.AddEventListenerIfNotExists(new GameEventListener
                            {
                                Name = "Vigilance",
                                Owner = owner,
                                OwnersTurnOnly = false,
                                Trigger = GameEvent.EndingCombat,
                                Effect = (game, owner) =>
                                {
                                    var creatures = owner.ActiveCombatCards.Where(e => e is CreatureCard).Cast<CreatureCard>();

                                    var creature = creatures.FirstOrDefault(e => e.AttachedEquipments.Any(e => e.Name == "Vigilance"));

                                    while (creature != null)
                                    {
                                        game.RemoveCardFromBattlefield(creature);

                                        owner.Hand.Add(creature);

                                        game.AddPublicLog($"{creature.Name} was returned to {owner.Name}'s hand");

                                        creature = creatures.FirstOrDefault(e => e.AttachedEquipments.Any(e => e.Name == "Vigilance"));
                                    }

                                    game.RemoveFirstListener("Vigilance", owner);
                                }
                            });
                        }
                    },
                }
            },

            new SupportCard
            {
                Name = "Leadership",
                Text = "Draw a card for each creature you have in play.",
                IsInShopPool = true,
                Cost = 5,
                AmountInShopPool = 2,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            int creaturesInPlay = owner.ActiveCombatCards.Where(e => e is CreatureCard).Count();

                            game.TriggerEvent(GameEvent.DrawingCardsFromCardEffect);
                            owner.DrawCards(creaturesInPlay);
                        }
                    },
                }
            },

            new Equipment
            {
                Name = "Wololo",
                Text = "Attach to an enemy creature. At the end of combat, if it did damage to you, put it onto your discard pile. Then, exile this card.",
                IsInShopPool = true,
                Cost = 5,
                AmountInShopPool = 2,
                MustTargetEnemy = true,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.AddEventListenerIfNotExists(new GameEventListener
                            {
                                Name = "Wololo",
                                Owner = owner,
                                OwnersTurnOnly = false,
                                Trigger = GameEvent.EndingCombat,
                                Effect = (game, owner) =>
                                {
                                    var enemy = game.Players.Where(e => e != owner).Single();

                                    var creatures = enemy.ActiveCombatCards.Where(e => e is CreatureCard).Cast<CreatureCard>();

                                    var convertedCreature = creatures.FirstOrDefault(e => e.AttachedEquipments.Any(e => e.Name == "Wololo") && e.DealtDamage);

                                    while (convertedCreature != null)
                                    {
                                        var wololo = convertedCreature.AttachedEquipments.Where(e => e.Name == "Wololo").First();

                                        game.RemoveCardFromBattlefield(wololo);

                                        game.RemoveCardFromBattlefield(convertedCreature);

                                        convertedCreature.Owner = owner;
                                        owner.DiscardPile.Add(convertedCreature);

                                        game.AddPublicLog($"{owner.Name}'s Wololo converted {convertedCreature.Name}");

                                        convertedCreature = creatures.FirstOrDefault(e => e.AttachedEquipments.Any(e => e.Name == "Wololo") && e.DealtDamage);
                                    }

                                    game.RemoveFirstListener("Wololo", owner);
                                }
                            });
                        }
                    },
                }
            },

            new Equipment
            {
                Name = "Wings",
                Text = "Attach to a creature. It has +3 power and flying.",
                IsInShopPool = true,
                Cost = 5,
                AmountInShopPool = 2,
                AddedPower = 3,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.AddEventListener(new GameEventListener
                            {
                                Name = "Wings",
                                Owner = owner,
                                OwnersTurnOnly = false,
                                Trigger = GameEvent.EndingCombat,
                                Effect = (game, owner) =>
                                {
                                    var creature = game.ActiveCombatCards
                                        .Where(e => e is CreatureCard)
                                        .Cast<CreatureCard>()
                                        .Where(e => e.AttachedEquipments.Any(e => e.Name == "Wings") && e.Owner == owner)
                                        .FirstOrDefault();

                                    if (creature != null)
                                    {
                                        creature.Types.Remove(CardType.Flying);
                                    }

                                    game.RemoveFirstListener("Wings", owner);
                                }
                            });
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnRemove,
                        Effect = (game, owner) =>
                        {
                            var creature = game.ActiveCombatCards
                                .Where(e => e is CreatureCard)
                                .Cast<CreatureCard>()
                                .Where(e => e.AttachedEquipments.Any(e => e.Name == "Wings") && e.Owner == owner)
                                .FirstOrDefault();

                            if (creature != null)
                            {
                                creature.Types.Remove(CardType.Flying);
                            }
                        }
                    },
                }
            },

            new SupportCard
            {
                Name = "Barrage",
                Text = "Target up to two creatures with cost 4 or less. Their owners discard them.",
                IsInShopPool = true,
                Cost = 6,
                AmountInShopPool = 2,
                TargetsOnPlay = true,
                MaxTargets = 2,
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Creature
                },
                AdditionalTargetConditions = (game, target) => target.Cost <= 4,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAccepted,
                        Effect = (game, owner) =>
                        {
                            var targets = game.TargetedCards;

                            foreach (var target in targets)
                            {
                                game.AddPublicLog($"{game.ActivePlayer.Name} used Barrage to discard {target.Name}");

                                game.RemoveCardFromBattlefield(target);
                                game.MoveToDiscard(target);
                            }
                        }
                    },
                }
            },

            new SupportCard
            {
                Name = "Castle",
                Text = "When combat resolves, prevent all damage dealt by attacking non-siege creatures.",
                IsInShopPool = true,
                Cost = 6,
                AmountInShopPool = 2,
                IsPermanent = true,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.CombatModifiers.Add(new CombatModifier
                            {
                                Name = "Castle",
                                Owner = owner,
                                OwnerOnly = false,
                                AddedPower = -99,
                                Conditions = (creature) => !creature.Types.Contains(CardType.Siege) && creature.Owner.IsAttacking
                            });
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnRemove,
                        Effect = (game, owner) =>
                        {
                            var modifier = game.CombatModifiers.First(e => e.Name == "Castle" && e.Owner == owner);
                            game.CombatModifiers.Remove(modifier);
                        }
                    },
                }
            },

            new SupportCard
            {
                Name = "Flanking",
                Text = "At the end of combat, deal 1 damage to your opponent for each creature you have in play.",
                IsInShopPool = true,
                Cost = 6,
                AmountInShopPool = 2,
                IsPermanent = true,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.AddEventListener(new GameEventListener
                            {
                                Name = "Flanking",
                                Owner = owner,
                                Trigger = GameEvent.EndingCombat,
                                Effect = (game, owner) =>
                                {
                                    game.DealDamage(owner.ActiveCombatCards.Count(e => e is CreatureCard));
                                    game.AddPublicLog($"{owner.Name}'s Flanking dealt {game.DamageBeingDealt} damage");
                                    game.RemoveFirstListener("Flanking", owner);
                                }
                            });
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnRemove,
                        Effect = (game, owner) =>
                        {
                            game.RemoveFirstListener("Flanking", owner);
                        }
                    },
                }
            },

            new SupportCard
            {
                Name = "Overwhelm",
                Text = "When you play a card with cost 4 or less during this combat, draw a card.",
                IsInShopPool = true,
                Cost = 5,
                AmountInShopPool = 2,
                IsPermanent = true,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            var effect = delegate (GameStateManager game, Player owner)
                            {
                                if (game.CardBeingPlayed.Cost > 4)
                                {
                                    return;
                                }

                                game.TriggerEvent(GameEvent.DrawingCardsFromCardEffect);
                                owner.DrawCards(1);
                            };

                            game.AddEventListener(new GameEventListener
                            {
                                Name = "OverwhelmCreature",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.PlayingCreature,
                                Effect = effect
                            });
                            game.AddEventListener(new GameEventListener
                            {
                                Name = "OverwhelmSupport",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.PlayingSupport,
                                Effect = effect
                            });
                            game.AddEventListener(new GameEventListener
                            {
                                Name = "OverwhelmEnd",
                                Owner = owner,
                                OwnersTurnOnly = false,
                                Trigger = GameEvent.EndingCombat,
                                Effect = (game, owner) =>
                                {
                                    game.RemoveFirstListener("OverwhelmCreature", owner);
                                    game.RemoveFirstListener("OverwhelmSupport", owner);
                                    game.RemoveFirstListener("OverwhelmEnd", owner);
                                }
                            });
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnRemove,
                        Effect = (game, owner) =>
                        {
                            game.RemoveFirstListener("OverwhelmCreature", owner);
                            game.RemoveFirstListener("OverwhelmSupport", owner);
                            game.RemoveFirstListener("OverwhelmEnd", owner);
                        }
                    },
                }
            },

            new Equipment
            {
                Name = "Ring of power",
                Text = "Attach to a friendly creature. It gets +3 power. At the end of combat, if this is still attached, exile that creature and gain Sauron.",
                IsInShopPool = true,
                Cost = 7,
                AmountInShopPool = 1,
                MustTargetFriend = true,
                AddedPower = 3,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.AddEventListener(new GameEventListener
                            {
                                Name = "Ring of power",
                                Owner = owner,
                                OwnersTurnOnly = false,
                                Trigger = GameEvent.EndingCombat,
                                Effect = (game, owner) =>
                                {
                                    var creatures = owner.ActiveCombatCards.Where(e => e is CreatureCard).Cast<CreatureCard>();
                                    var creature = creatures.FirstOrDefault(e => e.AttachedEquipments.Any(e => e.Name == "Ring of power") && e.Owner == owner);

                                    if (creature != null)
                                    {
                                        game.RemoveCardFromBattlefield(creature);

                                        var sauron = GetCard("Sauron");
                                        sauron.Owner = owner;

                                        owner.DiscardPile.Add(sauron);

                                        game.AddPublicLog($"{creature.Name} was exiled and {owner.Name} gained Sauron");
                                    }

                                    game.RemoveFirstListener("Ring of power", owner);
                                }
                            });
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnRemove,
                        Effect = (game, owner) =>
                        {
                            game.RemoveFirstListener("Ring of power", owner);
                        }
                    }
                }
            },

            new SupportCard
            {
                Name = "Valor",
                Text = "Your creatures have +3/+3.",
                IsInShopPool = true,
                Cost = 7,
                AmountInShopPool = 2,
                IsPermanent = true,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.CombatModifiers.Add(new CombatModifier
                            {
                                Name = "Valor",
                                Owner = owner,
                                OwnerOnly = true,
                                AddedPower = 3,
                                AddedDefense = 3
                            });
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnRemove,
                        Effect = (game, owner) =>
                        {
                            var modifier = game.CombatModifiers.First(e => e.Name == "Valor" && e.Owner == owner);
                            game.CombatModifiers.Remove(modifier);
                        }
                    },
                }
            },

            new Equipment
            {
                Name = "Engineering",
                Text = "Attach to a siege creature. It has +10 power.",
                IsInShopPool = true,
                Cost = 7,
                AmountInShopPool = 2,
                AddedPower = 10,
                AdditionalTargetConditions = (game, target) => target.Types.Contains(CardType.Siege)
            },

            new SupportCard
            {
                Name = "Wisdom",
                Text = "Choose two: Draw three cards; Gain 3 life; Legendary creatures have -4/-4; Your creatures have +2/+1.",
                IsInShopPool = true,
                Cost = 7,
                AmountInShopPool = 2,
                IsPermanent = true,
                MinTargets = 2,
                MaxTargets = 2,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.CardBeingPlayedIsTargeting();

                            game.OptionsPickerOptions = new List<string>
                            {
                                "Draw three cards",
                                "Gain 3 life",
                                "Legendary creatures have -4/-4",
                                "Your creatures have +2/+1",
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        Effect = (game, owner) =>
                        {
                            game.RemoveOptionsPickerFlag = true;

                            var optionsPicked = game.OptionsPicked;

                            if (optionsPicked.Contains("Draw three cards"))
                            {
                                game.TriggerEvent(GameEvent.DrawingCardsFromCardEffect);
                                owner.DrawCards(3);
                                game.AddPublicLog($"{owner.Name} drew three cards");
                            }

                            if (optionsPicked.Contains("Gain 3 life"))
                            {
                                owner.Life += 3;
                                game.AddPublicLog($"{owner.Name} gained 3 life");
                            }

                            if (optionsPicked.Contains("Legendary creatures have -4/-4"))
                            {
                                game.CombatModifiers.Add(new CombatModifier
                                {
                                    Name = "WisdomLegendary",
                                    Owner = owner,
                                    OwnerOnly = false,
                                    AddedPower = -4,
                                    AddedDefense = -4,
                                });

                                game.AddPublicLog($"Legendary creatures have -4/-4");

                                var legendaryCreatures = game.ActiveCombatCards.Where(e => e is CreatureCard && e.Types.Contains(CardType.Legendary)).Cast<CreatureCard>();

                                foreach (var creature in legendaryCreatures)
                                {
                                    game.RemoveIfDead(creature);
                                }
                            }

                            if (optionsPicked.Contains("Your creatures have +2/+1"))
                            {
                                game.CombatModifiers.Add(new CombatModifier
                                {
                                    Name = "WisdomBuff",
                                    Owner = owner,
                                    OwnerOnly = true,
                                    AddedPower = 2,
                                    AddedDefense = 1,
                                });
                                game.AddPublicLog($"{owner.Name}'s creatures have +2/+1");
                            }

                            game.SwitchActivePlayer();
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnRemove,
                        Effect = (game, owner) =>
                        {
                            game.RemoveFirstListener("WisdomLegendary", owner);
                            game.RemoveFirstListener("WisdomBuff", owner);
                        }
                    }
                }
            },
            #endregion

            #region Actions
            new Card
            {
                Name = "Trade",
                Text = "Draw two cards, then discard a card.",
                IsInShopPool = true,
                Cost = 2,
                AmountInShopPool = 2,
                TargetsHand = true,
                MinTargets = 1,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Currency,
                    CardType.Support,
                    CardType.Action,
                    CardType.Leader
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.TriggerEvent(GameEvent.DrawingCardsFromCardEffect);
                            owner.DrawCards(2);

                            game.CardBeingPlayedIsTargeting();

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "Discard a card",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        Effect = (game, owner) =>
                        {
                            var target = game.TargetedCards.Single();

                            owner.Hand.Remove(target);
                            game.MoveToDiscard(target);

                            game.AddPublicLog($"{owner.Name} discarded {target.Name}");
                        }
                    }
                }
            },

            new Card
            {
                Name = "Market",
                Text = "Look at the top four cards of a shop pile. You may put any of them on the bottom. Refresh your shop.",
                IsInShopPool = true,
                Cost = 2,
                AmountInShopPool = 2,
                MaxTargets = 4,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Currency,
                    CardType.Support,
                    CardType.Action,
                    CardType.Leader
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.ActivateShopCostPickerFlag = true;
                            game.CardBeingPlayedIsTargeting();

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "Pick a shop pile to view the top cards from",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        ResolveOrder = 0,
                        Effect = (game, owner) =>
                        {
                            game.RemoveShopCostPickerFlag = true;
                            var cost = game.ShopCostPicked;

                            game.ActivateSelectorFlag = true;
                            game.CardsToChooseFromSelector = game.ShopPool
                                .Where(e => e.Cost == cost)
                                .Take(4)
                                .ToList();

                            game.CardBeingPlayedIsTargeting();

                            game.AddPublicLog($"{owner.Name} looked at the top 4 cards of the {cost}-cost shop pile");

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "Select cards to put on the bottom",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        ResolveOrder = 1,
                        Effect = (game, owner) =>
                        {
                            game.RemoveSelectorFlag = true;

                            foreach (var card in game.TargetedCards)
                            {
                                game.ShopPool.Remove(card);
                                game.ShopPool.Add(card);
                            }

                            game.AddPublicLog($"{owner.Name} put {game.TargetedCards.Count} cards on the bottom");

                            game.RefreshShop(owner);
                        }
                    }
                }
            },

            new Card
            {
                Name = "Well",
                Text = "Gain 2 life.",
                IsInShopPool = true,
                Cost = 2,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            owner.Life += 2;
                        }
                    }
                }
            },

            new Card
            {
                Name = "Recall",
                Text = "Put a card from your discard pile into your hand.",
                IsInShopPool = true,
                Cost = 3,
                AmountInShopPool = 2,
                MinTargets = 1,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Leader,
                    CardType.Creature,
                    CardType.Currency,
                    CardType.Action,
                    CardType.Support
                },
                AdditionalPlayConditions = (game) => game.ActivePlayer.DiscardPile.Count > 0,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.CardBeingPlayedIsTargeting();

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "Select a card from your discard pile",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        Effect = (game, owner) =>
                        {
                            var target = game.TargetedCards.Single();

                            game.AddPublicLog($"{owner.Name} put {target.Name} from their discard pile into their hand");
                            owner.DiscardPile.Remove(target);
                            owner.Hand.Add(target);
                        }
                    }
                }
            },

            new Card
            {
                Name = "Sacrifice",
                Text = "Deal 2 damage to your opponent. You lose 4 life.",
                IsInShopPool = true,
                Cost = 3,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.DealDamage(2);
                            game.AddPublicLog($"Sacrifice dealt {game.DamageBeingDealt} damage");

                            owner.Life -= 4;
                            game.AddPublicLog($"{owner.Name} took 4 damage");
                        }
                    }
                }
            },

            new Card
            {
                Name = "Reason",
                Text = "Choose one: Gain a silver, or draw two cards.",
                IsInShopPool = true,
                Cost = 4,
                AmountInShopPool = 2,
                MinTargets = 1,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.CardBeingPlayedIsTargeting();

                            game.OptionsPickerOptions = new List<string>
                            {
                                "Gain a Silver",
                                "Draw two cards"
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        Effect = (game, owner) =>
                        {
                            game.RemoveOptionsPickerFlag = true;

                            var optionPicked = game.OptionsPicked.Single();

                            if (optionPicked == "Gain a Silver")
                            {
                                var silver = GetCard("Silver");
                                silver.Owner = owner;
                                owner.DiscardPile.Add(silver);

                                game.AddPublicLog($"{owner.Name} chose to gain a Silver");
                            }
                            else if (optionPicked == "Draw two cards")
                            {
                                game.TriggerEvent(GameEvent.DrawingCardsFromCardEffect);
                                owner.DrawCards(2);
                                game.AddPublicLog($"{owner.Name} chose to draw two cards");
                            }
                        }
                    }
                }
            },

            new Card
            {
                Name = "Merchant",
                Text = "For the rest of your turn, cards in your shop cost 1 currency less.",
                IsInShopPool = true,
                Cost = 3,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            owner.Shop.ForEach(e => e.Cost--);

                            game.EventListeners.Add(new GameEventListener
                            {
                                Name = "MerchantRefreshing",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.RefreshingShop,
                                Effect = (game, owner) =>
                                {
                                    owner.Shop.ForEach(e => e.Cost++);
                                }
                            });

                            game.EventListeners.Add(new GameEventListener
                            {
                                Name = "MerchantRefreshed",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.RefreshedShop,
                                Effect = (game, owner) =>
                                {
                                    owner.Shop.ForEach(e => e.Cost--);
                                }
                            });

                            game.EventListeners.Add(new GameEventListener
                            {
                                Name = "MerchantEnd",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.EndingTurn,
                                Effect = (game, owner) =>
                                {
                                    owner.Shop.ForEach(e => e.Cost++);

                                    game.RemoveFirstListener("MerchantRefreshing", owner);
                                    game.RemoveFirstListener("MerchantRefreshed", owner);
                                    game.RemoveFirstListener("MerchantEnd", owner);
                                }
                            });
                        }
                    }
                }
            },

            new Card
            {
                Name = "Breakthrough",
                Text = "Choose a cost. Your shop is of that level for the rest of this turn. Exile this card.",
                IsInShopPool = true,
                Cost = 4,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.ActivateShopCostPickerFlag = true;

                            game.CardBeingPlayedIsTargeting();

                            int currentShopLevel = owner.ShopLevel;
                            int currentShopRefreshCost = owner.ShopRefreshCost;

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "Pick a cost",
                                Severity = MessageSeverity.Information
                            };

                            game.EventListeners.Add(new GameEventListener
                            {
                                Name = "BreakthroughEnd",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.EndingTurn,
                                Effect = (game, owner) =>
                                {
                                    owner.ShopLevel = currentShopLevel;
                                    owner.ShopRefreshCost = currentShopRefreshCost;
                                    game.RemoveFirstListener("BreakthroughEnd");
                                }
                            });

                            game.EventListeners.Add(new GameEventListener
                            {
                                Name = "BreakthroughExile",
                                Owner = owner,
                                OwnersTurnOnly = true,
                                Trigger = GameEvent.DoneResolving,
                                Effect = (game, owner) =>
                                {
                                    owner.CardsPlayedThisTurn.RemoveAll(e => e.Name == "Breakthrough");
                                    game.RemoveFirstListener("BreakthroughExile");
                                }
                            });
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        Effect = (game, owner) =>
                        {
                            game.RemoveShopCostPickerFlag = true;
                            var cost = game.ShopCostPicked;

                            owner.ShopLevel = cost;
                            owner.ShopRefreshCost = cost;

                            game.AddPublicLog($"{owner.Name} used Breakthrough to upgrade their shop to level {cost}");
                        },
                    }
                }
            },

            new Card
            {
                Name = "Spy",
                Text = "Look at your opponent's hand and choose a non-currency card from it. They discard that card.",
                IsInShopPool = true,
                Cost = 3,
                AmountInShopPool = 2,
                MaxTargets = 1,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Support,
                    CardType.Action,
                    CardType.Leader
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.ActivateSelectorFlag = true;
                            game.CardsToChooseFromSelector = new List<Card>(game.Enemy.Hand);

                            game.CardBeingPlayedIsTargeting();

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "Choose a card for your opponent to discard",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        Effect = (game, owner) =>
                        {
                            game.RemoveSelectorFlag = true;

                            var target = game.TargetedCards.Single();

                            game.MoveToDiscard(target);
                            game.Enemy.Hand.Remove(target);

                            game.AddPublicLog($"{game.Enemy.Name} was forced to discard {target.Name}");
                        }
                    }
                }
            },

            new Card
            {
                Name = "Raid",
                Text = "Deal 2 damage to your opponent. If you discard a cavalry card from your hand, draw two cards.",
                IsInShopPool = true,
                Cost = 4,
                AmountInShopPool = 2,
                MaxTargets = 1,
                TargetsHand = true,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Cavalry
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.DealDamage(2);
                            game.AddPublicLog($"Raid dealt {game.DamageBeingDealt} damage");

                            if (!owner.Hand.Any(e => e.Types.Contains(CardType.Cavalry)))
                            {
                                return;
                            }

                            game.CardBeingPlayedIsTargeting();

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "You may discard a cavalry card",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        Effect = (game, owner) =>
                        {
                            var target = game.TargetedCards.SingleOrDefault();

                            if (target == null)
                            {
                                return;
                            }

                            game.MoveToDiscard(target);
                            owner.Hand.Remove(target);

                            game.TriggerEvent(GameEvent.DrawingCardsFromCardEffect);
                            owner.DrawCards(2);

                            game.AddPublicLog($"{owner.Name} discarded {target.Name} and drew two cards");
                        }
                    }
                }
            },

            new Card
            {
                Name = "Reform",
                Text = "Exile a card from your hand. Draw three cards.",
                IsInShopPool = true,
                Cost = 4,
                AmountInShopPool = 2,
                TargetsHand = true,
                MinTargets = 1,
                MaxTargets = 1,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Currency,
                    CardType.Support,
                    CardType.Action,
                    CardType.Leader
                },
                AdditionalPlayConditions = (game) => game.ActivePlayer.Hand.Count > 1,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.CardBeingPlayedIsTargeting();

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "Exile a card from your hand",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        ResolveOrder = 0,
                        Effect = (game, owner) =>
                        {
                            var target = game.TargetedCards.Single();

                            owner.Hand.Remove(target);
                            game.AddPublicLog($"{owner.Name} exiled {target.Name}");

                            game.TriggerEvent(GameEvent.DrawingCardsFromCardEffect);
                            owner.DrawCards(3);
                        }
                    }
                }
            },

            new Card
            {
                Name = "Upgrade",
                Text = "Exile a non-currency card from your hand. Gain a card from your shop.",
                IsInShopPool = true,
                Cost = 5,
                AmountInShopPool = 2,
                MaxTargets = 1,
                MinTargets = 1,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Currency,
                    CardType.Support,
                    CardType.Action,
                    CardType.Leader
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.CardBeingPlayedIsTargeting();

                            game.CardBeingPlayed.TargetsHand = true;
                            game.CardBeingPlayed.TargetsShop = false;
                            game.CardBeingPlayed.ValidTargetTypes.Remove(CardType.Currency);

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "Exile a non-currency card from your hand",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        ResolveOrder = 0,
                        Effect = (game, owner) =>
                        {
                            var target = game.TargetedCards.Single();

                            owner.Hand.Remove(target);

                            game.AddPublicLog($"{owner.Name} exiled {target.Name}");

                            game.TargetingCard.TargetsHand = false;
                            game.TargetingCard.TargetsShop = true;
                            game.TargetingCard.ValidTargetTypes.Add(CardType.Currency);

                            game.RequireAccept = true;

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "Select a card from your shop",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        ResolveOrder = 1,
                        Effect = (game, owner) =>
                        {
                            var target = game.TargetedCards.Single();

                            owner.Shop.Remove(target);
                            owner.DiscardPile.Add(target);
                            target.Owner = owner;

                            game.AddPublicLog($"{owner.Name} gained {target.Name} from their shop");
                        }
                    }
                }
            },

            new Card
            {
                Name = "Chapel",
                Text = "Exile at least two cards from your hand. Gain a Gold.",
                IsInShopPool = true,
                Cost = 5,
                AmountInShopPool = 2,
                TargetsHand = true,
                MinTargets = 2,
                MaxTargets = 99,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Currency,
                    CardType.Support,
                    CardType.Action,
                    CardType.Leader
                },
                AdditionalPlayConditions = (game) => game.ActivePlayer.Hand.Count > 2,
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.CardBeingPlayedIsTargeting();

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "Exile at least two cards from your hand",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        ResolveOrder = 0,
                        Effect = (game, owner) =>
                        {
                            var targets = game.TargetedCards;

                            foreach (var target in targets)
                            {
                                owner.Hand.Remove(target);
                                game.AddPublicLog($"{owner.Name} exiled {target.Name}");
                            }

                            var gold = GetCard("Gold");
                            gold.Owner = owner;
                            owner.DiscardPile.Add(gold);

                            game.AddPublicLog($"{owner.Name} gained a Gold");
                        }
                    }
                }
            },

            new Card
            {
                Name = "University",
                Text = "Gain 2 life. Draw two cards.",
                IsInShopPool = true,
                Cost = 5,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            owner.Life += 2;

                            game.TriggerEvent(GameEvent.DrawingCardsFromCardEffect);
                            owner.DrawCards(2);
                        }
                    }
                }
            },

            new Card
            {
                Name = "Redeploy",
                Text = "Put up to two non-legendary creatures from your discard pile into your hand.",
                IsInShopPool = true,
                Cost = 6,
                AmountInShopPool = 2,
                MaxTargets = 2,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Creature
                },
                AdditionalTargetConditions = (game, target) => !target.Types.Contains(CardType.Legendary),
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.CardBeingPlayedIsTargeting();

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "Target up to two creatures from your discard pile",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        Effect = (game, owner) =>
                        {
                            var targets = game.TargetedCards;

                            foreach (var target in targets)
                            {
                                game.AddPublicLog($"{owner.Name} put {target.Name} from their discard pile into their hand");
                                owner.DiscardPile.Remove(target);
                                owner.Hand.Add(target);
                            }
                        }
                    }
                }
            },

            new Card
            {
                Name = "Library",
                Text = "Look at the top card of your deck. You may discard it. Then, draw three cards.",
                IsInShopPool = true,
                Cost = 6,
                AmountInShopPool = 2,
                MaxTargets = 1,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Action,
                    CardType.Currency,
                    CardType.Creature,
                    CardType.Leader,
                    CardType.Support
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            var card = owner.GetCardsFromTopOfDeck(1).SingleOrDefault();

                            if (card == null)
                            {
                                return;
                            }
                            else
                            {
                                owner.Deck.Insert(0, card);
                            }

                            game.CardsToChooseFromSelector = new List<Card> { card };
                            game.ActivateSelectorFlag = true;

                            game.CardBeingPlayedIsTargeting();

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "Select cards to discard",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        Effect = (game, owner) =>
                        {
                            game.RemoveSelectorFlag = true;

                            var target = game.TargetedCards.SingleOrDefault();

                            if (target != null)
                            {
                                owner.Deck.Remove(target);
                                owner.DiscardPile.Add(target);
                                game.AddPublicLog($"{owner.Name} discarded {target.Name} from the top of their deck");
                            }

                            game.TriggerEvent(GameEvent.DrawingCardsFromCardEffect);
                            owner.DrawCards(3);
                        }
                    }
                }
            },

            new Card
            {
                Name = "Alchemy",
                Text = "Gain 1 life for each card in your hand. Then, exile a card from your hand.",
                IsInShopPool = true,
                Cost = 6,
                AmountInShopPool = 2,
                MaxTargets = 1,
                MinTargets = 1,
                TargetsHand = true,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                ValidTargetTypes = new List<CardType>
                {
                    CardType.Creature,
                    CardType.Currency,
                    CardType.Support,
                    CardType.Action,
                    CardType.Leader
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            int lifeGained = owner.Hand.Count - 1;

                            owner.Life += lifeGained;
                            game.AddPublicLog($"{owner.Name} gained {lifeGained} life");

                            game.CardBeingPlayedIsTargeting();

                            game.MessageToPlayer = new MessageToPlayerParams
                            {
                                Message = "Exile a card from your hand",
                                Severity = MessageSeverity.Information
                            };
                        }
                    },
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnAcceptedAfterPlay,
                        Effect = (game, owner) =>
                        {
                            var target = game.TargetedCards.Single();

                            owner.Hand.Remove(target);

                            game.AddPublicLog($"{owner.Name} exiled {target.Name}");
                        }
                    }
                }
            },

            new Card
            {
                Name = "Artillery",
                Text = "Deal 5 damage to your opponent.",
                IsInShopPool = true,
                Cost = 7,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            game.DealDamage(5);
                            game.AddPublicLog($"Artillery dealt {game.DamageBeingDealt} damage");
                        }
                    }
                }
            },

            new Card
            {
                Name = "Fires of industry",
                Text = "Draw cards until you have seven cards in hand, discarding any currency cards you draw.",
                IsInShopPool = true,
                Cost = 7,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            while (owner.Hand.Count < 7)
                            {
                                var card = owner.GetCardsFromTopOfDeck(1).SingleOrDefault();

                                if (card == null)
                                {
                                    break;
                                }

                                if (card is CurrencyCard)
                                {
                                    game.MoveToDiscard(card);
                                    game.AddPublicLog($"{owner.Name} discarded {card.Name}");
                                }
                                else
                                {
                                    owner.Hand.Add(card);
                                }
                            }
                        }
                    }
                }
            },

            new Card
            {
                Name = "Free trade",
                Text = "For the rest of this turn, you may buy cards from shop discard piles at or below your shop level.",
                IsInShopPool = true,
                Cost = 7,
                AmountInShopPool = 2,
                Types = new List<CardType>
                {
                    CardType.Action
                },
                Effects = new List<CardEffect>
                {
                    new CardEffect
                    {
                        EffectPhase = CardEffectPhase.OnPlay,
                        Effect = (game, owner) =>
                        {
                            owner.CanFreeTrade = true;
                        }
                    }
                }
            },
            #endregion
        };
    }
}
