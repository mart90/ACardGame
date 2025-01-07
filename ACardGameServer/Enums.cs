namespace ACardGameServer
{
    public enum ServerEndpoint
    {
        Login,
        Register,
        CreateChallenge,
        CancelChallenge,
        JoinNearestOpen,
        JoinFromClipboard,

        MakeMove,
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
        PlayingActionQueued
    }
}
