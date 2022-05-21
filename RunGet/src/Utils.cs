using System;
using System.Drawing;
using System.Reflection;

namespace RunGet
{
    public class Utils
    {
        public static string GetVariablesPath(RunsApi.Data run)
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

            // Just to be sure it's not empty. If it's, return empty string
            if (!string.IsNullOrEmpty(path))
            {
                // Removes the last '&' 
                return path.Remove(path.Length - 1, 1);
            }

            return "";
        }

        public static Color GetColorFromRank(int rank)
        {
            return rank switch
            {
                // Gold, Silver, and Bronze
                1 => Color.FromArgb(201, 176, 55),
                2 => Color.FromArgb(215, 215, 215),
                3 => Color.FromArgb(106, 56, 5),

                // Default color (Blue)
                _ => Color.FromArgb(42, 137, 231),
            };
        }

        public static void Seperator(int length = 112, char c = '=')
        {
            Console.WriteLine("".PadRight(length, c));
        }

        public static string GetVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            
            return version.Major + "." + version.Minor + (version.Build != 0 ? "." + version.Build.ToString() : "");
        }
    }
}
