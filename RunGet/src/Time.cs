using System;
using System.Collections.Generic;
using System.Linq;

namespace RunGet
{
    public class Time
    {
        public static string FormatTime(float time)
        {
            var roundedTime = new TimeSpan((long)Math.Round(1.0f * TimeSpan.FromSeconds(time).Ticks / 10000) * 10000);

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

        public static string GetTimeDifference(RunsModelLight.Root personalBests, RunsModel.Data run)
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

            return string.Empty;
        }

        public static string GetTimeDifferenceWorldRecord(LeaderboardModel.Root leaderboard, RunsModelLight.Root personalBests, RunsModel.Data run)
        {
            if (leaderboard.Data.Runs.Length < 2 && personalBests.Data.Length < 2)
            {
                return string.Empty;
            }

            float currentWorldRecord = leaderboard.Data.Runs[0].Run.Times.Primary_t;
            float previousWorldRecord = leaderboard.Data.Runs[1].Run.Times.Primary_t;
            float previousPersonalBest = float.MaxValue;

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

            if (previousPersonalBest != float.MaxValue || previousWorldRecord > previousPersonalBest)
            {
                return FormatTime(previousPersonalBest - currentWorldRecord);
            }

            if (previousWorldRecord - currentWorldRecord != 0f)
            {
                return FormatTime(previousWorldRecord - currentWorldRecord);
            }

            return string.Empty;
        }

        private static float GetTimeDifferenceFromMultipleRunners(RunsModelLight.Root personalBests, RunsModel.Data run)
        {
            float currentPersonalBest = 0f;
            float oldPersonalBest = 0f;

            for (int i = 0; i < personalBests.Data.Length; i++)
            {
                for (int j = 0; j < personalBests.Data[i].Players.Length; j++)
                {
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

                    if (currentPersonalBest == 0f)
                    {
                        currentPersonalBest = personalBests.Data[i].Times.Primary_t;
                    }
                    else
                    {
                        oldPersonalBest = personalBests.Data[i].Times.Primary_t;
                    }

                    if (currentPersonalBest != oldPersonalBest && currentPersonalBest != 0f && oldPersonalBest != 0f)
                    {
                        return oldPersonalBest - currentPersonalBest;
                    }
                }
            }

            return -1f;
        }
    }
}
