using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ACardGame.UI
{
    public class FaceDownCardStack : UiElement, IHoverable
    {
        public Texture2D HoverTexture { get; set; }
        public string ToolTipOnHover { get; set; }
        public Action OnHoverAction { get; set; }

        public FaceDownCardStack(AssetManager assetManager, double relativeSize, bool sizeExpressedInX)
            : base(assetManager.LoadTexture("UI/cardback"), assetManager.LoadFont("faceDownCardAmountFont"), Color.White, relativeSize, sizeExpressedInX, .6) { }
    }
}
