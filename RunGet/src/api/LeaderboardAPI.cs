using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RunGet
{
    public class LeaderboardAPI
    {
        public class Root
        {
            public Datum Data { get; set; }
        }

        public class Datum
        {
            public Runs[] Runs { get; set; }
        }

        public class Runs
        {
            public int Place { get; set; }
            [JsonProperty("run")]
            public Run Run { get; set; }
        }

        public class Run
        {
            public string Id { get; set; }
            public string Weblink { get; set; }
            public string Game { get; set; }
            public string Level { get; set; }
            public string Category { get; set; }
            public Players[] Players { get; set; }
        }

        public class Players
        {
            public string Rel { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
            public string Uri { get; set; }
        }
    }
}
