using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace ACardGame.UI
{
    public class TextInput : UiElement, ILeftClickable
    {
        public bool HasFocus { get; set; }

        public bool IsPassword { get; set; }

        public bool IsBeingLeftClicked { get; set; }
        public Texture2D BeingLeftClickedTexture { get; set; }
        public Action OnLeftClickAction { get; set; }

        public TextInput(AssetManager assetManager, string fontName, double relativeSize, bool sizeExpressedInX, double aspectRatio)
            : base(null, assetManager.LoadFont(fontName), Color.Black, relativeSize, sizeExpressedInX, aspectRatio)
        {
            Texture = assetManager.LoadTexture("UI/card_selector");
            TextColor = Color.White;
            TextOffset = new Vector2(2, 30);
            Text = "";

            OnLeftClickAction = delegate
            {
                HasFocus = true;
            };
        }

        public override void DrawText(SpriteBatch spriteBatch)
        {
            string text = Text;

            if (IsPassword)
            {
                text = "";
                for (int i = 0; i < Text.Length; i++)
                {
                    text += "*";
                }
            }

            if (HasFocus)
            {
                text += "_";
            }

            float fontScale = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 1440f;

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
                Vector2 size = TextFont.MeasureString(text) * fontScale;

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
                spriteBatch.DrawString(TextFont, text, textLocation, TextColor, 0, Vector2.Zero, fontScale, SpriteEffects.None, 0);
            }
            else
            {
                var lines = new List<string>();
                string[] words = text.Split(' ');
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
