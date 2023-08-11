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
            public DateTime? Date { get; set; }
            public RunsModel.Times Times { get; set; }
            public Players[] Players { get; set; }
            public Dictionary<string, string> Values { get; set; }
        }

        public class Players
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}