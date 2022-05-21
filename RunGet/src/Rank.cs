using System;

namespace RunGet
{
    public class Rank
    {
        public static int GetLeaderboardRank(RunsApi.Data runs)
        {
            LeaderboardApi.Root leaderboard = GetLeaderboardDataFromAPI(runs);

            // Loop through the leaderboard 
            for (int i = 0; i < leaderboard.Data.Runs.Length; i++)
            {
                // If the runner isn't a guest account, then check the id and return the rank
                if (runs.Players.Data[0].Rel == "user")
                {
                    if (leaderboard.Data.Runs[i].Run.Players[0].Id == runs.Players.Data[0].Id)
                    {
                        return leaderboard.Data.Runs[i].Place;
                    }
                }
                else
                {
                    if (leaderboard.Data.Runs[i].Run.Players[0].Name == runs.Players.Data[0].Name)
                    {
                        return leaderboard.Data.Runs[i].Place;
                    }
                }
            }

            // Hopefully it doesn't reach this. It has happened few times before, not sure how though
            throw new Exception("");
        }

        public static int GetCountryRank(RunsApi.Data runs)
        {
            // This was planned to be added but instead of removing it completly
            // I'll keep this here in-case if I want to use it in the future

            // However, it doesn't work with guest accounts since those doesn't have a country flag
            // And some users doesn't want to display a flag next to the username. So this isn't 100% accurate

            // Gets data from the leaderboard with the extra player info 
            LeaderboardApi.Root leaderboard = GetLeaderboardDataFromAPI(runs, true);
            int countryRank = 0;

            // Loop through the leaderboard 
            for (int i = 0; i < leaderboard.Data.Runs.Length; i++)
            {
                if (leaderboard.Data.Players.Data[i].Rel == "user")
                {
                    // Skip if the player doesn't have a flag
                    if (leaderboard.Data.Players.Data[i].Location != null)
                    {
                        // Match the country code and add 1 to the countryRank
                        if (leaderboard.Data.Players.Data[i].Location.Country.Code == runs.Players.Data[0].Location.Country.Code)
                        {
                            countryRank += 1;

                            // If we end up on players run, return the country rank
                            if (leaderboard.Data.Players.Data[i].Id == runs.Players.Data[0].Id)
                            {
                                return countryRank;
                            }
                        }
                    }
                }
            }

            // If player have no flag, return -1
            return -1;
        }

        public static LeaderboardApi.Root GetLeaderboardDataFromAPI(RunsApi.Data runs, bool embedPlayers = false)
        {
            string path = "leaderboards/" + runs.Game.Data.Id;

            // Append to the path if the run is a individual level or a fullgame
            if (runs.Level != null)
            {
                path += "/level/" + runs.Level.Data.Id + "/" + runs.Category.Data.Id;
            }
            else
            {
                path += "/category/" + runs.Category.Data.Id;
            }

            // After the path has been created, check if the run has 0 variables
            // If it doesn't, call a method to get the variable path and then get the data with the variable path
            if (runs.Values.Count == 0)
            {
                return Json.Deserialize<LeaderboardApi.Root>(Https.Get(path + (embedPlayers == true ? "?embed=players" : "")).Result);
            }

            return Json.Deserialize<LeaderboardApi.Root>(Https.Get(path + (embedPlayers == true ? "?embed=players&" : "?") + Utils.GetVariablesPath(runs)).Result);
        }
    }
}
