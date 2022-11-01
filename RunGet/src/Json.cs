using Newtonsoft.Json;

namespace RunGet
{
    public static class Json
    {
        public static RunsModel.Data[] DeserializeRuns(string data)
        { 
            if (data == null) { return null; }

            RunsModel.Root api = JsonConvert.DeserializeObject<RunsModel.Root>(data);

            for (int i = 0; i < api.Data.Length; i++)
            {
                if (api.Data[i].Levels.ToString().Length > 25)
                {
                    api.Data[i].Level = JsonConvert.DeserializeObject<RunsModel.Level>(api.Data[i].Levels.ToString());
                }
            }

            return api.Data;
        }

        public static T Deserialize<T>(string data) => data == null ? default : JsonConvert.DeserializeObject<T>(data);
    }
}
