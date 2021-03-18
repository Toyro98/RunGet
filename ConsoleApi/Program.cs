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
        static Api.Run runs;
        static Api.Category category;
        static Api.Leaderboard leaderboard;

        static string LatestRunID;
        static readonly string URL_Runs = "https://www.speedrun.com/api/v1/runs?status=verified&orderby=verify-date&direction=desc&game=yo1yyr1q";
        static readonly string URL_Category = "https://www.speedrun.com/api/v1/games/yo1yyr1q?embed=categories,levels";

        static void Main(string[] args)
        {
            Console.Title = "RunGet v1.0";

            // Deserialize Json
            runs = JsonConvert.DeserializeObject<Api.Run>(new HttpClient().GetStringAsync(URL_Runs).Result);
            category = JsonConvert.DeserializeObject<Api.Category>(new HttpClient().GetStringAsync(URL_Category).Result);

            // Save the run ID
            LatestRunID = runs.data[0].Id.ToString();

            // Write the latest it found
            Console.WriteLine("[" + DateTime.Now + "] Latest ID found: " + LatestRunID);

            // Infinite loop 
            while (true)
            {
                // Checks every 10min. Just incase we don't hit the rate limit for the API
                Thread.Sleep(TimeSpan.FromMinutes(5));

                LookForNewRuns();
            }
        }

        static void LookForNewRuns()
        {
            // Get runs from the API
            runs = JsonConvert.DeserializeObject<Api.Run>(new HttpClient().GetStringAsync(URL_Runs).Result);

            int num = 0;

            // Loop through all runs we get from the url
            for (int i = 0; i < runs.data.Length; i++)
            {
                // Checks if the latest run is new and how many new runs there are
                if (LatestRunID == runs.data[i].Id.ToString() && num != 0)
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
                        LatestRunID = runs.data[0].Id.ToString();
                    }

                    // Where the magic happens :)
                    DiscordWebhook(num);
                    num--;

                    Console.WriteLine("[" + DateTime.Now + $"] Message sent to the Discord channel. Run ID: {runs.data[num].Id}");

                    if (num != 0)
                    {
                        // Sleep for 15 seconds so it doesn't rapidly spam the channel
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
                Title = GetFormattedTime(num) + " by " + GetUsername(runs.data[num].Players[0].Id, num),
                Url = "https://www.speedrun.com/me/run/" + runs.data[num].Id,
                Color = Color.FromArgb(42, 137, 231),

                Thumbnail = new EmbedMedia() 
                {
                    Url = "https://www.speedrun.com/themes/me/cover-128.png"
                },

                Author = new EmbedAuthor() 
                { 
                    Name = "Mirror's Edge - " + GetCategoryName(runs.data[num].Category, runs.data[num].Level, num)
                },

                Fields = new[] 
                {
                    new EmbedField() 
                    { 
                        Name = "Leaderboard Rank:", 
                        Value = GetLeaderboardRank(runs.data[num].Players[0].Id, num) 
                    },
                    new EmbedField() 
                    {
                        Name = "Date Played:", 
                        Value = runs.data[num].Date 
                    }
                }
            };

            DiscordMessage message = new DiscordMessage
            {
                Embeds = new[] { embed }
            };

            hook.Send(message);
        }

        static string GetUsername(string id, int num)
        {
            // Check if the run is a guest account or not
            if (runs.data[num].Players[0].Rel == "guest")
            {
                return runs.data[num].Players[0].Name;
            }

            return JsonConvert.DeserializeObject<Api.User>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/users/" + id).Result).data.names.International;
        }

        static string GetLeaderboardRank(string id, int num)
        {
            leaderboard = JsonConvert.DeserializeObject<Api.Leaderboard>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/users/" + id + "/personal-bests?game=yo1yyr1q").Result);

            for (int i = 0; i < leaderboard.data.Length; i++)
            {
                if (runs.data[num].Id == leaderboard.data[i].run.Id)
                {
                    return leaderboard.data[i].Place.ToString();
                }
            }

            return "n/a";
        }

        static string GetCategoryName(string category_id, string level_id, int num)
        {
            string category_name = "";
            string level_name = "";

            // IL Run?
            if (level_id != null)
            {
                for (int i = 0; i < category.data.Levels.Data.Length; i++)
                {
                    if (level_id == category.data.Levels.Data[i].Id)
                    {
                        level_name = category.data.Levels.Data[i].Name + ": ";
                    }
                }
            }

            for (int i = 0; i < category.data.Categories.Data.Length; i++)
            {
                if (category_id == category.data.Categories.Data[i].Id)
                {
                    category_name = category.data.Categories.Data[i].Name;
                }
            }

            if (level_id != null)
            {
                return level_name + category_name;
            }

            return category_name;
        }

        static string GetFormattedTime(int num)
        {
            TimeSpan time = TimeSpan.FromSeconds(runs.data[num].Times.Primary_t);

            // IL Run?
            if (runs.data[num].Level != null)
            {
                return string.Format("{0}m {1}s {2}ms", time.Minutes, time.Seconds, time.Milliseconds);
            } 

            // Check if the run is an hour long or more
            if (runs.data[num].Times.Primary_t >= 3600)
            {
                return string.Format("{0}h {1}m {2}s", time.Hours, time.Minutes, time.Seconds);
            }

            return string.Format("{0}m {1}s", time.Minutes, time.Seconds);
        }
    }
}
