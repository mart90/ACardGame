using ACardGameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace ACardGame.UI
{
    public class CardContainer : UiContainer, ILeftClickable, IHoverable
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

        public bool IsHovered { get; set; }
        public Texture2D HoverTexture { get; set; }
        public string ToolTipOnHover { get; set; }
        public Action OnHoverAction { get; set; }

        public CardContainer(AssetManager assetManager, double relativeSize, bool sizeExpressedInX, Action onClickAction = null, float scale = 1)
            : base(assetManager, 0.6, relativeSize, sizeExpressedInX)
        {
            OnLeftClickAction = onClickAction;

            CardTitle = new CardText(assetManager.LoadFont("cardTitleFont"), 96, 3, scale) 
            { 
                TextIsCentered = true,
                ForceOneLine = true
            };
            CardText = new CardText(assetManager.LoadFont("cardTextFont"), 90, 0.7, scale);
            CardCost = new CardText(assetManager.LoadFont("cardCostFont"), 20, 1, scale);
            CardSubTypes = new CardText(assetManager.LoadFont("cardTextFont"), 90, 4, scale) 
            { 
                TextIsCentered = true,
                ForceOneLine = true
            };
            CardCombatStats = new CardText(assetManager.LoadFont("cardCombatStatsFont"), 30, 1.5, scale);
            CardCurrencyValue = new CardText(assetManager.LoadFont("cardCurrencyValueFont"), 30, 1, scale) 
            { 
                TextIsCentered = true 
            };

            SetCursor(2, 0);
            AddChild(CardTitle);

            SetCursor(6, 20);
            AddChild(CardText);

            SetCursor(8, 90);
            AddChild(CardCost);

            SetCursor(5, 76);
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
            CardText.Text = card.Text;

            CardCost.Text = card.Cost.ToString();
            CardSubTypes.Text = string.Join(", ", card.GetSubTypes());

            if (card is CreatureCard creature)
            {
                CardCombatStats.Text = $"{creature.Power + creature.TemporaryAddedPower}/{creature.Defense + creature.TemporaryAddedDefense}";
            }
            else
            {
                CardCombatStats.Text = "";
            }

            if (card is CurrencyCard currency && currency.CurrencyValue != 0)
            {
                CardCurrencyValue.Text = currency.CurrencyValue.ToString();
            }
            else
            {
                CardCurrencyValue.Text = "";
            }
        }

        public void ToggleTargeted()
        {
            Card.IsTargeted = !Card.IsTargeted;
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
            if (Card?.Name == "Eva" && Card?.Counters > 0)
            {
                CardCurrencyValue.Text = Card.Counters.ToString();
            }

            base.Draw(spriteBatch);

            if (Card?.IsTargeted == true)
            {
                spriteBatch.Draw(AssetManager.LoadTexture("UI/selected"), AbsoluteLocation, Color.Magenta);
            }
            else if (Card?.IsTargeting == true)
            {
                spriteBatch.Draw(AssetManager.LoadTexture("UI/selected"), AbsoluteLocation, Color.Lime);
            }
        }

        public override IHoverable Hover(Point position)
        {
            return this;
        }
    }
}
