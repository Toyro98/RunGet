using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Pastel;

namespace RunGet
{
    public static class Https
    {
        public static async Task<string> Get(string uri)
        {
            // Client
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://www.speedrun.com/api/v1/");
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "RunGet/2.0.0");

            // Try to send a get request
            try
            {
                // Store the data from API
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress + uri);
                string data = await response.Content.ReadAsStringAsync();

                // Update Stats
                Title.UpdateTitle(api: 1);

                // Check if the status code is 200
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // Return data 
                    return await Task.FromResult(data).ConfigureAwait(false);
                }
                else
                {
                    // Display error details. Most likely 420(Ratelimit) or 502 Badgateway
                    Console.WriteLine("[{0}] Status Code: {1} ({2}) Trying again in 10 min.", DateTime.Now.ToString().Pastel("#808080"), response.StatusCode, response.ReasonPhrase);

                    // Return null
                    return null;
                }
            }
            catch (HttpRequestException error)
            {
                // No internet connection
                Console.WriteLine("[{0}] {1} Trying again in 10 min.", DateTime.Now.ToString().Pastel("#808080"), error.Message);

                // Return null
                return null;
            }
        }
    }
}
