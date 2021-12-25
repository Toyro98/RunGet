using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Pastel;

namespace RunGet
{
    public class Speedrun 
    {
        public string ID { get; set; }
        public string LatestRunID { get; set; }
        public string DiscordWebhookURL { get; set; }
        public string DefaultPlatformName { get; set; }
        public RunsAPI.Root Runs { get; set; }
    }

    public class Runs
    {
        public static List<Speedrun> games = new List<Speedrun>()
        {  
            // Mirror's Edge
            new Speedrun() { ID = "yo1yyr1q", DefaultPlatformName = "PC",  DiscordWebhookURL = ""}, 
        
            // Mirror's Edge Catalyst 
            new Speedrun() { ID = "m1mgl312", DefaultPlatformName = "PC",  DiscordWebhookURL = ""},  

            // Mirror's Edge Category Extensions 
            new Speedrun() { ID = "76rkkvd8", DefaultPlatformName = "PC",  DiscordWebhookURL = ""},
            
            // Mirror's Edge 2D 
            new Speedrun() { ID = "w6jjy56j", DefaultPlatformName = "Web", DiscordWebhookURL = ""},
            
            // Mirror's Edge (iOS)  
            new Speedrun() { ID = "j1lr741g", DefaultPlatformName = "iOS", DiscordWebhookURL = ""}
        };

        public static void GetLatestRunId() 
        {
            // Loop through all games 
            for (int i = 0; i < games.Count; i++)
            {
                // If it fails to get data (there's probably a way better to do this but maybe another time)
                TryAgain:

                // Get runs from the API
                games[i].Runs = Json.DeserializeRuns(Https.Get("runs?status=verified&orderby=verify-date&direction=desc&game=" + games[i].ID + "&embed=category.variables,level.variables,players,game,platform&max=1").Result);

                // If it gives null, a reason is sent in Https.cs class
                if (games[i].Runs == null)
                {
                    Thread.Sleep(TimeSpan.FromMinutes(5));
                    goto TryAgain;
                }

                // Set latest run id it found when starting program
                games[i].LatestRunID = games[i].Runs.Data[0].Id;

                Console.WriteLine("[{0}] Latest Run ID: {1} | Game: {2}", DateTime.Now.ToString().Pastel("#808080"), games[i].LatestRunID.Pastel("#808080"), games[i].Runs.Data[0].Game.Data.Names.International.Pastel("#808080"));
            }

            Console.WriteLine("".PadRight(112, '='));
        }

        public static void LookForNewRuns() 
        {
            // Loop through all games 
            for (int i = 0; i < games.Count; i++)
            {
                // If it fails to get data (there's probably a way better to do this but maybe another time)
                TryAgain:

                // Get runs from the API
                games[i].Runs = Json.DeserializeRuns(Https.Get("runs?status=verified&orderby=verify-date&direction=desc&game=" + games[i].ID + "&embed=category.variables,level.variables,players,game,platform").Result);

                // If it gives null, a reason is sent in Https.cs class
                if (games[i].Runs == null)
                {
                    Thread.Sleep(TimeSpan.FromMinutes(5));
                    goto TryAgain;
                }

                // The most important variable
                int num = 0;

                // Loop through all runs we get from the url
                for (int j = 0; j < games[i].Runs.Data.Length; j++)
                {
                    // Checks if the latest run is new and how many new runs there are
                    if (games[i].LatestRunID == games[i].Runs.Data[j].Id && num != 0)
                    {
                        Console.WriteLine("[{0}] {1} new run" + ((num > 1) ? "s" : "") + " found on {2}", DateTime.Now.ToString().Pastel("#808080"), num.ToString().Pastel("#808080"), games[i].Runs.Data[0].Game.Data.Names.International.Pastel("#808080"));
                        break;
                    }

                    num++;
                }

                if (num != 20)
                {
                    while (num != 0)
                    {
                        // Save the run id 
                        if (num == 1)
                        {
                            // TODO: Save run id to a file
                            games[i].LatestRunID = games[i].Runs.Data[0].Id;
                        }

                        num--;

                        // Write to the console
                        Console.WriteLine("[{0}] Run ID: {1}", DateTime.Now.ToString().Pastel("#808080"), games[i].Runs.Data[num].Id.Pastel("#808080"));

                        // Update Title
                        Title.UpdateTitle(runs: 1);

                        // Creates the embed message and sends it
                        Discord.Webhook(num, games[i].Runs, games, i);

                        // Sleep for 5 seconds so it doesn't spam the channel
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }
            }
        }
    }
}
