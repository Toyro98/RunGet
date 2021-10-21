using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunGet
{
    public class User
    {
        public static string GetUserName(int num, RunsAPI.Root Runs)
        {
            // Create an empty string to store the runners name
            string runners;

            // If the run is one player only
            if (Runs.Data[num].Players.Data.Length == 1)
            {
                // If the run is a guest account. We can get the name without sending a request
                if (Runs.Data[num].Players.Data[0].Rel == "guest")
                {
                    // Returns the guest's name
                    runners = Runs.Data[num].Players.Data[0].Name;
                }
                else
                {
                    runners = Runs.Data[num].Players.Data[0].Names.International;
                }
            }
            else
            {
                // Creates an string array
                string[] players = new string[2];

                // Loop 2 times to get their names
                for (int i = 0; i < 2; i++)
                {
                    if (Runs.Data[num].Players.Data[i].Rel == "guest")
                    {
                        players[i] = Runs.Data[num].Players.Data[i].Name;
                    }
                    else
                    {
                        players[i] = Runs.Data[num].Players.Data[i].Names.International;
                    }
                }

                // Check if the run is co-op
                if (Runs.Data[num].Players.Data.Length == 2)
                {
                    runners = players[0] + " and " + players[1];
                }
                else
                {
                    // Instead of displaying everyones names in the embed title. It will show the first two players and say "x, y, and z more runners"
                    int amountOfRunners = Runs.Data[num].Players.Data.Length - 2;

                    runners = players[0] + ", " + players[1] + ", and " + amountOfRunners + " other runner" + ((amountOfRunners == 1) ? "" : "s");
                }
            }

            return runners;
        }
    }
}
