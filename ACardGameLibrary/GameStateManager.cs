namespace ACardGameLibrary
{
    public class GameStateManager
    {
        public GameStateManager()
        {
            CardLibrary.NextCardId = 1;

            Players = new List<Player>()
            {
                new("Player1") { IsActive = true },
                new("Player2")
            };

            TurnNumber = 1;

            EventListeners = new List<GameEventListener>();
            AttackingCreatures = new List<CreatureCard>();
            CombatModifiers = new List<CombatModifier>();
            CardsToChooseFromSelector = new List<Card>();

            PublicLog = new List<GameLog>() { new("---- START ----") };

            ShopPool = CardLibrary.GetStartingShop();
            ShopDiscard = new List<Card>();
        }

        public List<Player> Players { get; private set; }

        public Player ActivePlayer => Players.Single(e => e.IsActive);
        public Player Enemy => Players.Single(e => !e.IsActive);
        public Player? Winner { get; set; }

        public Player Attacker => Players.Single(e => e.IsAttacking);
        public Player Defender => Players.Single(e => !e.IsAttacking);

        public List<GameEventListener> EventListeners { get; private set; }

        public List<Card> ShopPool { get; set; }
        public List<Card> ShopDiscard { get; set; }

        public List<GameLog> PublicLog { get; set; }

        public int TurnNumber;

        public bool IsInCombat { get; set; }
        public bool CombatPassed { get; set; }

        public List<CreatureCard> AttackingCreatures { get; set; }
        public List<CombatModifier> CombatModifiers { get; set; }

        public bool ActivateSelectorFlag { get; set; }
        public bool RemoveSelectorFlag { get; set; }
        public List<Card> CardsToChooseFromSelector { get; set; }
        public List<Card> NonSelectedSelectorCards => CardsToChooseFromSelector.Where(e => !e.IsTargeted).ToList();

        public bool RevealOpponentHand { get; set; }

        public bool RequireAccept { get; set; }

        public bool ResolvingAfterPlay { get; set; }
        public int ResolvedEffects { get; set; }

        public MessageToPlayerParams MessageToPlayer { get; set; }

        public bool ActivateShopCostPickerFlag { get; set; }
        public bool RemoveShopCostPickerFlag { get; set; }
        public int ShopCostPicked { get; set; }

        public List<string> OptionsPickerOptions { get; set; }
        public bool RemoveOptionsPickerFlag { get; set; }
        public List<string> OptionsPicked { get; set; }

        public Card? CardGoingToDiscard { get; set; }
        public Card? CardBeingBought { get; set; }

        public int DamageBeingDealt { get; set; }

        public Card? ActionQueued { get; set; }

        public bool EinsteinResolvedFlag {  get; set; }

        public List<Card> AllCards => ActivePlayer.AllCards().Concat(Enemy.AllCards()).Concat(ShopPool).Concat(ShopDiscard).ToList();
        public List<Card> ActiveCombatCards => ActivePlayer.ActiveCombatCards.Concat(Enemy.ActiveCombatCards).ToList();
        public List<SupportCard> ActiveGlobalSupports => ActiveCombatCards.Where(e => e is SupportCard && e is not Equipment).Cast<SupportCard>().ToList();
        public List<Card> TargetedCards => AllCards.Where(e => e.IsTargeted).ToList();
        public Card? TargetingCard => AllCards.SingleOrDefault(e => e.IsTargeting);
        public Card? CardBeingPlayed => AllCards.SingleOrDefault(e => e.IsBeingPlayed);

        public Card GetCardById(int id)
        {
            return AllCards.Single(e => e.Id == id);
        }

        public bool CanPlayCard(bool isActivePlayer, Card card)
        {
            var type = card.GetMainType();
            var player = isActivePlayer ? ActivePlayer : Enemy;

            if (card is CreatureCard creature && creature.IsUnplayable)
            {
                return false;
            }

            if (!IsInCombat & card.IsCombatCard && player.HasAttackedThisRound)
            {
                return false;
            }

            if (!IsInCombat && (card is Equipment || (card is SupportCard support && !support.IsPermanent)))
            {
                return false;
            }

            if (IsInCombat && (type == CardType.Action || type == CardType.Currency || type == CardType.Leader))
            {
                return false;
            }

            if (card.AdditionalPlayConditions != null && !card.AdditionalPlayConditions(this))
            {
                return false;
            }

            return true;
        }

        public bool CanBuyCard(Card card)
        {
            return ActivePlayer.MoneyToSpend >= card.Cost;
        }

        public void PlayCard(Card card)
        {
            Player player = ActivePlayer;
            CardType type = card.GetMainType();

            card.IsBeingPlayed = true;

            if (type != CardType.Currency)
            {
                AddPublicLog($"{ActivePlayer.Name} played {card.Name}");
            }

            if (card.IsCombatCard && !IsInCombat)
            {
                IsInCombat = true;
                player.IsAttacking = true;
                player.HasAttackedThisRound = true;
                TriggerEvent(GameEvent.StartingCombat);
            }

            card.Effects.Where(e => e.EffectPhase == CardEffectPhase.OnPlay).SingleOrDefault()?.Effect(this, player);

            player.Hand.Remove(card);

            if (type == CardType.Leader)
            {
                PlayLeader(player, card);
            }
            else if (!IsInCombat || (card is SupportCard support && !support.IsPermanent && ActivePlayer.IsAttacking))
            {
                ActivePlayer.CardsPlayedThisTurn.Add(card);
            }
            else if (card is SupportCard s && !s.IsPermanent)
            {
                MoveToDiscard(s);
            }
            else
            {
                ActivePlayer.ActiveCombatCards.Add(card);
            }
            
            if (IsInCombat && player.IsAttacking && card is CreatureCard creature)
            {
                AttackingCreatures.Add(creature);
            }

            if (IsInCombat)
            {
                CombatPassed = false;
            }

            switch (type)
            {
                case CardType.Currency: TriggerEvent(GameEvent.PlayingCurrency); break;
                case CardType.Action: TriggerEvent(GameEvent.PlayingAction); break;
                case CardType.Creature: TriggerEvent(GameEvent.PlayingCreature); break;
                case CardType.Support: TriggerEvent(GameEvent.PlayingSupport); break;
                case CardType.Leader: TriggerEvent(GameEvent.PlayingLeader); break;
            }

            if (IsInCombat && !RequireAccept)
            {
                SwitchActivePlayer();
            }

            card.IsBeingPlayed = false;

            if (card.Effects.Any(e => e.EffectPhase == CardEffectPhase.OnAcceptedAfterPlay))
            {
                ResolvingAfterPlay = true;
            }
        }

        private void PlayLeader(Player player, Card card)
        {
            if (player.Leader != null)
            {
                RemoveAllListeners(player.Leader.Name);
            }

            player.Leader = card;
        }

        public void BuyCard(Card card)
        {
            Player player = ActivePlayer;
            
            CardBeingBought = card;

            card.Effects.SingleOrDefault(e => e.EffectPhase == CardEffectPhase.OnBuy)?.Effect(this, player);

            player.DiscardPile.Add(card);
            card.Owner = player;

            player.MoneyToSpend -= card.Cost.Value;

            AddPublicLog($"{player.Name} bought {card.Name} for {card.Cost.Value} currency");

            if (ActivePlayer.Shop.Contains(card))
            {
                card.Cost += EventListeners.Count(e => e.Name == "MerchantRefreshing");
                ActivePlayer.Shop.Remove(card);
            }

            TriggerEvent(GameEvent.Buying);

            CardBeingBought = null;
        }

        public bool PlayerIsFreeTradeBuying(Player player)
        {
            return player.CanFreeTrade
                && TargetedCards.Any()
                && !RequireAccept
                && TargetingCard == null;
        }

        public Card? TryBuyCardFromDiscardPile()
        {
            var card = TargetedCards.Single();
            card.IsTargeted = false;

            if (CanBuyCard(card) && ActivePlayer.ShopLevel >= card.Cost)
            {
                AddPublicLog($"{ActivePlayer.Name} is using Free trade to buy from a shop discard pile");

                BuyCard(card);

                ShopDiscard.Remove(card);

                return card;
            }

            return null;
        }

        public void AddEventListener(GameEventListener listener)
        {
            EventListeners.Add(listener);
        }

        public void RemoveFirstListener(string name, Player owner = null)
        {
            var query = EventListeners.AsEnumerable();

            if (owner != null)
            {
                query = query.Where(e => e.Owner == owner);
            }

            query = query.Where(e => e.Name == name);

            var listener = query.FirstOrDefault();

            if (listener == null)
            {
                return;
            }

            EventListeners.Remove(listener);
        }

        public void RemoveAllListeners (string name, Player owner = null)
        {
            if (owner != null)
            {
                EventListeners.RemoveAll(e => e.Name == name && e.Owner == owner);
            }
            else
            {
                EventListeners.RemoveAll(e => e.Name == name);
            }
        }

        /// <summary>
        /// Returns true if the triggered event needs input from user
        /// </summary>
        public bool TriggerEvent(GameEvent gameEvent)
        {
            var triggeredListeners = new List<GameEventListener>(EventListeners.Where(e => e.Trigger == gameEvent && !e.IsTriggered && (!e.OwnersTurnOnly || e.Owner.IsActive)));

            foreach (GameEventListener listener in triggeredListeners)
            {
                listener.Effect(this, listener.Owner);

                if (listener.NeedsUserInput)
                {
                    listener.IsTriggered = true;
                    return true;
                }
            }

            return false;
        }

        public void ResetTriggers(GameEvent gameEvent)
        {
            foreach (GameEventListener listener in EventListeners.Where(e => e.Trigger == gameEvent))
            {
                listener.IsTriggered = false;
            }
        }

        public void MoveToDiscard(Card card)
        {
            CardGoingToDiscard = card;
            card.Effects.SingleOrDefault(e => e.EffectPhase == CardEffectPhase.OnMoveToDiscard)?.Effect(this, card.Owner);
            CardGoingToDiscard = null;

            card.Owner.DiscardPile.Add(card);
        }

        public void SwitchTurn()
        {
            foreach (Card card in ActivePlayer.CardsPlayedThisTurn)
            {
                MoveToDiscard(card);
            }

            ActivePlayer.CardsPlayedThisTurn.Clear();

            if (TriggerEvent(GameEvent.EndingTurn))
            {
                return;
            }

            ResetTriggers(GameEvent.EndingTurn);

            ActivePlayer.DrawCards(TurnNumber == 1 ? 2 : 3);

            ActivePlayer.MoneyToSpend = 0;
            ActivePlayer.HasAttackedThisRound = false;
            ActivePlayer.FreeShopRefreshes = 1;
            ActivePlayer.CanFreeTrade = false;

            foreach (var player in Players)
            {
                player.IsActive = !player.IsActive;
            }

            var allCreaturesInHands = ActivePlayer.Hand.Concat(Enemy.Hand).Where(e => e is CreatureCard).Cast<CreatureCard>();
            foreach (var creature in allCreaturesInHands)
            {
                creature.IsUnplayable = false;
            }

            AddPublicLog($"{ActivePlayer.Name}'s turn");

            TriggerEvent(GameEvent.StartingTurn);

            TurnNumber++;
        }

        public void SwitchActivePlayer()
        {
            foreach (var player in Players)
            {
                player.IsActive = !player.IsActive;
            }
        }

        public void ResolveCombat()
        {
            if (TriggerEvent(GameEvent.ResolvingCombat))
            {
                return;
            }

            ResetTriggers(GameEvent.ResolvingCombat);

            if (!ActivePlayer.IsAttacking)
            {
                SwitchActivePlayer();
            }

            int totalDamageDealt = 0;

            foreach (var attacker in AttackingCreatures)
            {
                (int damageDealt, bool isSuccessfullyBlocked) = AttackerDamageDealt(attacker);

                if (attacker.Name == "Assassin" && isSuccessfullyBlocked)
                {
                    Attacker.ActiveCombatCards.Remove(attacker);
                    AddPublicLog("Assassin was exiled");
                }

                DealDamage(damageDealt);

                if (DamageBeingDealt > 0)
                {
                    attacker.DealtDamage = true;
                }

                totalDamageDealt += DamageBeingDealt;
            }

            AddPublicLog($"Combat resolved. {ActivePlayer.Name} dealt {totalDamageDealt} damage");

            ArchersDealDamage();

            TriggerEvent(GameEvent.EndingCombat);

            foreach (var creature in Attacker.ActiveCombatCards.Concat(Defender.ActiveCombatCards).Where(e => e is CreatureCard).Cast<CreatureCard>())
            {
                creature.Reset();
            }

            CombatModifiers.Clear();
            AttackingCreatures.Clear();

            foreach (var player in Players)
            {
                player.EndCombatCleanup();
            }

            IsInCombat = false;
            ActivePlayer.IsAttacking = false;
        }

        public void DealDamage(int amount)
        {
            DamageBeingDealt = amount;
            TriggerEvent(GameEvent.DealingDamage);
            Enemy.Life -= DamageBeingDealt;

            if (Enemy.Life <= 0)
            {
                Winner = ActivePlayer;
            }
        }

        public int CreaturePower(CreatureCard creature)
        {
            int power = creature.Power + creature.TemporaryAddedPower;

            foreach (var equipment in creature.AttachedEquipments)
            {
                power += equipment.AddedPower;
            }

            var modifiers = creature.Owner.IsAttacking ?
                CombatModifiers.Where(e => e.Owner == Attacker || !e.OwnerOnly) 
                : CombatModifiers.Where(e => e.Owner == Defender || !e.OwnerOnly);
            
            foreach (CombatModifier modifier in modifiers.Where(e => e.ConditionsEnemy == null))
            {
                if (modifier.CardMeetsConditions(creature))
                {
                    power += modifier.AddedPower;
                }
            }

            return power;
        }

        public int CreatureDefense(CreatureCard creature)
        {
            int defense = creature.Defense + creature.TemporaryAddedDefense;

            foreach (var equipment in creature.AttachedEquipments)
            {
                defense += equipment.AddedDefense;
            }

            var modifiers = creature.Owner.IsAttacking ?
                CombatModifiers.Where(e => e.Owner == Attacker || !e.OwnerOnly)
                : CombatModifiers.Where(e => e.Owner == Defender || !e.OwnerOnly);

            foreach (CombatModifier modifier in modifiers.Where(e => e.ConditionsEnemy == null))
            {
                if (modifier.CardMeetsConditions(creature))
                {
                    defense += modifier.AddedDefense;
                }
            }

            return defense;
        }

        public void RemoveIfDead(CreatureCard creature)
        {
            if (CreatureDefense(creature) > 0)
            {
                return;
            }

            AddPublicLog($"{creature.Name} dropped to 0 defense and was removed from the battlefield");

            RemoveCardFromBattlefield(creature);

            if (creature.Owner.IsAttacking)
            {
                creature.Owner.CardsPlayedThisTurn.Add(creature);
            }
            else
            {
                MoveToDiscard(creature);
            }
        }

        private (int damageDealt, bool isSuccessfullyBlocked) AttackerDamageDealt(CreatureCard attacker)
        {
            int attackerPower = CreaturePower(attacker);
            int attackerDefense = CreatureDefense(attacker);

            int defendingPower = 0;
            int defendingDefense = 0;

            var attackerModifiers = CombatModifiers.Where(e => e.Owner == Attacker || !e.OwnerOnly);
            var defenderModifiers = CombatModifiers.Where(e => e.Owner == Defender || !e.OwnerOnly);

            foreach (var defender in attacker.BlockedBy)
            {
                if (attacker.Types.Contains(CardType.Flying)
                    && !defender.Types.Contains(CardType.Flying)
                    && !defender.Types.Contains(CardType.Ranged)
                    && defender.Name != "Tree of life")
                {
                    continue;
                }

                foreach (CombatModifier modifier in attackerModifiers.Where(e => e.ConditionsEnemy != null))
                {
                    if (modifier.CardMeetsConditions(attacker) && modifier.EnemyMeetsConditions(defender))
                    {
                        attackerPower += modifier.AddedPower;
                        attackerDefense += modifier.AddedDefense;
                    }
                }

                defendingPower += CreaturePower(defender);
                defendingDefense += CreatureDefense(defender);

                foreach (CombatModifier modifier in defenderModifiers.Where(e => e.ConditionsEnemy != null))
                {
                    if (modifier.CardMeetsConditions(defender) && modifier.EnemyMeetsConditions(attacker))
                    {
                        defendingPower += modifier.AddedPower;
                        defendingDefense += modifier.AddedDefense;
                    }
                }
            }

            if (attackerPower < 0)
            {
                attackerPower = 0;
            }
            if (defendingPower < 0)
            {
                defendingPower = 0;
            }

            if (attackerPower < defendingDefense)
            {
                return (0, true);
            }
            else if (attackerDefense <= defendingPower)
            {
                if (attacker.HasTrample)
                {
                    return (attackerPower - defendingDefense, true);
                }
                else
                {
                    return (0, true);
                }
            }
            else if (attackerPower == 0) 
            {
                return (0, false);
            }

            return (attackerPower, false);
        }

        private void ArchersDealDamage()
        {
            foreach (var archer in AttackingCreatures.Where(e => e.Name == "Archer"))
            {
                int power = archer.Power + archer.TemporaryAddedPower;

                foreach (CombatModifier modifier in CombatModifiers.Where(e => e.ConditionsEnemy == null && (e.Owner == Attacker || !e.OwnerOnly)))
                {
                    if (modifier.CardMeetsConditions(archer))
                    {
                        power += modifier.AddedPower;
                    }
                }

                foreach (var equipment in archer.AttachedEquipments)
                {
                    power += equipment.AddedPower;
                }

                if (power < 0)
                {
                    power = 0;
                }

                DealDamage(power);

                if (DamageBeingDealt > 0)
                {
                    archer.DealtDamage = true;
                }

                AddPublicLog($"Archer dealt {DamageBeingDealt} damage");
            }
        }

        public void CombatPass()
        {
            AddPublicLog($"{ActivePlayer.Name} passed");

            if (CombatPassed)
            {
                ResolveCombat();
            }
            else
            {
                CombatPassed = true;
                SwitchActivePlayer();
            }
        }

        public List<SupportCard> GetPlayerSupports(bool isActivePlayer)
        {
            return ActiveGlobalSupports
                .Where(e => isActivePlayer == (e.Owner == ActivePlayer))
                .ToList();
        }

        public void RefreshShop(Player player)
        {
            TriggerEvent(GameEvent.RefreshingShop);

            ShopDiscard.AddRange(player.Shop);
            player.Shop.Clear();

            int refreshCost = player.ShopRefreshCost;

            for (int i = 0; i < 3; i++)
            {
                if (!ShopPool.Any(e => e.Cost == refreshCost))
                {
                    ShopDiscard.Shuffle();
                    ShopPool.AddRange(ShopDiscard.Where(e => e.Cost == refreshCost).ToList());
                    ShopDiscard.RemoveAll(e => e.Cost == refreshCost);
                }

                var card = ShopPool.FirstOrDefault(e => e.Cost == refreshCost);

                if (card == null)
                {
                    break;
                }

                player.Shop.Add(card);
                ShopPool.Remove(card);
            }

            TriggerEvent(GameEvent.RefreshedShop);
        }

        public bool IsValidTarget(Card card, Card target)
        {
            if (card.Owner == target.Owner && card.MustTargetEnemy)
            {
                return false;
            }
            else if (card.Owner != target.Owner && card.MustTargetFriend)
            {
                return false;
            }

            if (IsInCombat
                && !card.Owner.IsAttacking 
                && card is CreatureCard 
                && target is CreatureCard attacker
                && card.Owner != target.Owner
                && !ResolvingAfterPlay
                && !attacker.IsUnblockable
                && (attacker.HasTaunt || !MustBlockTauntCreature()))
            {
                if (target.Name == "Horse archer")
                {
                    return card.Types.Contains(CardType.Ranged);
                }

                return true;
            }

            if (!card.ValidTargetTypes.Intersect(target.Types).Any())
            {
                return false;
            }

            if (card.AdditionalTargetConditions != null && (card is not CreatureCard || ResolvingAfterPlay) && !card.AdditionalTargetConditions(this, target))
            {
                return false;
            }

            return true;
        }

        private bool MustBlockTauntCreature()
        {
            return AttackingCreatures.Any(e => e.HasTaunt && !e.BlockedBy.Any());
        }

        public void BuySilver()
        {
            if (ActivePlayer.MoneyToSpend < 3)
            {
                return;
            }

            var card = CardLibrary.GetCard("Silver");

            BuyCard(card);
        }

        public void BuyGold()
        {
            if (ActivePlayer.MoneyToSpend < 6)
            {
                return;
            }

            var card = CardLibrary.GetCard("Gold");

            BuyCard(card);
        }

        public void ActionRefreshShop()
        {
            if (ActivePlayer.FreeShopRefreshes > 0)
            {
                AddPublicLog($"{ActivePlayer.Name} refreshed their shop");
                ActivePlayer.FreeShopRefreshes--;
            }
            else
            {
                string payment = "1 currency";

                if (ActivePlayer.MoneyToSpend >= 1)
                {
                    ActivePlayer.MoneyToSpend--;
                }
                else if (ActivePlayer.Life >= 3)
                {
                    ActivePlayer.Life -= 3;
                    payment = "3 life";
                }
                else
                {
                    return;
                }

                AddPublicLog($"{ActivePlayer.Name} refreshed their shop for {payment}");
            }

            RefreshShop(ActivePlayer);
        }

        public void UpgradeShop()
        {
            ActivePlayer.UpgradeShop();
            AddPublicLog($"{ActivePlayer.Name} upgraded their shop to level {ActivePlayer.ShopLevel}");
        }

        public void RemoveCardFromBattlefield(Card card)
        {
            card.Effects.SingleOrDefault(e => e.EffectPhase == CardEffectPhase.OnRemove)?.Effect(this, card.Owner);

            if (card is CreatureCard creature)
            {
                RemoveCreatureFromBattlefield(creature);
            }
            else
            {
                RemoveSupportFromBattlefield((SupportCard)card);
            }
        }

        private void RemoveCreatureFromBattlefield(CreatureCard creature)
        {
            foreach (SupportCard support in new List<Equipment>(creature.AttachedEquipments))
            {
                RemoveCardFromBattlefield(support);

                if (support.Owner.IsAttacking)
                {
                    support.Owner.CardsPlayedThisTurn.Add(support);
                }
                else
                {
                    MoveToDiscard(support);
                }
            }

            creature.AttachedEquipments.Clear();

            if (AttackingCreatures.Contains(creature))
            {
                AttackingCreatures.Remove(creature);

                foreach (var blocker in new List<CreatureCard>(creature.BlockedBy))
                {
                    RemoveCardFromBattlefield(blocker);
                    MoveToDiscard(blocker);
                }

                creature.BlockedBy.Clear();
            }
            else
            {
                foreach (CreatureCard attackingCreature in AttackingCreatures)
                {
                    if (attackingCreature.BlockedBy.Contains(creature))
                    {
                        attackingCreature.BlockedBy.Remove(creature);
                    }
                }
            }

            creature.Owner.ActiveCombatCards.Remove(creature);
            creature.Reset();
        }

        private void RemoveSupportFromBattlefield(SupportCard support)
        {
            support.Owner.ActiveCombatCards.Remove(support);

            if (support is Equipment equipment)
            {
                foreach (CreatureCard attacker in AttackingCreatures)
                {
                    if (attacker.AttachedEquipments.Contains(equipment))
                    {
                        attacker.AttachedEquipments.Remove(equipment);
                        break;
                    }

                    foreach (CreatureCard blocker in attacker.BlockedBy)
                    {
                        if (blocker.AttachedEquipments.Contains(equipment))
                        {
                            blocker.AttachedEquipments.Remove(equipment);
                            break;
                        }
                    }
                }
            }
        }

        public void SetTargetingCard(Card card)
        {
            card.IsTargeting = true;
            RequireAccept = true;
        }

        public void CardBeingPlayedIsTargeting()
        {
            CardBeingPlayed.IsTargeting = true;
            RequireAccept = true;
        }

        public virtual void PlayActionQueued()
        {
            ActionQueued.Owner.Hand.Add(ActionQueued);

            PlayCard(ActionQueued);

            ActionQueued = null;
        }

        public void Worship()
        {
            var eva = ActivePlayer.Leader;

            if (ActivePlayer.MoneyToSpend < 6)
            {
                return;
            }

            eva.Counters++;
            ActivePlayer.MoneyToSpend -= 6;

            if (eva.Counters == 10)
            {
                Winner = ActivePlayer;
            }
        }

        public void AddPublicLog(string message)
        {
            PublicLog.Add(new GameLog(message));
        }
    }
}
