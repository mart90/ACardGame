using System.Diagnostics;
using System.Runtime.InteropServices;

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

            AddChild(new Button(assetManager, ButtonType.Long, 13, true, "Sandbox", delegate
            {
                NewUiState = UiState.HotSeatGame;
            }));

            AddSpacing(1.5);

            AddChild(new Button(assetManager, ButtonType.Long, 13, true, "vs Bot", delegate
            {
                NewUiState = UiState.BotGame;
            }));

            AddSpacing(1.5);

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

            AddSpacing(1.5);

            AddChild(new Button(assetManager, ButtonType.Long, 13, true, "How to play", delegate
            {
                NewUiState = UiState.HowToPlay;
            }));

            AddSpacing(1.5);

            AddChild(new Button(assetManager, ButtonType.Long, 13, true, "Exit", delegate
            {
                NewUiState = UiState.Exiting;
            }));

            GoRight();
            SetCursor(91, 96);

            AddChild(new TextArea(assetManager, "buttonFont", 8, true, 3) 
            { 
                Text = $"Version {Main.GameVersion}",
                ForceOneLine = true
            });
        }
    }
}
