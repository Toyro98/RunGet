using System;
using System.Collections.Generic;

namespace RunGet
{
    public class RunsModelLight
    {
        public class Root
        {
            public Data[] Data { get; set; }
        }

        public class Data
        {
            public string Id { get; set; }
            public string Game { get; set; }
            public string Level { get; set; }
            public string Category { get; set; }
            public DateTime? Date { get; set; }
            public RunsModel.Times Times { get; set; }
            public Players[] Players { get; set; }
            public Dictionary<string, string> Values { get; set; }
        }

        public class Players
        {
            public string Rel { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}