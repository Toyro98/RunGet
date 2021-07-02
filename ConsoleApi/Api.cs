namespace Api
{
    class Run
    {
        public Data[] data { get; set; }

        public class Data
        {
            public string Id { get; set; }
            public string Weblink { get; set; }
            public string Level { get; set; }
            public string Category { get; set; }
            public Player[] Players { get; set; }
            public string Date { get; set; }
            public Times Times { get; set; }
            public System System { get; set; }
            public Status Status { get; set; }
        }

        public class Status
        {
            public string Examiner { get; set; }
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

        public class System
        {
            public string Platform { get; set; }
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
            public string Abbreviation { get; set; }
            public string Weblink { get; set; }
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
            public Datum1[] Data { get; set; }
        }

        public class Datum1
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

    class Platform
    {
        public Data data { get; set; }

        public class Data
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public int Released { get; set; }
        }
    }
}