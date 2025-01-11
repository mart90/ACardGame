using System.ComponentModel.DataAnnotations.Schema;

namespace ACardGameLibrary
{
    public class GameMove
    {
        public int Id { get; set; }

        public int GameId { get; set; }

        public int UserId { get; set; }

        public MoveType Type { get; set; }

        public int TurnNumber { get; set; }

        public int? CardId { get; set; }

        public string CardName { get; set; }

        public List<GameMoveAcceptParams> AcceptParams { get; set; }

        public GameMove()
        {
            AcceptParams = new List<GameMoveAcceptParams>();
        }

        public void ToDatabaseFriendly()
        {
            foreach (var param in AcceptParams)
            {
                if (param.TargetedCardIds.Any())
                {
                    param.TargetedCardIdsString = string.Join(';', param.TargetedCardIds);
                }
                else if (param.OptionsPicked.Any())
                {
                    param.OptionsPickedString = string.Join(';', param.OptionsPicked);
                }
            }
        }
    }

    public class GameMoveAcceptParams
    {
        public int GameMoveId { get; set; }
        public GameMove GameMove { get; set; }

        public int Order { get; set; }

        public string TargetedCardIdsString { get; set; }

        public string OptionsPickedString { get; set; }

        public int? ShopCostPicked { get; set; }

        [NotMapped]
        public List<int> TargetedCardIds { get; set; }

        [NotMapped]
        public List<string> OptionsPicked { get; set; }

        public GameMoveAcceptParams()
        {
            TargetedCardIds = new List<int>();
            OptionsPicked = new List<string>();
        }
    }
}
