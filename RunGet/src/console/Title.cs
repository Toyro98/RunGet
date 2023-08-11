using System;

namespace RunGet
{
    public static class Title
    {
        public static string Version { get; set; }
        public static int RunsFound { get; set; }
        public static int ApiRequests { get; set; }

        public static void UpdateRunsFoundCounterBy(int amount)
        {
            RunsFound += amount;
            UpdateTitle();
        }
        
        public static void UpdateApiCounterBy(int amount)
        {
            ApiRequests += amount;
            UpdateTitle();
        }

        private static void UpdateTitle()
        {
            Console.Title = $"RunGet {Version} [{RunsFound} Run{((RunsFound == 1) ? "" : "s")} Found, {ApiRequests} Api Requests]";
        }
    }
}
