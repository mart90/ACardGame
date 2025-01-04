namespace ACardGameLibrary
{
    public class GameLog
    {
        public GameLog(string message)
        {
            TimeStamp = DateTime.Now;
            Message = message;
        }

        public DateTime TimeStamp { get; set; }

        public string Message { get; set; }
    }
}
