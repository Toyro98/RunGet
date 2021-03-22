using System;
using System.Drawing;
using System.Net.Http;
using System.Threading;
using System.Collections.Generic;
using Newtonsoft.Json;
using Discord;
using Discord.Webhook;

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

    class Program
    {   
        // A list of games to look for
        static List<Game> speedrun = new List<Game>()
        {
            new Game() { GameID = "yo1yyr1q" },  // Mirror's Edge
            new Game() { GameID = "76rkkvd8" }   // Mirror's Edge Category Extensions
        };

        static void Main(string[] args)
        {
            Console.Title = "RunGet v1.1";

            // Loop through all games 
            for (int i = 0; i < speedrun.Count; i++)
            {
                speedrun[i].Runs = JsonConvert.DeserializeObject<Api.Run>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/runs?status=verified&orderby=verify-date&direction=desc&game=" + speedrun[i].GameID).Result);
                speedrun[i].Category = JsonConvert.DeserializeObject<Api.Category>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/games/" + speedrun[i].GameID + "?embed=categories,levels").Result);
                speedrun[i].LatestRunID = speedrun[i].Runs.data[0].Id;

                Console.WriteLine("[" + DateTime.Now + "] Latest ID found: " + speedrun[i].LatestRunID + " (" + speedrun[i].Category.data.Names.International + ")");
            }

            Console.WriteLine("[" + DateTime.Now + "] -------------------------");

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
            for (int i = 0; i < speedrun.Count; i++)
            {
                // Get runs from the API
                speedrun[i].Runs = JsonConvert.DeserializeObject<Api.Run>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/runs?status=verified&orderby=verify-date&direction=desc&game=" + speedrun[i].GameID).Result);

                int num = 0;

                // Loop through all runs we get from the url
                for (int j = 0; j < speedrun[i].Runs.data.Length; j++)
                {
                    // Checks if the latest run is new and how many new runs there are
                    if (speedrun[i].LatestRunID == speedrun[i].Runs.data[j].Id && num != 0)
                    {
                        Console.WriteLine("[" + DateTime.Now + $"] {num} new runs found on {speedrun[i].Category.data.Names.International}");
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
                    Console.WriteLine("[" + DateTime.Now + $"] No new runs found on {speedrun[i].Category.data.Names.International}");
                }
                else
                {
                    while (num != 0)
                    {
                        // Save the run id 
                        if (num == 1)
                        {
                            speedrun[i].LatestRunID = speedrun[i].Runs.data[0].Id;
                        }

                        num--;

                        // Creates the embed message and sends it
                        DiscordWebhook(num, i);

                        // Write to the console
                        Console.WriteLine("[" + DateTime.Now + $"] Run ID: {speedrun[i].Runs.data[num].Id} Game: {speedrun[i].Category.data.Names.International}");

                        if (num == 0)
                        {
                            break;
                        }
                        else
                        {
                            // Sleep for ~15 seconds so it doesn't spam the channel
                            Thread.Sleep(TimeSpan.FromSeconds(15));
                        }
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }

                Thread.Sleep(TimeSpan.FromSeconds(1));
            }
        }

        static void DiscordWebhook(int num, int index)
        {
            DiscordWebhook hook = new DiscordWebhook
            {
                Url = ""
            };

            DiscordEmbed embed = new DiscordEmbed
            {
                Title = GetFormattedTime(num, index) + " by " + GetUsername(speedrun[index].Runs.data[num].Players[0].Id, num, index),
                Url = speedrun[index].Category.data.Weblink + "/run/" + speedrun[index].Runs.data[num].Id,
                Color = Color.FromArgb(42, 137, 231),

                Thumbnail = new EmbedMedia()
                {
                    Url = "https://www.speedrun.com/themes/" + speedrun[index].Category.data.Abbreviation + "/cover-128.png"
                },

                Author = new EmbedAuthor() 
                { 
                    Name = speedrun[index].Category.data.Names.International + " - " + GetCategoryName(speedrun[index].Runs.data[num].Category, speedrun[index].Runs.data[num].Level, index)
                },

                Fields = new[] 
                {
                    new EmbedField() 
                    { 
                        Name = "Leaderboard Rank:", 
                        Value = GetLeaderboardRank(speedrun[index].Runs.data[num].Players[0].Id, num, index) 
                    },
                    new EmbedField() 
                    {
                        Name = "Date Played:", 
                        Value = speedrun[index].Runs.data[num].Date 
                    }
                }
            };

            // Puts the embed in a message
            DiscordMessage message = new DiscordMessage
            {
                Embeds = new[] { embed }
            };

            // Send the discord message with the embed
            hook.Send(message);
        }

        static string GetUsername(string id, int num, int index)
        {
            // If the run is a guest account. We can get the name without sending a request
            if (speedrun[index].Runs.data[num].Players[0].Rel == "guest")
            {
                return speedrun[index].Runs.data[num].Players[0].Name;
            }

            // Returns the name from id. Example: Id = "zxz9yqn8" returns "Toyro98" from the API
            return JsonConvert.DeserializeObject<Api.User>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/users/" + id).Result).data.names.International;
        }

        static string GetLeaderboardRank(string id, int num, int index)
        {
            // Get all of the runners personal best from a game
            speedrun[index].Leaderboard = JsonConvert.DeserializeObject<Api.Leaderboard>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/users/" + id + "/personal-bests?game=" + speedrun[index].GameID).Result);

            // Loop through all of their runs
            for (int j = 0; j < speedrun[index].Leaderboard.data.Length; j++)
            {
                if (speedrun[index].Runs.data[num].Id == speedrun[index].Leaderboard.data[j].run.Id)
                {
                    // If we find the run. Return the rank
                    return speedrun[index].Leaderboard.data[j].Place.ToString();
                }
            }

            // If someone submits 2 runs and both get verified before the API updates, the slowest run will have no rank since it's obsolete
            return "n/a *(Obsolete)*";
        }

        static string GetCategoryName(string category_id, string level_id, int index)
        {
            // Empty string variables since we don't know the category and level name. Only the id
            string category_name = "";
            string level_name = "";

            // Get the category name
            for (int j = 0; j < speedrun[index].Category.data.Categories.Data.Length; j++)
            {
                if (category_id == speedrun[index].Category.data.Categories.Data[j].Id)
                {
                    // Sets the variable to the category name we found
                    category_name = speedrun[index].Category.data.Categories.Data[j].Name;
                }
            }

            // Is the run a IL run. If not, it will just return the category name
            if (level_id != null)
            {
                // Loop though all level ids until we find the the right one 
                for (int j = 0; j < speedrun[index].Category.data.Levels.Data.Length; j++)
                {
                    if (level_id == speedrun[index].Category.data.Levels.Data[j].Id)
                    {
                        // Sets the variable to the category name we found
                        level_name = speedrun[index].Category.data.Levels.Data[j].Name;
                    }
                }

                // Return the level name and categort name
                return level_name + ": " + category_name;
            }

            // Return the category name
            return category_name;
        }

        static string GetFormattedTime(int num, int index)
        {
            TimeSpan time = TimeSpan.FromSeconds(speedrun[index].Runs.data[num].Times.Primary_t);

            // Check if the run is a IL run
            if (speedrun[index].Runs.data[num].Level != null)
            {
                if (time.TotalSeconds > 60)
                {
                    if (time.Milliseconds > 10)
                    {
                        return string.Format("{0}m {1}s {2}ms", time.Minutes, time.Seconds, time.Milliseconds);
                    }

                    return string.Format("{0}m {1}s 0{2}ms", time.Minutes, time.Seconds, time.Milliseconds);
                }

                return string.Format("{0}s {1}ms", time.Seconds, time.Milliseconds);
            } 

            // Check if the run is an hour long or more
            if (time.TotalSeconds > 3600)
            {
                if (time.Milliseconds > 0)
                {
                    return string.Format("{0}h {1}m {2}s {3}ms", time.Hours, time.Minutes, time.Seconds, time.Milliseconds);
                }

                return string.Format("{0}h {1}m {2}s", time.Hours, time.Minutes, time.Seconds);
            }

            if (time.Milliseconds > 0)
            {
                return string.Format("{0}m {1}s {2}ms", time.Minutes, time.Seconds, time.Milliseconds);
            }

            return string.Format("{0}m {1}s", time.Minutes, time.Seconds);
        }
    }
}
