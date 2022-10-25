using System;
using System.Collections.Generic;
using System.Linq;

namespace RunGet
{
    public class Time
    {
        public static string FormatTime(float time)
        {
            // Rounds the milliseconds instead of getting the first 3 characters
            // 297.3099975 to 297.310
            var roundedTime = new TimeSpan((long)Math.Round(1.0f * TimeSpan.FromSeconds(time).Ticks / 10000) * 10000);

            // Create a dictionary with the data from the rounded timespan
            var timeDict = new Dictionary<string, int>
            {
                { "ms", roundedTime.Milliseconds },
                { "s", roundedTime.Seconds },
                { "m", roundedTime.Minutes },
                { "h", roundedTime.Hours },
                { "d", roundedTime.Days }
            };

            var finalTime = string.Empty;
            foreach (var item in timeDict)
            {
                if (item.Value == 0)
                {
                    continue;
                }

                if (item.Key == "ms")
                {
                    finalTime = string.Format("{0:000}", item.Value);
                    finalTime = finalTime.Remove(finalTime.Length - 1) + item.Key;
                }
                else
                {
                    finalTime = finalTime.Insert(0, item.Value + item.Key + (finalTime.Length > 0 ? " " : ""));
                }
            }

            return finalTime;
        }

        public static string GetTimeDifference(RunsApiLight.Root personalBests, RunsApi.Data run)
        {
            if (personalBests.Data.Length >= 2)
            {
                float timeDiff;

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
            return string.Empty;
        }

        public static string GetTimeDifferenceWorldRecord(LeaderboardApi.Root leaderboard, RunsApiLight.Root personalBests, RunsApi.Data run)
        {
            if (leaderboard.Data.Runs.Length >= 2)
            {
                float currentWorldRecord = leaderboard.Data.Runs[0].Run.Times.Primary_t;
                float previousWorldRecord = leaderboard.Data.Runs[1].Run.Times.Primary_t;
                float previousPersonalBest = float.MaxValue;

                if (personalBests.Data.Length >= 2)
                {
                    for (int i = 0; i < personalBests.Data.Length; i++)
                    {
                        if (personalBests.Data[i].Date == null)
                        {
                            continue;
                        }

                        if (leaderboard.Data.Runs[1].Run.Date == null)
                        {
                            return string.Empty;
                        }

                        if (!personalBests.Data[i].Values.SequenceEqual(run.Values))
                        {
                            continue;
                        }

                        if (previousWorldRecord > personalBests.Data[i].Times.Primary_t)
                        {
                            if (currentWorldRecord != personalBests.Data[i].Times.Primary_t) 
                            {
                                previousPersonalBest = personalBests.Data[i].Times.Primary_t;
                                break;
                            }
                        }
                    }
                }

                // After checking personal bests, checks if the time is bigger than 2nd place holder
                // If runner had less than 2 runs, then "previousWorldRecord" will be 1000hrs and we get the 2nd place time 
                if (previousPersonalBest != float.MaxValue || previousWorldRecord > previousPersonalBest)
                {
                    return FormatTime(previousPersonalBest - currentWorldRecord);
                    //previousWorldRecord = leaderboard.Data.Runs[1].Run.Times.Primary_t;
                }

                // If new world record is tied with previous world record then don't show "Improved World Record by"
                if (previousWorldRecord - currentWorldRecord != 0f)
                {
                    return FormatTime(previousWorldRecord - currentWorldRecord);
                }
            }

            return string.Empty;
        }

        private static float GetTimeDifferenceFromMultipleRunners(RunsApiLight.Root personalBests, RunsApi.Data run)
        {
            float currentPersonalBest = 0f;
            float oldPersonalBest = 0f;

            // Go through pbs and check if the runs are done by the same runners. If we have, we then can get a time difference
            for (int i = 0; i < personalBests.Data.Length; i++)
            {
                for (int j = 0; j < personalBests.Data[i].Players.Length; j++)
                {
                    // Only check if the length match. Some categories have same category and variables but different amount of runners
                    // So if the run was done by 3 runners, only check the pbs with 3 runners in them and ignore the rest
                    if (run.Players.Data.Length != personalBests.Data[i].Players.Length)
                    {
                        break;
                    }

                    if (run.Players.Data[j].Rel == "user")
                    {
                        if (run.Players.Data[j].Id != personalBests.Data[i].Players[j].Id)
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (run.Players.Data[j].Name != personalBests.Data[i].Players[j].Name)
                        {
                            break;
                        }
                    }


                    if (!personalBests.Data[j].Values.SequenceEqual(run.Values))
                    {
                        continue;
                    }

                    // The current run id has the same length and are done by the same runners
                    // If we reach here the first time, we set it as personal best and second time as old personal best
                    if (currentPersonalBest == 0f)
                    {
                        currentPersonalBest = personalBests.Data[i].Times.Primary_t;
                    }
                    else
                    {
                        oldPersonalBest = personalBests.Data[i].Times.Primary_t;
                    }

                    // Makes sure the current and old is not the same and does not equal to 0f
                    if (currentPersonalBest != oldPersonalBest && currentPersonalBest != 0f && oldPersonalBest != 0f)
                    {
                        return oldPersonalBest - currentPersonalBest;
                    }
                }
            }

            // No previous runs or too few runs to get time difference
            return -1f;
        }
    }
}
