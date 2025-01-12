using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ACardGame.UI
{
    public class HowToPlay : TopLevelUiWindow
    {
        public HowToPlay(AssetManager assetManager) : base(assetManager)
        {
            CorrespondingUiState = UiState.HowToPlay;
            Texture = assetManager.LoadTexture("UI/wallpaper");

            SetCursor(30, 35);
            
            GoDown();

            AddChild(new Button(assetManager, ButtonType.Long, 15, true, "Rule book", delegate
            {
                OpenUrl("https://drive.google.com/file/d/1381c7cNS7njsBP5je2U1k5gDwmjUEBmq/view?usp=drive_link");
            }));

            AddSpacing(1.5);

            AddChild(new Button(assetManager, ButtonType.Long, 15, true, "In-game UI", delegate
            {
                OpenUrl("https://drive.google.com/file/d/13AdChaIyaBOx2xSNA9y2VfSGP27ppS3N/view?usp=drive_link");
            }));

            AddSpacing(1.5);

            AddChild(new Button(assetManager, ButtonType.Long, 15, true, "Battlefield", delegate
            {
                OpenUrl("https://drive.google.com/file/d/13BlfkyemixZlA5QQEErKC7ZcWb8q-X8K/view?usp=drive_link");
            }));

            AddSpacing(1.5);

            AddChild(new Button(assetManager, ButtonType.Long, 15, true, "Back", delegate
            {
                NewUiState = UiState.MainMenu;
            }));

            GoRight();
            SetCursor(58, 22);

            AddChild(new UiElement(AssetManager.LoadTexture("UI/card_explain"), 50, false));

            AddSpacing(3);

            AddChild(new UiElement(AssetManager.LoadTexture("UI/currency_explain"), 50, false));
        }

        private static void OpenUrl(string url)
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
