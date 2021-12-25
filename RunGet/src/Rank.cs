using System;
using System.Threading;

namespace RunGet
{
    public class Rank
    {
        public static string GetLeaderboardRank(int num, RunsAPI.Root runs)
        {
            LeaderboardAPI.Root leaderboard;

            while (true)
            {
                if (runs.Data[num].Level != null)
                {
                    // GET leaderboards/{game}/level/{level}/{category}
                    leaderboard = Json.DeserializeLeaderboard(Https.Get("leaderboards/" + runs.Data[num].Game.Data.Id + "/level/" + runs.Data[num].Level.Data.Id + "/" + runs.Data[num].Category.Data.Id).Result);
                }
                else
                {
                    // GET leaderboards/{game}/category/{category}
                    leaderboard = Json.DeserializeLeaderboard(Https.Get("leaderboards/" + runs.Data[num].Game.Data.Id + "/category/" + runs.Data[num].Category.Data.Id).Result);
                }

                for (int i = 0; i < leaderboard.Data.Runs.Length; i++)
                {
                    if (runs.Data[num].Players.Data[0].Rel == "user")
                    {
                        if (leaderboard.Data.Runs[i].Run.Players[0].Id == runs.Data[num].Players.Data[0].Id)
                        {
                            return leaderboard.Data.Runs[i].Place.ToString();
                        }
                    }
                    else
                    {
                        if (leaderboard.Data.Runs[i].Run.Players[0].Name == runs.Data[num].Players.Data[0].Name)
                        {
                            return leaderboard.Data.Runs[i].Place.ToString();
                        }
                    }
                }

                // It sometimes can't find the rank after run has been verified.
                // Wait for 1 min and try again.
                Thread.Sleep(TimeSpan.FromMinutes(1));
            }
        }
    }
}
