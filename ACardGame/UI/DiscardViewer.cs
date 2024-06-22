using ACardGameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACardGame.UI
{
    public class DiscardViewer : UiContainer, IHoverable
    {
        public List<Card> Cards { get; private set; }

        public Texture2D HoverTexture { get; set; }
        public string ToolTipOnHover { get; set; }
        public Action OnHoverAction { get; set; }

        public DiscardViewer(AssetManager assetManager, double relativeSize, bool sizeExpressedInX)
            : base(assetManager, 0.15, relativeSize, sizeExpressedInX)
        {
            Cards = new List<Card>();
            GoDown();
            IsVisible = false;
        }

        public void SetCards(List<Card> cards)
        {
            Children.Clear();

            int startY = 80 - 4 * cards.Count;
            SetCursor(0, startY);

            Cards = cards;

            for (int i = 0; i < cards.Count; i++)
            {
                AddChild(new UiElement(AssetManager.LoadCardTexture(cards[i].Name), 100, true));
                SetCursor(0, startY + 4 * (i + 1));
            }
        }

        public Card GetHoveredCard(Point position)
        {
            var y = position.Y;

            var candidates = new List<UiElement>(Children);

            candidates.RemoveAll(e => e.AbsoluteLocation.Y > y);

            var hoveredChild = candidates.SingleOrDefault(e => e.AbsoluteLocation.Y == candidates.Max(e => e.AbsoluteLocation.Y));

            return hoveredChild == null ? null : Cards[Children.IndexOf(hoveredChild)];
        }
    }
}
