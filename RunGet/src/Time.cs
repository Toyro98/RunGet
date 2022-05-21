using System;
using System.Collections.Generic;

namespace RunGet
{
    public static class Time
    {
        public static string FormatTime(float time)
        {
            string finalTime = string.Empty;
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);

            // Rounds the milliseconds instead of getting the first 3 characters
            // 297.3099975 to 297.310
            TimeSpan roundedTimeSpan = new TimeSpan((long)Math.Round(1.0f * timeSpan.Ticks / 10000) * 10000);

            // Create a dictionary with the data from the rounded timespan
            Dictionary<string, int> timeDict = new Dictionary<string, int>
            {
                { "ms", roundedTimeSpan.Milliseconds },
                { "s", roundedTimeSpan.Seconds },
                { "m", roundedTimeSpan.Minutes },
                { "h", roundedTimeSpan.Hours },
                { "d", roundedTimeSpan.Days }
            };

            foreach (var item in timeDict)
            {
                // Check if the value is higher than 0 since we don't want it to say 0d 1h 0m 32s 910ms and instead only say 1h 32s 910ms
                if (item.Value > 0)
                {
                    if (item.Key == "ms")
                    {
                        finalTime = string.Format("{0:000}", item.Value) + item.Key;
                    }
                    else
                    {
                        // Insert the time and suffix to the string
                        finalTime = finalTime.Insert(0, item.Value + item.Key + ((finalTime.Length > 0) ? " " : ""));
                    }
                }
            }

            return finalTime;
        }

        public static string GetTimeDifference(RunsApi.Data run)
        {
            float timeDiff;
            string urlPath;
            string variablesPath = string.Empty;
            string categoryAndLevelPath = "&category=" + run.Category.Data.Id;

            if (run.Values.Count > 0)
            {
                variablesPath = Utils.GetVariablesPath(run);
            }

            if (run.Level != null)
            {
                categoryAndLevelPath += "&level=" + run.Level.Data.Id;
            }

            if (run.Players.Data[0].Rel == "user")
            {
                urlPath = "runs?user=" + run.Players.Data[0].Id; 
            }
            else
            {
                urlPath = "runs?user=" + run.Players.Data[0].Name;
            }

            urlPath += "&game=" + run.Game.Data.Id + categoryAndLevelPath + (string.IsNullOrEmpty(variablesPath) == true ? "" : "&" + variablesPath) +
                "&status=verified&orderby=date&direction=desc";

            // Get the user's personalbest from the Api
            // No need to request more data since we're looking for the latest runs with the same category, level and variables
            RunsApiLight.Root personalBests = Json.Deserialize<RunsApiLight.Root>(Https.Get(urlPath).Result);

            if (personalBests.Data.Length >= 2)
            {
                if (run.Players.Data.Length == 1)
                {
                    timeDiff = personalBests.Data[1].Times.Primary_t - personalBests.Data[0].Times.Primary_t;
                }
                else
                {
                    timeDiff = GetTimeDifferenceFromMultipleRunners(personalBests, run);
                }

                if (timeDiff > 0f)
                {
                    return FormatTime(timeDiff);
                }
            }

            // No previous runs or time difference is 0 or less than 0
            return null;
        }

        public static string GetTimeDifferenceWorldRecord(RunsApi.Data run)
        {
            LeaderboardApi.Root leaderboard = Rank.GetLeaderboardDataFromAPI(run);

            if (leaderboard.Data.Runs.Length >= 2)
            {
                float currentWorldRecord = leaderboard.Data.Runs[0].Run.Times.Primary_t;
                float previousWorldRecord = leaderboard.Data.Runs[1].Run.Times.Primary_t;

                // If new world record is tied with previous world record then don't show "Improved World Record by"
                if (previousWorldRecord - currentWorldRecord != 0f)
                {
                    return FormatTime(previousWorldRecord - currentWorldRecord);
                }

                return null;
            }

            return null;
        }

        private static float GetTimeDifferenceFromMultipleRunners(RunsApiLight.Root personalBests, RunsApi.Data run)
        {
            float currentPersonalBest = 0f;
            float oldPersonalBest = 0f;

            // Go through pbs and check if the runs are done by the same runners. If we have, we then can get a time difference
            for (int j = 0; j < personalBests.Data.Length; j++)
            {
                for (int k = 0; k < personalBests.Data[j].Players.Length; k++)
                {
                    // Only check if the length match. Some categories have same category and variables but different amount of runners
                    // So if the run was done by 3 runners, only check the pbs with 3 runners in them and ignore the rest
                    if (run.Players.Data.Length != personalBests.Data[j].Players.Length)
                    {
                        break;
                    }

                    if (run.Players.Data[k].Rel == "user")
                    {
                        if (run.Players.Data[k].Id != personalBests.Data[j].Players[k].Id)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (run.Players.Data[k].Name != personalBests.Data[j].Players[k].Name)
                        {
                            break;
                        }
                    }

                    // The current run id has the same length and are done by the same runners
                    // If we reach here the first time, we set it as personal best and second time as old personal best
                    if (currentPersonalBest == 0f)
                    {
                        currentPersonalBest = personalBests.Data[j].Times.Primary_t;
                    }
                    else
                    {
                        oldPersonalBest = personalBests.Data[j].Times.Primary_t;
                    }

                    // Makes sure the current and old is not the same and does not equal to 0f
                    if (currentPersonalBest != oldPersonalBest && currentPersonalBest != 0f && oldPersonalBest != 0f)
                    {
                        return oldPersonalBest - currentPersonalBest;
                    }
                }
            }

            // No previous runs or too few runs to get time difference
            return 0f;
        }
    }
}
