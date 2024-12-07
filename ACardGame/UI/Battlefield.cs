using ACardGameLibrary;

namespace ACardGame.UI
{
    public class Battlefield : UiContainer
    {
        public Battlefield(AssetManager assetManager, double relativeSize, bool sizeExpressedInX)
            : base(assetManager, 1.4, relativeSize, sizeExpressedInX)
        {
            Texture = assetManager.LoadTexture("UI/battlefield_background");
            IsVisible = false;

            //AttackingCreatures = new List<CardContainer>();
            //DefendingCreatures = new List<CardContainer>();
            //GlobalSupportsInPlay = new List<CardContainer>();
        }

        public void Refresh(GameStateManager gameState)
        {
            Children.Clear();

            var activePlayerIsAttacking = gameState.ActivePlayer.IsAttacking;

            SetCursor(50 - 8 * gameState.AttackingCreatures.Count, activePlayerIsAttacking ? 67 : 0);

            foreach (CreatureCard creature in gameState.AttackingCreatures)
            {
                var container = new CardContainer(AssetManager, 14, true, null);
                container.OnLeftClickAction = delegate { container.IsTargeted = true; };
                container.SetCard(creature);
                AddChild(container);
                AddSpacing(2.5);
            }
        }
    }
}
