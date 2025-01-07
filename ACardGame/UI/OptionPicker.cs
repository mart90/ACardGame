using System.Collections.Generic;
using System.Linq;

namespace ACardGame.UI
{
    public class OptionPicker : UiContainer
    {
        public List<string> OptionsPicked => Children
            .Where(e => e is Button).Cast<Button>()
            .Where(e => e.IsSelected)
            .Select(e => e.Text)
            .ToList();

        public OptionPicker(AssetManager assetManager, double relativeSize, bool sizeExpressedInX)
            : base(assetManager, 2, relativeSize, sizeExpressedInX)
        {
            Texture = AssetManager.LoadTexture("UI/card_selector");
            IsVisible = false;
            DrawLayer = 5;
        }

        public void Clear()
        {
            ClearChildren();
        }

        public void Show(List<string> options)
        {
            ClearChildren();

            RelativeSizeInParentY = 4 * options.Count;
            AspectRatio = 4.5f / options.Count;

            IsVisible = true;

            GoDown();
            SetCursor(5, 5);

            for (int i = 0; i < options.Count; i++) 
            {
                int currentIndex = i;
                var option = options[i];
                AddChild(new Button(AssetManager, ButtonType.Long, 90, true, option, delegate
                {
                    var button = (Button)Children[currentIndex];
                    button.IsSelected = !button.IsSelected;
                }));
                AddSpacing(8 / options.Count);
            }
        }
    }
}
