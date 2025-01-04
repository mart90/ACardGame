using ACardGameLibrary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ACardGame.UI
{
    public abstract class GameWindow : TopLevelUiWindow
    {
        public GameStateManager GameState { get; private set; }

        public ActivePlayerHand ActivePlayerHand { get; private set; }
        public FaceDownCardStack ActivePlayerDeck { get; set; }
        public CardContainer ActivePlayerDiscardPile { get; set; }
        public Shop ActivePlayerShop { get; set; }

        public FaceDownCardStack EnemyDeck { get; set; }
        public FaceDownCardStack EnemyHand { get; set; }
        public CardContainer EnemyDiscardPile { get; set; }
        public Shop EnemyShop { get; set; }

        public CardContainer ActivePlayerLeader { get; set; }
        public CardContainer EnemyLeader { get; set; }

        public Battlefield Battlefield { get; set; }

        public CardStackViewer CardStackViewer { get; set; }
        public CardContainer HoveredCardViewer { get; set; }

        public Button EndTurnButton { get; set; }
        public Button ToggleShopButton { get; set; }
        public Button ViewLogButton { get; set; }

        public UiElement Life { get; set; }
        public UiElement EnemyLife { get; set; }

        public UiElement MoneyToSpend { get; set; }

        public Button RefreshShopButton { get; set; }
        public Button UpgradeShopButton { get; set; }
        public Button BuySilverButton { get; set; }
        public Button BuyGoldButton { get; set; }
        public Button ShowCardsPlayedThisTurnButton { get; set; }

        public TextArea ViewShopDiscardText { get; set; }
        public List<Button> ViewShopDiscardButtons { get; set; }

        public TextArea ShopRefreshCostText { get; set; }
        public List<Button> ShopRefreshCostButtons { get; set; }

        public CardSelector CardSelector { get; set; }
        public Button AcceptButton { get; set; }

        public Button WorshipButton { get; set; }

        public TextArea MessageToPlayer { get; set; }

        public LogViewer LogViewer { get; set; }

        public ShopCostPicker ShopCostPicker { get; set; }

        public OptionPicker OptionPicker { get; set; }

        public bool ShopEnabled { get; set; }

        public GameWindow(Rectangle absoluteLocation, AssetManager assetManager, GameStateManager gameStateManager)
            : base(absoluteLocation, assetManager)
        {
            CorrespondingUiState = UiState.HotSeatGame;
            Texture = assetManager.LoadTexture("UI/wallpaper");
            GameState = gameStateManager;
            ShopEnabled = true;

            ViewShopDiscardButtons = new List<Button>();
            ShopRefreshCostButtons = new List<Button>();

            BuildUI();

            Update();
        }

        private void BuildUI()
        {
            GoRight();

            // Hand
            SetCursor(1.5, 78);
            ActivePlayerHand = new ActivePlayerHand(AssetManager);
            AddChild(ActivePlayerHand);

            // Money to spend
            SetCursor(84, 93);
            MoneyToSpend = new UiElement(AssetManager.LoadTexture("UI/coin"), AssetManager.LoadFont("buttonFont"), Color.Black, 4)
            {
                Text = "0",
                TextIsCentered = true
            };
            AddChild(MoneyToSpend);

            // Life
            SetCursor(73, 48);
            Life = new UiElement(AssetManager.LoadTexture("UI/life"), AssetManager.LoadFont("buttonFont"), Color.Black, 4) 
            { 
                Text = "20",
                TextIsCentered = true
            };
            AddChild(Life);

            // Leader
            SetCursor(60, 55);
            ActivePlayerLeader = new CardContainer(AssetManager, 20, false);
            AddChild(ActivePlayerLeader);

            // Deck
            AddSpacing(5);
            ActivePlayerDeck = new FaceDownCardStack(AssetManager, 20, false)
            {
                Text = "0",
                TextIsCentered = true
            };
            AddChild(ActivePlayerDeck);

            // Discard pile
            AddSpacing(.2);
            ActivePlayerDiscardPile = new CardContainer(AssetManager, 20, false, delegate
            {
                ShowDiscardViewer(true);
            });
            AddChild(ActivePlayerDiscardPile);

            // Shop
            SetCursor(1.5, 40);
            ActivePlayerShop = new Shop(AssetManager, 22, true);
            for (int i = 0; i < 3; i++)
            {
                int currentIndex = i;
                var cardContainer = new CardContainer(AssetManager, 33, true, delegate
                {
                    TryBuyCard(currentIndex);
                });
                ActivePlayerShop.AddChild(cardContainer);
                ActivePlayerShop.AddSpacing(.5);
            }
            AddChild(ActivePlayerShop);

            // Enemy discard pile
            GoLeft();
            SetCursor(98, 2);
            EnemyDiscardPile = new CardContainer(AssetManager, 20, false, delegate
            {
                ShowDiscardViewer(false);
            });
            AddChild(EnemyDiscardPile);

            // Enemy deck
            AddSpacing(.2);
            EnemyDeck = new FaceDownCardStack(AssetManager, 20, false)
            {
                Text = "0",
                TextIsCentered = true
            };
            AddChild(EnemyDeck);

            // Enemy leader
            GoRight();
            SetCursor(60, 2);
            EnemyLeader = new CardContainer(AssetManager, 20, false);
            AddChild(EnemyLeader);

            // Enemy hand
            AddSpacing(5);
            EnemyHand = new FaceDownCardStack(AssetManager, 20, false)
            {
                Text = "0",
                TextIsCentered = true
            };
            AddChild(EnemyHand);

            // Enemy shop
            SetCursor(1.5, 1.5);
            EnemyShop = new Shop(AssetManager, 22, true);
            for (int i = 0; i < 3; i++)
            {
                int currentIndex = i;
                var cardContainer = new CardContainer(AssetManager, 33, true);
                EnemyShop.AddChild(cardContainer);
                EnemyShop.AddSpacing(.5);
            }
            AddChild(EnemyShop);

            // Enemy life
            SetCursor(73, 23);
            EnemyLife = new UiElement(AssetManager.LoadTexture("UI/life"), AssetManager.LoadFont("buttonFont"), Color.Black, 4)
            {
                Text = "20",
                TextIsCentered = true
            };
            AddChild(EnemyLife);

            // Card stack viewer
            SetCursor(40, 3);
            CardStackViewer = new CardStackViewer(AssetManager, 72, false)
            {
                DrawLayer = 1
            };
            AddChild(CardStackViewer);

            // Hovered card viewer
            SetCursor(88, 65);
            HoveredCardViewer = new CardContainer(AssetManager, 11, true);
            HoveredCardViewer.CardTitle.TextFont = AssetManager.LoadFont("cardTitleFont_viewer");
            HoveredCardViewer.CardText.TextFont = AssetManager.LoadFont("cardTextFont_viewer");
            HoveredCardViewer.CardCost.TextFont = AssetManager.LoadFont("cardCostFont_viewer");
            HoveredCardViewer.CardSubTypes.TextFont = AssetManager.LoadFont("cardTextFont_viewer");
            HoveredCardViewer.CardCombatStats.TextFont = AssetManager.LoadFont("cardCombatStatsFont_viewer");
            HoveredCardViewer.CardCurrencyValue.TextFont = AssetManager.LoadFont("cardCurrencyValueFont_viewer");
            AddChild(HoveredCardViewer);

            // Button stack
            GoDown();
            SetCursor(89, 30);
            WorshipButton = new Button(AssetManager, ButtonType.Long, 10, true, "Worship", delegate
            {
                GameState.Worship();
            });
            AddChild(WorshipButton);
            AddSpacing(1);
            BuyGoldButton = new Button(AssetManager, ButtonType.Long, 10, true, "Buy gold (6)", delegate
            {
                GameState.BuyGold();
            });
            AddChild(BuyGoldButton);
            AddSpacing(1);
            UpgradeShopButton = new Button(AssetManager, ButtonType.Long, 10, true, "Upgrade shop (5)", delegate
            {
                UpgradeShop();
            });
            AddChild(UpgradeShopButton);
            AddSpacing(1);
            BuySilverButton = new Button(AssetManager, ButtonType.Long, 10, true, "Buy silver (3)", delegate
            {
                GameState.BuySilver();
            });
            AddChild(BuySilverButton);
            AddSpacing(1);
            RefreshShopButton = new Button(AssetManager, ButtonType.Long, 10, true, "Refresh shop", delegate
            {
                GameState.ActionRefreshShop();
            });
            AddChild(RefreshShopButton);
            AddSpacing(1);
            ViewLogButton = new Button(AssetManager, ButtonType.Long, 10, true, "View log", delegate
            {
                ViewLog();
            });
            AddChild(ViewLogButton);
            AddSpacing(1);
            EndTurnButton = new Button(AssetManager, ButtonType.Long, 10, true, "End turn", delegate
            {
                EndTurn();
            });
            AddChild(EndTurnButton);
            AddSpacing(-3.5);
            AcceptButton = new Button(AssetManager, ButtonType.Long, 10, true, "Accept", delegate
            {
                ResolveAccepted();
            })
            {
                IsVisible = false
            };
            AddChild(AcceptButton);

            GoRight();

            // Battlefield
            SetCursor(1.5, 7);
            Battlefield = new Battlefield(AssetManager, 57, true);
            Battlefield.AddButtons(this);
            AddChild(Battlefield);

            // Toggle shop button
            SetCursor(25, 1.5);
            ToggleShopButton = new Button(AssetManager, ButtonType.Long, 10, true, "Toggle shops", delegate
            {
                ToggleShopVisible();
            })
            {
                IsVisible = false
            };
            AddChild(ToggleShopButton);

            // Message
            SetCursor(1.5, 73);
            MessageToPlayer = new TextArea(AssetManager, "messageFont", 60, true, 10);
            AddChild(MessageToPlayer);

            // LogViewer
            SetCursor(28, 30);
            LogViewer = new LogViewer(AssetManager)
            {
                Texture = AssetManager.LoadTexture("UI/card_selector"),
                IsVisible = false,
                TextOffset = new Vector2(3, 3),
                TextColor = Color.White
            };
            AddChild(LogViewer);

            // View shop discard buttons
            SetCursor(1.5, 65);
            ViewShopDiscardText = new TextArea(AssetManager, "buttonFont", 9, true, 5)
            {
                Text = "View shop discard"
            };
            AddChild(ViewShopDiscardText);

            for (int i = 2; i <= 7; i++)
            {
                int currentIndex = i;
                var button = new Button(AssetManager, ButtonType.Short, 2.5, true, i.ToString(), delegate
                {
                    ViewShopDiscard(currentIndex);
                });
                ViewShopDiscardButtons.Add(button);
                AddChild(button);
                AddSpacing(.2);
            }

            // Shop refresh cost buttons
            SetCursor(1.5, 68);
            ShopRefreshCostText = new TextArea(AssetManager, "buttonFont", 9, true, 5)
            {
                Text = "Refresh pile"
            };
            AddChild(ShopRefreshCostText);
            for (int i = 2; i <= 7; i++)
            {
                int currentIndex = i;
                var button = new Button(AssetManager, ButtonType.Short, 2.5, true, i.ToString(), delegate
                {
                    SetShopRefreshCost(currentIndex);
                })
                {
                    IsVisible = currentIndex == 2,
                    IsSelected = currentIndex == 2
                };
                ShopRefreshCostButtons.Add(button);
                AddChild(button);
                AddSpacing(.2);
            }

            // Card selector
            SetCursor(35, 35);
            CardSelector = new CardSelector(AssetManager, 30, true);
            AddChild(CardSelector);

            // Shop cost picker
            SetCursor(40, 40);
            ShopCostPicker = new ShopCostPicker(AssetManager, 20, true);
            AddChild(ShopCostPicker);

            // Option picker
            SetCursor(40, 40);
            OptionPicker = new OptionPicker(AssetManager, 10, false);
            AddChild(OptionPicker);

            // View cards played this turn button
            SetCursor(81.2, 75.2);
            ShowCardsPlayedThisTurnButton = new Button(AssetManager, ButtonType.Short, 1.7, true, "...", delegate
            {
                CardStackViewer.Show(GameState.ActivePlayer.CardsPlayedThisTurn);
            });
            AddChild(ShowCardsPlayedThisTurnButton);

            RefreshHand();
        }

        private void EndTurn()
        {
            if (GameState.IsInCombat)
            {
                GameState.AddPublicLog($"{GameState.ActivePlayer.Name} passed");

                if (GameState.CombatPassed)
                {
                    GameState.ResolveCombat();
                }
                else
                {
                    GameState.CombatPassed = true;

                    GameState.SwitchActivePlayer();
                }
            }
            else
            {
                GameState.SwitchTurn();
            }
        }

        public void TryPlayCard(int containerIndex)
        {
            var card = ActivePlayerHand.Containers[containerIndex].Card;

            if (ActivePlayerHand.Containers[containerIndex].Card == null)
            {
                return;
            }

            bool cardTargetsOnPlay = CardTargetsOnPlay(card);

            if (GameState.TargetingCard != null)
            {
                if (GameState.TargetingCard == card && cardTargetsOnPlay)
                {
                    // We changed our mind about playing it
                    card.IsTargeting = false;
                    GameState.TargetedCards.ForEach(e => e.IsTargeted = false);
                    EndTurnButton.IsVisible = true;
                    AcceptButton.IsVisible = false;
                    GameState.RequireAccept = false;
                }
                else if (GameState.TargetingCard.TargetsHand)
                {
                    card.IsTargeted = !card.IsTargeted;
                }

                return;
            }

            if (GameState.CanPlayCard(true, card))
            {
                if (card.IsCombatCard && !GameState.IsInCombat)
                {
                    ToggleShopVisible();
                    ToggleShopButton.IsVisible = true;
                }

                if (cardTargetsOnPlay)
                {
                    card.IsTargeting = true;
                    GameState.RequireAccept = true;
                }
                else
                {
                    GameState.PlayCard(new PlayCardParams
                    {
                        Card = card,
                        IsActivePlayer = true
                    });
                }
            }

            if (GameState.IsInCombat)
            {
                Battlefield.Refresh(GameState);
            }

            RefreshHand();
        }

        private void TryBuyCard(int containerIndex)
        {
            var card = ActivePlayerShop.Cards[containerIndex].Card;

            if (card == null)
            {
                return;
            }

            if (GameState.TargetingCard != null)
            {
                if (GameState.TargetingCard.TargetsShop)
                {
                    card.IsTargeted = !card.IsTargeted;
                }

                return;
            }

            if (GameState.CanBuyCard(card))
            {
                GameState.BuyCard(true, card);
            }
        }

        private bool CardTargetsOnPlay(Card card)
        {
            return card.TargetsOnPlay || (card is CreatureCard && GameState.IsInCombat && !GameState.ActivePlayer.IsAttacking);
        }

        private void ShowDiscardViewer(bool isActivePlayer)
        {
            CardStackViewer.Show(isActivePlayer ? GameState.ActivePlayer.DiscardPile : GameState.Enemy.DiscardPile);
        }

        private void ToggleShopVisible()
        {
            ShopEnabled = !ShopEnabled;

            ActivePlayerShop.IsVisible = !ActivePlayerShop.IsVisible;
            EnemyShop.IsVisible = !EnemyShop.IsVisible;
            Battlefield.IsVisible = !Battlefield.IsVisible;
            ViewShopDiscardText.IsVisible = !ViewShopDiscardText.IsVisible;
            ViewShopDiscardButtons.ForEach(e => e.IsVisible = !e.IsVisible);
            ShopRefreshCostText.IsVisible = !ShopRefreshCostText.IsVisible;
        }

        private void ViewShopDiscard(int shopLevel)
        {
            CardStackViewer.Show(GameState.ShopDiscard.Where(e => e.Cost == shopLevel).ToList());
        }

        private void SetShopRefreshCost(int cost)
        {
            GameState.ActivePlayer.ShopRefreshCost = cost;
            ShopRefreshCostButtons.ForEach(e => e.IsSelected = false);
            ShopRefreshCostButtons.Single(e => e.Text == cost.ToString()).IsSelected = true;
        }

        public override IHoverable Hover(Point position)
        {
            var child = base.Hover(position);

            if (child is CardContainer container && container.Card != null)
            {
                ShowHoveredCard(container.Card);
            }
            else
            {
                HoveredCardViewer.Clear();
            }

            return child;
        }

        private void ViewLog()
        {
            LogViewer.Show(GameState.PublicLog);
        }

        private void UpgradeShop()
        {
            if (GameState.ActivePlayer.ShopLevel == 7)
            {
                return;
            }

            GameState.UpgradeShop();

            UpgradeShopButton.Text = $"Upgrade shop ({GameState.ActivePlayer.ShopLevel + 3})";
        }

        public override void LeftClick(Point position)
        {
            if (LogViewer.IsVisible)
            {
                LogViewer.IsVisible = false;
            }

            if (MessageToPlayer.IsVisible)
            {
                MessageToPlayer.IsVisible = false;
            }

            var child = FilterChildren(e => e.AbsoluteLocation.Contains(position) && e.IsVisible)
                .FirstOrDefault();

            if (child != CardStackViewer || (!GameState.RequireAccept && !GameState.ActivePlayer.CanFreeTrade))
            {
                CardStackViewer.IsVisible = false;
            }

            base.LeftClick(position);

            if (Battlefield.IsVisible)
            {
                Battlefield.Refresh(GameState);
            }

            RefreshHand();
        }

        private void RefreshHand()
        {
            var cardsInHand = GameState.ActivePlayer.Hand;
            ActivePlayerHand.SetCards(cardsInHand, this);
        }

        private void ShowHoveredCard(Card card)
        {
            HoveredCardViewer.SetCard(card);
        }

        private void ResolveAccepted()
        {
            var card = GameState.TargetingCard;
            var targets = GameState.TargetedCards;

            if (ShopCostPicker.IsVisible)
            {
                GameState.ShopCostPicked = ShopCostPicker.CostPicked;
            }
            else if (OptionPicker.IsVisible)
            {
                if (OptionPicker.OptionsPicked.Count > card.MaxTargets)
                {
                    SetMessageToPlayer(new MessageToPlayerParams
                    {
                        Message = "Too many options selected",
                        Severity = MessageSeverity.Error
                    });

                    return;
                }

                GameState.OptionsPicked = OptionPicker.OptionsPicked;
            }
            else
            {
                if (targets.Count < card.MinTargets)
                {
                    SetMessageToPlayer(new MessageToPlayerParams
                    {
                        Message = "Not enough targets",
                        Severity = MessageSeverity.Error
                    });

                    return;
                }

                if (targets.Any())
                {
                    if (card.MaxTargets < targets.Count)
                    {
                        SetMessageToPlayer(new MessageToPlayerParams
                        {
                            Message = "Too many targets",
                            Severity = MessageSeverity.Error
                        });

                        targets.ForEach(e => e.IsTargeted = false);
                        return;
                    }

                    foreach (var target in targets)
                    {
                        bool allValid = true;

                        if (!GameState.IsValidTarget(card, target))
                        {
                            target.IsTargeted = false;
                            allValid = false;
                        }

                        if (!allValid)
                        {
                            SetMessageToPlayer(new MessageToPlayerParams
                            {
                                Message = "Some targets were invalid. They were deselected",
                                Severity = MessageSeverity.Error
                            });
                            return;
                        }
                    }
                }
                else if (card is CreatureCard && !card.Owner.IsAttacking && !GameState.ResolvingAfterPlay)
                {
                    SetMessageToPlayer(new MessageToPlayerParams
                    {
                        Message = "Target a creature to block",
                        Severity = MessageSeverity.Error
                    });

                    return;
                }
            }

            GameState.RequireAccept = false;

            bool doneResolvingAfterplay = false;
            if (GameState.ResolvingAfterPlay)
            {
                card.IsBeingPlayed = true;

                IEnumerable<CardEffect> afterPlayEffects = card.Effects.Where(e => e.EffectPhase == CardEffectPhase.OnAcceptedAfterPlay);

                afterPlayEffects.SingleOrDefault(e => e.ResolveOrder == GameState.ResolvedEffects)?.Effect(GameState, card.Owner);
                GameState.ResolvedEffects++;

                if (GameState.ResolvedEffects >= afterPlayEffects.Count())
                {
                    doneResolvingAfterplay = true;
                }
            }
            else
            {
                CardEffect onAccepted = card.Effects.Where(e => e.EffectPhase == CardEffectPhase.OnAccepted).SingleOrDefault();
                onAccepted?.Effect(GameState, card.Owner);
            }

            if (GameState.IsInCombat)
            {
                Battlefield.Refresh(GameState);
            }

            if (!GameState.ResolvingAfterPlay || (GameState.ResolvingAfterPlay && doneResolvingAfterplay))
            {
                GameState.TargetingCard.IsTargeting = false;
            }

            targets.ForEach(e => e.IsTargeted = false);

            AcceptButton.IsVisible = false;
            EndTurnButton.IsVisible = true;

            if (CardTargetsOnPlay(card) && !GameState.ResolvingAfterPlay)
            {
                GameState.PlayCard(new PlayCardParams
                {
                    Card = card,
                    IsActivePlayer = true
                });
            }
            else if (GameState.ResolvingAfterPlay && doneResolvingAfterplay)
            {
                GameState.ResolvingAfterPlay = false;
                GameState.ResolvedEffects = 0;
                card.IsBeingPlayed = false;
                GameState.TriggerEvent(GameEvent.DoneResolving);
            }
        }

        private void SetMessageToPlayer(MessageToPlayerParams param)
        {
            MessageToPlayer.Text = param.Message;
            
            switch (param.Severity)
            {
                case MessageSeverity.Information: MessageToPlayer.TextColor = Color.Black; break;
                case MessageSeverity.Warning: MessageToPlayer.TextColor = Color.DarkOrange; break;
                case MessageSeverity.Error: MessageToPlayer.TextColor = Color.DarkRed; break;
            }

            MessageToPlayer.IsVisible = true;
        }

        public void ShowSupportsInPlay(bool isActivePlayer)
        {
            CardStackViewer.Show(GameState.GetPlayerSupports(isActivePlayer).Cast<Card>().ToList());
        }

        public override void Update()
        {
            if (GameState.ActivePlayer.Deck.Any())
            {
                ActivePlayerDeck.IsVisible = true;
                ActivePlayerDeck.Text = GameState.ActivePlayer.Deck.Count.ToString();
            }
            else
            {
                ActivePlayerDeck.IsVisible = false;
            }

            if (GameState.ActivePlayer.Leader != null)
            {
                ActivePlayerLeader.IsVisible = true;
                ActivePlayerLeader.SetCard(GameState.ActivePlayer.Leader);
            }
            else
            {
                ActivePlayerLeader.IsVisible = false;
                ActivePlayerLeader.Clear();
            }

            if (GameState.ActivePlayer.DiscardPile.Any())
            {
                ActivePlayerDiscardPile.IsVisible = true;
                ActivePlayerDiscardPile.SetCard(GameState.ActivePlayer.DiscardPile.Last());
            }
            else
            {
                ActivePlayerDiscardPile.IsVisible = false;
                ActivePlayerDiscardPile.Clear();
            }


            if (GameState.Enemy.Hand.Any())
            {
                EnemyHand.IsVisible = true;
                EnemyHand.Text = GameState.Enemy.Hand.Count.ToString();
            }
            else
            {
                EnemyDeck.IsVisible = false;
            }

            if (GameState.Enemy.Deck.Any())
            {
                EnemyDeck.IsVisible = true;
                EnemyDeck.Text = GameState.Enemy.Deck.Count.ToString();
            }
            else
            {
                EnemyDeck.IsVisible = false;
            }

            if (GameState.Enemy.Leader != null)
            {
                EnemyLeader.IsVisible = true;
                EnemyLeader.SetCard(GameState.Enemy.Leader);
            }
            else
            {
                EnemyLeader.IsVisible = false;
                EnemyLeader.Clear();
            }

            if (GameState.Enemy.DiscardPile.Any())
            {
                EnemyDiscardPile.IsVisible = true;
                EnemyDiscardPile.SetCard(GameState.Enemy.DiscardPile.Last());
            }
            else
            {
                EnemyDiscardPile.IsVisible = false;
                EnemyDiscardPile.Clear();
            }

            if (ActivePlayerShop.IsVisible)
            {
                ActivePlayerShop.SetCards(GameState.ActivePlayer.Shop);
            }

            if (EnemyShop.IsVisible)
            {
                EnemyShop.SetCards(GameState.Enemy.Shop);
            }

            if (GameState.ActivePlayer.MoneyToSpend > 0)
            {
                MoneyToSpend.IsVisible = true;
                MoneyToSpend.Text = GameState.ActivePlayer.MoneyToSpend.ToString();
            }
            else
            {
                MoneyToSpend.IsVisible = false;
            }

            if (GameState.RequireAccept)
            {
                EndTurnButton.IsVisible = false;
                AcceptButton.IsVisible = true;
            }

            Life.Text = GameState.ActivePlayer.Life.ToString();
            EnemyLife.Text = GameState.Enemy.Life.ToString();

            BuySilverButton.IsVisible = GameState.ActivePlayer.MoneyToSpend >= 3;
            UpgradeShopButton.IsVisible = GameState.ActivePlayer.MoneyToSpend >= GameState.ActivePlayer.ShopLevel + 3;
            BuyGoldButton.IsVisible = GameState.ActivePlayer.MoneyToSpend >= 6;
            WorshipButton.IsVisible = GameState.ActivePlayer.MoneyToSpend >= 6 && GameState.ActivePlayer.Leader?.Name == "Eva";

            if (GameState.ActivePlayer.FreeShopRefreshes > 0)
            {
                RefreshShopButton.Text = "Refresh shop";
            }
            else if (GameState.ActivePlayer.MoneyToSpend >= 1)
            {
                RefreshShopButton.Text = "Refresh shop (1)";
            }
            else
            {
                RefreshShopButton.Text = "Refresh shop (3 life)";
            }

            if (ShopEnabled)
            {
                ShopRefreshCostButtons.ForEach(e => e.IsVisible = int.Parse(e.Text) <= GameState.ActivePlayer.ShopLevel);
                ShopRefreshCostButtons.ForEach(e => e.IsSelected = false);
                ShopRefreshCostButtons.Single(e => e.Text == GameState.ActivePlayer.ShopRefreshCost.ToString()).IsSelected = true;
            }
            else
            {
                ShopRefreshCostButtons.ForEach(e => e.IsVisible = false);
            }

            UpdateCardSelector();

            if (GameState.RevealOpponentHand && !CardStackViewer.IsVisible)
            {
                CardStackViewer.Show(GameState.Enemy.Hand);
            }

            if (Battlefield.IsVisible && !GameState.IsInCombat)
            {
                ToggleShopVisible();
                ToggleShopButton.IsVisible = false;
            }

            if (GameState.MessageToPlayer != null)
            {
                SetMessageToPlayer(GameState.MessageToPlayer);
                GameState.MessageToPlayer = null;
            }

            if (GameState.ActivateShopCostPickerFlag)
            {
                ShopCostPicker.IsVisible = true;
                GameState.ActivateShopCostPickerFlag = false;
            }
            else if (GameState.RemoveShopCostPickerFlag)
            {
                ShopCostPicker.Reset();
                GameState.RemoveShopCostPickerFlag = false;
            }

            if (GameState.RemoveOptionsPickerFlag)
            {
                OptionPicker.IsVisible = false;
                GameState.RemoveOptionsPickerFlag = false;
            }
            else if (GameState.OptionsPickerOptions != null)
            {
                OptionPicker.Show(GameState.OptionsPickerOptions);
                GameState.OptionsPickerOptions = null;
            }

            ShowCardsPlayedThisTurnButton.IsVisible = GameState.ActivePlayer.CardsPlayedThisTurn.Any();

            if (GameState.ActionQueued != null && !GameState.RequireAccept)
            {
                GameState.PlayActionQueued();
                RefreshHand();
            }

            if (GameState.PlayerIsFreeTradeBuying())
            {
                var boughtCard = GameState.TryBuyCardFromDiscardPile();
                var cards = new List<Card>(CardStackViewer.Cards);
                cards.Remove(boughtCard);
                CardStackViewer.Show(cards);
            }
        }

        public void UpdateCardSelector()
        {
            if (GameState.ActivateSelectorFlag)
            {
                GameState.ActivateSelectorFlag = false;
                CardSelector.SetCards(GameState.CardsToChooseFromSelector);
                CardSelector.IsVisible = true;
                EndTurnButton.IsVisible = false;
                AcceptButton.IsVisible = true;
            }
            else if (GameState.RemoveSelectorFlag)
            {
                GameState.RemoveSelectorFlag = false;
                CardSelector.IsVisible = false;
                CardSelector.Clear();
                EndTurnButton.IsVisible = true;
                AcceptButton.IsVisible = false;
                
                foreach (var card in GameState.CardsToChooseFromSelector)
                {
                    card.IsTargeted = false;
                }

                GameState.CardsToChooseFromSelector.Clear();
            }
        }
    }
}
