using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace RunGet
{
    public class RunsModel
    {
        public class Root
        {
            public Data[] Data { get; set; }
        }

        public class Data
        {
            public string Id { get; set; }
            public string Weblink { get; set; }
            public Game Game { get; set; }

            [JsonProperty("level")]
            public object Levels { get; set; }

            [JsonIgnore]
            public Level Level { get; set; }
            public Category Category { get; set; }
            public Players Players { get; set; }
            public string Date { get; set; }
            public DateTime Submitted { get; set; }
            public Times Times { get; set; }
            public Platform Platform { get; set; }
            public Dictionary<string, string> Values { get; set; }
        }

        public class Game
        {
            public GameData Data { get; set; }
        }

        public class GameData
        {
            public string Id { get; set; }
            public GameNames Names { get; set; }
            public GameAssets Assets { get; set; }
        }

        public class GameNames
        {
            public string International { get; set; }
        }

        public class Category
        {
            public CategoryData Data { get; set; }
        }

        public class CategoryData
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public CategoryVariables Variables { get; set; }
        }

        public class CategoryVariables
        {
            public CategoryVariablesData[] Data { get; set; }
        }

        public class CategoryVariablesData
        {
            public CategoryVariablesValues Values { get; set; }

            [JsonProperty("is-subcategory")]
            public bool IsSubCategory { get; set; }
        }

        public class CategoryVariablesValues
        {
            public Dictionary<string, CategoryValueData> Values { get; set; }
        }

        public class CategoryValueData
        {
            public string Label { get; set; }
        }

        public class Level
        {
            public LevelData Data { get; set; }
        }

        public class LevelData
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public LevelVariables Variables { get; set; }
        }

        public class LevelVariables
        {
            public LevelVariablesData[] Data { get; set; }
        }

        public class LevelVariablesData
        {
            public LevelVariablesValues Values { get; set; }
        }

        public class LevelVariablesValues
        {
            public Dictionary<string, LevelValueData> Values { get; set; }
        }

        public class LevelValueData
        {
            public string Label { get; set; }
        }

        public class Players
        {
            public PlayerData[] Data { get; set; }
        }

        public class PlayerData
        {
            public string Rel { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
            public PlayerNames Names { get; set; }
        }

        public class PlayerNames
        {
            public string International { get; set; }
        }

        public class Times
        {
            public decimal Primary_t { get; set; }
        }

        public class Platform
        {
            public PlatformData Data { get; set; }
        }

        public class PlatformData
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class GameAssets
        {
            [JsonProperty("cover-small")]
            public CoverSmall Coversmall { get; set; }
        }

        public class CoverSmall
        {
            public string Uri { get; set; }
        }
    }
}
