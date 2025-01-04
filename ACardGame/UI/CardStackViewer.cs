using ACardGameLibrary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ACardGame.UI
{
    public class CardStackViewer : UiContainer
    {
        public List<Card> Cards { get; set; }

        public List<CardContainer> Containers => ChildrenOfType<CardContainer>().ToList();

        private readonly UiElement _goToTop;
        private readonly UiElement _goToBottom;

        private int _firstCardIndex;

        private const int MAX_CARDS = 16;

        public CardStackViewer(AssetManager assetManager, double relativeSize, bool sizeExpressedInX)
            : base(assetManager, 0.2, relativeSize, sizeExpressedInX)
        {
            DrawLayer = 4;
            Texture = AssetManager.LoadTexture("UI/card_stack_viewer");
            GoDown();
            IsVisible = false;

            SetCursor(40, 1.5);
            _goToTop = new UiElement(assetManager.LoadTexture("UI/go_top"), 20, true)
            {
                IsVisible = false
            };
            AddChild(_goToTop);

            SetCursor(40, 96.5);
            _goToBottom = new UiElement(assetManager.LoadTexture("UI/go_bottom"), 20, true)
            {
                IsVisible = false
            };
            AddChild(_goToBottom);

            SetContainers();
        }

        public void Show(List<Card> cards)
        {
            Containers.ForEach(e => e.Clear());

            IsVisible = true;
            Cards = cards;

            if (cards.Count > MAX_CARDS)
            {
                _firstCardIndex = cards.Count - MAX_CARDS;
                _goToTop.IsVisible = true;
            }
            else
            {
                _firstCardIndex = 0;
            }

            SetCards();
        }

        private void SetContainers()
        {
            int startX = 8;
            int startY = 6;

            SetCursor(startX, startY);

            for (int i = 0; i < MAX_CARDS; i++)
            {
                var cardContainer = new CardContainer(AssetManager, 84, true, null)
                {
                    DrawLayer = i
                };
                cardContainer.OnLeftClickAction = delegate
                {
                    cardContainer.ToggleTargeted();
                };
                AddChild(cardContainer);
                SetCursor(startX, startY + 4 * (i + 1));
            }
        }

        private void SetCards()
        {
            var cards = new List<Card>(Cards);
            cards.RemoveRange(0, _firstCardIndex);

            for (int i = 0; i < MAX_CARDS && i < cards.Count; i++)
            {
                Containers[i].SetCard(cards[i]);
            }
        }

        public override void ScrollUp(Point position)
        {
            if (Cards.Count <= MAX_CARDS || _firstCardIndex == 0)
            {
                return;
            }

            _firstCardIndex--;

            _goToBottom.IsVisible = true;

            if (_firstCardIndex == 0)
            {
                _goToTop.IsVisible = false;
            }

            SetCards();
        }

        public override void ScrollDown(Point position)
        {
            if (Cards.Count - _firstCardIndex <= MAX_CARDS)
            {
                return;
            }

            _firstCardIndex++;

            _goToTop.IsVisible = true;

            if (Cards.Count - _firstCardIndex == MAX_CARDS)
            {
                _goToBottom.IsVisible = false;
            }

            SetCards();
        }
    }
}
