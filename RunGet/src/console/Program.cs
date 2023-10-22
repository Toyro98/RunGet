using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Pastel;

namespace RunGet
{
    public class Program
    {
        public static void Main(string[] args) 
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Title.Version = version.Major + "." + version.Minor + (version.Build != 0 ? "." + version.Build.ToString() : "");

            Runs.games = new List<Speedrun>()
            {
                // How to find the id for a game
                // 1. Go to the speedrun page "https://www.speedrun.com/sm64" (Super Mario 64 used as example)
                // 2. Grab what's left after removing "https://www.speedrun.com/" (sm64)
                // 3. Go to "https://www.speedrun.com/api/v1/games/" and paste what you copied before
                // 4. The url should have changed to the id you're looking for

                new Speedrun() 
                { 
                    Id = "yo1yyr1q", 
                    DefaultPlatform = "PC", 
                    WebhookUrls = new List<string> 
                    {
                        "",
                    }
                },
                new Speedrun()
                {
                    Id = "m1mgl312", 
                    DefaultPlatform = "PC",
                    WebhookUrls = new List<string> 
                    {
                        "",
                    }
                },
                new Speedrun()
                {
                    Id = "76rkkvd8", 
                    DefaultPlatform = "PC",
                    WebhookUrls = new List<string> 
                    {
                        "",
                    }
                },
                new Speedrun()
                {
                    Id = "w6jjy56j", 
                    DefaultPlatform = "Web",
                    WebhookUrls = new List<string> 
                    {
                        "",
                    }
                },
                new Speedrun()
                {
                    Id = "j1lr741g", 
                    DefaultPlatform = "iOS",
                    WebhookUrls = new List<string> 
                    {
                        "",
                    }
                }
            };

            Runs.GetLatestRunId();
            Utils.Seperator();

            while (true)
            {
                Thread.Sleep(TimeSpan.FromMinutes(5));

                try
                {
                    Runs.LookForNewRuns();
                }
                catch (Exception ex)
                {
                    Utils.Seperator();

                    Console.WriteLine("[{0}] {1}\n\n{2}", 
                        DateTime.Now.ToString().Pastel("#808080"), 
                        "Error!!".Pastel("#8C0000"), 
                        ex.ToString()
                    );

                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
        }
    }
}
