using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RunGet
{
    public class Category
    {
        public static string GetCategoryName(int num, RunsAPI.Root runs, string defaultPlatformName)
        {
            string platform = runs.Data[num].Platform.Data.Name;
            string category = runs.Data[num].Category.Data.Name;
            string level = string.Empty;
            string value = string.Empty;

            if (runs.Data[num].Level != null)
            {
                level = runs.Data[num].Level.Data.Name;
            }

            List<string> values = new List<string>();

            // Get variables names if there are any
            if (runs.Data[num].Values.Count > 0)
            {
                foreach (var item in runs.Data[num].Values)
                {
                    foreach (var varCategory in runs.Data[num].Category.Data.Variables.Data)
                    {
                        foreach (var varCategoryData in varCategory.Values.Values)
                        {
                            if (item.Value == varCategoryData.Key && varCategory.Issubcategory)
                            {
                                values.Add(varCategoryData.Value.Label);
                            }
                        }
                    }
                }
            }

            // Add the variables names to the string
            if (values.Count > 0)
            {
                for (int i = 0; i < values.Count; i++)
                {
                    if (i == 0)
                    {
                        value += values[i];
                    }
                    else
                    {
                        value += ", " + values[i];
                    }
                }
            }

            if (runs.Data[num].Category.Data.Type == "per-game")
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Example: Any% (Xbox360)
                    return category + IsPlatformNameDefault(platform, defaultPlatformName);
                }
                else
                {
                    // Example: Any% - Macro, Steam (Linux)
                    return category + " - " + value + IsPlatformNameDefault(platform, defaultPlatformName);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(value))
                {
                    // Example: Stormdrains One: TAS - Any% (Mac)
                    return level + ": " + category + IsPlatformNameDefault(platform, defaultPlatformName);
                }
                else
                {
                    // Example: Stormdrains One: TAS - Any%, Macro, Australian Mode
                    return level + ": " + category + " - " + value + IsPlatformNameDefault(platform, defaultPlatformName);
                }
            }
        }

        private static string IsPlatformNameDefault(string platformName, string defaultPlatformName)
        {
            if (string.IsNullOrWhiteSpace(platformName))
            {
                return " (" + platformName + ")";
            }

            return (platformName == defaultPlatformName) ? "" : " (" + platformName + ")";
        }
    }
}
