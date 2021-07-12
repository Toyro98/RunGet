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
                UpdateTitle(2, 0);

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

                // Catch errors
                try
                {
                    LookForNewRuns();
                }
                catch (Exception e)
                {
                    Console.Title = "Error!! :(";

                    Console.Write("[" + DateTime.Now + "] ");
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                    // Write the error message
                    Console.WriteLine(e.Message);
                    Console.ReadKey();
                }
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
                UpdateTitle(1,0);

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
                        UpdateTitle(0, 1);

                        // Sleep for ~5 seconds so it doesn't spam the channel
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }
            }
        }

        static void DiscordWebhook(int num, int gameindex)
        {
            // Discord webhook url
            DiscordWebhook webhook = new DiscordWebhook
            {
                Url = Speedrun[gameindex].DiscordWebhookURL
            };

            // Create the embed
            DiscordEmbed embed = new DiscordEmbed
            {
                Title = GetFormattedTime(Speedrun[gameindex].Runs.data[num].Times.Primary_t) + "by " + GetUsername(num, gameindex),
                Url = Speedrun[gameindex].Runs.data[num].Weblink,
                Color = Color.FromArgb(42, 137, 231),

                Thumbnail = new EmbedMedia()
                {
                    Url = "https://www.speedrun.com/themes/" + Speedrun[gameindex].Category.data.Abbreviation + "/cover-128.png"
                },

                Author = new EmbedAuthor()
                {
                    Name = Speedrun[gameindex].Category.data.Names.International + " - " + GetCategoryName(Speedrun[gameindex].Runs.data[num].Category, Speedrun[gameindex].Runs.data[num].Level, Speedrun[gameindex].Runs.data[num].System.Platform, gameindex)
                },

                Fields = new[]
                {
                    new EmbedField()
                    {
                        Name = "Leaderboard Rank:",
                        Value = GetLeaderboardRank(num, gameindex)
                    },
                    new EmbedField()
                    {
                        Name = "Date Played:",
                        Value = Speedrun[gameindex].Runs.data[num].Date
                    }
                }
            };

            // Puts the embed in a message
            DiscordMessage message = new DiscordMessage
            {
                Username = "Verified Runs",
                AvatarUrl = "https://raw.githubusercontent.com/Toyro98/RunGet/main/ConsoleApi/Image/Avatar.png",
                Embeds = new[] { embed }
            };

            // Send the discord message with the embed
            webhook.Send(message);
        }

        static string GetUsername(int num, int gameindex)
        {
            // Create an empty string to store the runners name
            string runners = string.Empty;

            // If the run is one player only
            if (Speedrun[gameindex].Runs.data[num].Players.Length == 1)
            {
                // If the run is a guest account. We can get the name without sending a request
                if (Speedrun[gameindex].Runs.data[num].Players[0].Rel == "guest")
                {
                    // Returns the guest's name
                    runners = Speedrun[gameindex].Runs.data[num].Players[0].Name;
                }
                else
                { 
                    // Returns the name from id
                    // Example: Id = "zxz9yqn8" returns "Toyro98" from the API
                    runners = JsonConvert.DeserializeObject<Api.User>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/users/" + Speedrun[gameindex].Runs.data[num].Players[0].Id).Result).data.names.International;

                    // Update title
                    UpdateTitle(1, 0);
                }
            }
            else
            {
                // Create a string 
                string[] players = new string[2];

                // Loop 2 times to get their names
                for (int i = 0; i < 2; i++)
                {
                    if (Speedrun[gameindex].Runs.data[num].Players[i].Rel == "guest")
                    {
                        players[i] = Speedrun[gameindex].Runs.data[num].Players[i].Name;
                    }
                    else
                    {
                        // Returns the name from id.
                        players[i] = JsonConvert.DeserializeObject<Api.User>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/users/" + Speedrun[gameindex].Runs.data[num].Players[i].Id).Result).data.names.International;

                        // Update title
                        UpdateTitle(1, 0);
                    }

                    // Check if the run is co-op
                    if (Speedrun[gameindex].Runs.data[num].Players.Length == 2)
                    {
                        runners = players[0] + " and " + players[1];
                    }
                    else
                    {
                        // Instead of displaying everyones names in the embed title. It will show the first two players and say "x, y, and z more runners"
                        int amountofrunners = Speedrun[gameindex].Runs.data[num].Players.Length - 2;

                        runners = players[0] + ", " + players[1] + ", and " + amountofrunners + $" other runner{((amountofrunners == 1) ? "" : "s")}";
                    }
                }
            }

            return runners;
        }

        static string GetLeaderboardRank(int num, int gameindex)
        {
            string leaderboardRank = string.Empty;

            // Get all of the runners personal best from a game
            Speedrun[gameindex].Leaderboard = JsonConvert.DeserializeObject<Api.Leaderboard>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/users/" + Speedrun[gameindex].Runs.data[num].Players[0].Id + "/personal-bests?game=" + Speedrun[gameindex].GameID).Result);

            // Update title
            UpdateTitle(1, 0);

            // Loop through all of their runs
            for (int j = 0; j < Speedrun[gameindex].Leaderboard.data.Length; j++)
            {
                if (Speedrun[gameindex].Runs.data[num].Id == Speedrun[gameindex].Leaderboard.data[j].run.Id)
                {
                    // If we find the run. Return the rank
                    leaderboardRank = Speedrun[gameindex].Leaderboard.data[j].Place.ToString();
                }
            }

            // If we don't find the run. Return "n/a"
            if (string.IsNullOrEmpty(leaderboardRank))
            {
                return "n/a";
            }

            return leaderboardRank;
        }

        static string GetCategoryName(string category_id, string level_id, string platform_id, int index)
        {
            // Empty string variables since we don't know the category and level name. Only the id
            string category_name = string.Empty;
            string level_name = string.Empty;
            string platform_name = string.Empty;

            // Is the run not a PC run. Then get the platform name
            if (platform_id != "8gej2n93")
            {
                // Sends an API request and gets the name
                platform_name = JsonConvert.DeserializeObject<Api.Platform>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/platforms/" + platform_id).Result).data.Name;

                // Update title
                UpdateTitle(1, 0);
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
                UpdateTitle(1, 0);

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
                    UpdateTitle(1, 0);

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

            Dictionary<string, int> a = new Dictionary<string, int>() { };
            a.Add("ms", time.Milliseconds);
            a.Add("h",  time.Hours);
            a.Add("d",  time.Days);
            // Create an empty dictionary 
            Dictionary<string, int> timeDictionary = new Dictionary<string, int>() {};

            // Add the data from the timespan
            timeDictionary.Add("ms", time.Milliseconds);
            timeDictionary.Add("s", time.Seconds);
            timeDictionary.Add("m", time.Minutes);
            timeDictionary.Add("h", time.Hours);
            timeDictionary.Add("d", time.Days);

            // Empty string variable
            string timerString = string.Empty;

            // Loop through the dictionary 
            foreach (var item in timeDictionary)
            {
                // Check if the value is higher than 0. So we don't have to print 0h 0m 32s 0ms and instead print 32s
                if (item.Value > 0)
                {
                    // Check if milliseconds is eiher than 0. If milliseconds is 50. It would print 5ms instead of 050ms
                    if (item.Key.Contains("ms") && item.Value > 0)
                    {
                        // If true, the milliseconds is added first and checks if it should add zeros to it
                        timerString = timerString.Insert(0, ((item.Value < 9) ? "00" : ((item.Value < 99) ? "0" : "")) + item.Value + item.Key + " ");
                    } 
                    else
                    {
                        // Insert the time and suffix to the string
                        timerString = timerString.Insert(0, item.Value + item.Key + " ");
                    }
                }
            }

            // Returns the time like for example: 4m 20s 690ms
            return timerString;
        }

        static void UpdateTitle(int api, int runs)
        {
            numAPIcalls += api;
            numRunsFound += runs;

            Console.Title = "Run Get [" + numRunsFound + " Run" + ((numRunsFound == 1) ? "" : "s") + " Found] [" + numAPIcalls + " Api Requests]";
        }
    }
}
