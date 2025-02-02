using ACardGame.UI;
using ACardGameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace ACardGame
{
    public class Main : Microsoft.Xna.Framework.Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private readonly AssetManager _assetManager;

        private List<TopLevelUiWindow> _topLevelWindows;
        private TopLevelUiWindow _activeWindow;

        private bool _leftMouseHeld;
        private bool _rightMouseHeld;
        private int _lastScrollWheelValue;

        private IHoverable _hoveredElement;

        private ServerConnection _serverConnection;

        public static int ScreenWidth;
        public static int ScreenHeight;

        public const string GameVersion = "0.3.2";
        public const bool DebugLogEnabled = true;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _serverConnection = new ServerConnection();

            TargetElapsedTime = new TimeSpan(100000);
            _assetManager = new AssetManager(Content);
        }

        protected override void Initialize()
        {
            ScreenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            ScreenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.IsFullScreen = true;

            ReadConfig();

            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;
            _graphics.HardwareModeSwitch = false;
            _graphics.ApplyChanges();

            Window.TextInput += HandleTextInput;

            System.IO.Directory.CreateDirectory($"{AppDomain.CurrentDomain.BaseDirectory}/log");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _topLevelWindows = new List<TopLevelUiWindow>
            {
                new MainMenu(_assetManager, _serverConnection),
                new LoginScreen(_assetManager, _serverConnection),
                new MultiplayerHome(_assetManager, _serverConnection),
                new HowToPlay(_assetManager),
            };

            _activeWindow = _topLevelWindows.Single(e => e is MainMenu);
        }

        protected override void Update(GameTime gameTime)
        {
            if (_activeWindow.NewUiState != null)
            {
                if (_activeWindow.NewUiState == UiState.HotSeatGame)
                {
                    _topLevelWindows.RemoveAll(e => e is HotSeatGame); 
                    _topLevelWindows.Add(new HotSeatGame(_assetManager, new GameStateManager()));
                }
                else if (_activeWindow.NewUiState == UiState.BotGame)
                {
                    _topLevelWindows.RemoveAll(e => e is BotGame);
                    _topLevelWindows.Add(new BotGame(_assetManager, new GameStateManager()));
                }
                else if (_activeWindow.NewUiState == UiState.MultiplayerGame)
                {
                    _topLevelWindows.RemoveAll(e => e is MultiplayerGame);
                    _topLevelWindows.Add(((MultiplayerHome)_activeWindow).CreatedGame);
                }
                else if (_activeWindow.NewUiState == UiState.Exiting)
                {
                    Exit();
                    return;
                }

                var newWindow = _topLevelWindows.Single(e => e.CorrespondingUiState == _activeWindow.NewUiState);
                _activeWindow.NewUiState = null;
                _activeWindow = newWindow;
            }

            if (IsActive)
            {
                HandleMouseState();
            }

            _activeWindow.Update();

            base.Update(gameTime);
        }

        private void HandleMouseState()
        {
            MouseState mouseState = Mouse.GetState();

            if (mouseState.LeftButton == ButtonState.Released && _leftMouseHeld)
            {
                _leftMouseHeld = false;
            }
            else if (mouseState.LeftButton == ButtonState.Pressed && !_leftMouseHeld)
            {
                _leftMouseHeld = true;
                _activeWindow.LeftClick(mouseState.Position);
            }

            if (mouseState.RightButton == ButtonState.Released && _rightMouseHeld)
            {
                _rightMouseHeld = false;
            }
            else if (mouseState.RightButton == ButtonState.Pressed && !_rightMouseHeld)
            {
                _rightMouseHeld = true;
                _activeWindow.RightClick(mouseState.Position);
            }

            if (mouseState.ScrollWheelValue > _lastScrollWheelValue)
            {
                _activeWindow.ScrollUp(mouseState.Position);
            }
            else if (mouseState.ScrollWheelValue < _lastScrollWheelValue)
            {
                _activeWindow.ScrollDown(mouseState.Position);
            }

            _lastScrollWheelValue = mouseState.ScrollWheelValue;

            if (mouseState.RightButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Released)
            {
                var hoveredElement = _activeWindow.Hover(mouseState.Position);

                if (_hoveredElement != null)
                {
                    _hoveredElement.IsHovered = false;
                }

                if (hoveredElement != null)
                {
                    _hoveredElement = hoveredElement;
                    _hoveredElement.IsHovered = true;
                }
            }
        }

        private void HandleTextInput(object sender, TextInputEventArgs args)
        {
            _activeWindow.ReceiveKeyboardInput(args);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin();
            _activeWindow.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void ReadConfig()
        {
            int lineCounter = 0;

            try
            {
                var config = File.ReadAllText($"{AppDomain.CurrentDomain.BaseDirectory}/config.txt");

                foreach (string line in config.Split("\r\n"))
                {
                    lineCounter++;

                    if (line.StartsWith("#") || !line.Contains('='))
                    {
                        continue;
                    }

                    string[] keyValue = line.Split('=');
                    string key = keyValue[0];
                    string value = keyValue[1];

                    if (key.ToLower() == "fullscreen")
                    {
                        _graphics.IsFullScreen = bool.Parse(value);
                    }
                    else if (key.ToLower() == "windowwidth" && _graphics.IsFullScreen == false)
                    {
                        ScreenWidth = int.Parse(value);
                    }
                    else if (key.ToLower() == "windowheight" && _graphics.IsFullScreen == false)
                    {
                        ScreenHeight = int.Parse(value);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error parsing config.txt on line {lineCounter}: {ex}");
            }
        }
    }
}
