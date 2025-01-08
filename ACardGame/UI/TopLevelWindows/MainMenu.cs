namespace ACardGame.UI
{
    public class MainMenu : TopLevelUiWindow
    {
        private readonly ServerConnection _server;

        public MainMenu(AssetManager assetManager, ServerConnection server) : base(assetManager)
        {
            CorrespondingUiState = UiState.MainMenu;
            Texture = assetManager.LoadTexture("UI/wallpaper");
            _server = server;

            SetCursor(43, 40);
            
            GoDown();

            AddChild(new Button(assetManager, ButtonType.Long, 14, true, "Hot seat", delegate
            {
                NewUiState = UiState.HotSeatGame;
            }));

            AddSpacing(2);

            AddChild(new Button(assetManager, ButtonType.Long, 14, true, "Multiplayer", delegate
            {
                if (_server.AuthenticatedUser == null)
                {
                    NewUiState = UiState.LoginScreen;
                }
                else
                {
                    NewUiState = UiState.MultiplayerHome;
                }
            }));
        }
    }
}
