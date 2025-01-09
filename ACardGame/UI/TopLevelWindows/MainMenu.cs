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

            SetCursor(44, 35);
            
            GoDown();

            AddChild(new Button(assetManager, ButtonType.Long, 13, true, "Hot seat", delegate
            {
                NewUiState = UiState.HotSeatGame;
            }));

            AddSpacing(2);

            AddChild(new Button(assetManager, ButtonType.Long, 13, true, "Multiplayer", delegate
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

            AddSpacing(2);

            AddChild(new Button(assetManager, ButtonType.Long, 13, true, "Exit", delegate
            {
                NewUiState = UiState.Exiting;
            }));
        }
    }
}
