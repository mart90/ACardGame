using ACardGameLibrary;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
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

        public void Refresh(CreatureCard attackingCreature, bool playerIsAttacking)
        {
            var blockers = attackingCreature.BlockedBy;
            var supports = attackingCreature.AttachedEquipments;

            double relativeCardSizeX = blockers.Any() ? 98 / blockers.Count : 98;

            AspectRatio = blockers.Any() ? 0.2 * blockers.Count : 0.2;

            int attackerX = blockers.Any() ? 51 - 50 / blockers.Count : 1;
            int supportSpacing = 5;

            if (supports.Any())
            {
                GoDown();
                int startY = playerIsAttacking ? 67 - supportSpacing * supports.Count : 0;
                SetCursor(attackerX, startY);

                for (int i = 0; i < supports.Count; i++)
                {
                    var support = supports[i];
                    var supportContainer = new CardContainer(AssetManager, relativeCardSizeX, true, null);
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
            var attackerContainer = new CardContainer(AssetManager, relativeCardSizeX, true, null);
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
                    SetCursor(1 + relativeCardSizeX * i, playerIsAttacking ? 0 : 67);

                    var blocker = blockers[i];
                    var blockerSupports = blocker.AttachedEquipments;

                    if (blockerSupports.Any())
                    {
                        GoDown();

                        int startY = playerIsAttacking ? 0 : 67 - supportSpacing * blockerSupports.Count;
                        SetCursorY(startY);

                        for (int si = 0; si < blockerSupports.Count; si++)
                        {
                            var support = blockerSupports[si];
                            var supportContainer = new CardContainer(AssetManager, relativeCardSizeX, true, null);
                            supportContainer.OnLeftClickAction = delegate
                            {
                                supportContainer.ToggleTargeted();
                            };
                            supportContainer.DrawLayer = si;
                            supportContainer.SetCard(support);
                            AddChild(supportContainer);
                            SetCursor(attackerX, startY + supportSpacing * (si + 1));
                        }
                    }

                    var blockerContainer = new CardContainer(AssetManager, relativeCardSizeX, true, null);
                    blockerContainer.OnLeftClickAction = delegate 
                    { 
                        blockerContainer.ToggleTargeted();
                    };
                    blockerContainer.DrawLayer = blockerSupports.Count;
                    blockerContainer.SetCard(blocker);
                    AddChild(blockerContainer);
                    AddSpacing(1);
                }

                SetCursor(50 - 50 / blockers.Count, 40);
                AddChild(new UiElement(AssetManager.LoadTexture("UI/crossed_swords"), 98 / blockers.Count));
            }
        }
    }
}
