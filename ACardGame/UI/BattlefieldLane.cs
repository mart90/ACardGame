using ACardGameLibrary;
using System.Linq;

namespace ACardGame.UI
{
    public class BattlefieldLane : UiContainer
    {
        public BattlefieldLane(AssetManager assetManager, double relativeSize, bool sizeExpressedInX)
            : base(assetManager, 0.2, relativeSize, sizeExpressedInX)
        {
            Texture = assetManager.LoadTexture("UI/lane_background");
        }

        public void Refresh(CreatureCard attackingCreature, bool playerIsAttacking, double scaleDownFactor)
        {
            var blockers = attackingCreature.BlockedBy;
            var supports = attackingCreature.AttachedEquipments;

            double relativeCardSizeX = blockers.Any() ? 98 / blockers.Count : 98;

            double bottomStartY = 100 - 37.5 / scaleDownFactor;

            float cardScale = 1 / (float)scaleDownFactor;

            AspectRatio = blockers.Any() ? (0.23 / scaleDownFactor) * blockers.Count : 0.23 / scaleDownFactor;

            int attackerX = blockers.Any() ? 51 - 50 / blockers.Count : 1;
            double supportSpacing = 5 / scaleDownFactor;

            if (supports.Any())
            {
                GoDown();
                double startY = playerIsAttacking ? bottomStartY - supportSpacing * supports.Count : 0;
                SetCursor(attackerX, startY);

                for (int i = 0; i < supports.Count; i++)
                {
                    var support = supports[i];
                    var supportContainer = new CardContainer(AssetManager, relativeCardSizeX, true, null, cardScale);
                    supportContainer.OnLeftClickAction = delegate
                    {
                        supportContainer.ToggleTargeted();
                    };
                    supportContainer.DrawLayer = i;
                    supportContainer.SetCard(support);
                    AddChild(supportContainer);
                    SetCursor(attackerX, startY + supportSpacing * (i + 1));
                }
            }

            GoRight();
            SetCursor(attackerX, playerIsAttacking ? 67 : supportSpacing * supports.Count);
            var attackerContainer = new CardContainer(AssetManager, relativeCardSizeX, true, null, cardScale);
            attackerContainer.OnLeftClickAction = delegate 
            { 
                attackerContainer.ToggleTargeted();
            };
            attackerContainer.DrawLayer = supports.Count;
            attackerContainer.SetCard(attackingCreature);
            AddChild(attackerContainer);

            if (blockers.Any())
            {
                for (int i = 0; i < blockers.Count; i++)
                {
                    var blockerX = 1 + relativeCardSizeX * i;
                    SetCursor(blockerX, playerIsAttacking ? 0 : bottomStartY);

                    var blocker = blockers[i];
                    var blockerSupports = blocker.AttachedEquipments;

                    if (blockerSupports.Any())
                    {
                        GoDown(false);

                        double startY = playerIsAttacking ? 0 : bottomStartY - supportSpacing * blockerSupports.Count;
                        SetCursorY(startY);

                        for (int si = 0; si < blockerSupports.Count; si++)
                        {
                            var support = blockerSupports[si];
                            var supportContainer = new CardContainer(AssetManager, relativeCardSizeX, true, null, cardScale);
                            supportContainer.OnLeftClickAction = delegate
                            {
                                supportContainer.ToggleTargeted();
                            };
                            supportContainer.DrawLayer = si;
                            supportContainer.SetCard(support);
                            AddChild(supportContainer);
                            SetCursor(blockerX, startY + supportSpacing * (si + 1));
                        }
                    }

                    var blockerContainer = new CardContainer(AssetManager, relativeCardSizeX, true, null, cardScale);
                    blockerContainer.OnLeftClickAction = delegate 
                    { 
                        blockerContainer.ToggleTargeted();
                    };
                    blockerContainer.DrawLayer = blockerSupports.Count;
                    blockerContainer.SetCard(blocker);
                    AddChild(blockerContainer);
                    AddSpacing(1);
                }

                SetCursor(50 - 40 / blockers.Count, 40 + 2.5 * scaleDownFactor);
                AddChild(new UiElement(AssetManager.LoadTexture("UI/crossed_swords"), 80 / blockers.Count));
            }
        }
    }
}
