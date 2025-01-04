using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace ACardGame.UI
{
    public class CardText : UiElement
    {
        public float ParentScale { get; set; }

        public CardText(SpriteFont font, double relativeSizeInParent, double aspectRatio, float parentScale) 
            : base(null, font, Color.Black, relativeSizeInParent, true, aspectRatio) 
        {
            ParentScale = parentScale;
        }

        public override void DrawText(SpriteBatch spriteBatch)
        {
            float fontScale = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 1440f * ParentScale;
            float lineSpacing = TextFont.LineSpacing * fontScale;

            float textOffsetX = TextOffset.X * AbsoluteLocation.Width / 100;
            float textOffsetY = TextOffset.Y * AbsoluteLocation.Height / 100;

            Vector2 textLocation;

            if (ForceOneLine)
            {
                while ((TextFont.MeasureString(Text) * fontScale).X > AbsoluteLocation.Width)
                {
                    fontScale -= .01f;
                }
            }

            if (TextIsCentered)
            {
                Vector2 size = TextFont.MeasureString(Text) * fontScale;

                textLocation = new Vector2(
                    AbsoluteLocation.X + AbsoluteLocation.Width / 2 - size.X / 2,
                    AbsoluteLocation.Y + AbsoluteLocation.Height / 2 - size.Y / 2.5f);
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

                    if (size.X > AbsoluteLocation.Width - 5 && testLine.Contains(' '))
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
