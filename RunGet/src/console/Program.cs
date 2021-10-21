using System;
using System.Threading;

namespace RunGet
{
    class Program
    {
        static void Main(string[] args) 
        {
            Runs.GetLatestRunId();
            
            while (true)
            {
                Thread.Sleep(TimeSpan.FromMinutes(5));
                Runs.LookForNewRuns();
            }
        }
    }
}
