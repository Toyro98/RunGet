using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunGet
{
    public class VariablesModel
    {
        public class Root
        {
            public Data[] Data { get; set; }
        }

        public class Data
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Category { get; set; }
            public VariablesValues Values { get; set; }
        }

        public class VariablesValues
        {
            public Dictionary<string, ValueData> Values { get; set; }
            public string Default { get; set; }
        }

        public class ValueData
        {
            public string Label { get; set; }
        }
    }
}
