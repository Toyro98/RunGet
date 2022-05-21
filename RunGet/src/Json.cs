using Newtonsoft.Json;

namespace RunGet
{
    public static class Json
    {
        public static RunsApi.Data[] DeserializeRuns(string data)
        {
            // Check if data is null before continuing 
            if (data == null)
            {
                return null;
            }

            // Deserialize data
            RunsApi.Root api = JsonConvert.DeserializeObject<RunsApi.Root>(data);

            int num = 0;

            // Go through all data
            foreach (var item in api.Data)
            {
                // If levels data length is longer than 25. Then we know it's not null and then we deserialize the level data
                if (item.Levels.ToString().Length > 25)
                {
                    api.Data[num].Level = JsonConvert.DeserializeObject<RunsApi.Level>(api.Data[num].Levels.ToString());
                }

                num++;
            }

            return api.Data;
        }

        public static T Deserialize<T>(string data) => data == null ? default : JsonConvert.DeserializeObject<T>(data);
    }
}
