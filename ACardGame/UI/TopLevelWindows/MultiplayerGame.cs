using ACardGameLibrary;
using ACardGameServer;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace ACardGame.UI
{
    public class MultiplayerGame : GameWindow
    {
        public int Id { get; set; }
        public User AuthenticatedUser => _server.AuthenticatedUser;

        private readonly ServerConnection _server;

        public override Player Player => GameState.Players.Single(e => e.Name ==  AuthenticatedUser.Name);
        public override Player Enemy => GameState.Players.Single(e => e.Name != AuthenticatedUser.Name);
        public override bool IsMyTurn => Player.IsActive;

        private GameMove _makeMoveMessage;

        public MultiplayerGame(AssetManager assetManager, GameStateManager gameStateManager, ServerConnection server) : base(assetManager, gameStateManager)
        {
            Texture = assetManager.LoadTexture("UI/wallpaper");
            CorrespondingUiState = UiState.MultiplayerGame;

            _server = server;
        }

        public override void Update()
        {
            base.Update();

            if (UpdateCounter % 15 == 0)
            {
                _server.PollEvents();
            }

            if (_server.IncomingMessage != null)
            {
                if (_server.IncomingMessage.MessageType == ServerMessageType.MakeMove)
                {
                    ResolveGameMove(JsonConvert.DeserializeObject<GameMove>(_server.IncomingMessage.Data));
                }

                _server.IncomingMessage = null;
            }

            if (!IsMyTurn)
            {
                BuySilverButton.IsVisible = false;
                UpgradeShopButton.IsVisible = false;
                BuyGoldButton.IsVisible = false;
                WorshipButton.IsVisible = false;
                RefreshShopButton.IsVisible = false;
                EndTurnButton.IsVisible = false;
                AcceptButton.IsVisible = false;

                ShopCostPicker.IsVisible = false;
                CardSelector.IsVisible = false;
                OptionPicker.IsVisible = false;

                EnemyName.Text = Enemy.Name + "*";
                PlayerName.Text = Player.Name;
            }
            else
            {
                PlayerName.Text = Player.Name + "*";
                EnemyName.Text = Enemy.Name;
            }
        }

        private void ResolveGameMove(GameMove move)
        {
            Card card = null;
            if ( (move.Type == MoveType.PlayingActionQueued))
            {
                card = GameState.ActionQueued;
            }
            else if (move.CardId != null)
            {
                card = GameState.GetCardById(move.CardId.Value);
            }

            if (move.Type == MoveType.EndingTurn)
            {
                GameState.SwitchTurn();
            }
            else if (move.Type == MoveType.Passing)
            {
                GameState.CombatPass();
            }
            else if (move.Type == MoveType.BuyingFromShop)
            {
                GameState.BuyCard(card);
            }
            else if (move.Type == MoveType.RefreshingShop)
            {
                GameState.ActionRefreshShop();
            }
            else if (move.Type == MoveType.UpgradingShop)
            {
                GameState.UpgradeShop();
            }
            else if (move.Type == MoveType.BuyingSilver)
            {
                GameState.BuySilver();
            }
            else if (move.Type == MoveType.BuyingGold)
            {
                GameState.BuyGold();
            }
            else if (move.Type == MoveType.Worshiping)
            {
                GameState.Worship();
            }
            else if (move.Type == MoveType.PlayingCard)
            {
                ResolvePlayCard(move, card);
            }
            else if (move.Type == MoveType.PlayingActionQueued)
            {
                GameState.ActionQueued.Owner.Hand.Add(GameState.ActionQueued);
                ResolvePlayCard(move, GameState.ActionQueued);
                GameState.ActionQueued = null;
            }
            else if (move.Type == MoveType.EventUsedInput)
            {
                ResolveAcceptParams(move);
            }

            if (GameState.IsInCombat)
            {
                Battlefield.Refresh(GameState, Player.IsAttacking);
            }

            RefreshHand();
        }

        private void ResolvePlayCard(GameMove move, Card card)
        {
            if (card.IsCombatCard && !GameState.IsInCombat)
            {
                ToggleShopVisible();
                ToggleShopButton.IsVisible = true;
            }

            if (!move.AcceptParams.Any())
            {
                GameState.PlayCard(card);
                
                return;
            }

            if (!CardTargetsOnPlay(card))
            {
                GameState.PlayCard(card);
            }
            else
            {
                card.IsTargeting = true;
                GameState.RequireAccept = true;
            }

            ResolveAcceptParams(move);
        }

        private void ResolveAcceptParams(GameMove move)
        {
            List<GameMoveAcceptParams> acceptParams = move.AcceptParams.OrderBy(e => e.Order).ToList();

            foreach (GameMoveAcceptParams acceptParam in acceptParams)
            {
                if (acceptParam.TargetedCardIds.Any())
                {
                    foreach (var targetedCardId in acceptParam.TargetedCardIds)
                    {
                        GameState.GetCardById(targetedCardId).IsTargeted = true;
                    }
                }
                else if (acceptParam.OptionsPicked.Any())
                {
                    GameState.OptionsPicked = acceptParam.OptionsPicked;
                }
                else if (acceptParam.ShopCostPicked != null)
                {
                    GameState.ShopCostPicked = acceptParam.ShopCostPicked.Value;
                }

                base.ResolveAccepted(true);
            }

            GameState.RemoveOptionsPickerFlag = true;
            GameState.RemoveSelectorFlag = true;
            GameState.RemoveShopCostPickerFlag = true;

            GameState.MessageToPlayer = null;
            MessageToPlayer.Text = "";
        }

        public override void LeftClick(Point position)
        {
            if (IsMyTurn)
            {
                base.LeftClick(position);
            }
            else
            {
                if (LogViewer.IsVisible)
                {
                    LogViewer.IsVisible = false;
                }

                if (MessageToPlayer.IsVisible)
                {
                    MessageToPlayer.IsVisible = false;
                }

                if (CardStackViewer.IsVisible)
                {
                    CardStackViewer.IsVisible = false;
                }

                var child = GetChildByPosition(position);

                if (child == null)
                {
                    return;
                }

                var enabledChildren = new List<UiElement>
                {
                    ViewLogButton,
                    ActivePlayerDiscardPile,
                    EnemyDiscardPile,
                    ToggleShopButton,
                    ShowCardsPlayedThisTurnButton
                };

                enabledChildren.AddRange(ViewShopDiscardButtons);

                if (enabledChildren.Contains(child) && child is ILeftClickable clickableChild && clickableChild.OnLeftClickAction != null)
                {
                    clickableChild.OnLeftClickAction();
                }
            }
        }

        protected override void PlayCard(Card card)
        {
            _makeMoveMessage = new GameMove
            {
                GameId = Id,
                Type = MoveType.PlayingCard,
                CardId = card.Id
            };

            base.PlayCard(card);

            if (!GameState.RequireAccept)
            {
                SendMakeMoveMessage();
            }
        }

        protected override void PlayActionQueued()
        {
            _makeMoveMessage = new GameMove
            {
                GameId = Id,
                Type = MoveType.PlayingActionQueued,
                CardId = GameState.ActionQueued.Id
            };

            base.PlayActionQueued();

            if (!GameState.RequireAccept)
            {
                SendMakeMoveMessage();
            }
        }

        protected override void BuyCard(Card card)
        {
            _makeMoveMessage = new GameMove
            {
                GameId = Id,
                Type = MoveType.BuyingFromShop,
                CardId = card.Id
            };

            base.BuyCard(card);

            SendMakeMoveMessage();
        }

        protected override void EndTurn()
        {
            if (GameState.IsInCombat)
            {
                _makeMoveMessage = new GameMove
                {
                    GameId = Id,
                    Type = MoveType.Passing
                };

                GameState.CombatPass();
            }
            else
            {
                _makeMoveMessage = new GameMove
                {
                    GameId = Id,
                    Type = MoveType.EndingTurn
                };

                GameState.SwitchTurn();
            }

            SendMakeMoveMessage();
        }

        protected override void UpgradeShop()
        {
            _makeMoveMessage = new GameMove
            {
                GameId = Id,
                Type = MoveType.UpgradingShop
            };

            base.UpgradeShop();

            SendMakeMoveMessage();
        }

        protected override void Worship()
        {
            _makeMoveMessage = new GameMove
            {
                GameId = Id,
                Type = MoveType.Worshiping
            };

            base.Worship();

            SendMakeMoveMessage();
        }

        protected override void BuyGold()
        {
            _makeMoveMessage = new GameMove
            {
                GameId = Id,
                Type = MoveType.BuyingGold
            };

            base.BuyGold();

            SendMakeMoveMessage();
        }

        protected override void BuySilver()
        {
            _makeMoveMessage = new GameMove
            {
                GameId = Id,
                Type = MoveType.BuyingSilver
            };

            base.BuySilver();

            SendMakeMoveMessage();
        }

        protected override void ActionRefreshShop()
        {
            _makeMoveMessage = new GameMove
            {
                GameId = Id,
                Type = MoveType.RefreshingShop
            };

            base.ActionRefreshShop();

            SendMakeMoveMessage();
        }

        protected override void ResolveAccepted(bool skipValidityChecks = false)
        {
            var card = GameState.TargetingCard;
            var targets = GameState.TargetedCards;

            bool makeMoveMessageOriginatedHere = false;
            bool isValid = true;

            if (CardTargetsOnPlay(card) && !GameState.ResolvingAfterPlay)
            {
                _makeMoveMessage = new GameMove
                {
                    GameId = Id,
                    CardId = card.Id,
                    Type = MoveType.PlayingCard
                };
                makeMoveMessageOriginatedHere = true;
            }
            else if (card.GetMainType() == CardType.Leader)
            {
                _makeMoveMessage = new GameMove
                {
                    GameId = Id,
                    CardId = card.Id,
                    Type = MoveType.EventUsedInput
                };
                makeMoveMessageOriginatedHere = true;
            }

            if (ShopCostPicker.IsVisible)
            {
                GameState.ShopCostPicked = ShopCostPicker.CostPicked;

                _makeMoveMessage.AcceptParams.Add(new GameMoveAcceptParams 
                { 
                    Order = _makeMoveMessage.AcceptParams.Count,
                    ShopCostPicked = ShopCostPicker.CostPicked
                });
            }
            else if (OptionPicker.IsVisible)
            {
                if (OptionPicker.OptionsPicked.Count > card.MaxTargets)
                {
                    SetMessageToPlayer(new MessageToPlayerParams
                    {
                        Message = "Too many options selected",
                        Severity = MessageSeverity.Error
                    });

                    isValid = false;
                }
                else if (OptionPicker.OptionsPicked.Count < card.MinTargets)
                {
                    SetMessageToPlayer(new MessageToPlayerParams
                    {
                        Message = $"Select at least {card.MinTargets} option(s)",
                        Severity = MessageSeverity.Error
                    });

                    isValid = false;
                }
                else
                {
                    GameState.OptionsPicked = OptionPicker.OptionsPicked;

                    _makeMoveMessage.AcceptParams.Add(new GameMoveAcceptParams
                    {
                        Order = _makeMoveMessage.AcceptParams.Count,
                        OptionsPicked = OptionPicker.OptionsPicked
                    });
                }
            }
            else
            {
                if (targets.Count < card.MinTargets)
                {
                    SetMessageToPlayer(new MessageToPlayerParams
                    {
                        Message = "Not enough targets",
                        Severity = MessageSeverity.Error
                    });

                    isValid = false;
                }
                else if (targets.Any())
                {
                    if (card.MaxTargets < targets.Count)
                    {
                        SetMessageToPlayer(new MessageToPlayerParams
                        {
                            Message = "Too many targets",
                            Severity = MessageSeverity.Error
                        });

                        targets.ForEach(e => e.IsTargeted = false);

                        isValid = false;
                    }
                    else
                    {
                        foreach (var target in targets)
                        {
                            bool allValid = true;

                            if (!GameState.IsValidTarget(card, target))
                            {
                                target.IsTargeted = false;
                                allValid = false;
                            }

                            if (!allValid)
                            {
                                SetMessageToPlayer(new MessageToPlayerParams
                                {
                                    Message = "Some targets were invalid. They were deselected",
                                    Severity = MessageSeverity.Error
                                });

                                isValid = false;
                            }
                        }
                    }
                }
                else if (card is CreatureCard && !card.Owner.IsAttacking && !GameState.ResolvingAfterPlay)
                {
                    SetMessageToPlayer(new MessageToPlayerParams
                    {
                        Message = "Target a creature to block",
                        Severity = MessageSeverity.Error
                    });

                    isValid = false;
                }

                if (isValid)
                {
                    _makeMoveMessage.AcceptParams.Add(new GameMoveAcceptParams
                    {
                        Order = _makeMoveMessage.AcceptParams.Count,
                        TargetedCardIds = targets.Select(e => e.Id).ToList()
                    });
                }
            }

            if (!isValid)
            {
                if (makeMoveMessageOriginatedHere)
                {
                    _makeMoveMessage = null;
                }

                return;
            }

            GameState.RequireAccept = false;

            bool doneResolvingAfterplay = false;
            if (GameState.ResolvingAfterPlay)
            {
                card.IsBeingPlayed = true;

                IEnumerable<CardEffect> afterPlayEffects = card.Effects.Where(e => e.EffectPhase == CardEffectPhase.OnAcceptedAfterPlay);

                afterPlayEffects.SingleOrDefault(e => e.ResolveOrder == GameState.ResolvedEffects)?.Effect(GameState, card.Owner);
                GameState.ResolvedEffects++;

                if (GameState.ResolvedEffects >= afterPlayEffects.Count())
                {
                    doneResolvingAfterplay = true;
                }
            }
            else
            {
                CardEffect onAccepted = card.Effects.Where(e => e.EffectPhase == CardEffectPhase.OnAccepted).SingleOrDefault();
                onAccepted?.Effect(GameState, card.Owner);
            }

            if (GameState.IsInCombat)
            {
                Battlefield.Refresh(GameState, Player.IsAttacking);
            }

            if (!GameState.ResolvingAfterPlay || (GameState.ResolvingAfterPlay && doneResolvingAfterplay))
            {
                GameState.TargetingCard.IsTargeting = false;
            }

            targets.ForEach(e => e.IsTargeted = false);

            AcceptButton.IsVisible = false;
            EndTurnButton.IsVisible = true;

            if (CardTargetsOnPlay(card) && !GameState.ResolvingAfterPlay)
            {
                GameState.PlayCard(card);

                if (!GameState.RequireAccept)
                {
                    SendMakeMoveMessage();
                }
            }
            else if (card.GetMainType() == CardType.Leader)
            {
                SendMakeMoveMessage();
            }
            else if (GameState.ResolvingAfterPlay && doneResolvingAfterplay)
            {
                GameState.ResolvingAfterPlay = false;
                GameState.ResolvedEffects = 0;
                card.IsBeingPlayed = false;
                GameState.TriggerEvent(GameEvent.DoneResolving);

                SendMakeMoveMessage();
            }
        }

        private void SendMakeMoveMessage()
        {
            var response = _server.SendMakeMove(_makeMoveMessage);

            // TODO error handling

            _makeMoveMessage = null;
        }
    }
}
