using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunGet
{
    public class Rank
    {
        public static string GetLeaderboardRank(int num, RunsAPI.Root runs)
        {
            LeaderboardAPI.Root leaderboard;

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

            // If for some reason, we don't find the run at all. Return "n/a"
            return "n/a";
        }
    }
}
