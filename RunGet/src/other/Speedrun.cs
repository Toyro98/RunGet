namespace RunGet
{
    public class Speedrun
    {
        public string Id { get; set; }
        public string LatestRunId { get; set; }
        public string WebhookUrl { get; set; }
        public string DefaultPlatform { get; set; }
        public RunsModel.Data[] Runs { get; set; }
    }
}
