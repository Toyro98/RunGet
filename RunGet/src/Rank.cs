using System;
using System.Threading;
using Pastel;

namespace RunGet
{
    public class Rank
    {
        public static string GetLeaderboardRank(int num, RunsAPI.Root runs)
        {
            LeaderboardAPI.Root leaderboard;
            int attempts = 0;
            int rank = 0;

            while (rank == 0)
            {
                if (runs.Data[num].Level != null)
                {
                    // GET leaderboards/{game}/level/{level}/{category}
                    leaderboard = Json.DeserializeLeaderboard(Https.Get("leaderboards/" + runs.Data[num].Game.Data.Id + "/level/" + runs.Data[num].Level.Data.Id + "/" + runs.Data[num].Category.Data.Id).Result);
                }
                else
                {
                    // This string is here so I don't have to paste this long string all over the place
                    string path = "leaderboards/" + runs.Data[num].Game.Data.Id + "/category/" + runs.Data[num].Category.Data.Id;

                    if (runs.Data[num].Values.Count == 0)
                    {
                        // GET leaderboards/{game}/category/{category}
                        leaderboard = Json.DeserializeLeaderboard(Https.Get(path).Result);
                    }
                    else
                    {
                        // TODO: Make this work with multiple variables

                        // Stores the key and get the value from it
                        string key = runs.Data[num].Category.Data.Variables.Data[0].Id;
                        runs.Data[num].Values.TryGetValue(key, out string value);

                        // GET leaderboards/{game}/category/{category}?var-{subcategoryid}={variables}
                        leaderboard = Json.DeserializeLeaderboard(Https.Get(path + "?var-" + key + "=" + value).Result);
                    }
                }

                // Loops through the leaderboard 
                for (int i = 0; i < leaderboard.Data.Runs.Length; i++)
                {
                    if (runs.Data[num].Players.Data[0].Rel == "user")
                    {
                        if (leaderboard.Data.Runs[i].Run.Players[0].Id == runs.Data[num].Players.Data[0].Id)
                        {
                            if (attempts > 0)
                            {
                                Console.WriteLine("[{0}] Leaderboard rank for run id \"{3}\" found after {1} {2}", DateTime.Now.ToString().Pastel("#808080"), attempts.ToString().Pastel("#808080"), (attempts > 1) ? "s" : "", runs.Data[num].Id);
                            }

                            rank = leaderboard.Data.Runs[i].Place;
                            break;
                        }
                    }
                    else
                    {
                        if (leaderboard.Data.Runs[i].Run.Players[0].Name == runs.Data[num].Players.Data[0].Name)
                        {
                            if (attempts > 0)
                            {
                                Console.WriteLine("[{0}] Leaderboard rank for run id \"{3}\" found after {1} {2}", DateTime.Now.ToString().Pastel("#808080"), attempts.ToString().Pastel("#808080"), (attempts > 1) ? "s" : "", runs.Data[num].Id);
                            }

                            rank = leaderboard.Data.Runs[i].Place;
                            break;
                        }
                    }
                }

                if (rank == 0)
                {
                    attempts++;
                    Thread.Sleep(TimeSpan.FromMinutes(1));
                }
            }

            return rank.ToString();
        }
    }
}
