using ACardGameLibrary;
using System.Collections.Generic;
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
            SetCursor(43, 1);
            EnemySupportsButton = new Button(AssetManager, ButtonType.Long, 14, true, "Supports", delegate
            {
                game.ShowSupportsInPlay(false);
            })
            {
                IsVisible = false
            };
            AddChild(EnemySupportsButton);

            SetCursor(43, 95);
            ActivePlayerSupportsButton = new Button(AssetManager, ButtonType.Long, 14, true, "Supports", delegate
            {
                game.ShowSupportsInPlay(true);
            })
            {
                IsVisible = false
            };
            AddChild(ActivePlayerSupportsButton);
        }

        public void Refresh(GameStateManager gameState, Player visiblePlayer)
        {
            Children.RemoveAll(e => e is BattlefieldLane);

            double cardWidth = GetCardWidth(gameState.AttackingCreatures);

            double startX = cardWidth == 12.5 ? 50 - 7.5 * gameState.AttackingCreatures.Count : 1;

            SetCursor(startX, 6);

            foreach (CreatureCard creature in gameState.AttackingCreatures)
            {
                var lane = new BattlefieldLane(AssetManager, creature.BlockedBy.Any() ? creature.BlockedBy.Count * cardWidth : cardWidth, true);
                lane.Refresh(creature, visiblePlayer.IsAttacking, 12.5 / cardWidth);
                AddChild(lane);
                AddSpacing(cardWidth * 0.25);
            }

            var activePlayerSupports = gameState.GetPlayerSupports(visiblePlayer.IsActive);
            if (activePlayerSupports.Any())
            {
                ActivePlayerSupportsButton.IsVisible = true;
                ActivePlayerSupportsButton.Text = $"Supports ({activePlayerSupports.Count})";
            }
            else
            {
                ActivePlayerSupportsButton.IsVisible = false;
            }

            var enemySupports = gameState.GetPlayerSupports(!visiblePlayer.IsActive);
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

        private double GetCardWidth(List<CreatureCard> attackingCreatures)
        {
            int creatures = 0;

            foreach (CreatureCard creature in attackingCreatures)
            {
                creatures++;

                if (creature.BlockedBy.Count > 1)
                {
                    creatures += creature.BlockedBy.Count - 1;
                }
            }

            double cardPlusSpacingwidth = 98.0f / creatures;
            double cardWidth = cardPlusSpacingwidth * 0.8;

            return cardWidth > 12.5 ? 12.5 : cardWidth;
        }
    }
}
