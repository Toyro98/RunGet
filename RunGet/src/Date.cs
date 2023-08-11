using System;
using System.Collections.Generic;
using System.Linq;
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

            int commaIndex = date.LastIndexOf(",");

            if (commaIndex != -1)
            {
                date = date.Remove(commaIndex, 2).Insert(commaIndex, " and ");
            }

            return date;
        }

        public static PreviousRecordHolder GetDifferenceInDays(LeaderboardModel.Root leaderboard, RunsModelLight.Root personalBests, RunsModel.Data run = null)
        {
            if (leaderboard.Data.Runs.Length < 2)
            {
                return new();
            }

            var currentWorldRecordDate = leaderboard.Data.Runs[0].Run.Date ?? DateTime.MaxValue;
            var previousWorldRecordDate = leaderboard.Data.Runs[1].Run.Date ?? DateTime.MaxValue;

            if (personalBests.Data.Length < 2)
            {
                return new(previousWorldRecordDate.Date, currentWorldRecordDate.Date);
            }

            var personalBestsSorted = personalBests.Data.OrderBy(x => x.Times.Primary_t).ToList();

            for (int i = 0; i < personalBestsSorted.Count; i++)
            {
                if (personalBests.Data[i].Date == null)
                {
                    continue;
                }

                int index = i + 1;

                if (run == null)
                {
                    if (leaderboard.Data.Runs[0].Run.Times.Primary_t == personalBests.Data[i].Times.Primary_t)
                    {
                        if (index == personalBests.Data.Length)
                        {
                            return new(personalBests.Data[i].Date, currentWorldRecordDate.Date);
                        }

                        if (leaderboard.Data.Runs[1].Run.Times.Primary_t > personalBests.Data[index].Times.Primary_t)
                        {
                            return new(personalBests.Data[index].Date, currentWorldRecordDate.Date);
                        }

                        return new(previousWorldRecordDate.Date, currentWorldRecordDate.Date);
                    }
                }
                else
                {
                    if (run.Times.Primary_t == personalBestsSorted[i].Times.Primary_t)
                    {
                        if (index == personalBestsSorted.Count)
                        {
                            return new(personalBestsSorted[i].Date, DateTime.Parse(run.Date));
                        }

                        return new(personalBestsSorted[index].Date, DateTime.Parse(run.Date));
                    }
                }
            }

            return new();
        }
    }
}
