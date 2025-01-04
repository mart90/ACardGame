using ACardGameLibrary;
using System.Linq;

namespace ACardGame.UI
{
    public class Battlefield : UiContainer
    {
        public Button ActivePlayerSupportsButton { get; set; }
        public Button EnemySupportsButton { get; set; }

        public Battlefield(AssetManager assetManager, double relativeSize, bool sizeExpressedInX)
            : base(assetManager, 1.6, relativeSize, sizeExpressedInX)
        {
            Texture = assetManager.LoadTexture("UI/card_selector");
            IsVisible = false;
        }

        public void AddButtons(GameWindow game)
        {
            SetCursor(0, 0);
            EnemySupportsButton = new Button(AssetManager, ButtonType.Long, 15, true, "Supports", delegate
            {
                game.ShowSupportsInPlay(false);
            })
            {
                IsVisible = false
            };
            AddChild(EnemySupportsButton);

            SetCursor(0, 95.5);
            ActivePlayerSupportsButton = new Button(AssetManager, ButtonType.Long, 15, true, "Supports", delegate
            {
                game.ShowSupportsInPlay(true);
            })
            {
                IsVisible = false
            };
            AddChild(ActivePlayerSupportsButton);
        }

        public void Refresh(GameStateManager gameState)
        {
            Children.RemoveAll(e => e is BattlefieldLane);

            var activePlayerIsAttacking = gameState.ActivePlayer.IsAttacking;

            SetCursor(50 - 8 * gameState.AttackingCreatures.Count, 0);

            foreach (CreatureCard creature in gameState.AttackingCreatures)
            {
                var lane = new BattlefieldLane(AssetManager, creature.BlockedBy.Any() ? creature.BlockedBy.Count * 12.5 : 12.5, true);
                lane.Refresh(creature, activePlayerIsAttacking);
                AddChild(lane);
                AddSpacing(2.5);
            }

            var activePlayerSupports = gameState.GetPlayerSupports(true);
            if (activePlayerSupports.Any())
            {
                ActivePlayerSupportsButton.IsVisible = true;
                ActivePlayerSupportsButton.Text = $"Supports ({activePlayerSupports.Count})";
            }
            else
            {
                ActivePlayerSupportsButton.IsVisible = false;
            }

            var enemySupports = gameState.GetPlayerSupports(false);
            if (enemySupports.Any())
            {
                EnemySupportsButton.IsVisible = true;
                EnemySupportsButton.Text = $"Supports ({enemySupports.Count})";
            }
            else
            {
                EnemySupportsButton.IsVisible = false;
            }
        }
    }
}
