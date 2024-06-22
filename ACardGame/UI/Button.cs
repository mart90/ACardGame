using Microsoft.Xna.Framework.Graphics;
using System;

namespace ACardGame.UI
{
    public class Button : UiElement, ILeftClickable, IHoverable
    {
        public Action OnLeftClickAction { get; set; }
        public bool IsBeingLeftClicked { get; set; }
        public Texture2D BeingLeftClickedTexture { get; set; }

        public Texture2D HoverTexture { get; set; }
        public string ToolTipOnHover { get; set; }
        public Action OnHoverAction { get; set; }

        public Button(Texture2D backgroundTexture, double relativeSize, bool sizeExpressedInX, Action onClickAction, double? aspectRatio = null)
            : base(backgroundTexture, relativeSize, sizeExpressedInX, aspectRatio)
        {
            OnLeftClickAction = onClickAction;
        }
    }
}
