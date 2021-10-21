using Newtonsoft.Json;

namespace RunGet
{
    public static class Json
    {
        public static RunsAPI.Root DeserializeRuns(string data)
        {
            // Check if data is null before continuing 
            if (data == null)
            {
                return null;
            }

            // Deserialize data
            RunsAPI.Root api = JsonConvert.DeserializeObject<RunsAPI.Root>(data);

            int num = 0;

            // Go through all data
            foreach (var item in api.Data)
            {
                // If levels data length is longer than 25. Then we know it's not null and then we deserialize the level data
                if (item.Levels.ToString().Length > 25)
                {
                    api.Data[num].Level = JsonConvert.DeserializeObject<RunsAPI.Level>(api.Data[num].Levels.ToString());
                }

                num++;
            }

            return api;
        }

        public static LeaderboardAPI.Root DeserializeLeaderboard(string data) => data == null ? null : JsonConvert.DeserializeObject<LeaderboardAPI.Root>(data);
    }
}
