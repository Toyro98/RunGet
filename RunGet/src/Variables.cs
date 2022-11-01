namespace RunGet
{
    public static class Variables
    {
        public static string GetValuesPath(RunsModel.Data run)
        {
            string path = string.Empty;

            foreach (var item in run.Values)
            {
                if (run.Level != null)
                {
                    foreach (var level in run.Level.Data.Variables.Data)
                    {
                        foreach (var levelData in level.Values.Values)
                        {
                            if (levelData.Key == item.Value)
                            {
                                path += "var-" + item.Key + "=" + item.Value + "&";
                            }
                        }
                    }
                }
                else
                {
                    foreach (var category in run.Category.Data.Variables.Data)
                    {
                        foreach (var categoryData in category.Values.Values)
                        {
                            if (categoryData.Key == item.Value && category.IsSubCategory)
                            {
                                path += "var-" + item.Key + "=" + item.Value + "&";
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(path))
            {
                return path.Remove(path.Length - 1, 1);
            }

            return "";
        }

        public static string GetPersonalBestPath(RunsModel.Data run)
        {
            string path = "runs?user=";

            if (run.Players.Data[0].Rel == "user")
            {
                path += run.Players.Data[0].Id;
            }
            else
            {
                path += run.Players.Data[0].Name;
            }

            path += "&game=" + run.Game.Data.Id + "&category=" + run.Category.Data.Id + "&";

            if (run.Level != null)
            {
                path += "level=" + run.Level.Data.Id;
            }

            if (run.Values.Count > 0)
            {
                path += GetValuesPath(run);
            }

            if (path.EndsWith('&'))
            {
                path = path.Remove(path.Length - 1, 1);
            }

            return path += "&status=verified&orderby=date&direction=desc";
        }

        public static string GetLeaderboardPath(RunsModel.Data run, bool isConsole)
        {
            var path = "leaderboards/" + run.Game.Data.Id;

            if (run.Level != null)
            {
                path += "/level/" + run.Level.Data.Id + "/";
            }
            else
            {
                path += "/category/";
            }

            path += run.Category.Data.Id;

            if (isConsole)
            {
                path += "?platform=" + run.Platform.Data.Id;
            }

            if (run.Values.Count > 0)
            {
                path += isConsole ? "&" : "?" + GetValuesPath(run);
            }

            return path;
        }
    }
}
