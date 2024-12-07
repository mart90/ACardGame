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
        StartingRound,
        EndingRound,
        RefreshingShop,
        StartingCombat,
        ResolvingCombat,
        EndingCombat,
        PlayingAction,
        PlayingSupport,
        PlayingCreature,
        PlayingLeader,
        PlayingCurrency,
        DealingDamage,
        Investing,
        Buying
    }

    public enum CardEffectPhase
    {
        OnPlay,
        OnTurnEnd
    }
}
