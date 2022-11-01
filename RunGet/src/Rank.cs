using System;

namespace RunGet
{
    public class Rank
    {
        public static int GetLeaderboardRank(LeaderboardModel.Root leaderboard, RunsModel.Players run)
        {
            for (int i = 0; i < leaderboard.Data.Runs.Length; i++)
            {
                if (run.Data[0].Rel == "user")
                {
                    if (leaderboard.Data.Runs[i].Run.Players[0].Id == run.Data[0].Id)
                    {
                        return leaderboard.Data.Runs[i].Place;
                    }

                    continue;
                }

                if (leaderboard.Data.Runs[i].Run.Players[0].Name == run.Data[0].Name)
                {
                    return leaderboard.Data.Runs[i].Place;
                }
            }

            throw new Exception();
        }
    }
}
