using System;
using System.Drawing;
using System.Net.Http;
using System.Threading;
using Newtonsoft.Json;
using Discord;
using Discord.Webhook;

namespace ConsoleApi
{
    class Program
    {
        static Api.Run Runs;
        static Api.Category Category;
        static Api.Leaderboard Leaderboard;

        static string LatestRunID;

        static void Main(string[] args)
        {
            Console.Title = "RunGet v1.1";

            // Deserialize Json
            Runs = JsonConvert.DeserializeObject<Api.Run>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/runs?status=verified&orderby=verify-date&direction=desc&game=yo1yyr1q").Result);
            Category = JsonConvert.DeserializeObject<Api.Category>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/games/yo1yyr1q?embed=categories,levels").Result);

            // Save the run ID
            LatestRunID = Runs.data[0].Id;

            // Write the latest it found
            Console.WriteLine("[" + DateTime.Now + "] Latest ID found: " + LatestRunID + " (" + Category.data.Names.International + ")");

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
            // Get runs from the API
            Runs = JsonConvert.DeserializeObject<Api.Run>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/runs?status=verified&orderby=verify-date&direction=desc&game=yo1yyr1q").Result);

            int num = 0;

            // Loop through all runs we get from the url
            for (int i = 0; i < Runs.data.Length; i++)
            {
                // Checks if the latest run is new and how many new runs there are
                if (LatestRunID == Runs.data[i].Id && num != 0)
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
                        LatestRunID = Runs.data[0].Id;
                    }

                    // Creates the embed message and sends it
                    DiscordWebhook(num);

                    // Decrease the num variable once we have sent the embed message
                    num--;

                    // Write to the console
                    Console.WriteLine("[" + DateTime.Now + $"] Message sent to the Discord channel. Run ID: {Runs.data[num].Id}");

                    if (num != 0)
                    {
                        // Sleep for ~15 seconds so it doesn't spam the channel
                        Thread.Sleep(TimeSpan.FromSeconds(15));
                    }
                }
            }
        }

        static void DiscordWebhook(int num)
        {
            DiscordWebhook hook = new DiscordWebhook
            {
                Url = ""
            };

            DiscordEmbed embed = new DiscordEmbed
            {
                Title = GetFormattedTime(num) + " by " + GetUsername(Runs.data[num].Players[0].Id, num),
                Url = Category.data.Weblink + "/run/" + Runs.data[num].Id,
                Color = Color.FromArgb(42, 137, 231),

                Thumbnail = new EmbedMedia()
                {
                    Url = "https://www.speedrun.com/themes/" + Category.data.Abbreviation + "/cover-128.png"
                },

                Author = new EmbedAuthor() 
                { 
                    Name = Category.data.Names.International + " - " + GetCategoryName(Runs.data[num].Category, Runs.data[num].Level)
                },

                Fields = new[] 
                {
                    new EmbedField() 
                    { 
                        Name = "Leaderboard Rank:", 
                        Value = GetLeaderboardRank(Runs.data[num].Players[0].Id, num) 
                    },
                    new EmbedField() 
                    {
                        Name = "Date Played:", 
                        Value = Runs.data[num].Date 
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

        static string GetUsername(string id, int num)
        {
            // If the run is a guest account. We can get the name without sending a request
            if (Runs.data[num].Players[0].Rel == "guest")
            {
                return Runs.data[num].Players[0].Name;
            }

            // Returns the name from id. Example: Id = "zxz9yqn8" returns "Toyro98" from the API
            return JsonConvert.DeserializeObject<Api.User>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/users/" + id).Result).data.names.International;
        }

        static string GetLeaderboardRank(string id, int num)
        {
            // Get all of the runners personal best from a game
            Leaderboard = JsonConvert.DeserializeObject<Api.Leaderboard>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/users/" + id + "/personal-bests?game=yo1yyr1q").Result);

            // Loop through all of their runs
            for (int i = 0; i < Leaderboard.data.Length; i++)
            {
                if (Runs.data[num].Id == Leaderboard.data[i].run.Id)
                {
                    // If we find the run. Return the rank
                    return Leaderboard.data[i].Place.ToString();
                }
            }

            // If someone submits 2 runs and both get verified before the API updates, the slowest run will have no rank since it's obsolete
            return "n/a *(Obsolete)*";
        }

        static string GetCategoryName(string category_id, string level_id)
        {
            // Empty string variables since we don't know the category and level name. Only the id
            string category_name = "";
            string level_name = "";

            // Get the category name
            for (int i = 0; i < Category.data.Categories.Data.Length; i++)
            {
                if (category_id == Category.data.Categories.Data[i].Id)
                {
                    // Sets the variable to the category name we found
                    category_name = Category.data.Categories.Data[i].Name;
                }
            }

            // Is the run a IL run. If not, it will just return the category name
            if (level_id != null)
            {
                // Loop though all level ids until we find the the right one 
                for (int i = 0; i < Category.data.Levels.Data.Length; i++)
                {
                    if (level_id == Category.data.Levels.Data[i].Id)
                    {
                        // Sets the variable to the category name we found
                        level_name = Category.data.Levels.Data[i].Name;
                    }
                }

                // Return the level name and categort name
                return level_name + ": " + category_name;
            }

            // Return the category name
            return category_name;
        }

        static string GetFormattedTime(int num)
        {
            TimeSpan time = TimeSpan.FromSeconds(Runs.data[num].Times.Primary_t);

            // Check if the run is a IL run
            if (Runs.data[num].Level != null)
            {
                if (time.TotalSeconds >= 60)
                {
                    return string.Format("{0}m {1}s {2}ms", time.Minutes, time.Seconds, time.Milliseconds / 10);
                }

                return string.Format("{0}s {1}ms", time.Seconds, time.Milliseconds / 10);
            } 

            // Check if the run is an hour long or more
            if (time.TotalSeconds >= 3600)
            {
                if (time.Milliseconds > 0)
                {
                    return string.Format("{0}h {1}m {2}s {3}ms", time.Hours, time.Minutes, time.Seconds, time.Milliseconds / 10);
                }

                return string.Format("{0}h {1}m {2}s", time.Hours, time.Minutes, time.Seconds);
            }

            if (time.Milliseconds > 0)
            {
                return string.Format("{0}m {1}s {2}ms", time.Minutes, time.Seconds, time.Milliseconds / 10);
            }

            return string.Format("{0}m {1}s", time.Minutes, time.Seconds);
        }
    }
}
