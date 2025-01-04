namespace ACardGameLibrary
{
    public enum CardType
    {
        Currency,
        Action,
        Creature,
        Support,
        Leader,

        Infantry,
        Ranged,
        Cavalry,
        Flying,
        Siege,
        Legendary
    }

    public enum GameEvent
    {
        StartingTurn,
        EndingTurn,

        RefreshingShop,
        RefreshedShop,

        StartingCombat,
        ResolvingCombat,
        DealingDamage,
        EndingCombat,

        PlayingAction,
        PlayingSupport,
        PlayingCreature,
        PlayingLeader,
        PlayingCurrency,

        Buying,
        DrawingCardsFromCardEffect,
        DoneResolving
    }

    public enum CardEffectPhase
    {
        OnBuy,
        OnPlay,
        OnMoveToDiscard,
        OnAccepted,
        OnAcceptedAfterPlay,
        OnRemove
    }

    public enum MessageSeverity
    {
        Information,
        Warning,
        Error
    }
}
