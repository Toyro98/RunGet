using System;
using System.Collections.Generic;
using System.Threading;
using Pastel;
using Discord;
using Discord.Webhook;

namespace RunGet
{
    public class Speedrun
    {
        public string ID { get; set; }
        public string LatestRunID { get; set; }
        public string WebhookURL { get; set; }
        public string DefaultPlatform { get; set; }
        public RunsApi.Data[] Runs { get; set; }
    }

    public class Runs
    {
        public static List<Speedrun> games = new List<Speedrun>()
        {
            // Mirror's Edge, Catalyst, Category Extensions, 2D, and (iOS)
            new Speedrun() { ID = "yo1yyr1q", DefaultPlatform = "PC",  WebhookURL = ""},
            new Speedrun() { ID = "m1mgl312", DefaultPlatform = "PC",  WebhookURL = ""},
            new Speedrun() { ID = "76rkkvd8", DefaultPlatform = "PC",  WebhookURL = ""},
            new Speedrun() { ID = "w6jjy56j", DefaultPlatform = "Web", WebhookURL = ""},
            new Speedrun() { ID = "j1lr741g", DefaultPlatform = "iOS", WebhookURL = ""}
        };

        public static void GetLatestRunId() 
        {
            for (int i = 0; i < games.Count; i++)
            {
                games[i].Runs = GetLatestData(games[i].ID, 1);

                // Save the latest run id
                games[i].LatestRunID = games[i].Runs[0].Id;

                // Write the latest run id from the Api to the console
                Console.WriteLine("[{0}] Latest Run ID: {1} Game: {2}", 
                    DateTime.Now.ToString().Pastel("#808080"), 
                    games[i].LatestRunID.Pastel("#808080"), 
                    games[i].Runs[0].Game.Data.Names.International.Pastel("#808080")
                );

                // Set to null. No need to keep it
                games[i].Runs = null;
            }
        }

        public static void LookForNewRuns()
        {
            for (int i = 0; i < games.Count; i++)
            {
                byte runsFound = 0;

                games[i].Runs = GetLatestData(games[i].ID, 20);

                for (int j = 0; j < games[i].Runs.Length; j++)
                {
                    if (games[i].LatestRunID != games[i].Runs[j].Id)
                    {
                        runsFound++;
                    }
                    else
                    {
                        if (runsFound == 0)
                        {
                            break;
                        }

                        // Write to console that we found new run(s)
                        Console.WriteLine("[{0}] {1} new run" + ((runsFound > 1) ? "s" : "") + " found on {2}",
                            DateTime.Now.ToString().Pastel("#808080"),
                            runsFound.ToString().Pastel("#808080"),
                            games[i].Runs[0].Game.Data.Names.International.Pastel("#808080")
                        );

                        // Get the newest latest run by getting the first element in the array
                        games[i].LatestRunID = games[i].Runs[0].Id;

                        while (runsFound != 0)
                        {
                            runsFound--;

                            // Checks if the first runner is a guest and if the name contains "n/a"
                            // If it does, ignore it
                            if (games[i].Runs[runsFound].Players.Data[0].Rel == "guest")
                            {
                                if (games[i].Runs[runsFound].Players.Data[0].Name.ToLower().Contains("n/a"))
                                {
                                    Console.WriteLine("[{0}] Run ID: {1} (Ignored due to name contains \"n/a\")",
                                        DateTime.Now.ToString().Pastel("#808080"),
                                        games[i].Runs[runsFound].Id.Pastel("#808080")
                                    );

                                    continue;
                                }
                            }

                            // Create webhook and message
                            DiscordWebhook webhook = new DiscordWebhook() { Url = games[i].WebhookURL };
                            DiscordMessage message = Webhook.Create(games[i].Runs[runsFound], games[i].DefaultPlatform);

                            // Send webhook
                            Webhook.Send(webhook, message);

                            // Write to the console which run id it found
                            Console.WriteLine("[{0}] Run ID: {1}",
                                DateTime.Now.ToString().Pastel("#808080"),
                                games[i].Runs[runsFound].Id.Pastel("#808080")
                            );

                            Title.Update(runs: 1);
                            Thread.Sleep(TimeSpan.FromSeconds(5));
                        }

                        break;
                    }
                }

                games[i].Runs = null;
            }
        }

        private static RunsApi.Data[] GetLatestData(string gameId, byte amount)
        {
            // Api only accepts max 200. If we were above limit or below 1, set it to 20
            if (amount > 200 || amount < 1)
            {
                amount = 20;
            }

            RunsApi.Data[] data = Array.Empty<RunsApi.Data>();

            string url = "runs?status=verified&orderby=verify-date&direction=desc&game=" + gameId +
                    "&embed=category.variables,level.variables,players,game,platform&max=" + amount;

            while (data.Length == 0)
            {
                data = Json.DeserializeRuns(Https.Get(url).Result);

                // If data is null or 0 length, sleep for 5 min
                if (data == null || data.Length == 0)
                {
                    data = Array.Empty<RunsApi.Data>();
                    Thread.Sleep(TimeSpan.FromMinutes(5));
                }
            }

            return data;
        }
    }
}
