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
    class Program
    {

        static HttpClient client = new HttpClient();

        static void Main(string[] args)
        {
            //Some examples of online REST API mocking tools:
            // 1. http://www.fakeresponse.com
            // 2. http://slowwly.robertomurray.co.uk
            // 3. https://getsandbox.com/pricing
            // 4. http://www.mocky.io
            // 5. https://www.mockable.io

            client.BaseAddress = new Uri("http://www.fakeresponse.com/api/");
            //client.BaseAddress = new Uri("http://localhost:7654/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            RunCalls();

            Console.ReadKey();

        }

        static void RunCalls()
        {
            Console.WriteLine("Starting RunCalls()...");

            // If we use 'await' here, then the application will wait until we get the response back from the API.
            // Currently, it continues on (the await inside of 'GetRequestAsync' will wait for the response to come back). 
            var helloMsg = GetRequestAsync("?data={%22Hello%22:%22World%22}&sleep=5&status=500")
                            .ContinueWith(callback =>
                                {
                                    Console.WriteLine($"Result = {callback.Result}");
                                });

            Console.WriteLine("Ending RunCalls()...");
        }

        static async Task<string> GetRequestAsync(string url)
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

                    HttpResponseMessage response = await client.GetAsync(url);

                    // Specify that we should throw an exception for any HTTP response that falls outside of 2XX-3XX.
                    response.EnsureSuccessStatusCode();

                    return await response.Content.ReadAsStringAsync();

                });

        }
    }
}
