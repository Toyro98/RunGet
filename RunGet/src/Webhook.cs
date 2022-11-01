using System;
using System.Collections.Generic;
using System.Net;
using Discord;
using Discord.Webhook;
using Pastel;

namespace RunGet
{
    public class Webhook
    {
        public int rank;
        public int consoleRank = -1;
        public string timeDifference;
        public string defaultPlatform;

        public RunsModel.Data run;
        public RunsModelLight.Root personalBest;
        public LeaderboardModel.Root leaderboard;

        public Webhook() {}

        public DiscordMessage CreateEmbedMessage()
        {
            DiscordEmbed embed = CreateEmbedBase();

            AddLeaderboardFields(embed);

            embed.Fields.Add(new EmbedField()
            {
                Name = "Date Played",
                Value = Date.FormatDate(DateTime.Parse(run.Date)),
                InLine = true
            });

            // Add time difference if the user had a personal best before or
            // submitted for the first time and it's a new world record and the leaderboard wasn't empty
            if (!string.IsNullOrEmpty(timeDifference))
            {
                AddTimeDifferenceField(embed);
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

                Console.WriteLine(error.Message);
                Console.ReadLine();
                Environment.Exit(0);
            }
        }

        private DiscordEmbed CreateEmbedBase()
        {
            return new DiscordEmbed()
            {
                Title = Time.FormatTime(run.Times.Primary_t) + " by " + User.GetNames(run.Players),
                Url = run.Weblink,
                Color = Utils.GetColorFromRank(rank),

                Thumbnail = new EmbedMedia()
                {
                    Url = run.Game.Data.Assets.Coversmall.Uri
                },

                Author = new EmbedAuthor()
                {
                    Name = run.Game.Data.Names.International + " - " + Category.GetCategoryName(run)
                },

                Fields = new List<EmbedField>() { }
            };
        }

        private DiscordEmbed AddTimeDifferenceField(DiscordEmbed embed)
        {
            if (rank == 1)
            {
                if (leaderboard.Data.Runs.Length > 1)
                {
                    var previousRecordHolder = new PreviousRecordHolder
                    {
                        days = Date.GetDifferenceInDays(leaderboard, personalBest)
                    };

                    if (Runs.IsWorldRecordAPersonalBestImprovement(leaderboard, personalBest))
                    {
                        previousRecordHolder.names = User.GetNames(run.Players);
                    }
                    else
                    {
                        previousRecordHolder.names = User.GetNames(Json.Deserialize<UserModel.Root>(Https.Get("runs/" + leaderboard.Data.Runs[1].Run.Id + "?embed=players").Result).Data.Players);
                    }

                    if (previousRecordHolder.days > 0)
                    {
                        timeDifference += " (was held by " + previousRecordHolder.names + " for " + previousRecordHolder.days + " day" + (previousRecordHolder.days != 1 ? "s" : "") + ")";
                    }
                    else if (previousRecordHolder.days == 0)
                    {
                        timeDifference += " (was held by " + previousRecordHolder.names + " for less than a day)";
                    }
                    else
                    {
                        previousRecordHolder.days *= -1;
                        timeDifference += " (was held by " + previousRecordHolder.names + " for " + previousRecordHolder.days + " day" + (previousRecordHolder.days != 1 ? "s" : "") + ")";
                    }
                }
            }

            embed.Fields.Add(new EmbedField()
            {
                Name = "Improved" + (rank == 1 ? " World Record " : " Personal Best ") + "by",
                Value = timeDifference,
                InLine = true
            });

            if (run.Platform.Data.Name != defaultPlatform)
            {
                return AddEmptyField(embed);
            }

            return embed; 
        }

        private DiscordEmbed AddLeaderboardFields(DiscordEmbed embed)
        {
            if (consoleRank != -1)
            {
                // Overall Rank
                embed.Fields.Add(new EmbedField()
                {
                    Name = "Leaderboard Rank",
                    Value = rank.ToString(),
                    InLine = true
                });

                // Console Rank
                embed.Fields.Add(new EmbedField()
                {
                    Name = run.Platform.Data.Name + " Rank",
                    Value = consoleRank.ToString(),
                    InLine = true
                });

                return AddEmptyField(embed);
            }
            
            embed.Fields.Add(new EmbedField()
            {
                Name = "Leaderboard Rank",
                Value = rank.ToString()
            });

            return embed;
        }

        private static DiscordEmbed AddEmptyField(DiscordEmbed embed)
        {
            // Discord can't display 2 coloumns, it needs 3 for whatever reason
            embed.Fields.Add(new EmbedField()
            {
                Name = "\u200b",
                Value = "\u200b",
                InLine = true
            });

            return embed;
        }
    }
}
