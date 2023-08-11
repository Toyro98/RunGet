using System;
using System.Collections.Generic;
using System.Linq;

namespace RunGet
{
    public class Time
    {
        public const decimal MinTime = 0m;
        public const decimal MaxTime = 35999999.999m;

        public const string Milliseconds = "ms";
        public const string Seconds = "s";
        public const string Minutes = "m";
        public const string Hours = "h";

        public static string FormatTime(decimal totalTime)
        {
            if (totalTime > MaxTime || totalTime <= MinTime)
            {
                return "";
            }

            Dictionary<string, int> timeDictionary = new Dictionary<string, int>
            {
                { Milliseconds, (int)((totalTime - (int)totalTime) * 1000) },
                { Seconds, (int)totalTime % 60 },
                { Minutes, (int)(totalTime / 60 % 60) },
                { Hours, (int)(totalTime / 60 / 60) }
            };

            string time = string.Empty;
            foreach (var item in timeDictionary)
            {
                if (item.Value == 0)
                {
                    continue;
                }

                if (item.Key == Milliseconds)
                {
                    time = item.Value.ToString("000") + item.Key;
                }
                else
                {
                    time = time.Insert(0, item.Value + item.Key + " ");
                }
            }

            return time.TrimEnd();
        }

        public static string GetTimeDifference(RunsModelLight.Root personalBests, RunsModel.Data run)
        {
            if (personalBests.Data.Length >= 2)
            {
                decimal timeDiff = 0m;

                if (run.Players.Data.Length == 1)
                {
                    var personalBestsSorted = personalBests.Data.OrderBy(x => x.Times.Primary_t).ToList();

                    for (int i = 0; i < personalBestsSorted.Count; i++)
                    {
                        if (run.Times.Primary_t == personalBestsSorted[i].Times.Primary_t)
                        {
                            if (i + 1 == personalBestsSorted.Count)
                            {
                                timeDiff = personalBestsSorted[i].Times.Primary_t - run.Times.Primary_t;
                            }
                            else
                            {
                                timeDiff = personalBestsSorted[i + 1].Times.Primary_t - run.Times.Primary_t;
                            }

                            break;
                        }
                    }
                }
                else
                {
                    timeDiff = GetTimeDifferenceFromMultipleRunners(personalBests, run);
                }

                if (timeDiff > 0m)
                {
                    return FormatTime(timeDiff);
                }
            }

            return string.Empty;
        }

        public static string GetTimeDifferenceWorldRecord(LeaderboardModel.Root leaderboard, RunsModelLight.Root personalBests, RunsModel.Data run)
        {
            if (leaderboard.Data.Runs.Length < 2)
            {
                return string.Empty;
            }

            decimal currentWorldRecord = leaderboard.Data.Runs[0].Run.Times.Primary_t;
            decimal previousWorldRecord = leaderboard.Data.Runs[1].Run.Times.Primary_t;
            decimal previousPersonalBest = decimal.MaxValue;

            if (leaderboard.Data.Runs[1].Run.Date == null)
            {
                return FormatTime(previousWorldRecord - currentWorldRecord);
            }

            for (int i = 0; i < personalBests.Data.Length; i++)
            {
                if (personalBests.Data[i].Date == null)
                {
                    continue;
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

            if (previousPersonalBest != decimal.MaxValue || previousWorldRecord > previousPersonalBest)
            {
                return FormatTime(previousPersonalBest - currentWorldRecord);
            }

            if (previousWorldRecord - currentWorldRecord != 0m)
            {
                return FormatTime(previousWorldRecord - currentWorldRecord);
            }

            return string.Empty;
        }

        private static decimal GetTimeDifferenceFromMultipleRunners(RunsModelLight.Root personalBests, RunsModel.Data run)
        {
            decimal currentPersonalBest = 0m;
            decimal oldPersonalBest = 0m;

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

                    if (currentPersonalBest == 0m)
                    {
                        currentPersonalBest = personalBests.Data[i].Times.Primary_t;
                    }
                    else
                    {
                        oldPersonalBest = personalBests.Data[i].Times.Primary_t;
                    }

                    if (currentPersonalBest != oldPersonalBest && currentPersonalBest != 0m && oldPersonalBest != 0m)
                    {
                        return oldPersonalBest - currentPersonalBest;
                    }
                }
            }

            return -1m;
        }
    }
}
