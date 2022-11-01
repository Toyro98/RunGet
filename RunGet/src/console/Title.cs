using System;

namespace RunGet
{
    public static class Title
    {
        public static string version;
        public static int runsFound;
        public static int apiRequests;

        public static void UpdateRunsFoundCounterBy(int amount)
        {
            runsFound += amount;
            UpdateTitle();
        }
        
        public static void UpdateApiCounterBy(int amount)
        {
            apiRequests += amount;
            UpdateTitle();
        }

        private static void UpdateTitle()
        {
            Console.Title = $"RunGet {version} [{runsFound} Run{((runsFound == 1) ? "" : "s")} Found, {apiRequests} Api Requests]";
        }
    }
}
