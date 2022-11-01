using System.Collections.Generic;

namespace RunGet
{
    public class Category
    {
        public static string GetCategoryName(RunsModel.Data runs)
        {
            string variables = string.Empty;
            List<string> variableNames = new List<string>();

            if (runs.Values.Count > 0)
            {
                foreach (var item in runs.Values)
                {
                    foreach (var category in runs.Category.Data.Variables.Data)
                    {
                        foreach (var categoryData in category.Values.Values)
                        {
                            if (item.Value == categoryData.Key && category.IsSubCategory)
                            {
                                variableNames.Add(categoryData.Value.Label);
                            }
                        }
                    }
                }

                if (variableNames.Count > 0)
                {
                    for (int i = 0; i < variableNames.Count; i++)
                    {
                        if (i == 0)
                        {
                            variables += variableNames[i];
                        }
                        else
                        {
                            variables += ", " + variableNames[i];
                        }
                    }
                }
            }

            if (runs.Category.Data.Type == "per-game")
            {
                if (string.IsNullOrEmpty(variables))
                {
                    return runs.Category.Data.Name;
                }

                return runs.Category.Data.Name + " - " + variables;
            }

            if (string.IsNullOrEmpty(variables))
            {
                return runs.Level.Data.Name + ": " + runs.Category.Data.Name;
            }

            return runs.Level.Data.Name + ": " + runs.Category.Data.Name + " - " + variables;
        }
    }
}
