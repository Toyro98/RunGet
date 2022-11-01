using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunGet
{
    public class UserModel
    {
        public class Root
        {
            public Data Data { get; set; }
        }

        public class Data
        {
            public RunsModel.Players Players { get; set; }
        }
    }
}
