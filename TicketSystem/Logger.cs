namespace TicketSystem
{
    public static class Logger
    {
        public static void Log(string message)
        {
            File.AppendAllText("Log.txt", $"{DateTime.Now}\t{message}\n");
        }
    }
}
