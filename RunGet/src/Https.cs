﻿using System;
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
            // Client
            HttpClient client = new HttpClient
            {
                BaseAddress = new Uri("https://www.speedrun.com/api/v1/")
            };

            // Headers
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("User-Agent", "RunGet/" + Utils.GetVersion());

            // Try to send a get request
            try
            {
                // Store the data from API
                HttpResponseMessage response = await client.GetAsync(client.BaseAddress + uri);
                string data = await response.Content.ReadAsStringAsync();

                // Update Stats
                Title.Update(api: 1);

                // Check if the status code is 200
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return await Task.FromResult(data).ConfigureAwait(false);
                }
                else
                {
                    // Display error details. Most likely 420(Ratelimit) or 502 Badgateway
                    Console.WriteLine("[{0}] Status Code: {1} ({2}) Trying again in 5 min.", 
                        DateTime.Now.ToString().Pastel("#808080"), 
                        response.StatusCode, 
                        response.ReasonPhrase
                    );

                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
