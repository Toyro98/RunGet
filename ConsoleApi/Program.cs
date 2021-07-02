using System;
using System.Drawing;
using System.Net.Http;
using System.Threading;
using System.Collections.Generic;
using Discord;
using Discord.Webhook;
using Newtonsoft.Json;

namespace ConsoleApi
{
    class Game
    {
        public string GameID { get; set; }
        public string LatestRunID { get; set; }
        public string DiscordWebhookURL { get; set; }
        public Api.Run Runs { get; set; }
        public Api.Category Category { get; set; }
        public Api.Leaderboard Leaderboard { get; set; }
    }

    class Program
    {
        // Console Title
        public static int numRunsFound = 0;
        public static int numAPIcalls = 0;
        static string title = "Run Get [0 Runs Found] [0 Api Requests]";

        // A list of games to look for
        static readonly List<Game> Speedrun = new List<Game>()
        {   
            // Mirror's Edge
            new Game() { GameID = "yo1yyr1q", DiscordWebhookURL = "" },
            
            // Mirror's Edge Catalyst
            new Game() { GameID = "m1mgl312", DiscordWebhookURL = "" },  

            // Mirror's Edge Category Extensions
            new Game() { GameID = "76rkkvd8", DiscordWebhookURL = "" },  

            // Mirror's Edge 2D 
            // new Game() { GameID = "w6jjy56j", DiscordWebhookURL = "" }
        };

