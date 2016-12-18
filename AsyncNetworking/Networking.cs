using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace AsyncNetworking
{
    public class Networking
    {

        private static HttpClient client = new HttpClient();

        public Networking(string baseUrl) : this(new HttpClientHandler(), baseUrl)
        {   
        }

        public Networking(HttpMessageHandler handler, string baseUrl)
        {
            client = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl)
            };

            //client.BaseAddress = new Uri("http://localhost:7654/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<HttpResponseMessage> MakeRequestAsync(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);

            // Specify that we should throw an exception for any HTTP response that falls outside of 2XX-3XX.
            response.EnsureSuccessStatusCode();

            return response;

        }

        public async Task<string> GetRequestAsync(string url)
        {

            return await Policy.Handle<HttpRequestException>()
                .WaitAndRetryAsync(5, retryAttempt =>

                    // Exponential backoff.
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),

                    // Log everytime Polly catches an exception.
                    (exception, timeSpan) => {

                        Console.WriteLine($"Polly caught an error -> {exception}. Retrying in {timeSpan.Seconds} secs.");
                        url = "?data={%22Hello%22:%22World%22}";

                    }
                )
                // Execute the following code asynchronously, catching exceptions as they come in.
                // TODO - Add checks for specific HTTP response codes.
                .ExecuteAsync(async () =>
                {

                    //return await MakeRequestAsync(url);

                    var test = await MakeRequestAsync(url) as HttpResponseMessage;

                    return await test.Content.ReadAsStringAsync();

                });

        }

    }
}
