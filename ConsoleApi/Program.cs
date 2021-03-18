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
        static Api.RunInfo runs;
        static Api.CategoryInfo category;
        static Api.LeaderboardInfo leaderboard;

        static string LatestRunID;
        static readonly string URL_Runs = "https://www.speedrun.com/api/v1/runs?status=verified&orderby=verify-date&direction=desc&game=yo1yyr1q";
        static readonly string URL_Category = "https://www.speedrun.com/api/v1/games/yo1yyr1q?embed=categories";

        static void Main(string[] args)
        {
            Console.Title = "RunGet v1.0";

            // Deserialize Json
            runs = JsonConvert.DeserializeObject<Api.RunInfo>(new HttpClient().GetStringAsync(URL_Runs).Result);
            category = JsonConvert.DeserializeObject<Api.CategoryInfo>(new HttpClient().GetStringAsync(URL_Category).Result);

            // Save the run ID
            LatestRunID = runs.data[0].Id.ToString();

            // Write the latest it found
            Console.WriteLine("[" + DateTime.Now + "] Latest ID found: " + LatestRunID);

            // Infinite loop 
            while (true)
            {
                Thread.Sleep(TimeSpan.FromMinutes(2));

                LookForNewRuns();
            }
        }

        static void LookForNewRuns()
        {
            // Get runs from the API
            runs = JsonConvert.DeserializeObject<Api.RunInfo>(new HttpClient().GetStringAsync(URL_Runs).Result);

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

                    DiscordWebhook(num);

                    num--;
                    Console.WriteLine("[" + DateTime.Now + $"] ID: {runs.data[num].Id}");

                    if (num != 0)
                    {
                        // Sleep for few seconds. Just incase we don't hit the Discord's rate limit
                        Thread.Sleep(TimeSpan.FromSeconds(5));
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
                Title = runs.data[num].Times.Primary_t + " by " + GetUsername(runs.data[num].Players[0].Id),
                Url = "https://www.speedrun.com/me/run/" + runs.data[num].Id,
                Color = Color.FromArgb(42, 137, 231),
                Thumbnail = new EmbedMedia() { Url = "https://www.speedrun.com/themes/me/cover-128.png" },
                Author = new EmbedAuthor() { Name = "Mirror's Edge - " + GetCategoryName(runs.data[num].Category), Url = runs.data[num].Weblink },

                Fields = new[] {
                    new EmbedField() { Name = "Leaderboard Rank:", Value = GetLeaderboardRank(runs.data[num].Players[0].Id, num) },
                    new EmbedField() { Name = "Date Played:", Value = runs.data[num].Submitted.ToString("yyyy-MM-dd") }
                }
            };

            DiscordMessage message = new DiscordMessage
            {
                Embeds = new[] { embed }
            };

            hook.Send(message);
        }

        static string GetUsername(string id)
        {
            return JsonConvert.DeserializeObject<Api.UserInfo>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/users/" + id).Result).data.names.International;
        }

        static string GetLeaderboardRank(string id, int num)
        {
            leaderboard = JsonConvert.DeserializeObject<Api.LeaderboardInfo>(new HttpClient().GetStringAsync("https://www.speedrun.com/api/v1/users/" + id + "/personal-bests?game=yo1yyr1q").Result);

            for (int i = 0; i < leaderboard.data.Length; i++)
            {
                if (runs.data[num].Id == leaderboard.data[i].run.Id)
                {
                    return leaderboard.data[i].Place.ToString();
                }
            }

            return "n/a";
        }

        static string GetCategoryName(string id)
        {
            for (int i = 0; i < category.data.Categories.Data.Length; i++)
            {
                if (id == category.data.Categories.Data[i].Id)
                {
                    return category.data.Categories.Data[i].Name;
                }
            }

            return "n/a";
        }
    }
}
