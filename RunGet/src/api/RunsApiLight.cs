using System;

namespace RunGet
{
    public struct RunsApiLight
    {
        public struct Root
        {
            public Data[] Data { get; set; }
        }

        public struct Data
        {
            public string Id { get; set; }
            public string Game { get; set; }
            public string Level { get; set; }
            public string Category { get; set; }
            public DateTime? Date { get; set; }
            public RunsApi.Times Times { get; set; }
            public Players[] Players { get; set; }
        }

        public struct Players
        {
            public string Rel { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}
