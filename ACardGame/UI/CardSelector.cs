using ACardGameLibrary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ACardGame.UI
{
    public class CardSelector : UiContainer
    {        
        public List<Card> Cards { get; set; }
        public List<CardContainer> Containers => ChildrenOfType<CardContainer>().ToList();

        public UiElement GoLeftIcon { get; set; }
        public UiElement GoRightIcon { get; set; }

        private int _firstCardIndex;

        public CardSelector(AssetManager assetManager, double relativeSize, bool sizeExpressedInX)
            : base(assetManager, 2, relativeSize, sizeExpressedInX)
        {
            Texture = AssetManager.LoadTexture("UI/card_selector");
            IsVisible = false;
            DrawLayer = 5;
            _firstCardIndex = 0;
        }

        public void Build(List<Card> cards)
        {
            Clear();

            Cards = cards;
            int cardsCount = cards.Count > 10 ? 10 : cards.Count;

            RelativeLocationInParent.X = 50 - 4 * cardsCount - 2;
            RelativeSizeInParentX = cardsCount * 8 + 4;
            AspectRatio = cardsCount / 1.7;
            double cardSizeX = (94 - 3f) / cardsCount;

            SetCursor(3, 5);

            for (int i = 0; i < cardsCount; i++)
            {
                var cardContainer = new CardContainer(AssetManager, cardSizeX, true);
                cardContainer.OnLeftClickAction = delegate
                {
                    cardContainer.ToggleTargeted();
                };
                AddChild(cardContainer);
                AddSpacing(3f / cardsCount);
            }

            SetCursor(0.5, 42);
            GoLeftIcon = new UiElement(AssetManager.LoadTexture("UI/go_left"), 16, false)
            {
                IsVisible = false
            };
            AddChild(GoLeftIcon);

            SetCursor(97.5, 42);
            GoRightIcon = new UiElement(AssetManager.LoadTexture("UI/go_right"), 16, false)
            {
                IsVisible = cards.Count > 10
            };
            AddChild(GoRightIcon);

            InsertCards();
        }

        public void InsertCards()
        {
            for (int i = 0; i < Cards.Count && i < 10; i++)
            {
                Containers[i].SetCard(Cards[i + _firstCardIndex]);
            }
        }

        public void Clear()
        {
            ClearChildren();
        }

        public override void ScrollDown(Point position)
        {
            if (_firstCardIndex < Cards.Count - 10)
            {
                _firstCardIndex++;
                GoLeftIcon.IsVisible = true;

                if (_firstCardIndex == Cards.Count - 10)
                {
                    GoRightIcon.IsVisible = false;
                }
            }

            InsertCards();
        }

        public override void ScrollUp(Point position)
        {
            if (_firstCardIndex > 0)
            {
                _firstCardIndex--;
                GoRightIcon.IsVisible = true;

                if (_firstCardIndex == 0)
                {
                    GoLeftIcon.IsVisible = false;
                }
            }

            InsertCards();
        }
    }
}
