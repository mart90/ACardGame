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
        Siege
    }

    public enum GameEvent
    {
        StartingRound,
        EndingRound,
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
        OnRemove
    }
}
