using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunGet
{
    public class Title
    {
        public static int runsFound = 0;
        public static int requestsSent = 0;

        public static void UpdateTitle(int runs = 0, int api = 0)
        {
            runsFound += runs;
            requestsSent += api;

            Console.Title = $"RunGet [{runsFound} Run{((runsFound == 1) ? "" : "s")} Found, {requestsSent} Api Requests]";
        }
    }
}
