using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ACardGame.UI
{
    public class Button : UiElement, ILeftClickable, IHoverable, ISelectable
    {
        public Action OnLeftClickAction { get; set; }
        public bool IsBeingLeftClicked { get; set; }
        public Texture2D BeingLeftClickedTexture { get; set; }

        public bool IsHovered { get; set; }
        public Texture2D HoverTexture { get; set; }
        public string ToolTipOnHover { get; set; }
        public Action OnHoverAction { get; set; }

        public bool IsSelected { get; set; }
        public Texture2D SelectedTexture { get; set; }

        public Button(AssetManager assetManager, ButtonType type, double relativeSize, bool sizeExpressedInX, string text, Action onClickAction, double? aspectRatio = null)
            : base(type == ButtonType.Long ? assetManager.LoadTexture("UI/button_long") : assetManager.LoadTexture("UI/button_short"), relativeSize, sizeExpressedInX, aspectRatio)
        {
            OnLeftClickAction = onClickAction;
            TextIsCentered = true;
            Text = text;
            TextColor = Color.White;
            TextFont = assetManager.LoadFont("buttonFont");
            ForceOneLine = true;
            HoverTexture = type == ButtonType.Long ? assetManager.LoadTexture("UI/button_long_hovered") : assetManager.LoadTexture("UI/button_short_hovered");
            SelectedTexture = type == ButtonType.Long ? assetManager.LoadTexture("UI/button_long_selected") : assetManager.LoadTexture("UI/button_short_selected");
        }
    }
}
