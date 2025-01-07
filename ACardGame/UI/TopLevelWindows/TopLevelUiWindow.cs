using Microsoft.Xna.Framework;

namespace ACardGame.UI
{
    public class TopLevelUiWindow : UiContainer
    {
        public TopLevelUiWindow(AssetManager assetManager) 
            : base(assetManager, (double)Main.ScreenWidth / Main.ScreenHeight, 100, true)
        {
            AbsoluteLocation = new Rectangle(0, 0, Main.ScreenWidth, Main.ScreenHeight);
            UpdateCounter = 0;
        }

        protected int UpdateCounter { get; set; }

        public UiState CorrespondingUiState { get; set; }
        public UiState? NewUiState { get; set; }

        public virtual void Update() 
        {
            UpdateCounter++;

            if (UpdateCounter == int.MaxValue)
            {
                UpdateCounter = 0;
            }
        }
    }
}
