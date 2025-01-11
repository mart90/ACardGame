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

    public enum ServerEndpoint
    {
        Login,
        Register,
        CreateChallenge,
        CancelChallenge,
        JoinNearestOpen,
        JoinFromClipboard,

        MakeMove,
        SetResult
    }

    public enum ServerMessageType
    {
        GameStart,
        MakeMove
    }

    public enum StatusCode
    {
        Ok = 200,
        BadRequest = 400,
        Error = 500,
        Unauthorized = 401
    }

    public enum MoveType
    {
        PlayingCard,
        BuyingFromShop,
        UpgradingShop,
        RefreshingShop,
        BuyingSilver,
        BuyingGold,
        Worshiping,
        Passing,
        EndingTurn,
        EventUsedInput,
        PlayingActionQueued,
        FreeTradeBuying,
        Resigning
    }
}
