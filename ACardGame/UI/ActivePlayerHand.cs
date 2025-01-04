using ACardGameLibrary;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ACardGame.UI
{
    public class ActivePlayerHand : UiContainer
    {
        public List<CardContainer> Containers => ChildrenOfType<CardContainer>().ToList();

        public int ScaleLevel;

        public ActivePlayerHand(AssetManager assetManager) : base(assetManager, 7.5, 85, true)
        {
            IsVisible = true;
            ScaleLevel = 0;
        }

        public void SetContainers(GameWindow game)
        {
            ClearChildren();

            SetCursor(0, 0);

            int amount = 12 + 3 * ScaleLevel;

            for (int i = 0; i < amount; i++)
            {
                int currentIndex = i;

                var cardContainer = new CardContainer(
                    AssetManager, 
                    (100 - 0.2 * amount) / amount, 
                    true, 
                    delegate { game.TryPlayCard(currentIndex); }, 
                    1 - ScaleLevel / 8f
                );

                AddChild(cardContainer);
                AddSpacing(.2);
            }
        }

        public void SetCards(List<Card> cards, GameWindow game)
        {
            int scaleLevel = cards.Count <= 12 ? 0 : (int)Math.Ceiling((double)(cards.Count - 12) / 3);

            if (ScaleLevel != scaleLevel)
            {
                ScaleLevel = scaleLevel;
            }

            SetContainers(game);

            for (int i = 0; i < cards.Count; i++)
            {
                Containers[i].SetCard(cards[i]);
            }
        }
    }
}
