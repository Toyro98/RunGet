using System;

namespace RunGet
{
    public static class Title
    {
        public static int runsFound = 0;
        public static int requestsSent = 0;

        public static void Update(int runs = 0, int api = 0)
        {
            runsFound += runs;
            requestsSent += api;

            Console.Title = $"RunGet { Utils.GetVersion() } [{runsFound} Run{((runsFound == 1) ? "" : "s")} Found, {requestsSent} Api Requests]";
        }
    }
}
