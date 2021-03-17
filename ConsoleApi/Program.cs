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
        private static string LatestRunID;
        private static readonly string URL = "https://www.speedrun.com/api/v1/runs?status=verified&orderby=verify-date&direction=desc&game=yo1yyr1q";

        static void Main(string[] args)
        {
            Console.Title = "RunGet v1.0";

            var client = new HttpClient();
            var content = client.GetStringAsync(URL).Result;

            // Deserialize Json
            Api api = JsonConvert.DeserializeObject<Api>(content);

            // Save the run ID
            LatestRunID = api.data[0].Id.ToString();

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
            var client = new HttpClient();
            var content = client.GetStringAsync(URL).Result;

            Api api = JsonConvert.DeserializeObject<Api>(content);

            int num = 0;

            // Loop through all 20 runs we get from the url
            for (int i = 0; i < 20; i++)
            {
                // Checks if the latest run is new and how many new runs there are
                if ("yo9qq05y" == api.data[i].Id.ToString() && num != 0)
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
                        LatestRunID = api.data[0].Id.ToString();
                    }

                    DiscordWebhook(api, num);

                    num--;
                    Console.WriteLine("[" + DateTime.Now + $"] ID: {api.data[num].Id}");

                    if (num != 0)
                    {
                        // Sleep for few seconds. Just incase we don't hit the Discord's rate limit
                        Thread.Sleep(TimeSpan.FromSeconds(5));
                    }
                }
            }
        }

        static void DiscordWebhook(Api api, int num)
        {
            DiscordWebhook hook = new DiscordWebhook
            {
                Url = ""
            };

            DiscordEmbed embed = new DiscordEmbed
            {
                Title = "44s 310ms by Toyro98",
                Url = "https://www.speedrun.com/me/run/" + api.data[num].Id,
                Color = Color.FromArgb(42, 137, 231),
                Thumbnail = new EmbedMedia() { Url = "https://www.speedrun.com/themes/me/cover-128.png" },
                Author = new EmbedAuthor() { Name = "Mirror's Edge - Any%", Url = api.data[num].Weblink },
                Fields = new[] {
                    new EmbedField() { Name="Leaderboard Rank:", Value="n/a" },
                    new EmbedField() { Name="Date Played:", Value = api.data[num].Submitted.ToString("yyyy-MM-dd") }
                }
            };

            DiscordMessage message = new DiscordMessage
            {
                Embeds = new[] { embed }
            };

            hook.Send(message);
        }
    }
}
