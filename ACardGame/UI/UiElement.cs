using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

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
        public Vector2 TextOffset { get; set; }
        public bool ForceOneLine { get; set; }

        /// <summary>
        /// 0 is bottom
        /// </summary>
        public int DrawLayer { get; set; }

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
                if (this is IHoverable hoverable && hoverable.IsHovered && hoverable.HoverTexture != null)
                {
                    spriteBatch.Draw(hoverable.HoverTexture, AbsoluteLocation, null, Color.White);
                }
                else if (this is ISelectable selectable && selectable.IsSelected && selectable.SelectedTexture != null)
                {
                    spriteBatch.Draw(selectable.SelectedTexture, AbsoluteLocation, null, Color.White);
                }
                else
                {
                    spriteBatch.Draw(Texture, AbsoluteLocation, null, Color.White);
                }
            }

            if (Text != null)
            {
                DrawText(spriteBatch);
            }
        }

        public virtual void DrawText(SpriteBatch spriteBatch)
        {
            float fontScale = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 1440f;

            float textOffsetX = TextOffset.X * AbsoluteLocation.Width / 100;
            float textOffsetY = TextOffset.Y * AbsoluteLocation.Height / 100;

            Vector2 textLocation;

            if (ForceOneLine)
            {
                if ((TextFont.MeasureString(Text) * fontScale).X > AbsoluteLocation.Width)
                {
                    while ((TextFont.MeasureString(Text) * fontScale).X > AbsoluteLocation.Width)
                    {
                        fontScale -= .01f;
                    }

                    fontScale -= .1f;
                }
            }

            if (TextIsCentered)
            {
                Vector2 size = TextFont.MeasureString(Text) * fontScale;

                textLocation = new Vector2(
                    AbsoluteLocation.X + AbsoluteLocation.Width / 2 - size.X / 2, 
                    AbsoluteLocation.Y + AbsoluteLocation.Height / 2 - size.Y / 3);
            }
            else
            {
                textLocation = new Vector2(AbsoluteLocation.X + textOffsetX, AbsoluteLocation.Y + textOffsetY);
            }

            if (ForceOneLine)
            {
                spriteBatch.DrawString(TextFont, Text, textLocation, TextColor, 0, Vector2.Zero, fontScale, SpriteEffects.None, 0);
            }
            else
            {
                var lines = new List<string>();
                string[] words = Text.Split(' ');
                string currentLine = "";

                foreach (string word in words)
                {
                    string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                    Vector2 size = TextFont.MeasureString(testLine) * fontScale;

                    if (size.X > AbsoluteLocation.Width)
                    {
                        lines.Add(currentLine);
                        currentLine = word;
                    }
                    else
                    {
                        currentLine = testLine;
                    }
                }

                if (!string.IsNullOrEmpty(currentLine))
                {
                    lines.Add(currentLine);
                }

                for (int i = 0; i < lines.Count; i++)
                {
                    var lineLocation = textLocation + new Vector2(0, i * TextFont.LineSpacing * fontScale);
                    spriteBatch.DrawString(TextFont, lines[i], lineLocation, TextColor, 0, Vector2.Zero, fontScale, SpriteEffects.None, 0);
                }
            }
        }
    }
}
