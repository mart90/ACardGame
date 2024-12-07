namespace ACardGameLibrary
{
    public static class Extensions
    {
        public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> list)
        {
            var rng = new Random();
            return list.OrderBy(e => rng.Next());
        }
    }
}
