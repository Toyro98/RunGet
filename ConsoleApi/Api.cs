namespace Api
{
    class Run
    {
        public Data[] data { get; set; }

        public class Data
        {
            public string Id { get; set; }
            public string Level { get; set; }
            public string Category { get; set; }
            public Player[] Players { get; set; }
            public string Date { get; set; }
            public Times Times { get; set; }
        }

        public class Times
        {
            public float Primary_t { get; set; }
        }

        public class Player
        {
            public string Rel { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }

    class Leaderboard
    {
        public Data[] data { get; set; }

        public class Data
        {
            public Names names { get; set; }
            public int Place { get; set; }
            public Run run { get; set; }
        }

        public class Names
        {
            public string International { get; set; }
        }

        public class Run
        {
            public string Id { get; set; }
        }
    }

    class Category
    {
        public Data data { get; set; }

        public class Data
        {
            public Categories Categories { get; set; }
            public Levels Levels { get; set; }
            public Names Names { get; set; }
            public Assets Assets { get; set; }
            public string Abbreviation { get; set; }
            public string Weblink { get; set; }
        }

        public class Assets
        {
            public CoverTiny Covertiny { get; set; }
            public CoverSmall Coversmall { get; set; }
            public CoverMedium Covermedium { get; set; }
            public CoverLarge Coverlarge { get; set; }
        }

        public class CoverTiny
        {
            public string Uri { get; set; }
        }

        public class CoverSmall
        {
            public string Uri { get; set; }
        }

        public class CoverMedium
        {
            public string Uri { get; set; }
        }

        public class CoverLarge
        {
            public string Uri { get; set; }
        }

        public class Names
        {
            public string International { get; set; }
        }

        public class Categories
        {
            public Datum[] Data { get; set; }
        }

        public class Datum
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class Levels
        {
            public Datum2[] Data { get; set; }
        }

        public class Datum2
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }

    class User
    {
        public Data data { get; set; }

        public class Data
        {
            public Names names { get; set; }
        }

        public class Names
        {
            public string International { get; set; }
        }
    }
}
