using System;
using System.Drawing;
using System.Net.Http;
using System.Threading;
using System.Collections.Generic;

using Console = Colorful.Console;
using Discord;
using Discord.Webhook;
using Newtonsoft.Json;
using SharpConfig;

namespace ConsoleApi
{
    class Game
    {
        public string GameID { get; set; }
        public string LatestRunID { get; set; }
        public Api.Run Runs { get; set; }
        public Api.Category Category { get; set; }
        public Api.Leaderboard Leaderboard { get; set; }
    }

    class Discord
    {
        public string URL { get; set; }
    }

    class Program
    {
        // Config Filename
        static readonly string filename = "Runs.ini";

        // A list of games to look for
        static readonly List<Game> Speedrun = new List<Game>()
        {
            new Game() { GameID = "yo1yyr1q" },  // Mirror's Edge
            new Game() { GameID = "m1mgl312" },  // Mirror's Edge Catalyst
            new Game() { GameID = "76rkkvd8" }   // Mirror's Edge Category Extensions
        };

        // Discord Webhook urls
        static readonly List<Discord> Discord = new List<Discord>()
        {
            new Discord() { URL = "" },
            new Discord() { URL = "" }
        };

        static void Main(string[] args)
        {
            Console.Title = "RunGet v1.2";
            Configuration.SpaceBetweenEquals = true;

            // Try to load the configuration file and see if it can find it
            try
            {
                Configuration.LoadFromFile(filename);
            } 
            catch (System.IO.FileNotFoundException)
            {
                Console.Write("[" + DateTime.Now + "]");
                Console.WriteLine(" File not found! Creating a new one", Color.Gray);

                CreateNewConfig();
            }

            // Load .ini file 
            var config = Configuration.LoadFromFile(filename);
            var section = config["LatestRunIDs"];

            // Loop through all games 
            for (int i = 0; i < Speedrun.Count; i++)
            {
                Speedrun[i].Runs = JsonConvert.DeserializeObject<Api.Run>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/runs?status=verified&orderby=verify-date&direction=desc&game=" + Speedrun[i].GameID).Result);
                Speedrun[i].Category = JsonConvert.DeserializeObject<Api.Category>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/games/" + Speedrun[i].GameID + "?embed=categories,levels").Result);
                Speedrun[i].LatestRunID = Speedrun[i].Runs.data[0].Id;

                // Save the latest run id to the ini file
                section[Speedrun[i].Category.data.Abbreviation].StringValue = Speedrun[i].LatestRunID;

                // if (section[Speedrun[i].Category.data.Abbreviation].StringValue != Speedrun[i].LatestRunID) {}

                Console.Write("[" + DateTime.Now + "] ");
                Console.WriteLine("Latest ID found: " + Speedrun[i].LatestRunID + " (" + Speedrun[i].Category.data.Names.International + ")", Color.Gray);
            }

            config.SaveToFile(filename);

            // Seperator
            Console.Write("[" + DateTime.Now + "] ");
            Console.WriteLine(string.Concat(System.Linq.Enumerable.Repeat("-", 69)), Color.Gray);

            // Infinite loop 
            while (true)
            {
                // Checks every ~10min. Just incase we don't hit the rate limit for the API
                Thread.Sleep(TimeSpan.FromMinutes(10));

                LookForNewRuns();
            }
        }

