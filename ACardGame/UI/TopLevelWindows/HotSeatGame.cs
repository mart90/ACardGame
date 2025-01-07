using ACardGameLibrary;

namespace ACardGame.UI
{
    public class HotSeatGame : GameWindow
    {
        public override Player Player => GameState.ActivePlayer;
        public override Player Enemy => GameState.Enemy;
        public override bool IsMyTurn => true;

        public HotSeatGame(AssetManager assetManager, GameStateManager gameStateManager) : base(assetManager, gameStateManager)
        {
            Texture = assetManager.LoadTexture("UI/wallpaper");
            CorrespondingUiState = UiState.HotSeatGame;

            PrepareShop();
            BuildUI();
            Update();
        }

        public override void Update()
        {
            base.Update();

            PlayerName.Text = Player.Name;
            EnemyName.Text = Enemy.Name;
        }
    }
}
