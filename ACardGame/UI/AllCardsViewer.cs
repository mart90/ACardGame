using ACardGameLibrary;
using System.Collections.Generic;
using System.Linq;

namespace ACardGame.UI
{
    public class AllCardsViewer : UiContainer
    {
        public List<CardStackViewer> CardStackViewers { get; set; }

        public AllCardsViewer(AssetManager assetManager, double relativeSize, bool sizeExpressedInX)
            : base(assetManager, 1.2, relativeSize, sizeExpressedInX)
        {
            Texture = assetManager.LoadTexture("UI/card_selector");
            IsVisible = false;
            CardStackViewers = new List<CardStackViewer>();
        }

        public void Build(List<Card> cards)
        {
            SetCursor(2, 2);

            for (int i = 2; i <= 7; i++)
            {
                var cardStack = new CardStackViewer(AssetManager, 16, true);
                CardStackViewers.Add(cardStack);
                AddChild(cardStack);

                cardStack.Show(cards.Where(e => e.Cost == i).ToList(), $"{i}-cost:");
                cardStack.GoToTop();
            }
        }
    }
}
