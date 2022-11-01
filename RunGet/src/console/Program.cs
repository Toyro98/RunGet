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
            Title.version = version.Major + "." + version.Minor + (version.Build != 0 ? "." + version.Build.ToString() : "");

            Runs.games = new List<Speedrun>()
            {
                // Mirror's Edge, Catalyst, Category Extensions, 2D, and (iOS)
                new Speedrun() { Id = "yo1yyr1q", DefaultPlatform = "PC", WebhookUrl = ""},
                new Speedrun() { Id = "m1mgl312", DefaultPlatform = "PC", WebhookUrl = ""},
                new Speedrun() { Id = "76rkkvd8", DefaultPlatform = "PC", WebhookUrl = ""},
                new Speedrun() { Id = "w6jjy56j", DefaultPlatform = "Web", WebhookUrl = ""},
                new Speedrun() { Id = "j1lr741g", DefaultPlatform = "iOS", WebhookUrl = ""}
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
