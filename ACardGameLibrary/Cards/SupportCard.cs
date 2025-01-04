namespace ACardGameLibrary
{
    public class SupportCard : Card
    {
        public SupportCard()
        {
            Types = new List<CardType> 
            { 
                CardType.Support 
            };
        }

        public bool IsPermanent { get; set; }
    }
}
