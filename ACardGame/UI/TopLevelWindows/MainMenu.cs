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

            AddChild(new Button(assetManager, ButtonType.Long, 13, true, "Hot seat", delegate
            {
                NewUiState = UiState.HotSeatGame;
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
        }

        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                // hack because of this: https://github.com/dotnet/corefx/issues/10361
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
