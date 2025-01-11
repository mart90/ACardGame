using ACardGameLibrary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace ACardGame.UI
{
    public class LogViewer : UiContainer
    {
        public List<GameLog> Logs { get; set; }

        public LogTextArea TextArea { get; set; }

        private readonly UiElement _goToTop;

        private int _firstLogIndex;

        public LogViewer(AssetManager assetManager) : base(assetManager, 1.5, 40, true)
        {
            SetCursor(2, 2);
            TextArea = new LogTextArea(assetManager);
            AddChild(TextArea);

            SetCursor(95, 3);
            _goToTop = new UiElement(assetManager.LoadTexture("UI/go_top"), 3, true)
            {
                IsVisible = false
            };
            AddChild(_goToTop);

            TextColor = Color.White;
            Texture = assetManager.LoadTexture("UI/card_selector");
        }

        public void Show(List<GameLog> logs)
        {
            Logs = new List<GameLog>(logs);
            Logs.Reverse();
            IsVisible = true;
            _firstLogIndex = 0;
            SetText();
        }

        public void Refresh(List<GameLog> logs)
        {
            var diff = logs.Count - Logs.Count;

            Logs = new List<GameLog>(logs);
            Logs.Reverse();

            if (_firstLogIndex != 0)
            {
                _firstLogIndex += diff;
            }

            SetText();
        }

        public override void ScrollUp(Point position)
        {
            _firstLogIndex -= 3;

            if (_firstLogIndex < 0)
            {
                _firstLogIndex = 0;
                _goToTop.IsVisible = false;
            }

            SetText();
        }

        public override void ScrollDown(Point position)
        {
            if (Logs.Count < 10)
            {
                return;
            }

            _firstLogIndex += 3;

            if ( _firstLogIndex >= Logs.Count - 10)
            {
                _firstLogIndex = Logs.Count - 10;

                if (_firstLogIndex < 0)
                {
                    _firstLogIndex = 0;
                }
            }

            SetText();

            _goToTop.IsVisible = true;
        }

        private void SetText()
        {
            TextArea.Text = "";

            var logs = new List<GameLog>(Logs);
            logs.RemoveRange(0, _firstLogIndex);

            foreach (var log in logs)
            {
                TextArea.Text += $"{log.TimeStamp.ToShortTimeString()} | {log.Message}\n";
            }

            TextArea.Text = TextArea.Text.Trim();
        }
    }
}
