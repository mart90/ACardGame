using ACardGameLibrary;
using System.Collections.Generic;
using System.Linq;

namespace ACardGame.UI
{
    public class Shop : UiContainer
    {
        public List<CardContainer> Cards => Children.Where(e => e is CardContainer).Cast<CardContainer>().ToList();

        public Shop(AssetManager assetManager, double relativeSize, bool sizeExpressedInX)
            : base(assetManager, 3, relativeSize, sizeExpressedInX)
        {
            IsVisible = true;
        }

        public void SetCards(List<Card> cards)
        {
            List<CardContainer> containers = Children
                .Where(e => e is CardContainer)
                .Cast<CardContainer>()
                .ToList();

            foreach (var container in containers)
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
