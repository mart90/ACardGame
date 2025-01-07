namespace ACardGameLibrary
{
    public static class Extensions
    {
        public static int ShuffleSeed { get; set; }
        
        public static void Shuffle<T>(this IList<T> list)
        {
            Random rng;

            if (ShuffleSeed == 0)
            {
                rng = new Random();
            }
            else
            {
                rng = new Random(ShuffleSeed);
                ShuffleSeed++;
            }

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                (list[n], list[k]) = (list[k], list[n]);
            }
        }
    }
}
