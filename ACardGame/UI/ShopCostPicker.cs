using System.Linq;

namespace ACardGame.UI
{
    public class ShopCostPicker : UiContainer
    {
        public int CostPicked => Children
            .Where(e => e is Button).Cast<Button>()
            .Where(e => e.IsSelected)
            .Select(e => int.Parse(e.Text))
            .Single();

        public ShopCostPicker(AssetManager assetManager, double relativeSize, bool sizeExpressedInX)
            : base(assetManager, 4, relativeSize, sizeExpressedInX)
        {
            Texture = AssetManager.LoadTexture("UI/card_selector");
            IsVisible = false;
            DrawLayer = 5;

            Build();
        }

        public void Build()
        {
            SetCursor(10, 40);
            for (int i = 2; i <= 7; i++)
            {
                int currentIndex = i;
                var button = new Button(AssetManager, ButtonType.Short, 13, true, i.ToString(), delegate
                {
                    foreach (var child in Children.Where(e => e is Button).Cast<Button>())
                    {
                        child.IsSelected = false;
                    }

                    ((Button)Children[currentIndex - 2]).IsSelected = true;
                });
                AddChild(button);
                AddSpacing(.2);
            }
        }

        public void Reset()
        {
            foreach (var child in Children.Where(e => e is Button).Cast<Button>())
            {
                child.IsSelected = false;
            }

            IsVisible = false;
        }
    }
}
