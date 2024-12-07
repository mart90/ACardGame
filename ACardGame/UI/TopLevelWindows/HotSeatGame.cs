using ACardGameLibrary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACardGame.UI
{
    public class HotSeatGame : TopLevelUiWindow
    {
        public GameStateManager GameState { get; private set; }

        public List<CardContainer> ActivePlayerHand { get; private set; }
        public FaceDownCardStack ActivePlayerDeck { get; set; }
        public CardContainer ActivePlayerDiscardPile { get; set; }

        public FaceDownCardStack EnemyDeck { get; set; }
        public FaceDownCardStack EnemyHand { get; set; }
        public CardContainer EnemyDiscardPile { get; set; }

        public CardContainer ActivePlayerLeader { get; set; }
        public CardContainer EnemyLeader { get; set; }

        public Shop Shop { get; set; }

        public Battlefield Battlefield { get; set; }

        public DiscardViewer DiscardViewer { get; set; }
        public CardContainer HoveredCardViewer { get; set; }

        public Button PassButton { get; set; }
        public Button ToggleShopButton { get; set; }

        public bool PlayerIsTargeting { get; set; }
        public Action TargetedCallback { get; set; }
        public List<Card> TargetedCards => Shop.Children.Concat(Battlefield.Children).Concat(DiscardViewer.Children)
            .Where(e => e is CardContainer cc && cc.Card != null && cc.IsTargeted)
            .Select(e => ((CardContainer)e).Card)
            .ToList();

        public HotSeatGame(Rectangle absoluteLocation, AssetManager assetManager, GameStateManager gameStateManager)
            : base(absoluteLocation, assetManager)
        {
            CorrespondingUiState = UiState.HotSeatGame;
            Texture = assetManager.LoadTexture("UI/wallpaper");
            GameState = gameStateManager;
            ActivePlayerHand = new List<CardContainer>();

            BuildUI();

            Update();
        }

        private void BuildUI()
        {
            GoRight();

            // Hand
            SetCursor(1.5, 78);
            for (int i = 0; i < 12; i++)
            {
                int currentIndex = i;
                var cardContainer = new CardContainer(AssetManager, 20, false, delegate
                {
                    TryPlayCard(currentIndex);
                });
                ActivePlayerHand.Add(cardContainer);
                AddChild(cardContainer);
                AddSpacing(.2);
            }

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
                ToggleDiscardViewer(true);
            });
            AddChild(ActivePlayerDiscardPile);

            // Enemy discard pile
            GoLeft();
            SetCursor(98, 2);
            EnemyDiscardPile = new CardContainer(AssetManager, 20, false, delegate
            {
                ToggleDiscardViewer(false);
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

            // Discard viewer
            SetCursor(50, 0);
            DiscardViewer = new DiscardViewer(AssetManager, 80, false);
            AddChild(DiscardViewer);

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

            // Shop
            SetCursor(1.5, 1.5);
            Shop = new Shop(AssetManager, 34, true);
            for (int i = 0; i < 5; i++)
            {
                int currentIndex = i;
                var cardContainer = new CardContainer(AssetManager, 20, true, delegate
                {
                    TryBuyCard(currentIndex);
                });
                Shop.AddChild(cardContainer);
                Shop.AddSpacing(.5);
            }
            AddChild(Shop);

            // Pass button
            SetCursor(89, 55);
            PassButton = new Button(AssetManager, ButtonType.Long, 10, true, "Pass", delegate
            {
                Pass();
            });
            AddChild(PassButton);

            // Battlefield
            SetCursor(1.5, 12);
            Battlefield = new Battlefield(AssetManager, 50, true);
            AddChild(Battlefield);

            // Toggle shop button
            SetCursor(40, 1.5);
            ToggleShopButton = new Button(AssetManager, ButtonType.Long, 10, true, "Toggle shop", delegate
            {
                ToggleShop();
            })
            {
                IsVisible = false
            };
            AddChild(ToggleShopButton);
        }

        private void TryPlayCard(int containerIndex)
        {
            if (PlayerIsTargeting)
            {
                return;
            }

            var card = ActivePlayerHand[containerIndex].Card;

            if (ActivePlayerHand[containerIndex].Card == null)
            {
                return;
            }

            if (GameState.CanPlayCard(true, card))
            {
                if (card.GetMainType() == CardType.Creature && GameState.IsInCombat && !GameState.ActivePlayer.IsAttacking)
                {
                    PlayerIsTargeting = true;

                    TargetedCallback = delegate
                    {
                        GameState.PlayCard(new PlayCardParams
                        {
                            Card = card,
                            IsActivePlayer = true,
                            IsBlockingCreature = (CreatureCard)TargetedCards.First()
                        });
                    };

                    return;
                }

                GameState.PlayCard(new PlayCardParams
                {
                    Card = card,
                    IsActivePlayer = true
                });
            }

            if (GameState.IsInCombat)
            {
                Shop.IsVisible = false;
                ToggleShopButton.IsVisible = true;
                Battlefield.IsVisible = true;
            }
        }

        private void TryBuyCard(int containerIndex)
        {
            if (Shop.Cards.Count <= containerIndex)
            {
                return;
            }

            var card = Shop.Cards[containerIndex].Card;

            if (GameState.CanBuyCard(card))
            {
                GameState.BuyCard(true, card);
            }
        }

        private void Pass()
        {
            GameState.Pass(GameState.ActivePlayer);
        }

        private void ToggleDiscardViewer(bool isActivePlayer)
        {
            if (DiscardViewer.IsVisible && DiscardViewer.IsActivePlayer == isActivePlayer)
            {
                DiscardViewer.IsVisible = false;
                return;
            }

            DiscardViewer.SetCards(isActivePlayer ? GameState.ActivePlayer.DiscardPile : GameState.Enemy.DiscardPile);
            DiscardViewer.IsActivePlayer = isActivePlayer;
            DiscardViewer.IsVisible = true;
        }

        private void ToggleShop()
        {
            Shop.IsVisible = !Shop.IsVisible;
            Battlefield.IsVisible = !Battlefield.IsVisible;
        }

        private void EndCombat()
        {
            // TODO
            // Hide battlefield
            // Hide toggle shop button
            // Show shop
        }

        public override void Hover(Point position)
        {
            var child = GetHoveredChildRecursive(position, this);

            if (child == this)
            {
                HoveredCardViewer.Clear();
                return;
            }

            if (child is CardContainer container && container.Card != null)
            {
                ShowHoveredCard(container.Card.Name);
            }
            else 
            {
                HoveredCardViewer.Clear();
            }
        }

        public override void LeftClick(Point position)
        {
            base.LeftClick(position);

            var child = Children
                .Where(e => e.AbsoluteLocation.Contains(position) && e.IsVisible)
                .FirstOrDefault();

            if (child == null)
            {
                return;
            }

            // TODO?
        }

        private void ShowHoveredCard(string name)
        {
            HoveredCardViewer.SetCard(CardLibrary.GetCard(name));
        }

        public override void Update()
        {
            if (PlayerIsTargeting && TargetedCards.Any())
            {
                TargetedCallback();
                TargetedCallback = null;
                PlayerIsTargeting = false;
            }

            ActivePlayerHand.ForEach(e => e.Clear());
            var cardsInHand = GameState.ActivePlayer.Hand;
            for (int i = 0; i < cardsInHand.Count; i++)
            {
                if (cardsInHand[i] != null)
                {
                    ActivePlayerHand[i].SetCard(cardsInHand[i]);
                }
            }

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
            }

            if (GameState.ActivePlayer.DiscardPile.Any())
            {
                ActivePlayerDiscardPile.IsVisible = true;
                ActivePlayerDiscardPile.SetCard(GameState.ActivePlayer.DiscardPile.Last());
            }
            else
            {
                ActivePlayerDiscardPile.IsVisible = false;
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
            }

            if (GameState.Enemy.DiscardPile.Any())
            {
                EnemyDiscardPile.IsVisible = true;
                EnemyDiscardPile.SetCard(GameState.Enemy.DiscardPile.Last());
            }
            else
            {
                EnemyDiscardPile.IsVisible = false;
            }

            if (Shop.IsVisible)
            {
                Shop.SetCards(GameState.CurrentShop);
            }

            if (Battlefield.IsVisible)
            {
                Battlefield.Refresh(GameState);
            }
        }
    }
}
