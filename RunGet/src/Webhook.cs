using System;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using Discord;
using Discord.Webhook;
using Pastel;

namespace RunGet
{
    public static class Webhook
    {
        public static DiscordMessage Create(RunsApi.Data runs, string platform)
        {
            // Get the rank first since we need if for getting the time difference if it's a wr
            int rank = Rank.GetLeaderboardRank(runs);
            string timeDifference;

            // Get the time difference as a string. Example, "4s 200ms"
            // Returns null if no previous pb was found or if it's a wr but no 2nd place run found 
            if (rank == 1)
            {
                timeDifference = Time.GetTimeDifferenceWorldRecord(runs);
            }
            else
            {
                timeDifference = Time.GetTimeDifference(runs);
            }

            DiscordEmbed embed = new DiscordEmbed
            {
                Title = Time.FormatTime(runs.Times.Primary_t) + " by " + User.GetNames(runs),
                Url = runs.Weblink,
                Color = Utils.GetColorFromRank(rank),

                Thumbnail = new EmbedMedia()
                {
                    Url = runs.Game.Data.Assets.Coversmall.Uri
                },

                Author = new EmbedAuthor()
                {
                    Name = runs.Game.Data.Names.International + " - " +
                        Category.GetCategoryName(runs) + 
                        Category.GetPlatformName(runs.Platform.Data.Name, platform)
                },

                Fields = new List<EmbedField>() {}
            };

            embed.Fields.Add(new EmbedField()
            {
                Name = "Leaderboard Rank",
                Value = rank.ToString()
            }); 

            embed.Fields.Add(new EmbedField()
            {
                Name = "Date Played",
                Value = runs.Date,
                InLine = true
            });

            // Only add this field if the user had a pb before or submitted for the first time and it's a new wr and the leaderboard wasn't empty
            if (!string.IsNullOrEmpty(timeDifference))
            {
                embed.Fields.Add(new EmbedField()
                {
                    Name = "Improved" + (rank == 1 ? " World Record " : " Personal Best ") + "by",
                    Value = timeDifference,
                    InLine = true
                }); 
            }

            return new DiscordMessage
            {
                Embeds = new List<DiscordEmbed>()
                {
                    embed
                },

                Username = "Verified Runs",
                AvatarUrl = "https://raw.githubusercontent.com/Toyro98/RunGet/main/RunGet/src/image/Avatar.png"
            };
        }

        public static void Send(DiscordWebhook webhook, DiscordMessage message) 
        { 
            // Error codes can be found at 
            // https://discord.com/developers/docs/topics/opcodes-and-status-codes#http

            try
            {
                webhook.Send(message);
            }
            catch (WebException error)
            {
                Utils.Seperator();

                Console.Write("[{0}] Discord Error: ",
                    DateTime.Now.ToString().Pastel("#808080")
                );

                if (error.Status == WebExceptionStatus.ProtocolError)
                {
                    if (error.Response is HttpWebResponse response)
                    {
                        switch ((int)response.StatusCode)
                        {
                            // Bad Request
                            case 400:
                                Console.WriteLine("The request was improperly formatted, or the server couldn't understand it. (400 Bad Request)");
                                Console.ReadLine();
                                Environment.Exit(0);
                                break;

                            // Not Found
                            case 404:
                                Console.WriteLine("The url no longer works (404 Not Found). Please update the url before restarting this program!");
                                Console.ReadLine();
                                Environment.Exit(0);
                                break;

                            // Too Many Requests
                            case 429:
                                Console.WriteLine("Time to chill for a bit (429 Too Many Requests). ");
                                break;

                            // Gateway Unavailable
                            case 502:
                                Console.WriteLine("There was not a gateway available to process your request (502 Gateway Unavailable). ");
                                break;

                            default:
                                Console.Write("Unknown error. ");
                                Console.WriteLine(error.Message);
                                break;
                        }

                        Console.WriteLine("Waiting 5 min before trying again.");

                        // Sleep for 5 min and try again
                        Thread.Sleep(TimeSpan.FromMinutes(5));
                        Send(webhook, message);
                    }
                }
                else
                {
                    Console.WriteLine(error.Message);
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
        }
    }
}
