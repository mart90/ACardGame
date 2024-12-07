using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ACardGame.UI
{
    public class UiElement
    {
        /// <summary>
        /// The location in pixels. Only known at runtime
        /// </summary>
        public Rectangle AbsoluteLocation { get; set; }

        /// <summary>
        /// The relative location within the parent container. Usually known at compile time
        /// </summary>
        public PointDouble RelativeLocationInParent { get; set; }

        /// <summary>
        /// The relative size within the parent, horizontally. E.g. 50 would mean 50% of the parent's width
        /// </summary>
        public double? RelativeSizeInParentX { get; set; }

        public double? RelativeSizeInParentY { get; set; }
        
        public double AspectRatio { get; set; }

        public Texture2D Texture { get; set; }

        public bool IsVisible { get; set; }

        public string Text { get; set; }
        public SpriteFont TextFont { get; set; }
        public Color TextColor { get; set; }
        public bool TextIsCentered { get; set; }

        /// <param name="relativeSizeInParent">Relative size in the parent, either expressed in X or Y</param>
        /// <param name="sizeExpressedInX">True if the previous param was expressed in X, false if Y</param>
        public UiElement(double aspectRatio, double relativeSizeInParent, bool sizeExpressedInX = true)
        {
            RelativeLocationInParent = new PointDouble(0, 0);
            RelativeSizeInParentX = sizeExpressedInX ? relativeSizeInParent : null;
            RelativeSizeInParentY = sizeExpressedInX ? null : relativeSizeInParent;
            AspectRatio = aspectRatio;
            IsVisible = true;
            TextColor = Color.Black;
        }

        /// <param name="relativeSizeInParent">Relative size in the parent, either expressed in X or Y</param>
        /// <param name="sizeExpressedInX">True if the previous param was expressed in X, false if Y</param>
        /// <param name="aspectRatio">Automatically set based on the texture if left empty</param>
        public UiElement(Texture2D texture, double relativeSizeInParent, bool sizeExpressedInX = true, double? aspectRatio = null)
        {
            Texture = texture;
            RelativeLocationInParent = new PointDouble(0, 0);
            RelativeSizeInParentX = sizeExpressedInX ? relativeSizeInParent : null;
            RelativeSizeInParentY = sizeExpressedInX ? null : relativeSizeInParent;
            AspectRatio = aspectRatio ?? (double)texture.Width / texture.Height;
            IsVisible = true;
            TextColor = Color.Black;
        }

        /// <param name="relativeSizeInParent">Relative size in the parent, either expressed in X or Y</param>
        /// <param name="sizeExpressedInX">True if the previous param was expressed in X, false if Y</param>
        /// <param name="aspectRatio">Automatically set based on the texture if left empty</param>
        public UiElement(Texture2D texture, SpriteFont font, Color textColor, double relativeSizeInParent, bool sizeExpressedInX = true, double? aspectRatio = null)
        {
            Texture = texture;
            RelativeLocationInParent = new PointDouble(0, 0);
            RelativeSizeInParentX = sizeExpressedInX ? relativeSizeInParent : null;
            RelativeSizeInParentY = sizeExpressedInX ? null : relativeSizeInParent;
            AspectRatio = aspectRatio ?? (double)texture.Width / texture.Height;
            IsVisible = true;
            TextFont = font;
            TextColor = textColor;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible)
            {
                return;
            }

            if (Texture != null)
            {
                spriteBatch.Draw(Texture, new Rectangle(AbsoluteLocation.X, AbsoluteLocation.Y, AbsoluteLocation.Width, AbsoluteLocation.Height), null, Color.White);
            }

            if (Text != null)
            {
                Vector2 textLocation;

                if (TextIsCentered)
                {
                    textLocation = new Vector2(AbsoluteLocation.X + AbsoluteLocation.Width / 2 - (TextFont.LineSpacing / 5) * Text.Length, AbsoluteLocation.Y + AbsoluteLocation.Height / 2 - TextFont.LineSpacing / 3);
                }
                else
                {
                    textLocation = new Vector2(AbsoluteLocation.X, AbsoluteLocation.Y);
                }

                spriteBatch.DrawString(TextFont, Text, textLocation, TextColor);
            }
        }
    }
}
