using System;
using System.Collections.Generic;

namespace RunGet
{
    public class Time
    {
        public static string FormatTime(int num, RunsAPI.Root runs)
        {
            // Create a timespan. Easier to get the minutes, seconds, and etc
            TimeSpan timer = TimeSpan.FromSeconds(runs.Data[num].Times.Primary_t);

            // Create an empty dictionary 
            Dictionary<string, int> timeDict = new Dictionary<string, int>() { };

            // Add the data from the timespan
            timeDict.Add("ms", timer.Milliseconds);
            timeDict.Add("s", timer.Seconds);
            timeDict.Add("m", timer.Minutes);
            timeDict.Add("h", timer.Hours);
            timeDict.Add("d", timer.Days);

            // Empty string variable
            string time = string.Empty;

            // Loop through the dictionary 
            foreach (var item in timeDict)
            {
                // Check if the value is higher than 0. So we don't have to print 0h 0m 32s 0ms and instead print 32s
                if (item.Value > 0)
                {
                    if (item.Key == "ms")
                    {
                        // Formats the ms
                        time = time.Insert(0, string.Format("{0:000}", item.Value) + item.Key);
                    }
                    else
                    {
                        // Insert the time and suffix to the string
                        time = time.Insert(0, item.Value + item.Key + ((time.Length > 0) ? " " : ""));
                    }
                }
            }

            // Returns the time
            return time;
        }
    }
}
