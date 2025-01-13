using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace ACardGame.UI
{
    public class LogTextArea : TextArea
    {
        public LogTextArea(AssetManager assetManager) : base(assetManager, "logFont", 96, true, 1.5) { }

        public override void DrawText(SpriteBatch spriteBatch)
        {
            float fontScale = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height / 1440f;

            var logEntries = Text.Split('\n').ToList();

            int newLines = 0;

            for (int i = 0; i < logEntries.Count; i++)
            {
                var entry = logEntries[i];
                var entryLocation = new Vector2(AbsoluteLocation.X, AbsoluteLocation.Y) + new Vector2(0, (newLines + i) * TextFont.LineSpacing * fontScale);

                if (entryLocation.Y > AbsoluteLocation.Y + AbsoluteLocation.Height - 50)
                {
                    return;
                }

                var lines = new List<string>();
                string[] words = entry.Split(' ');
                string currentLine = "";

                foreach (string word in words)
                {
                    string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                    Vector2 size = TextFont.MeasureString(testLine) * fontScale;

                    if (size.X > AbsoluteLocation.Width - 5)
                    {
                        lines.Add(currentLine);
                        newLines++;
                        currentLine = "..........." + word;
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

                for (int i2 = 0; i2 < lines.Count; i2++)
                {
                    var lineLocation = entryLocation + new Vector2(0, i2 * TextFont.LineSpacing * fontScale);

                    var color = entry.EndsWith("'s turn") ? Color.Aqua : Color.White;

                    spriteBatch.DrawString(TextFont, lines[i2], lineLocation, color, 0, Vector2.Zero, fontScale, SpriteEffects.None, 0);
                }
            }
        }
    }
}
