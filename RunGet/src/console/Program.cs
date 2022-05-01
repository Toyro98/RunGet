using System;
using System.Threading;
using Pastel;

namespace RunGet
{
    class Program
    {
        static void Main(string[] args) 
        {
            // Get the latests run id from the API
            Runs.GetLatestRunId();

            // Infinite loop
            while (true)
            {
                // Sleep for 5 min
                Thread.Sleep(TimeSpan.FromMinutes(5));

                // Catch any errors and write it to the console
                try
                {
                    Runs.LookForNewRuns();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("".PadRight(112, '='));
                    Console.WriteLine("[{0}] {1}\n", DateTime.Now.ToString().Pastel("#808080"), "Error!! :(".Pastel("#8C0000"));
                    Console.WriteLine(ex.ToString());
                    Console.ReadLine();
                    Environment.Exit(0);
                }
            }
        }
    }
}
