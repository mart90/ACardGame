using ACardGameLibrary;
using System.Collections.Generic;
using System.Linq;

namespace ACardGame.UI
{
    public class Shop : UiContainer
    {
        public List<CardContainer> Cards => ChildrenOfType<CardContainer>().ToList();

        public Shop(AssetManager assetManager, double relativeSize, bool sizeExpressedInX)
            : base(assetManager, 1.8, relativeSize, sizeExpressedInX)
        {
            IsVisible = true;
        }

        public void SetCards(List<Card> cards)
        {
            List<CardContainer> containers = Cards.ToList();

            foreach (var container in Cards)
            {
                container.Clear();
            }

            for (int i = 0; i < cards.Count; i++)
            {
                containers[i].SetCard(cards[i]);
            }
        }
    }
}
