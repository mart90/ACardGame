using Microsoft.Xna.Framework;
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

        public Button(AssetManager assetManager, ButtonType type, double relativeSize, bool sizeExpressedInX, string text, Action onClickAction, double? aspectRatio = null)
            : base(type == ButtonType.Long ? assetManager.LoadTexture("UI/button_long") : assetManager.LoadTexture("UI/button_short"), relativeSize, sizeExpressedInX, aspectRatio)
        {
            OnLeftClickAction = onClickAction;
            TextIsCentered = true;
            Text = text;
            TextColor = Color.White;
            TextFont = assetManager.LoadFont("buttonFont");
        }
    }
}
