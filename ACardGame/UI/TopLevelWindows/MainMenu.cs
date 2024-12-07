using Microsoft.Xna.Framework;

namespace ACardGame.UI
{
    public class MainMenu : TopLevelUiWindow
    {
        public MainMenu(Rectangle absoluteLocation, AssetManager assetManager) : base(absoluteLocation, assetManager)
        {
            CorrespondingUiState = UiState.MainMenu;
            Texture = assetManager.LoadTexture("UI/wallpaper");

            SetCursor(45, 40);
            
            GoDown();

            AddChild(new Button(assetManager, ButtonType.Long, 10, true, "Hot seat", delegate
            {
                NewUiState = UiState.HotSeatGame;
            }));

            AddSpacing(2);

            AddChild(new Button(assetManager, ButtonType.Long, 10, true, "Multiplayer", delegate
            {
                NewUiState = UiState.LobbyBrowser;
            }));
        }
    }
}
