using ACardGameLibrary;
using System;
using System.Linq;

namespace ACardGame.UI
{
    public class BotGame : GameWindow
    {
        public override Player Player => GameState.ActivePlayer;
        public override Player Enemy => GameState.Enemy;
        public override bool IsMyTurn => true;

        private readonly Bot _bot;

        public BotGame(AssetManager assetManager, GameStateManager gameStateManager) : base(assetManager, gameStateManager)
        {
            Texture = assetManager.LoadTexture("UI/wallpaper");
            CorrespondingUiState = UiState.BotGame;

            _bot = new Bot(gameStateManager);

            PrepareShop();
            BuildUI();
            Update();

            Player.Name = "Player";
            Enemy.Name = "Bot";

            PlayerName.Text = Player.Name;
            EnemyName.Text = Enemy.Name;

            if (new Random().Next() % 2 == 0)
            {
                gameStateManager.SwitchActivePlayer();
                DoBotTurn();
            }

            BackToMenuButton.IsVisible = true;
        }

        public void DoBotTurn()
        {
            if (GameState.IsInCombat)
            {
                _bot.DoTurnCombat();

                if (!GameState.IsInCombat)
                {
                    _bot.DoTurnPostCombat();
                }
            }
            else
            {
                _bot.DoTurn();

                if (GameState.IsInCombat)
                {
                    ToggleShopVisible();
                    ToggleShopButton.IsVisible = true;
                }
            }

            if (GameState.ResolvingAfterPlay)
            {
                Card target = _bot.GetTarget(GameState.TargetingCard.Name);

                if (target != null)
                {
                    target.IsTargeted = true;
                }

                ResolveAccepted();
            }
            else if (GameState.RequireAccept)
            {
                ResolveAccepted();
            }

            if (GameState.IsInCombat)
            {
                Battlefield.Refresh(GameState, Player);
            }
            else
            {
                LogViewer.Show(GameState.PublicLog);
            }
        }

        public override void LeftClick(Microsoft.Xna.Framework.Point position)
        {
            base.LeftClick(position);

            if (GameState.ActivePlayer.Name == "Bot")
            {
                DoBotTurn();
            }

            RefreshHand();
        }
    }
}
