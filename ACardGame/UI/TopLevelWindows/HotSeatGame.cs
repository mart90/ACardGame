using ACardGameLibrary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ACardGame.UI
{
    public class HotSeatGame : TopLevelUiWindow
    {
        public GameStateManager GameState { get; private set; }

        public List<CardContainer> ActivePlayerHand { get; private set; }
        public UiElement ActivePlayerDeck { get; set; }
        public UiElement ActivePlayerDiscardPile { get; set; }
        public UiElement EnemyDiscardPile { get; set; }
        public UiElement EnemyDeck { get; set; }
        public UiElement EnemyHand { get; set; }

        public DiscardViewer DiscardViewer { get; set; }

        public UiElement HoveredCardViewer { get; set; }

        public HotSeatGame(Rectangle absoluteLocation, AssetManager assetManager, GameStateManager gameStateManager)
            : base(absoluteLocation, assetManager)
        {
            CorrespondingUiState = UiState.HotSeatGame;
            Texture = assetManager.LoadTexture("UI/wallpaper");
            GameState = gameStateManager;
            ActivePlayerHand = new List<CardContainer>();

            BuildUI();

            LoadActivePlayerBoardState();
        }

        private void BuildUI()
        {
            GoRight();

            // Hand
            SetCursor(1.5, 78);
            for (int i = 0; i < 12; i++)
            {
                var cardContainer = new CardContainer(20, false, null);
                ActivePlayerHand.Add(cardContainer);
                AddChild(cardContainer);
                AddSpacing(.2);
            }

            // Deck
            SetCursor(71, 55);
            ActivePlayerDeck = new UiElement(AssetManager.LoadTexture("UI/cardback"), 20, false);
            AddChild(ActivePlayerDeck);

            // Discard pile
            AddSpacing(.2);
            ActivePlayerDiscardPile = new CardContainer(20, false, delegate
            {
                ShowActivePlayerDiscardPile();
            });
            AddChild(ActivePlayerDiscardPile);

            GoLeft();

            // Enemy discard pile
            SetCursor(98, 2);
            EnemyDiscardPile = new CardContainer(20, false, delegate
            {
                ShowEnemyDiscardPile();
            });
            AddChild(EnemyDiscardPile);

            // Enemy deck
            AddSpacing(.2);
            EnemyDeck = new UiElement(AssetManager.LoadTexture("UI/cardback"), 20, false);
            AddChild(EnemyDeck);

            // Enemy hand
            GoRight();
            SetCursor(71, 2);
            EnemyHand = new UiElement(AssetManager.LoadTexture("UI/cardback"), 20, false);
            AddChild(EnemyHand);

            // Discard viewer
            SetCursor(60, 2);
            DiscardViewer = new DiscardViewer(AssetManager, 73, false);
            AddChild(DiscardViewer);

            // Hovered card viewer
            SetCursor(88, 65);
            HoveredCardViewer = new UiElement(0.6, 11, true);
            AddChild(HoveredCardViewer);
        }

        private void PlayCard(Card card)
        {
            // TODO
        }

        private void ShowActivePlayerDiscardPile()
        {
            DiscardViewer.SetCards(GameState.ActivePlayer.DiscardPile);
            DiscardViewer.IsVisible = true;
        }

        private void ShowEnemyDiscardPile()
        {
            DiscardViewer.SetCards(GameState.Enemy.DiscardPile);
            DiscardViewer.IsVisible = true;
        }

        public override void Hover(Point position)
        {
            var child = Children.Where(e => e.AbsoluteLocation.Contains(position)).FirstOrDefault();

            if (child == null || !child.IsVisible)
            {
                HoveredCardViewer.Texture = null;
                return;
            }

            if (child is CardContainer container && container.Card != null)
            {
                ShowHoveredCard(container.Card.Name);
            }
            else if (child is DiscardViewer discardViewer)
            {
                var hoveredCard = discardViewer.GetHoveredCard(position);
                if (hoveredCard != null)
                {
                    ShowHoveredCard(hoveredCard.Name);
                }
            }
            else 
            {
                HoveredCardViewer.Texture = null;
            }
        }

        public override void LeftClick(Point position)
        {
            base.LeftClick(position);

            var child = Children.Where(e => e.AbsoluteLocation.Contains(position)).FirstOrDefault();

            if (child == null || !child.IsVisible)
            {
                return;
            }

            if (child is CardContainer container && container.Card != null)
            {
                PlayCard(container.Card);
            }
        }

        private void ShowHoveredCard(string name)
        {
            HoveredCardViewer.Texture = AssetManager.LoadCardTexture(name);
        }

        public void LoadActivePlayerBoardState()
        {
            var cardsInHand = GameState.ActivePlayer.CardsInHand;
            for (int i = 0; i < cardsInHand.Count; i++)
            {
                ActivePlayerHand[i].Card = cardsInHand[i];
                ActivePlayerHand[i].Texture = AssetManager.LoadCardTexture(cardsInHand[i].Name);
            }

            if (GameState.ActivePlayer.Deck.Any())
            {
                ActivePlayerDeck.IsVisible = true;
            }
            else
            {
                ActivePlayerDeck.IsVisible = false;
            }

            if (GameState.ActivePlayer.DiscardPile.Any())
            {
                ActivePlayerDiscardPile.IsVisible = true;
                ActivePlayerDiscardPile.Texture = AssetManager.LoadCardTexture(GameState.ActivePlayer.DiscardPile.Last().Name);
            }
            else
            {
                ActivePlayerDiscardPile.IsVisible = false;
            }


            if (GameState.Enemy.CardsInHand.Any())
            {
                EnemyDeck.IsVisible = true;
            }
            else
            {
                EnemyDeck.IsVisible = false;
            }

            if (GameState.Enemy.Deck.Any())
            {
                EnemyDeck.IsVisible = true;
            }
            else
            {
                EnemyDeck.IsVisible = false;
            }

            if (GameState.Enemy.DiscardPile.Any())
            {
                EnemyDiscardPile.IsVisible = true;
                EnemyDiscardPile.Texture = AssetManager.LoadCardTexture(GameState.Enemy.DiscardPile.Last().Name);
            }
            else
            {
                EnemyDiscardPile.IsVisible = false;
            }
        }

        public void SwitchTurn()
        {
            ActivePlayerHand.Clear();
            GameState.SwitchTurn();
            LoadActivePlayerBoardState();
        }
    }
}
