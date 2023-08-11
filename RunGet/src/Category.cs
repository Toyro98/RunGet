using System.Collections.Generic;

namespace RunGet
{
    public class Category
    {
        public static string GetCategoryName(RunsModel.Data runs)
        {
            string variables = string.Empty;
            List<string> variableNames = new List<string>();

            if (runs.Values.Count == 0)
            {
                return NewMethod(runs, variables);
            }

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
                    variables += (i == 0 ? "" : ", ") + variableNames[i];
                }
            }

            return NewMethod(runs, variables);

            static string NewMethod(RunsModel.Data runs, string variables)
            {
                if (runs.Category.Data.Type == "per-game")
                {
                    return runs.Category.Data.Name + (string.IsNullOrEmpty(variables) ? "" : " - " + variables);
                }

                return runs.Level.Data.Name + ": " + runs.Category.Data.Name + (string.IsNullOrEmpty(variables) ? "" : " - " + variables);
            }
        }
    }
}
