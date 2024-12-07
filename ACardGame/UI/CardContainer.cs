using ACardGameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ACardGame.UI
{
    public class CardContainer : UiContainer, ILeftClickable
    {
        public Card Card { get; private set; }

        public UiElement CardTitle { get; set; }
        public UiElement CardText { get; set; }
        public UiElement CardCost { get; set; }
        public UiElement CardSubTypes { get; set; }
        public UiElement CardCombatStats { get; set; }
        public UiElement CardCurrencyValue { get; set; }

        public bool IsBeingLeftClicked { get; set; }
        public Texture2D BeingLeftClickedTexture { get; set; }
        public Action OnLeftClickAction { get; set; }

        public bool IsTargeted { get; set; }

        public CardContainer(AssetManager assetManager, double relativeSize, bool sizeExpressedInX, Action onClickAction = null)
            : base(assetManager, 0.6, relativeSize, sizeExpressedInX)
        {
            OnLeftClickAction = onClickAction;

            CardTitle = new UiElement(null, assetManager.LoadFont("cardTitleFont"), Color.Black, 90, true, 3) { TextIsCentered = true };
            CardText = new UiElement(null, assetManager.LoadFont("cardTextFont"), Color.Black, 94, true, 0.7);
            CardCost = new UiElement(null, assetManager.LoadFont("cardCostFont"), Color.Black, 20, true, 1);
            CardSubTypes = new UiElement(null, assetManager.LoadFont("cardTextFont"), Color.Black, 90, true, 4) { TextIsCentered = true };
            CardCombatStats = new UiElement(null, assetManager.LoadFont("cardCombatStatsFont"), Color.Black, 30, true, 1.5);
            CardCurrencyValue = new UiElement(null, assetManager.LoadFont("cardCurrencyValueFont"), Color.Black, 30, true, 1) { TextIsCentered = true };

            SetCursor(5, 0);
            AddChild(CardTitle);

            SetCursor(6, 20);
            AddChild(CardText);

            SetCursor(8, 90);
            AddChild(CardCost);

            SetCursor(5, 75);
            AddChild(CardSubTypes);

            SetCursor(78, 90);
            AddChild(CardCombatStats);

            SetCursor(37, 50);
            AddChild(CardCurrencyValue);
        }

        public void SetCard(Card card)
        {
            Card = card;

            Texture = AssetManager.LoadCardTexture(Card.GetMainType().ToString());

            CardTitle.Text = card.Name;
            CardText.Text = "";

            if (card.Text != null)
            {
                int lineIndex = 0;

                foreach (string word in card.Text.Split(' '))
                {
                    lineIndex += word.Length;

                    if (lineIndex > 19)
                    {
                        lineIndex = word.Length;
                        CardText.Text += "\n";
                    }

                    CardText.Text += word + " ";
                }
            }

            CardCost.Text = card.Cost.ToString();
            CardSubTypes.Text = string.Join(", ", card.GetSubTypes());

            if (card is CreatureCard creature)
            {
                CardCombatStats.Text = $"{creature.Power}/{creature.Defense}";
            }
            else
            {
                CardCombatStats.Text = "";
            }

            if (card is CurrencyCard currency)
            {
                CardCurrencyValue.Text = currency.CurrencyValue.ToString();
            }
            else
            {
                CardCurrencyValue.Text = "";
            }
        }

        public void Clear()
        {
            Card = null;
            Texture = null;
            CardTitle.Text = null;
            CardText.Text = null;
            CardCost.Text = null;
            CardSubTypes.Text = null;
            CardCombatStats.Text = null;
            CardCurrencyValue.Text = null;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        protected override UiElement GetHoveredChildRecursive(Point position, UiContainer parent)
        {
            return this;
        }
    }
}
