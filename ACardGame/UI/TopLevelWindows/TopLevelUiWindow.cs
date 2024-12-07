using Microsoft.Xna.Framework;

namespace ACardGame.UI
{
    public class TopLevelUiWindow : UiContainer
    {
        public TopLevelUiWindow(Rectangle absoluteLocation, AssetManager assetManager) 
            : base(assetManager, (double)absoluteLocation.Width / absoluteLocation.Height, 100, true)
        {
            AbsoluteLocation = absoluteLocation;
        }

        public UiState CorrespondingUiState { get; set; }
        public UiState? NewUiState { get; set; }

        public virtual void Update() { }
    }
}
