using System;

namespace RunGet
{
    public class Rank
    {
        public static int GetLeaderboardRank(LeaderboardModel.Root leaderboard, RunsModel.Data run)
        {
            for (int i = 0; i < leaderboard.Data.Runs.Length; i++)
            {
                if (leaderboard.Data.Runs[i].Run.Times.Primary_t >= run.Times.Primary_t)
                {
                    for (int j = i; j < leaderboard.Data.Runs.Length; j++)
                    {
                        if (leaderboard.Data.Runs[j].Place == 0)
                        {
                            if (leaderboard.Data.Runs.Length == j + 1)
                            {
                                return leaderboard.Data.Runs[j].Place;
                            }

                            return leaderboard.Data.Runs[j + 1].Place;
                        }
                        else
                        { 
                            return leaderboard.Data.Runs[j].Place;
                        }
                    }

                    return leaderboard.Data.Runs[i].Place;
                }
            }

            return -1;
        }
    }
}
