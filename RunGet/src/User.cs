namespace RunGet
{
    public class User
    {
        public static string GetNames(RunsApi.Data run)
        {
            // Check if the run only has one player
            if (run.Players.Data.Length == 1)
            {
                if (run.Players.Data[0].Rel == "user")
                {
                    return run.Players.Data[0].Names.International;
                }

                return run.Players.Data[0].Name;
            }

            // The run has multiple runners. We only want to display the first 2 names
            string[] players = new string[2];

            for (int i = 0; i < players.Length; i++)
            {
                if (run.Players.Data[i].Rel == "user")
                {
                    players[i] = run.Players.Data[i].Names.International;
                }
                else
                {
                    players[i] = run.Players.Data[i].Name;
                }
            }

            if (run.Players.Data.Length == 2)
            {
                return players[0] + " and " + players[1];
            }
            
            // Instead of displaying everyones names in the embed title
            // It will show the first two players and say "x, y, and z more runner(s)"
            int amountOfRunners = run.Players.Data.Length - 2;
            return players[0] + ", " + players[1] + ", and " + amountOfRunners + " other runner" + ((amountOfRunners == 1) ? "" : "s");
        }
    }
}
