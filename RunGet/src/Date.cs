using System;
using System.Collections.Generic;
using NodaTime;

namespace RunGet
{
    public static class Date
    {
        private const string Year = "year";
        private const string Month = "month";
        private const string Day = "day";

        public static string FormatDate(DateTime date)
        {
            return string.Format("{0} {1:MMM yyyy}", date.Day, date);
        }

        public static string FormatDate(DateTime startDate, DateTime endDate)
        {
            if (startDate == endDate)
            {
                return "less than a day";
            }

            if (startDate > endDate)
            {
                (endDate, startDate) = (startDate, endDate);
            }

            LocalDate start = new LocalDate(startDate.Year, startDate.Month, startDate.Day);
            LocalDate end = new LocalDate(endDate.Year, endDate.Month, endDate.Day);
            Period period = Period.Between(start, end);

            Dictionary<string, int> keyValues = new Dictionary<string, int>
            {
                { Year, period.Years },
                { Month, period.Months },
                { Day, period.Days }
            };

            string date = "";

            foreach (var item in keyValues)
            {
                if (item.Value == 0)
                {
                    continue;
                }

                date += item.Value + " " + (item.Value < 2 ? item.Key : item.Key + "s") + ", ";
            }

            if (date.EndsWith(", "))
            {
                date = date[..^2];
            }

            int a = date.LastIndexOf(",");

            if (a != -1)
            {
                date = date.Remove(a, 2).Insert(a, " and ");
            }

            return date;
        }

        public static PreviousRecordHolder GetDifferenceInDays(LeaderboardModel.Root leaderboard, RunsModelLight.Root personalBests)
        {
            if (leaderboard.Data.Runs.Length < 2 && personalBests.Data.Length < 2)
            {
                return new();
            }

            var currentWorldRecordDate = leaderboard.Data.Runs[0].Run.Date ?? DateTime.MaxValue;
            var previousWorldRecordDate = leaderboard.Data.Runs[1].Run.Date ?? DateTime.MaxValue;

            var currentWorldRecordTime = leaderboard.Data.Runs[0].Run.Times.Primary_t;
            var previousWorldRecordTime = leaderboard.Data.Runs[1].Run.Times.Primary_t;
            
            var previousPersonalBestDate = DateTime.MaxValue;
            var previousPersonalBestTime = float.MaxValue;

            for (int i = 0; i < personalBests.Data.Length; i++)
            {
                if (personalBests.Data[i].Date == null)
                {
                    continue;
                }

                if (currentWorldRecordTime == personalBests.Data[i].Times.Primary_t)
                {
                    return new(previousWorldRecordDate.Date, currentWorldRecordDate.Date);
                }

                if (currentWorldRecordTime > personalBests.Data[i].Times.Primary_t)
                {
                    previousPersonalBestTime = personalBests.Data[i].Times.Primary_t;
                    previousPersonalBestDate = (DateTime)personalBests.Data[i].Date;
                    break;
                }
            }

            if (previousPersonalBestDate == DateTime.MaxValue)
            {
                return new(personalBests.Data[0].Date, personalBests.Data[1].Date);
            }

            if (previousWorldRecordDate > previousPersonalBestDate || previousWorldRecordTime > previousPersonalBestTime)
            {
                return new(currentWorldRecordDate.Date, previousPersonalBestDate.Date);
            }

            return new();
        }
    }
}