        static void Main(string[] args)
        {
            Console.Title = title;

            // Loop through all games 
            for (int i = 0; i < Speedrun.Count; i++)
            {
                Speedrun[i].Runs = JsonConvert.DeserializeObject<Api.Run>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/runs?status=verified&orderby=verify-date&direction=desc&game=" + Speedrun[i].GameID).Result);
                Speedrun[i].Category = JsonConvert.DeserializeObject<Api.Category>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/games/" + Speedrun[i].GameID + "?embed=categories,levels").Result);
                
                // Update title
                Console.Title = $"Run Get [0 Runs Found] [{numAPIcalls += 2} Api Requests]";

                // Get latest run id
                Speedrun[i].LatestRunID = Speedrun[i].Runs.data[0].Id;

                // Write to console
                Console.Write("[" + DateTime.Now + "] ");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Latest ID found: " + Speedrun[i].LatestRunID + " (" + Speedrun[i].Category.data.Names.International + ")");
                Console.ResetColor();
            }

            // Seperator
            Console.Write("[" + DateTime.Now + "] ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine(string.Concat(System.Linq.Enumerable.Repeat("-", 69)));
            Console.ResetColor();

            // Infinite loop 
            while (true)
            {
                // Checks every ~10min. Just incase we don't hit the rate limit for the API
                Thread.Sleep(TimeSpan.FromMinutes(10));
                LookForNewRuns();
            }
        }

        static void LookForNewRuns()
        {
            // Loop through all games 
            for (int i = 0; i < Speedrun.Count; i++)
            {
                // Get runs from the API
                Speedrun[i].Runs = JsonConvert.DeserializeObject<Api.Run>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/runs?status=verified&orderby=verify-date&direction=desc&game=" + Speedrun[i].GameID).Result);

                // Update title
                Console.Title = $"Run Get [{numRunsFound} Run{((numRunsFound == 1) ? "" : "s")} Found] [{numAPIcalls++} Api Requests]";

                int num = 0;

                // Loop through all runs we get from the url
                for (int j = 0; j < Speedrun[i].Runs.data.Length; j++)
                {
                    // Checks if the latest run is new and how many new runs there are
                    if (Speedrun[i].LatestRunID == Speedrun[i].Runs.data[j].Id && num != 0)
                    {
                        Console.Write("[" + DateTime.Now + "] ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine($"{num} new run{((num > 1) ? "s" : "")} found on {Speedrun[i].Category.data.Names.International}");
                        Console.ResetColor();
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
                            Speedrun[i].LatestRunID = Speedrun[i].Runs.data[0].Id;
                        }

                        num--;

                        // Creates the embed message and sends it
                        DiscordWebhook(num, i);

                        // Write to the console
                        Console.Write("[" + DateTime.Now + "] ");
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.WriteLine($"Run ID: {Speedrun[i].Runs.data[num].Id}");
                        Console.ResetColor();

                        // Update Title
                        numRunsFound++;
                        Console.Title = $"Run Get [{numRunsFound} Run{((numRunsFound == 1) ? "" : "s")} Found] [{numAPIcalls} Api Requests]";

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
                Url = Speedrun[index].DiscordWebhookURL
            };

            // Create the embed
            DiscordEmbed embed = new DiscordEmbed
            {
                Title = GetFormattedTime(Speedrun[index].Runs.data[num].Times.Primary_t) + " by " + GetUsername(Speedrun[index].Runs.data[num].Players[0].Id, num, index),
                Url = Speedrun[index].Runs.data[num].Weblink,
                Color = Color.FromArgb(42, 137, 231),

                Thumbnail = new EmbedMedia()
                {
                    Url = "https://www.speedrun.com/themes/" + Speedrun[index].Category.data.Abbreviation + "/cover-128.png"
                },

                Author = new EmbedAuthor()
                {
                    Name = Speedrun[index].Category.data.Names.International + " - " + GetCategoryName(Speedrun[index].Runs.data[num].Category, Speedrun[index].Runs.data[num].Level, Speedrun[index].Runs.data[num].System.Platform, index)
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
            // If the run is one player only
            if (Speedrun[index].Runs.data[num].Players.Length == 1)
            {
                // If the run is a guest account. We can get the name without sending a request
                if (Speedrun[index].Runs.data[num].Players[0].Rel == "guest")
                {
                    return Speedrun[index].Runs.data[num].Players[0].Name;
                }

                // Update title
                Console.Title = $"Run Get [{numRunsFound} Run{((numRunsFound == 1) ? "" : "s")} Found] [{numAPIcalls++} Api Requests]";

                // Returns the name from id
                // Example: Id = "zxz9yqn8" returns "Toyro98" from the API
                return JsonConvert.DeserializeObject<Api.User>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/users/" + id).Result).data.names.International;
            } 
            else
            {
                string[] players = new string[2];

                if (Speedrun[index].Runs.data[num].Players.Length == 2)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (Speedrun[index].Runs.data[num].Players[i].Rel == "guest")
                        {
                            players[i] = Speedrun[index].Runs.data[num].Players[i].Name;
                        } 
                        else
                        {
                            // Returns the name from id.
                            players[i] = JsonConvert.DeserializeObject<Api.User>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/users/" + Speedrun[index].Runs.data[num].Players[i].Id).Result).data.names.International;

                            // Update title
                            Console.Title = $"Run Get [{numRunsFound} Run{((numRunsFound == 1) ? "" : "s")} Found] [{numAPIcalls++} Api Requests]";
                        }
                    }

                    return players[0] + " and " + players[1];
                } 
                else
                {
                    int amountofrunners = Speedrun[index].Runs.data[num].Players.Length - 2;

                    for (int i = 0; i < 2; i++)
                    {
                        if (Speedrun[index].Runs.data[num].Players[i].Rel == "guest")
                        {
                            players[i] = Speedrun[index].Runs.data[num].Players[i].Name;
                        }
                        else
                        {
                            // Returns the name from id.
                            players[i] = JsonConvert.DeserializeObject<Api.User>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/users/" + Speedrun[index].Runs.data[num].Players[i].Id).Result).data.names.International;

                            // Update title
                            Console.Title = $"Run Get [{numRunsFound} Run{((numRunsFound == 1) ? "" : "s")} Found] [{numAPIcalls++} Api Requests]";
                        }
                    }

                    return players[0] + ", " + players[1] + ", and " + amountofrunners + $" other runner{((amountofrunners == 1) ? "" : "s")}";
                }
            }
        }

        static string GetLeaderboardRank(string id, int num, int index)
        {
            // Get all of the runners personal best from a game
            Speedrun[index].Leaderboard = JsonConvert.DeserializeObject<Api.Leaderboard>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/users/" + id + "/personal-bests?game=" + Speedrun[index].GameID).Result);

            // Update title
            Console.Title = $"Run Get [{numRunsFound} Run{((numRunsFound == 1) ? "" : "s")} Found] [{numAPIcalls++} Api Requests]";

            // Loop through all of their runs
            for (int j = 0; j < Speedrun[index].Leaderboard.data.Length; j++)
            {
                if (Speedrun[index].Runs.data[num].Id == Speedrun[index].Leaderboard.data[j].run.Id)
                {
                    // If we find the run. Return the rank
                    return Speedrun[index].Leaderboard.data[j].Place.ToString();
                }
            }

            // Sometimes it can return n/a even if it's a new run and I don't know why..
            return "n/a";
        }

        static string GetCategoryName(string category_id, string level_id, string platform_id, int index)
        {
            // Empty string variables since we don't know the category and level name. Only the id
            string category_name = "";
            string level_name = "";
            string platform_name = "";

            // Is the run not a PC run. Then get the platform name
            if (platform_id != "8gej2n93")
            {
                // Sends an API request and gets the name
                platform_name = JsonConvert.DeserializeObject<Api.Platform>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/platforms/" + platform_id).Result).data.Name;

                // Update title
                Console.Title = $"Run Get [{numRunsFound} Run{((numRunsFound == 1) ? "" : "s")} Found] [{numAPIcalls++} Api Requests]";
            }

            // Get the category name
            for (int j = 0; j < Speedrun[index].Category.data.Categories.Data.Length; j++)
            {
                if (category_id == Speedrun[index].Category.data.Categories.Data[j].Id)
                {
                    // Sets the variable to the category name we found
                    category_name = Speedrun[index].Category.data.Categories.Data[j].Name;
                }
            }

            // Check if it found the category name
            if (string.IsNullOrEmpty(category_name))
            {
                // Send an api request
                Speedrun[index].Category = JsonConvert.DeserializeObject<Api.Category>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/games/" + Speedrun[index].GameID + "?embed=categories,levels").Result);

                // Update title
                Console.Title = $"Run Get [{numRunsFound} Run{((numRunsFound == 1) ? "" : "s")} Found] [{numAPIcalls++} Api Requests]";

                // Loop again
                for (int j = 0; j < Speedrun[index].Category.data.Categories.Data.Length; j++)
                {
                    if (category_id == Speedrun[index].Category.data.Categories.Data[j].Id)
                    {
                        // Sets the variable to the category name we found
                        category_name = Speedrun[index].Category.data.Categories.Data[j].Name;
                    }
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

                // Check if it found the level name 
                if (string.IsNullOrEmpty(level_name))
                {
                    // Send an api request
                    Speedrun[index].Category = JsonConvert.DeserializeObject<Api.Category>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/games/" + Speedrun[index].GameID + "?embed=categories,levels").Result);
                    
                    // Update title
                    Console.Title = $"Run Get [{numRunsFound} Run{((numRunsFound == 1) ? "" : "s")} Found] [{numAPIcalls++} Api Requests]";

                    // Loop again
                    for (int j = 0; j < Speedrun[index].Category.data.Levels.Data.Length; j++)
                    {
                        if (level_id == Speedrun[index].Category.data.Levels.Data[j].Id)
                        {
                            // Sets the variable to the category name we found
                            level_name = Speedrun[index].Category.data.Levels.Data[j].Name;
                        }
                    }
                }

                if (string.IsNullOrEmpty(platform_name))
                {
                    // Return the level name and category name
                    return level_name + ": " + category_name;
                }

                // Return the level name and category name
                return level_name + ": " + category_name + " (" + platform_name + ")";
            }

            if (string.IsNullOrEmpty(platform_name))
            {
                // Return the category name
                return category_name;
            }

            // Return the category name
            return category_name + " (" + platform_name + ")";
        }

        static string GetFormattedTime(float timer)
        {
            TimeSpan time = TimeSpan.FromSeconds(timer);

            // There's probably a better way to do this but this works just fine
            // Check if the run is an hour long or more
            if (time.TotalSeconds > 3600)
            {
                if (time.Milliseconds == 0)
                {
                    if (time.Seconds == 0)
                    {
                        if (time.Minutes == 0)
                        {
                            // 1h
                            return string.Format("{0:0}h", time.Hours);
                        }

                        // 1h 23m
                        return string.Format("{0:0}h {1:0}m", time.Hours, time.Minutes);
                    }

                    // 1h 23m 45s
                    return string.Format("{0:0}h {1:0}m {2:0}s", time.Hours, time.Minutes, time.Seconds);
                }

                // 1h 23m 45s 670ms
                return string.Format("{0:0}h {1:0}m {2:0}s {3:000}ms", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
            }

            if (time.TotalSeconds > 60)
            {
                if (time.Milliseconds == 0)
                {
                    if (time.Seconds == 0)
                    {
                        // 23m
                        return string.Format("{0:0}m", time.Minutes);
                    }

                    // 23m 45s
                    return string.Format("{0:0}m {1:0}s", time.Minutes, time.Seconds);
                }

                // 23m 45s 060ms
                return string.Format("{0:0}m {1:0}s {2:000}ms", time.Minutes, time.Seconds, time.Milliseconds);
            }

            if (time.Milliseconds == 0)
            {
                // 45s
                return string.Format("{0:0}s", time.Seconds);
            }

            // 45s 060ms
            return string.Format("{0:0}s {1:000}ms", time.Seconds, time.Milliseconds);
        }
    }
}
