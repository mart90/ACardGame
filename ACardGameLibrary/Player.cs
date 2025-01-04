namespace ACardGameLibrary
{
    public class Player
    {
        public Player(string name) 
        {
            Name = name;
            Life = 20;
            MoneyToSpend = 0;
            ShopLevel = 2;
            ShopRefreshCost = 2;

            Deck = new List<Card>();
            DiscardPile = new List<Card>();
            CardsPlayedThisTurn = new List<Card>();
            Shop = new List<Card>();
            ActiveCombatCards = new List<Card>();
            Hand = new List<Card>
            {
                CardLibrary.GetCard("Silver"),
                CardLibrary.GetCard("Silver"),
                CardLibrary.GetCard("Silver"),
                CardLibrary.GetCard("Silver")
            };

            foreach (var card in Hand)
            {
                card.Owner = this;
            }
        }

        public string Name { get; set; }

        public int Life { get; set; }

        public bool IsActive { get; set; }

        public int MoneyToSpend { get; set; }

        public bool IsAttacking { get; set; }
        public bool HasAttackedThisRound { get; set; }
        public int ShopLevel { get; set; }
        public int ShopRefreshCost { get; set; }
        public int FreeShopRefreshes { get; set; }

        public List<Card> Deck { get; private set; }
        public List<Card> DiscardPile { get; private set; }
        public List<Card> Hand { get; private set; }
        public List<Card> Shop { get; private set; }
        public List<Card> ActiveCombatCards { get; private set; }
        public List<Card> CardsPlayedThisTurn { get; private set; }

        public Card Leader { get; set; }

        public bool CanWorship { get; set; }
        public bool CanFreeTrade { get; set; }

        public void DrawCards(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                var card = GetCardFromTopOfDeck();

                if (card != null)
                {
                    Deck.Remove(card);
                    Hand.Add(card);
                }
            }
        }

        public void EndCombatCleanup()
        {
            if (HasAttackedThisRound)
            {
                CardsPlayedThisTurn.AddRange(ActiveCombatCards);
            }
            else
            {
                DiscardPile.AddRange(ActiveCombatCards);
            }

            ActiveCombatCards.Clear();
        }

        public List<Card> GetCardsFromTopOfDeck(int amount)
        {
            var cards = new List<Card>();
            
            if (Deck.Count < amount)
            {
                cards.AddRange(Deck);

                Deck.Clear();
                
                if (!DiscardPile.Any())
                {
                    return cards;
                }

                DiscardPile.Shuffle();
                Deck.AddRange(DiscardPile);
                DiscardPile.Clear();
            }

            int currentCount = cards.Count;

            for (int i = 0; i < amount - currentCount; i++)
            {
                if (!Deck.Any())
                {
                    break;
                }

                cards.Add(Deck.First());
                Deck.RemoveAt(0);
            }

            return cards;
        }

        private Card? GetCardFromTopOfDeck()
        {
            var card = Deck.FirstOrDefault();

            if (card == null)
            {
                if (!DiscardPile.Any())
                {
                    // Empty deck and empty discard pile
                    return null;
                }

                DiscardPile.Shuffle();
                Deck.AddRange(DiscardPile);
                DiscardPile.Clear();

                card = Deck.First();
            }

            if (card != null)
            {
                Deck.Remove(card);
            }

            return card;
        }

        public List<Card> AllCards()
        {
            List<Card> list = Deck
                .Concat(DiscardPile)
                .Concat(Hand)
                .Concat(Shop)
                .Concat(CardsPlayedThisTurn)
                .Concat(ActiveCombatCards)
                .ToList();
            
            if (Leader != null)
            {
                list.Add(Leader);
            }

            return list;
        }

        public void UpgradeShop()
        {
            if (MoneyToSpend < ShopLevel + 3 || ShopLevel == 7)
            {
                return;
            }

            MoneyToSpend -= ShopLevel + 3;

            if (ShopRefreshCost == ShopLevel)
            {
                ShopRefreshCost++;
            }

            ShopLevel++;
        }
    }
}
