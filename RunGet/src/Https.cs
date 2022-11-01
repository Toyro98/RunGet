using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Pastel;

namespace RunGet
{
    public static class Https
    {
        public static async Task<string> Get(string uri)
        {
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("https://www.speedrun.com/api/v1/")
            };

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "RunGet/" + Title.version);

            try
            {
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress + uri);
                string data = await response.Content.ReadAsStringAsync();

                Title.UpdateApiCounterBy(1);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return await Task.FromResult(data).ConfigureAwait(false);
                }
                
                Console.WriteLine("[{0}] Status Code: {1} ({2}) Trying again in 5 min.", 
                    DateTime.Now.ToString().Pastel("#808080"), 
                    response.StatusCode, 
                    response.ReasonPhrase
                );

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
