using System;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;

namespace ConsoleApi
{
    class Program
    {
        private static string LatestRunID;
        private static readonly string URL = "https://www.speedrun.com/api/v1/runs?status=verified&orderby=verify-date&direction=desc&game=yo1yyr1q";

        static void Main(string[] args)
        {
            var client = new HttpClient();
            var content = client.GetStringAsync(URL).Result;

            // Deserialize Json
            Api api = JsonConvert.DeserializeObject<Api>(content);

            // Save the run ID
            LatestRunID = api.data[0].Id.ToString();

            // Write the latest it found
            Console.WriteLine("[" + DateTime.Now + "] Latest ID found: " + LatestRunID);

            // infinite loop 
            while (true)
            {
                Thread.Sleep(TimeSpan.FromMinutes(5));

                LookForNewRuns();
            }
        }

        static void LookForNewRuns()
        {
            var client = new HttpClient();
            var content = client.GetStringAsync(URL).Result;

            Api api = JsonConvert.DeserializeObject<Api>(content);

            int num = 0;

            // Loop through all 20 runs we get from the url
            for (int i = 0; i < 20; i++)
            {
                // Checks if the latest run is new and how many new runs there are
                if (LatestRunID == api.data[i].Id.ToString() && num != 0)
                {
                    Console.WriteLine("[" + DateTime.Now + $"] {num} new runs found");
                    break;
                }
                else
                {
                    num++;
                }
            }

            // If there are no new runs. Write it to the console
            if (num == 20)
            {
                Console.WriteLine("[" + DateTime.Now + $"] No new runs found");
            }
            else
            {
                while (num != 0)
                {
                    // Save the run id 
                    if (num == 1)
                    {
                        LatestRunID = api.data[0].Id.ToString();
                    }

                    num--;
                    Console.WriteLine("[" + DateTime.Now + $"] ID: {api.data[num].Id}");

                    if (num != 0)
                    {
                        // Sleep for few seconds. Just incase we don't hit the Discord's rate limit
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }
            }
        }
    }
}
