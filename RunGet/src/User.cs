namespace RunGet
{
    public class User
    {
        public static string GetNames(RunsApi.Players run)
        {
            // Return the name if it's only one runner
            if (run.Data.Length == 1)
            {
                if (run.Data[0].Rel == "user")
                {
                    return run.Data[0].Names.International;
                }

                return run.Data[0].Name;
            }

            // The run has multiple runners. Only display the first 2 names
            string[] players = new string[2];

            for (int i = 0; i < players.Length; i++)
            {
                if (run.Data[i].Rel == "user")
                {
                    players[i] = run.Data[i].Names.International;
                }
                else
                {
                    players[i] = run.Data[i].Name;
                }
            }

            if (run.Data.Length == 2)
            {
                return players[0] + " and " + players[1];
            }

            // Instead of displaying everyones names in the embed title
            // It will show the first two players and say "x, y, and z more runner(s)"
            return players[0] + ", " + players[1] + ", and " + (run.Data.Length - 2) + " other runner" + ((run.Data.Length - 2) == 1 ? "" : "s");
        }
    }
}
