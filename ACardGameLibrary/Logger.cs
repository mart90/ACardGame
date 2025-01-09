namespace ACardGameLibrary
{
    public static class Logger
    {
        public static void LogDebug(string message)
        {
            var now = DateTime.Now;
            File.AppendAllText($"{AppDomain.CurrentDomain.BaseDirectory}/log/debug.log", $"\n\n{now:yyyy-MM-dd HH:mm:ss.fff}: {message}");
        }

        public static void LogError(string message)
        {
            var now = DateTime.Now;
            File.AppendAllText($"{AppDomain.CurrentDomain.BaseDirectory}/log/error.log", $"\n\n{now:yyyy-MM-dd HH:mm:ss.fff}: {message}");
        }
    }
}
