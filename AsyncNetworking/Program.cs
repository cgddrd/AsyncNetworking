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
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            RunCalls();

            Console.ReadLine();

        }

        static void RunCalls()
        {
            Console.WriteLine("Starting RunCalls()...");

            // If we use 'await' here, then the application will wait until we get the response back from the API.
            // Currently, it continues on (the await inside of 'GetRequestAsync' will wait for the response to come back). 
            var helloMsg = GetRequestAsync("?data={%22Hello%22:%22World%22}&sleep=20")
                            .ContinueWith(callback =>
                                {
                                    Console.WriteLine($"Result = {callback.Result}");
                                });

            Console.WriteLine("Ending RunCalls()...");
        }

        static async Task<string> GetRequestAsync(string url)
        {
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string result = await response.Content.ReadAsStringAsync();
                return result;
            }

            return null;
        }
    }
}
