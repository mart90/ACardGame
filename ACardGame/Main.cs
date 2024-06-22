using ACardGame.UI;
using ACardGameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACardGame
{
    public class Main : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private readonly AssetManager _assetManager;

        private List<TopLevelUiWindow> _topLevelWindows;
        private TopLevelUiWindow _activeWindow;

        private bool _leftMouseHeld;
        private bool _rightMouseHeld;

        //private readonly NetManager _client;

        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            TargetElapsedTime = new TimeSpan(100000);
            _assetManager = new AssetManager(Content);
        }

        protected override void Initialize()
        {
            _graphics.IsFullScreen = true;
            _graphics.HardwareModeSwitch = false;
            _graphics.ApplyChanges();

            //EventBasedNetListener listener = new EventBasedNetListener();
            //_client = new(listener);

            //_client.Start();
            //_client.Connect("localhost", 9050, "SomeConnectionKey");
            //listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
            //{
            //    var str = dataReader.GetString(100);
            //    dataReader.Recycle();
            //};

            base.Initialize();
        }

        protected override void LoadContent()
        {
            int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            _spriteBatch = new SpriteBatch(GraphicsDevice);

            GameStateManager gameStateManager = new();

            _topLevelWindows = new List<TopLevelUiWindow>
            {
                new MainMenu(new Rectangle(0, 0, screenWidth, screenHeight), _assetManager),
                new HotSeatGame(new Rectangle(0, 0, screenWidth, screenHeight), _assetManager, gameStateManager),
            };

            _activeWindow = _topLevelWindows.Single(e => e is MainMenu);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (_activeWindow.NewUiState != null)
            {
                var newWindow = _topLevelWindows.Single(e => e.CorrespondingUiState == _activeWindow.NewUiState);
                _activeWindow.NewUiState = null;
                _activeWindow = newWindow;
            }

            HandleMouseState();

            //_client.PollEvents();

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

            if (mouseState.RightButton == ButtonState.Released && mouseState.LeftButton == ButtonState.Released)
            {
                _activeWindow.Hover(mouseState.Position);
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Transparent);

            _spriteBatch.Begin();
            _activeWindow.Draw(_spriteBatch);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
