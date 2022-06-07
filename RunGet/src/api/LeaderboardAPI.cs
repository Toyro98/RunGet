using System;

namespace RunGet
{
    public struct LeaderboardApi
    {
        public struct Root
        {
            public Data Data { get; set; }
        }

        public struct Data
        {
            public Runs[] Runs { get; set; }
            public RunsApi.Players Players { get; set; }
        }

        public struct Runs
        {
            public int Place { get; set; }
            public Run Run { get; set; }
        }

        public struct Run
        {
            public string Id { get; set; }
            public string Game { get; set; }
            public DateTime? Date { get; set; }
            public string Level { get; set; }
            public string Category { get; set; }
            public RunsApi.Times Times { get; set; }
            public RunsApiLight.Players[] Players { get; set; }
        }
    }
}
