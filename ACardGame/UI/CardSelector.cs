using ACardGameLibrary;
using System.Collections.Generic;

namespace ACardGame.UI
{
    public class CardSelector : UiContainer
    {        
        public IEnumerable<CardContainer> Cards => ChildrenOfType<CardContainer>();

        public CardSelector(AssetManager assetManager, double relativeSize, bool sizeExpressedInX)
            : base(assetManager, 2, relativeSize, sizeExpressedInX)
        {
            Texture = AssetManager.LoadTexture("UI/card_selector");
            IsVisible = false;
            DrawLayer = 5;
        }

        public void SetCards(List<Card> cards)
        {
            RelativeLocationInParent.X = 50 - 4 * cards.Count;
            RelativeSizeInParentX = cards.Count * 8;
            AspectRatio = cards.Count / 1.7;
            double cardSizeX = (96 - 3f) / cards.Count;

            SetCursor(2, 5);

            for (int i = 0; i < cards.Count; i++)
            {
                var cardContainer = new CardContainer(AssetManager, cardSizeX, true);
                cardContainer.SetCard(cards[i]);
                cardContainer.OnLeftClickAction = delegate
                {
                    cardContainer.ToggleTargeted();
                };
                AddChild(cardContainer);
                AddSpacing(3f / cards.Count);
            }
        }

        public void Clear()
        {
            ClearChildren();
        }
    }
}