        static void CreateNewConfig()
        {
            var config = new Configuration();

            // Loop through all games it should look for
            for (int i = 0; i < Speedrun.Count; i++)
            {
                config["LatestRunIDs"][JsonConvert.DeserializeObject<Api.Category>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/games/" + Speedrun[i].GameID).Result).data.Abbreviation].StringValue = "null";
            }

            // Save changes
            config.SaveToFile(filename);
        }

        static void LookForNewRuns()
        {
            // Loop through all games 
            for (int i = 0; i < Speedrun.Count; i++)
            {
                // Get runs from the API
                Speedrun[i].Runs = JsonConvert.DeserializeObject<Api.Run>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/runs?status=verified&orderby=verify-date&direction=desc&game=" + Speedrun[i].GameID).Result);

                int num = 0;

                // Loop through all runs we get from the url
                for (int j = 0; j < Speedrun[i].Runs.data.Length; j++)
                {
                    // Checks if the latest run is new and how many new runs there are
                    if (Speedrun[i].LatestRunID == Speedrun[i].Runs.data[j].Id && num != 0)
                    {
                        Console.Write("[" + DateTime.Now + "]");
                        Console.WriteLine($" {num} new run{((num > 1) ? "s" : "")} found on {Speedrun[i].Category.data.Names.International}", Color.Gray);
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
                    Console.Write("[" + DateTime.Now + "]");
                    Console.WriteLine($" No new runs found on {Speedrun[i].Category.data.Names.International}", Color.Gray);
                }
                else
                {
                    while (num != 0)
                    {
                        // Save the run id 
                        if (num == 1)
                        {
                            Speedrun[i].LatestRunID = Speedrun[i].Runs.data[0].Id;
                        }

                        num--;

                        // Creates the embed message and sends it
                        DiscordWebhook(num, i);

                        // Write to the console
                        Console.Write("[" + DateTime.Now + "]");
                        Console.WriteLine($" Run ID: {Speedrun[i].Runs.data[num].Id}", Color.Gray);
  
                        // Sleep for ~5 seconds so it doesn't spam the channel
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }
            }
        }

        static void DiscordWebhook(int num, int index)
        {
            // Discord webhook url
            DiscordWebhook webhook = new DiscordWebhook
            {
                // Check if the game if is from Catalyst
                Url = Discord[(Speedrun[index].GameID == "m1mgl312") ? 1 : 0].URL
            };

            // Create the embed
            DiscordEmbed embed = new DiscordEmbed
            {
                Title = GetFormattedTime(num, index) + " by " + GetUsername(Speedrun[index].Runs.data[num].Players[0].Id, num, index),
                Url = Speedrun[index].Category.data.Weblink + "/run/" + Speedrun[index].Runs.data[num].Id,
                Color = Color.FromArgb(42, 137, 231),

                Thumbnail = new EmbedMedia()
                {
                    Url = "https://www.speedrun.com/themes/" + Speedrun[index].Category.data.Abbreviation + "/cover-128.png"
                },

                Author = new EmbedAuthor()
                {
                    Name = Speedrun[index].Category.data.Names.International + " - " + GetCategoryName(Speedrun[index].Runs.data[num].Category, Speedrun[index].Runs.data[num].Level, index)
                },

                Fields = new[]
                {
                    new EmbedField()
                    {
                        Name = "Leaderboard Rank:",
                        Value = GetLeaderboardRank(Speedrun[index].Runs.data[num].Players[0].Id, num, index)
                    },
                    new EmbedField()
                    {
                        Name = "Date Played:",
                        Value = Speedrun[index].Runs.data[num].Date
                    }
                }
            };

            // Puts the embed in a message
            DiscordMessage message = new DiscordMessage
            {
                Username = "Run Get",
                AvatarUrl = "https://raw.githubusercontent.com/Toyro98/RunGet/main/ConsoleApi/Image/Avatar.png",
                Embeds = new[] { embed }
            };

            // Send the discord message with the embed
            webhook.Send(message);
        }

        static string GetUsername(string id, int num, int index)
        {
            // If the run is a guest account. We can get the name without sending a request
            if (Speedrun[index].Runs.data[num].Players[0].Rel == "guest")
            {
                return Speedrun[index].Runs.data[num].Players[0].Name;
            }

            // Returns the name from id. Example: Id = "zxz9yqn8" returns "Toyro98" from the API
            return JsonConvert.DeserializeObject<Api.User>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/users/" + id).Result).data.names.International;
        }

        static string GetLeaderboardRank(string id, int num, int index)
        {
            // Get all of the runners personal best from a game
            Speedrun[index].Leaderboard = JsonConvert.DeserializeObject<Api.Leaderboard>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/users/" + id + "/personal-bests?game=" + Speedrun[index].GameID).Result);

            // Loop through all of their runs
            for (int j = 0; j < Speedrun[index].Leaderboard.data.Length; j++)
            {
                if (Speedrun[index].Runs.data[num].Id == Speedrun[index].Leaderboard.data[j].run.Id)
                {
                    // If we find the run. Return the rank
                    return Speedrun[index].Leaderboard.data[j].Place.ToString();
                }
            }

            // If someone submits an old run, it wont have a rank since it's obsolete
            return "n/a *(Obsolete)*";
        }

        static string GetCategoryName(string category_id, string level_id, int index)
        {
            // Empty string variables since we don't know the category and level name. Only the id
            string category_name = "";
            string level_name = "";

            // Get the category name
            for (int j = 0; j < Speedrun[index].Category.data.Categories.Data.Length; j++)
            {
                if (category_id == Speedrun[index].Category.data.Categories.Data[j].Id)
                {
                    // Sets the variable to the category name we found
                    category_name = Speedrun[index].Category.data.Categories.Data[j].Name;
                }
            }

            // Is the run a IL run. If not, it will just return the category name
            if (level_id != null)
            {
                // Loop though all level ids until we find the the right one 
                for (int j = 0; j < Speedrun[index].Category.data.Levels.Data.Length; j++)
                {
                    if (level_id == Speedrun[index].Category.data.Levels.Data[j].Id)
                    {
                        // Sets the variable to the category name we found
                        level_name = Speedrun[index].Category.data.Levels.Data[j].Name;
                    }
                }

                // Return the level name and category name
                return level_name + ": " + category_name;
            }

            // Return the category name
            return category_name;
        }

        static string GetFormattedTime(int num, int index)
        {
            TimeSpan time = TimeSpan.FromSeconds(Speedrun[index].Runs.data[num].Times.Primary_t);

            // There's probably a better way to do this but this works just fine
            // Check if the run is an hour long or more
            if (time.TotalSeconds > 3600)
            {
                if (time.Milliseconds == 0)
                {
                    if (time.Seconds == 0)
                    {
                        // 1h 23m
                        return string.Format("{0}h {1}m", time.Hours, time.Minutes);
                    }

                    // 1h 23m 45s
                    return string.Format("{0}h {1}m {2}s", time.Hours, time.Minutes, time.Seconds);
                }

                // 1h 23m 45s 670ms
                return string.Format("{0}h {1}m {2}s {3}ms", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
            } 

            if (time.TotalSeconds > 60)
            {
                if (time.Milliseconds == 0)
                {
                    if (time.Seconds == 0)
                    {
                        // 23m
                        return string.Format("{0}m", time.Minutes);
                    }

                    // 23m 45s
                    return string.Format("{0}m {1}s", time.Minutes, time.Seconds);
                }

                // 23m 45s 670ms
                return string.Format("{0}m {1}s {2}ms", time.Minutes, time.Seconds, time.Milliseconds);
            }

            if (time.Milliseconds == 0)
            {
                // 45s
                return string.Format("{0}s", time.Seconds);
            }

            // 45s 670ms
            return string.Format("{0}s {1}ms", time.Seconds, time.Milliseconds);
        }
    }
}
