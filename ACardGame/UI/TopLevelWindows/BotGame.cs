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

            if (new Random().Next() % 2 == 0)
            {
                gameStateManager.SwitchActivePlayer();
                DoBotTurn();
            }

            BackToMenuButton.IsVisible = true;
        }

        public void DoBotTurn()
        {
            _bot.DoTurn();

            CardStackViewer.Show(GameState.ActivePlayer.CardsPlayedThisTurn, "Opponent played:");

            GameState.SwitchTurn();
        }

        protected override void EndTurn()
        {
            base.EndTurn();
            DoBotTurn();
        }
    }
}
