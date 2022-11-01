using System;

namespace RunGet
{
    public class LeaderboardModel
    {
        public class Root
        {
            public Data Data { get; set; }
        }

        public class Data
        {
            public Runs[] Runs { get; set; }
            public RunsModel.Players Players { get; set; }
        }

        public class Runs
        {
            public int Place { get; set; }
            public Run Run { get; set; }
        }

        public class Run
        {
            public string Id { get; set; }
            public string Game { get; set; }
            public DateTime? Date { get; set; }
            public string Level { get; set; }
            public string Category { get; set; }
            public RunsModel.Times Times { get; set; }
            public RunsModelLight.Players[] Players { get; set; }
        }
    }
}
