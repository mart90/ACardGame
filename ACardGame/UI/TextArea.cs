using Microsoft.Xna.Framework;

namespace ACardGame.UI
{
    public class TextArea : UiElement
    {
        public TextArea(AssetManager assetManager, string fontName, double relativeSize, bool sizeExpressedInX, double aspectRatio)
            : base(null, assetManager.LoadFont(fontName), Color.Black, relativeSize, sizeExpressedInX, aspectRatio)
        {
        }
    }
}
