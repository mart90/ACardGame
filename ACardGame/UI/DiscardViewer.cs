using ACardGameLibrary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace ACardGame.UI
{
    public class DiscardViewer : UiContainer
    {
        public List<CardContainer> Cards => Children.Where(e => e is CardContainer).Cast<CardContainer>().ToList();

        public bool IsActivePlayer { get; set; }

        public DiscardViewer(AssetManager assetManager, double relativeSize, bool sizeExpressedInX)
            : base(assetManager, 0.15, relativeSize, sizeExpressedInX)
        {
            GoDown();
            IsVisible = false;
        }

        public void SetCards(List<Card> cards)
        {
            Children.Clear();

            int startY = 75 - 4 * cards.Count;
            SetCursor(0, startY);

            for (int i = 0; i < cards.Count; i++)
            {
                var cardContainer = new CardContainer(AssetManager, 100, true, null);
                cardContainer.SetCard(cards[i]);
                AddChild(cardContainer);
                SetCursor(0, startY + 4 * (i + 1));
            }
        }

        protected override UiElement GetHoveredChildRecursive(Point position, UiContainer parent)
        {
            var y = position.Y;

            var candidates = new List<CardContainer>(Cards);

            candidates.RemoveAll(e => e.AbsoluteLocation.Y > y);

            var hoveredChild = candidates.SingleOrDefault(e => e.AbsoluteLocation.Y == candidates.Max(e => e.AbsoluteLocation.Y));

            return hoveredChild ?? (UiElement)this;
        }
    }
}
