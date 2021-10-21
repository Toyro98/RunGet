using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using Discord;
using Discord.Webhook;
using Pastel;

namespace RunGet
{
    public class Discord
    {
        public static void Webhook(int num, RunsAPI.Root runs, List<Speedrun> game, int gameIndex)
        {
            // Discord webhook url
            DiscordWebhook webhook = new DiscordWebhook
            {
                Url = game[gameIndex].DiscordWebhookURL
            };

            // Create the embed
            DiscordEmbed embed = new DiscordEmbed
            {
                Title = Time.FormatTime(num, runs) + " by " + User.GetUserName(num, runs),
                Url = runs.Data[num].Weblink,
                Color = Color.FromArgb(42, 137, 231),

                Thumbnail = new EmbedMedia()
                {
                    Url = runs.Data[num].Game.Data.Assets.Coversmall.Uri
                },

                Author = new EmbedAuthor()
                {
                    Name = runs.Data[num].Game.Data.Names.International + " - " + Category.GetCategoryName(num, runs, game[gameIndex].DefaultPlatformName)
                },

                Fields = new List<EmbedField>()
                {
                    new EmbedField()
                    {
                        Name = "Leaderboard Rank:",

                        // Returns the rank or "n/a" if not found
                        Value = Rank.GetLeaderboardRank(num, runs)
                    },
                    new EmbedField()
                    {
                        Name = "Date Played:",

                        // ISO 8601
                        Value = runs.Data[num].Date
                    }
                }
            };

            // Create message with the embed
            DiscordMessage message = new DiscordMessage
            {
                Username = "Verified Runs",
                AvatarUrl = "https://raw.githubusercontent.com/Toyro98/RunGet/main/RunGet/src/image/Avatar.png",

                // Add Embed
                Embeds = new List<DiscordEmbed>()
                {
                    embed
                }
            };

            // Send the discord message with the embed message
            // TODO: Handle webhook error and other discord errors
            try
            {
                webhook.Send(message);
            }
            catch (Exception error)
            {
                Console.WriteLine("[{0}] Discord Error: {1}", DateTime.Now.ToString().Pastel("#808080"), error.Message);
                Console.ReadLine();
            }
        }
    }
}
