using System;

namespace RunGet
{
    public static class Date
    {
        public static string FormatDate(DateTime date)
        {
            return string.Format("{0} {1:MMM yyyy}", date.Day, date);
        }

        public static int GetDifferenceInDays(LeaderboardModel.Root leaderboard, RunsModelLight.Root personalBests)
        {
            if (leaderboard.Data.Runs.Length < 2 && personalBests.Data.Length < 2)
            {
                return int.MinValue;
            }

            var currentWorldRecordDate = leaderboard.Data.Runs[0].Run.Date ?? DateTime.MaxValue;
            var previousWorldRecordDate = leaderboard.Data.Runs[1].Run.Date ?? DateTime.MaxValue;
            var previousPersonalBestDate = DateTime.MaxValue;

            float currentWorldRecordTime = leaderboard.Data.Runs[0].Run.Times.Primary_t;
            float previousWorldRecordTime = leaderboard.Data.Runs[1].Run.Times.Primary_t;
            float previousPersonalBestTime = float.MaxValue;

            for (int i = 0; i < personalBests.Data.Length; i++)
            {
                if (personalBests.Data[i].Date == null)
                {
                    continue;
                }

                if (previousWorldRecordTime > personalBests.Data[i].Times.Primary_t)
                {
                    if (currentWorldRecordTime != personalBests.Data[i].Times.Primary_t)
                    {
                        previousPersonalBestTime = personalBests.Data[i].Times.Primary_t;
                        previousPersonalBestDate = (DateTime)personalBests.Data[i].Date;
                        break;
                    }
                }
            }

            if (previousPersonalBestDate == DateTime.MaxValue)
            {
                return (currentWorldRecordDate.Date - previousWorldRecordDate.Date).Days;
            }

            if (previousWorldRecordDate > previousPersonalBestDate || previousWorldRecordTime > previousPersonalBestTime)
            {
                return (currentWorldRecordDate.Date - previousPersonalBestDate.Date).Days;
            }

            return int.MinValue;
        }
    }
}
