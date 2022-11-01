using System;
using System.Collections.Generic;
using System.Threading;
using Pastel;
using Discord.Webhook;
using Discord;

namespace RunGet
{
    public class Runs
    {
        public static List<Speedrun> games = new List<Speedrun>(){};

        public static void GetLatestRunId() 
        {
            foreach (var game in games)
            {
                game.Runs = GetLatestData(game.Id, 1);
                game.LatestRunId = game.Runs[0].Id;

                Console.WriteLine("[{0}] Latest Run ID: {1} Game: {2}",
                    DateTime.Now.ToString().Pastel("#808080"),
                    game.LatestRunId.Pastel("#808080"),
                    game.Runs[0].Game.Data.Names.International.Pastel("#808080")
                );

                game.Runs = Array.Empty<RunsModel.Data>();
            }
        }

        public static void LookForNewRuns()
        {
            for (int i = 0; i < games.Count; i++)
            {
                int runsFound = 0;

                games[i].Runs = GetLatestData(games[i].Id);

                for (int j = 0; j < games[i].Runs.Length; j++)
                {
                    if (games[i].LatestRunId != games[i].Runs[j].Id)
                    {
                        runsFound++;
                    }
                    else
                    {
                        if (runsFound == 0) 
                        { 
                            break; 
                        }

                        Console.WriteLine("[{0}] {1} new run" + ((runsFound > 1) ? "s" : "") + " found on {2}",
                            DateTime.Now.ToString().Pastel("#808080"),
                            runsFound.ToString().Pastel("#808080"),
                            games[i].Runs[0].Game.Data.Names.International.Pastel("#808080")
                        );

                        games[i].LatestRunId = games[i].Runs[0].Id;

                        while (runsFound != 0)
                        {
                            runsFound--;

                            // Ignore the run if the username is "n/a"
                            if (games[i].Runs[runsFound].Players.Data[0].Rel == "guest")
                            {
                                if (games[i].Runs[runsFound].Players.Data[0].Name.ToLower().Contains("n/a"))
                                {
                                    Console.WriteLine("[{0}] Run ID: {1} (Ignored due to name contains \"n/a\")",
                                        DateTime.Now.ToString().Pastel("#808080"),
                                        games[i].Runs[runsFound].Id.Pastel("#808080")
                                    );

                                    continue;
                                }
                            }

                            DiscordWebhook url = new DiscordWebhook() 
                            { 
                                Url = games[i].WebhookUrl 
                            };

                            RunsModel.Data run = games[i].Runs[runsFound];

                            var leaderboard = Json.Deserialize<LeaderboardModel.Root>(Https.Get(Variables.GetLeaderboardPath(run, false)).Result);
                            var personalBests = Json.Deserialize<RunsModelLight.Root>(Https.Get(Variables.GetPersonalBestPath(run)).Result);

                            Webhook webhook = new Webhook
                            {
                                run = run,
                                leaderboard = leaderboard,
                                personalBest = personalBests,
                                defaultPlatform = games[i].DefaultPlatform,
                                rank = Rank.GetLeaderboardRank(leaderboard, run.Players)
                            };

                            if (webhook.rank == 1)
                            {
                                webhook.timeDifference = Time.GetTimeDifferenceWorldRecord(leaderboard, personalBests, run);
                            }
                            else
                            {
                                webhook.timeDifference = Time.GetTimeDifference(personalBests, run);
                            }

                            if (run.Platform.Data.Name != webhook.defaultPlatform)
                            {
                                leaderboard = Json.Deserialize<LeaderboardModel.Root>(Https.Get(Variables.GetLeaderboardPath(run, true)).Result);
                                webhook.consoleRank = Rank.GetLeaderboardRank(leaderboard, run.Players);
                            }

                            DiscordMessage message = webhook.CreateEmbedMessage();
                            Webhook.Send(url, message);

                            Console.WriteLine("[{0}] Run ID: {1}",
                                DateTime.Now.ToString().Pastel("#808080"),
                                run.Id.Pastel("#808080")
                            );

                            Title.UpdateRunsFoundCounterBy(1);
                            Thread.Sleep(1500);
                        }

                        break;
                    }
                }

                games[i].Runs = Array.Empty<RunsModel.Data>();
            }
        }

        private static RunsModel.Data[] GetLatestData(string gameId, int amount = 20)
        {
            var data = Array.Empty<RunsModel.Data>();

            string url = "runs?status=verified&orderby=verify-date&direction=desc&game=" + gameId + "&embed=category.variables,level.variables,players,game,platform&max=" + amount;

            while (data.Length == 0)
            {
                data = Json.DeserializeRuns(Https.Get(url).Result);

                if (data == null || data.Length == 0)
                {
                    data = Array.Empty<RunsModel.Data>();
                    Thread.Sleep(TimeSpan.FromMinutes(5));
                }
            }

            return data;
        }

        public static bool IsWorldRecordAPersonalBestImprovement(LeaderboardModel.Root leaderboard, RunsModelLight.Root personalBests)
        {
            if (leaderboard.Data.Runs.Length < 2 && personalBests.Data.Length < 2)
            {
                return false;
            }

            var previousWorldRecordDate = leaderboard.Data.Runs[1].Run.Date ?? DateTime.MaxValue;
            var previousPersonalBestDate = DateTime.MaxValue;

            float currentWorldRecord = leaderboard.Data.Runs[0].Run.Times.Primary_t;
            float previousWorldRecord = leaderboard.Data.Runs[1].Run.Times.Primary_t;
            float previousPersonalBest = float.MaxValue;

            for (int i = 0; i < personalBests.Data.Length; i++)
            {
                if (personalBests.Data[i].Date == null)
                {
                    continue;
                }

                if (previousWorldRecord > personalBests.Data[i].Times.Primary_t)
                {
                    if (currentWorldRecord != personalBests.Data[i].Times.Primary_t)
                    {
                        previousPersonalBest = personalBests.Data[i].Times.Primary_t;
                        previousPersonalBestDate = (DateTime)personalBests.Data[i].Date;
                        break;
                    }
                }
            }

            if (previousPersonalBestDate == DateTime.MaxValue)
            {
                return false;
            }

            if (previousWorldRecord > previousPersonalBest)
            {
                if (previousPersonalBestDate > previousWorldRecordDate)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
