﻿namespace ACardGameLibrary
{
    public class CardEffect
    {
        public CardEffectPhase EffectPhase { get; set; }
        public Action<GameStateManager, Player> Effect { get; set; }
    }
}
