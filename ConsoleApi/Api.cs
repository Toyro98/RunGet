using System;

namespace Api
{
    class RunInfo
    {
        public Data[] data { get; set; }

        public class Data
        {
            public string Id { get; set; }
            public string Weblink { get; set; }
            public string Game { get; set; }
            public string Level { get; set; }
            public string Category { get; set; }
            public object Comment { get; set; }
            public Status Status { get; set; }
            public Player[] Players { get; set; }
            public string Date { get; set; }
            public DateTime Submitted { get; set; }
            public Times Times { get; set; }
        }
        public class Status
        {
            public string status { get; set; }
            public string Examiner { get; set; }
            public DateTime Verifydate { get; set; }
        }

        public class Times
        {
            public string Primary { get; set; }
            public float Primary_t { get; set; }
            public string Realtime { get; set; }
            public float Realtime_t { get; set; }
            public object Realtime_noloads { get; set; }
            public int Realtime_noloads_t { get; set; }
            public object Ingame { get; set; }
            public int Ingame_t { get; set; }
        }

        public class Player
        {
            public string Rel { get; set; }
            public string Id { get; set; }
            public string Uri { get; set; }
        }
    }

    class LeaderboardInfo
    {
        public Data[] data { get; set; }

        public class Data
        {
            public int Place { get; set; }
            public Run run { get; set; }
        }

        public class Run
        {
            public string Id { get; set; }
            public string Game { get; set; }
            public string Level { get; set; }
            public string Category { get; set; }
        }
    }

    class CategoryInfo
    {
        public Data data { get; set; }

        public class Data
        {
            public Categories Categories { get; set; }
        }

        public class Categories
        {
            public Datum[] Data { get; set; }
        }

        public class Datum
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}
