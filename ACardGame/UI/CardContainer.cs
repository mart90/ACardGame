using ACardGameLibrary;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ACardGame.UI
{
    public class CardContainer : UiElement, ILeftClickable, IHoverable
    {
        public Card Card { get; set; }

        public bool IsBeingLeftClicked { get; set; }
        public Texture2D BeingLeftClickedTexture { get; set; }
        public Action OnLeftClickAction { get; set; }

        public Texture2D HoverTexture { get; set; }
        public string ToolTipOnHover { get; set; }
        public Action OnHoverAction { get; set; }

        public CardContainer(double relativeSize, bool sizeExpressedInX, Action onClickAction)
            : base(0.6, relativeSize, sizeExpressedInX)
        {
            OnLeftClickAction = onClickAction;
        }
    }
}
