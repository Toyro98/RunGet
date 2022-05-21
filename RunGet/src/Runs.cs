﻿using System;
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
            for (int i = 0; i < games.Count; i++)
            {
                TryAgain:

                games[i].Runs = Json.DeserializeRuns(Https.Get("runs?status=verified&orderby=verify-date&direction=desc&game=" + games[i].ID + "&embed=category.variables,level.variables,players,game,platform&max=1").Result);

                if (games[i].Runs == null)
                {
                    Thread.Sleep(TimeSpan.FromMinutes(5));
                    goto TryAgain;
                }

                games[i].LatestRunID = games[i].Runs.Data[0].Id;

                Console.WriteLine("[{0}] Latest Run ID: {1} Game: {2}", DateTime.Now.ToString().Pastel("#808080"), games[i].LatestRunID.Pastel("#808080"), games[i].Runs.Data[0].Game.Data.Names.International.Pastel("#808080"));
            }

            Console.WriteLine("".PadRight(112, '='));
        }

        public static void LookForNewRuns() 
        {
            for (int i = 0; i < games.Count; i++)
            {
                int runsFound = 0;

                TryAgain:

                games[i].Runs = Json.DeserializeRuns(Https.Get("runs?status=verified&orderby=verify-date&direction=desc&game=" + games[i].ID + "&embed=category.variables,level.variables,players,game,platform").Result);

                if (games[i].Runs == null)
                {
                    Thread.Sleep(TimeSpan.FromMinutes(5));
                    goto TryAgain;
                }

                for (int j = 0; j < games[i].Runs.Data.Length; j++)
                {
                    if (games[i].LatestRunID == games[i].Runs.Data[j].Id && runsFound != 0)
                    {
                        Console.WriteLine("[{0}] {1} new run" + ((runsFound > 1) ? "s" : "") + " found on {2}", DateTime.Now.ToString().Pastel("#808080"), runsFound.ToString().Pastel("#808080"), games[i].Runs.Data[0].Game.Data.Names.International.Pastel("#808080"));
                        break;
                    }

                    runsFound++;
                }
                
                if (runsFound != 20)
                {
                    while (runsFound != 0)
                    {
                        if (runsFound == 1)
                        {
                            // TODO: Save run id to a file
                            games[i].LatestRunID = games[i].Runs.Data[0].Id;
                        }

                        runsFound--;

                        Console.WriteLine("[{0}] Run ID: {1}", DateTime.Now.ToString().Pastel("#808080"), games[i].Runs.Data[runsFound].Id.Pastel("#808080"));
                        Title.UpdateTitle(runs: 1);
                        Discord.Webhook(runsFound, games[i].Runs, games, i);
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }
            }
        }
    }
}